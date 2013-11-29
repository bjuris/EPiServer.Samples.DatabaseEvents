using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.Events.Database
{
    /// <summary>
    /// Interface to the database client
    /// </summary>
    public interface IDbClient
    {
        /// <summary>
        /// Get the latest Id in the database
        /// </summary>
        /// <returns>Should return 0 if no Id is found in the database</returns>
        int ReadLatestEventId();

        /// <summary>
        /// Get all events after the specified Id
        /// </summary>
        /// <param name="readEventsAfterId"></param>
        /// <param name="lastReadId">The Id to check for</param>
        /// <returns>A list of events or an empty collection if no events was found</returns>
        IList<ReceivedEventMessage> ReadEvents(int readEventsAfterId, out int? lastReadId);

        /// <summary>
        /// Store an interface down to the database
        /// </summary>
        /// <param name="eventData">The data to store</param>
        void StoreEvent(EventMessage message);
    }
}
