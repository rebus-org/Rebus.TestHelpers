using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Rebus.DataBus;
using Rebus.DataBus.InMem;
using Rebus.Testing;
using Rebus.Time;
using InMemDataBusStorage = Rebus.TestHelpers.Internals.InMemDataBusStorage;

namespace Rebus.TestHelpers
{
    /// <summary>
    /// Test helper that can be used to fake the presence of a configured data bus, using the given in-mem data store to store data
    /// </summary>
    public class FakeDataBus : IDataBus
    {
        readonly IDataBusStorage _dataBusStorage;
        readonly InMemDataStore _inMemDataStore;

        /// <summary>
        /// Establishes a fake presence of a configured data bus, using the given <see cref="InMemDataStore"/> to retrieve data
        /// </summary>
        public static IDisposable EstablishContext(InMemDataStore dataStore, IRebusTime rebusTime)
        {
            if (dataStore == null) throw new ArgumentNullException(nameof(dataStore));

            TestBackdoor.EnableTestMode(new InMemDataBusStorage(dataStore, rebusTime));

            return new CleanUp(TestBackdoor.Reset);
        }

        /// <summary>
        /// Creates the fake data bus, optionally using the given in-mem data store to store attachments
        /// </summary>
        public FakeDataBus(IRebusTime rebusTime)
        {
            // if there is an "ambient" storage, use that
            if (TestBackdoor.TestDataBusStorage != null)
            {
                _dataBusStorage = TestBackdoor.TestDataBusStorage;
            }
            // last resort: just fake it in mem
            else
            {
                _inMemDataStore = new InMemDataStore();
                _dataBusStorage = new InMemDataBusStorage(_inMemDataStore, rebusTime);
            }
        }

        /// <inheritdoc />
        public async Task<DataBusAttachment> CreateAttachment(Stream source, Dictionary<string, string> optionalMetadata = null)
        {
            var id = Guid.NewGuid().ToString();

            await _dataBusStorage.Save(id, source, optionalMetadata).ConfigureAwait(false);

            return new DataBusAttachment(id);
        }

        /// <inheritdoc />
        public async Task<Stream> OpenRead(string dataBusAttachmentId)
        {
            return await _dataBusStorage.Read(dataBusAttachmentId).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<Dictionary<string, string>> GetMetadata(string dataBusAttachmentId)
        {
            return await _dataBusStorage.ReadMetadata(dataBusAttachmentId).ConfigureAwait(false);
        }

        class CleanUp : IDisposable
        {
            readonly Action _disposeAction;

            public CleanUp(Action disposeAction)
            {
                _disposeAction = disposeAction;
            }

            public void Dispose()
            {
                _disposeAction();
            }
        }

        internal InMemDataStore GetDataBusDataStore() => _inMemDataStore;

        public Task Delete(string dataBusAttachmentId)
        {
            return Task.FromResult(0);
        }

        public IEnumerable<string> Query(TimeRange readTime = null, TimeRange saveTime = null)
        {
            return new string[0];
        }
    }
}