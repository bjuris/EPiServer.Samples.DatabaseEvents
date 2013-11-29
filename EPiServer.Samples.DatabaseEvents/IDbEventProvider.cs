using System;
namespace EPiServer.Events.Database
{
    public interface IDbEventProvider
    {
        void ForwardReceivedMessage(EPiServer.Events.EventMessage message);
    }
}
