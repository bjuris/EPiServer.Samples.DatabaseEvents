using System;
namespace EPiServer.Events.Database
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
