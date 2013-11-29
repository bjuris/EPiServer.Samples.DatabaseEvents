using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.Events.Database.Test
{
    class MockDbClient : IDbClient
    {
        #region IDbClient Members

        public IList<ReceivedEventMessage> Events { get; set; }
        public int LatestEventId { get; set; }

        public bool LatestEventIdCalled { get; set; }
        public bool ReadEventsCalled { get; set; }
        public bool StoreEventCalled { get; set; }

        public EventMessage StoredEvent { get; set; }

        public bool ThrowOnGetLatestEventId { get; set; }

        public int ReadLatestEventId()
        {
            if (ThrowOnGetLatestEventId)
            {
                throw new System.Data.DataException("SimulatedDatabaseError");
            }
            LatestEventIdCalled = true;
            return LatestEventId;
        }

        public IList<ReceivedEventMessage> ReadEvents(int readEventsAfterId, out int? lastReadIdFromDb)
        {
            lastReadIdFromDb = Events==null || Events.Count==0 ? (int?)null : Events.OrderByDescending(e => e.Id).First().Id;
            ReadEventsCalled = true;
            return Events;
        }

        public void StoreEvent(EventMessage ev)
        {
            StoreEventCalled = true;
            StoredEvent = ev;
        }

        #endregion
    }
}
