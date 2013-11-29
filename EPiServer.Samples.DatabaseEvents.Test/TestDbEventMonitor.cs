using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using EPiServer.Events;

namespace EPiServer.Samples.DatabaseEvents.Test
{
    [TestClass]
    public class TestDbEventMonitor
    {
        [TestMethod]
        public void WorkerThreadCallback_WhenDbThrowsExceptions_ShouldEatAndLogIt()
        {
            var db = new Mock<IDbClient>();
            db.Setup(d=>d.ReadEvents()).Throws(new Exception("MockedError"));

            new DbEventMonitor(null, db.Object).WorkerThreadCallback(null);

            db.Verify(d => d.ReadEvents(), Times.Once());
        }

        [TestMethod]
        public void UpdateEventsFromDb_WhenNewEventsArrive_ShouldForwardToProvider()
        {
            var db = new Mock<IDbClient>();
            var prov = new Mock<IDbEventProvider>();

            db.Setup(d => d.ReadEvents()).Returns(new List<EventMessage>(new EventMessage[]{new EventMessage() { EventId = Guid.NewGuid(), Parameter = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site" } }));

            new DbEventMonitor(prov.Object, db.Object).UpdateEventsFromDb();

            prov.Verify(p => p.ForwardReceivedMessage(It.Is<EventMessage>(m=>m.SiteId=="site")), Times.Once());
        }
    }
}
