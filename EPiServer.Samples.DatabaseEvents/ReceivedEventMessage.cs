using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EPiServer.Events.Database
{
    /// <summary>
    /// Event received from db
    /// </summary>
    public class ReceivedEventMessage
    {
        public int Id { get; set; }
        public EventMessage Message { get; set; }
    }
}
