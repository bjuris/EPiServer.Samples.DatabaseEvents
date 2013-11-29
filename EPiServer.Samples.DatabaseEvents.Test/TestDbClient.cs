using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPiServer.Data.Providers;

namespace EPiServer.Events.Database.Test
{
    [TestClass]
    public class TestDbClient
    {
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            GetDbClient().TruncateTable();
        }

        [TestMethod]
        public void WhenReadLatestEvenId_AndDatabaseIsEmpty_ShouldReturnZero()
        {
            DbClient db = GetDbClient();
            Assert.AreEqual(0, db.ReadLatestEventId());
        }

        [TestMethod]
        public void WhenReadEvents_AndDatabaseIsEmpty_ShouldReturnZeroRows()
        {
            DbClient db = GetDbClient();
            Assert.AreEqual(0, db.ReadEvents().Count);
        }

        [TestMethod]
        public void WhenReadEvents_AndSingleEventIsStored_ShouldReturnEvent()
        {
            DbClient dbWriter = GetDbClient();
            DbClient dbReader = GetDbClient();

            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 } };

            dbWriter.StoreEvent(ev);

            var events = dbReader.ReadEvents();
            Assert.AreEqual(1, events.Count);

            Assert.AreEqual(events[0].EventId, ev.EventId);
            Assert.AreEqual(events[0].RaiserId, ev.RaiserId);
            Assert.AreEqual(events[0].SequenceNumber, ev.SequenceNumber);
            Assert.AreEqual(events[0].SiteId, ev.SiteId);
        }

        private static DbClient GetDbClient()
        {
            return new DbClient(() => new SqlDatabaseHandler(System.Configuration.ConfigurationManager.ConnectionStrings["TestDB"], 0, TimeSpan.Zero, TimeSpan.FromSeconds(30)), new DataContractBinarySerializer(typeof(EventMessage), Enumerable.Empty<Type>()));
        }

        [TestMethod]
        public void WhenReadEvents_AndEventHasCustomParam_ShouldReturnEventWithParam()
        {
            DbClient dbWriter = GetDbClient();
            DbClient dbReader = GetDbClient();
            
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 }, Parameter = Guid.NewGuid() };

            dbWriter.StoreEvent(ev);

            var events = dbReader.ReadEvents();
            Assert.AreEqual(1, events.Count);

            Assert.AreEqual(events[0].EventId, ev.EventId);
            Assert.AreEqual(events[0].RaiserId, ev.RaiserId);
            Assert.AreEqual(events[0].SequenceNumber, ev.SequenceNumber);
            Assert.AreEqual(events[0].SiteId, ev.SiteId);
            Assert.AreEqual(events[0].ApplicationName, ev.ApplicationName);
            Assert.AreEqual(events[0].ServerName, ev.ServerName);
        }

        [TestMethod]
        public void WhenReadEvents_AndAlreadyReadEvents_ShouldNotReturnAnyData()
        {
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 }, Parameter = Guid.NewGuid() };

            DbClient dbWriter = GetDbClient();
            DbClient dbReader = GetDbClient();
            dbWriter.StoreEvent(ev);

            var events = dbReader.ReadEvents();
            Assert.AreEqual(1, events.Count);

            events = dbReader.ReadEvents();
            Assert.AreEqual(0, events.Count);
        }

        [TestMethod]
        public void WhenStoreEvents_AndReadFromSameSource_ShouldNotReturnAnyData()
        {
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 }, Parameter = Guid.NewGuid() };

            DbClient db = GetDbClient();

            db.StoreEvent(ev);
            var events = db.ReadEvents();
            Assert.AreEqual(0, events.Count);
        }
    }
}
