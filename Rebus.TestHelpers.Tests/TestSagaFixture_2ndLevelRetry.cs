using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Retry.Simple;
using Rebus.Sagas;
using Rebus.TestHelpers.Tests.Extensions;
#pragma warning disable CS1998

namespace Rebus.TestHelpers.Tests
{
    [TestFixture]
    public class TestSagaFixture_2ndLevelRetry : FixtureBase
    {
        [Test]
        public void CanDispatchFailedMessage()
        {
            var failedMessageWasReceived = new ManualResetEvent(false);

            Using(failedMessageWasReceived);

            using var fixture = SagaFixture.For(() => new MySaga(failedMessageWasReceived));
            
            fixture.DumpLogsOnDispose();
            fixture.Add(new MySagaState { Text = "known-string" });
            fixture.DeliverFailed(new TestMessage("known-string"), new DriveNotFoundException("B:"));
            failedMessageWasReceived.WaitOrDie(TimeSpan.FromSeconds(3));
        }

        class MySagaState : SagaData
        {
            public string Text { get; set; }
        }

        class MySaga : Saga<MySagaState>, IAmInitiatedBy<TestMessage>, IAmInitiatedBy<IFailed<TestMessage>>
        {
            readonly ManualResetEvent _gotFailedMessage;

            public MySaga(ManualResetEvent gotFailedMessage)
            {
                _gotFailedMessage = gotFailedMessage;
            }

            protected override void CorrelateMessages(ICorrelationConfig<MySagaState> config)
            {
                config.Correlate<TestMessage>(m => m.Text, d => d.Text);
                config.Correlate<IFailed<TestMessage>>(m => m.Message.Text, d => d.Text);
            }

            public async Task Handle(TestMessage message) => throw new ApplicationException("Sorry, this was not expected in this test");

            public async Task Handle(IFailed<TestMessage> message)
            {
                Data.Text = message.Message.Text;

                _gotFailedMessage.Set();
            }
        }

        class TestMessage
        {
            public TestMessage(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }
    }
}