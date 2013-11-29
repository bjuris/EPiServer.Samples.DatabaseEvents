using System;
namespace EPiServer.Samples.DatabaseEvents
{
    /// <summary>
    /// Interface for database monitor
    /// </summary>
    public interface IDbEventMonitor
    {
        void Initialize();
        System.Threading.Timer Timer { get; }
    }
}
