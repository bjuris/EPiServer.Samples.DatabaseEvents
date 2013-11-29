using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using EPiServer.Data.Providers;

namespace EPiServer.Events.Database.Test
{
    /// <summary>
    /// Summary description for TestSqlDbClient
    /// </summary>
    [TestClass]
    public class TestDbClient : TestBase
    {
        public TestDbClient()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize() 
        {
            TurnOnLogging();
            GetDbClient().TruncateTable();
        }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Empty_table_should_return_zero()
        {
            DbClient db = GetDbClient();
            Assert.AreEqual(0, db.ReadLatestEventId());
        }

        [TestMethod]
        public void Empty_table_should_return_no_rows()
        {
            DbClient db = GetDbClient();
            Assert.AreEqual(0, db.ReadEvents(0).Count);
        }

        [TestMethod]
        public void Should_Store_and_load_Simple_event()
        {
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 } };

            DbClient db = GetDbClient();
            db.StoreEvent(ev);

            var events = db.ReadEvents(0);
            Assert.AreEqual(1, events.Count);

            Assert.AreEqual(events[0].Message.EventId, ev.EventId);
            Assert.AreEqual(events[0].Message.RaiserId, ev.RaiserId);
            Assert.AreEqual(events[0].Message.SequenceNumber, ev.SequenceNumber);
            Assert.AreEqual(events[0].Message.SiteId, ev.SiteId);
            Assert.IsTrue(CompareBytes(events[0].Message.VerificationData, ev.VerificationData));
        }

        private static DbClient GetDbClient()
        {
            return new DbClient(() => new SqlDatabaseHandler(System.Configuration.ConfigurationManager.ConnectionStrings["TestDB"], 0, TimeSpan.Zero, TimeSpan.FromSeconds(30)), new DataContractBinarySerializer(typeof(EventMessage), Enumerable.Empty<Type>()));
        }

        [TestMethod]
        public void Should_Store_and_load_Event_with_Params()
        {
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 }, Parameter = Guid.NewGuid() };

            DbClient db = GetDbClient();
            db.StoreEvent(ev);

            var events = db.ReadEvents(0);
            Assert.AreEqual(1, events.Count);

            Assert.AreEqual(events[0].Message.EventId, ev.EventId);
            Assert.AreEqual(events[0].Message.RaiserId, ev.RaiserId);
            Assert.AreEqual(events[0].Message.SequenceNumber, ev.SequenceNumber);
            Assert.AreEqual(events[0].Message.SiteId, ev.SiteId);
            Assert.AreEqual(events[0].Message.ApplicationName, ev.ApplicationName);
            Assert.AreEqual(events[0].Message.ServerName, ev.ServerName);
        }

        [TestMethod]
        public void Should_Only_Load_Events_After_LatestId()
        {
            EventMessage ev = new EventMessage() { EventId = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site", VerificationData = new byte[] { 1, 2, 3, 4, 5, 6 }, Parameter = Guid.NewGuid() };

            DbClient db = GetDbClient();
            db.StoreEvent(ev);

            var events = db.ReadEvents(0);
            Assert.AreEqual(1, events.Count);

            events = db.ReadEvents(1);
            Assert.AreEqual(0, events.Count);

        }
    }
}
