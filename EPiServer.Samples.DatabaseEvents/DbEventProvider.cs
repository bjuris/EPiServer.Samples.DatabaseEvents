using EPiServer.Events;
using EPiServer.Events.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace EPiServer.Samples.DatabaseEvents
{
    /// <summary>
    /// Provider for EPiServer Events
    /// </summary>
    public class DbEventProvider : EventProvider, IDbEventProvider
    {
        private IDbClient _db;
        private IDbEventMonitor _eventMonitor;

        public DbEventProvider()
        {
            _db = ServiceLocation.ServiceLocator.Current.GetInstance<IDbClient>();
            _eventMonitor = ServiceLocation.ServiceLocator.Current.GetInstance<IDbEventMonitor>();
        }

        public override bool ValidateMessageIntegrity
        {
            get
            {
                return false;
            }
        }

        public override System.Threading.Tasks.Task InitializeAsync()
        {
            return Task.Factory.StartNew(() =>
                {
                    _eventMonitor.Initialize();
                });
        }

        public override void SendMessage(EventMessage message)
        {
            _db.StoreEvent(message);
        }

        public void ForwardReceivedMessage(EventMessage message)
        {
            base.OnMessageReceived(new EventMessageEventArgs(message));
        }
    }
}
