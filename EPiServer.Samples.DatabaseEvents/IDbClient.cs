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
        /// Get all events after the specified Id
        /// </summary>
        /// <param name="readEventsAfterId"></param>
        /// <param name="lastReadId">The Id to check for</param>
        /// <returns>A list of events or an empty collection if no events was found</returns>
        IList<EventMessage> ReadEvents();

        /// <summary>
        /// Store an interface down to the database
        /// </summary>
        /// <param name="eventData">The data to store</param>
        void StoreEvent(EventMessage message);
    }
}
