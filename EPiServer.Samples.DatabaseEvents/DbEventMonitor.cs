using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using EPiServer.Events.Remote;
using log4net;
using StructureMap;
using System.Globalization;
using EPiServer.ServiceLocation;

namespace EPiServer.Events.Database
{
    /// <summary>
    /// Used as a singleton instance that reads event information from the database
    /// </summary>
    [ServiceConfiguration(typeof(DbEventMonitor), Lifecycle=ServiceInstanceScope.Singleton)]
    public sealed class DbEventMonitor : IDisposable, IDbEventMonitor
    {
        private static ILog _log = LogManager.GetLogger(typeof(DbEventMonitor));
        private object _workerLocker = new object();
        private Timer _timer;
        private IDbEventProvider _provider;
        private IDbClient _dbClient;

        public DbEventMonitor(IDbEventProvider provider, IDbClient dbClient)
        {
            _provider = provider;
            _dbClient = dbClient;
        }

        /// <summary>
        /// The callback method for the worker thread as initialized by InitializeTimer
        /// </summary>
        /// <param name="state"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void WorkerThreadCallback(object state)
        {
            try
            {
                UpdateEventsFromDb();
            }
            catch (Exception e)
            {
                _log.Error("Cannot read events from the database", e);
            }
        }

        /// <summary>
        /// Update events from the database
        /// </summary>
        /// <returns>True if another thread is alread calling this code</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public bool UpdateEventsFromDb()
        {
            //Make sure we cant get here twice since we must handle the events in sequence
            if (!Monitor.TryEnter(_workerLocker))
            {
                return false;
            }

            try
            {

                if (_log.IsDebugEnabled)
                {
                    _log.Debug("Reading latest event information from the database");
                }

                var events = _dbClient.ReadEvents();

                foreach (var ev in events)
                {
                    if (_log.IsDebugEnabled)
                    {
                        _log.DebugFormat("Forwarding event {0} from the database to the site", ev.SequenceNumber);
                    }

                    try
                    {
                        _provider.ForwardReceivedMessage(ev);
                    }
                    catch (Exception e)
                    {
                        //We cannot throw here since its the responsibility of the event system to handle currupt or missing events
                        //and we do not want to get stuck in the event chain since _lastForwardedEvent will not continue to update
                        _log.Error(String.Format(CultureInfo.InvariantCulture, "Failed to forward event {0} from the database",ev.SequenceNumber),e);
                    }
                }

                if (_log.IsDebugEnabled)
                {
                    _log.DebugFormat("Processed {0} events from the database", events.Count);
                }
            }
            finally
            {
                Monitor.Exit(_workerLocker);
            }

            return true;
        }

        /// <summary>
        /// First time initialization of the timer that triggers databas reads
        /// </summary>
        /// <remarks>This class is built to be a singleton, calling this method twice will throw an exception</remarks>
        public void Initialize()
        {
            if (_timer != null)
            {
                throw new InvalidOperationException("Cannot initialize timer since it has already been initialized");
            }

            _timer = new Timer(new TimerCallback(WorkerThreadCallback), null, TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(20));
        }

        #region IDisposable Members

        /// <summary>
        /// Disposes the internal timer (if initialized)
        /// </summary>
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }
        }

        #endregion

        public Timer Timer { get { return _timer; } }
    }
}
