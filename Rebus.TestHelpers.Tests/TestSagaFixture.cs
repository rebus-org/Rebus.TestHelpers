using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Rebus.Exceptions;
using Rebus.Sagas;
#pragma warning disable 1998

namespace Rebus.TestHelpers.Tests
{
    [TestFixture]
    public class TestSagaFixture : FixtureBase
    {
        [Test]
        public void CanSetUpFakeSagaData()
        {
            using var fixture = SagaFixture.For<MySaga>();
            
            fixture.Add(new MySagaState { Text = "I know you!" });
            fixture.AddRange(new[] { new MySagaState { Text = "I know you too!" } });

            Assert.That(fixture.Data.Count(), Is.EqualTo(2));
            Assert.That(fixture.Data.OfType<MySagaState>().Count(d => d.Text == "I know you!"), Is.EqualTo(1));
            Assert.That(fixture.Data.OfType<MySagaState>().Count(d => d.Text == "I know you too!"), Is.EqualTo(1));
        }

        [Test]
        public void CanRetrieveSagaData()
        {
            using var fixture = SagaFixture.For<MySaga>();
            
            fixture.Deliver(new TestMessage("hej"));

            var current = fixture.Data.OfType<MySagaState>().ToList();

            Assert.That(current.Count, Is.EqualTo(1));
            Assert.That(current[0].Text, Is.EqualTo("hej"));
        }

        [Test]
        public void EmitsCouldNotCorrelateEvent()
        {
            using (var fixture = SagaFixture.For<MySaga>())
            {
                var gotEvent = false;
                fixture.CouldNotCorrelate += () => gotEvent = true;

                fixture.Deliver(new TestMessage("hej"));

                Assert.That(gotEvent, Is.True);
            }
        }

        [Test]
        public void EmitsCreatedEvent()
        {
            using var fixture = SagaFixture.For<MySaga>();
            
            var gotEvent = false;
            fixture.Created += _ => gotEvent = true;

            fixture.Deliver(new TestMessage("hej"));

            Assert.That(gotEvent, Is.True);
        }

        [Test]
        public void EmitsUpdatedEvent()
        {
            using var fixture = SagaFixture.For<MySaga>();
            
            fixture.Deliver(new TestMessage("hej"));

            var gotEvent = false;
            fixture.Updated += _ => gotEvent = true;

            fixture.Deliver(new TestMessage("hej"));

            Assert.That(gotEvent, Is.True);
        }

        [Test]
        public void EmitsDeletedEvent()
        {
            using (var fixture = SagaFixture.For<MySaga>())
            {
                fixture.Deliver(new TestMessage("hej"));

                var gotEvent = false;
                fixture.Deleted += d => gotEvent = true;

                fixture.Deliver(new TestMessage("hej") { Die = true });

                Assert.That(gotEvent, Is.True);
            }
        }

        [Test]
        public void DoesNotTimeOutWhenDebuggerIsAttached()
        {

        }

        ///<summary>
        /// SagaData with non-empty Id is added to SagaFixture.Data.
        ///</summary>
        [Test]
        public void SagaDataWithNonEmptyIdIsAddedToSagaFixtureData()
        {
            using (var fixture = SagaFixture.For<MySaga>())
            {
                // Arrange

                // Act
                fixture.Add(new MySagaState { Id = Guid.NewGuid() });

                // Assert
                Assert.That(fixture.Data.Count(), Is.EqualTo(1));
            }
        }

        ///<summary>
        /// Verify that Id is set upon null Id.
        ///</summary>
        [Test]
        public void IdIsSetUponNullId()
        {
            using (var fixture = SagaFixture.For<MySaga>())
            {
                // Arrange

                // Act
                fixture.Add(new MySagaState());

                // Asert
                Assert.That(fixture.Data.Count(), Is.EqualTo(1));
                Assert.That(fixture.Data.Single().Id, Is.Not.Null);
            }
        }

        [Test]
        public void CanGetCaughtException()
        {
            using (var fixture = SagaFixture.For<MySaga>())
            {
                fixture.Deliver(new FailingTestMessage("whoohoo"));

                var handlerExceptions = fixture.HandlerExceptions.ToList();

                Console.WriteLine(string.Join(Environment.NewLine + Environment.NewLine, handlerExceptions));

                Assert.That(handlerExceptions.Count, Is.EqualTo(1));

                var exception = handlerExceptions.Single();

                Assert.That(exception.Exception, Is.TypeOf<RebusApplicationException>());
                Assert.That(exception.Exception.ToString(), Contains.Substring("oh no something bad happened"));
            }
        }

        class MySaga : Saga<MySagaState>, IAmInitiatedBy<TestMessage>, IAmInitiatedBy<FailingTestMessage>
        {
            protected override void CorrelateMessages(ICorrelationConfig<MySagaState> config)
            {
                config.Correlate<TestMessage>(m => m.Text, d => d.Text);
                config.Correlate<FailingTestMessage>(m => m.Text, d => d.Text);
            }

            public async Task Handle(TestMessage message)
            {
                Data.Text = message.Text;

                if (message.Die) MarkAsComplete();
            }

            public Task Handle(FailingTestMessage message)
            {
                throw new RebusApplicationException("oh no something bad happened");
            }
        }

        class TestMessage
        {
            public TestMessage(string text)
            {
                Text = text;
            }

            public string Text { get; }
            public bool Die { get; set; }
        }

        class FailingTestMessage
        {
            public FailingTestMessage(string text)
            {
                Text = text;
            }

            public string Text { get; }
        }

        class MySagaState : ISagaData
        {
            public Guid Id { get; set; }
            public int Revision { get; set; }
            public string Text { get; set; }
        }
    }
}