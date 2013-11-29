using System;
namespace EPiServer.Events.Database
{
    /// <summary>
    /// Interface for database monitor
    /// </summary>
    public interface IDbEventMonitor
    {
        void Initialize();
        int LastForwardedEvent { get; set; }
        System.Threading.Timer Timer { get; }
    }
}
