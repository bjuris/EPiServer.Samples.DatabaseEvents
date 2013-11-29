using System;
namespace EPiServer.Samples.DatabaseEvents
{
    public interface IDbEventProvider
    {
        void ForwardReceivedMessage(EPiServer.Events.EventMessage message);
    }
}
