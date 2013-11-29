using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using StructureMap;
using StructureMap.AutoMocking;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;
using Rhino.Mocks.Expectations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
 

namespace EPiServer.Events.Database.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class TestDbEventMonitor : TestBase
    {
        public TestDbEventMonitor()
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
             base.TurnOnLogging();
         }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
         public void WhenUpdateEventsFromDb_AndLastIdIsNotInitialized_ThenItShouldCallReadLatestEventId()
        {
            RhinoAutoMocker<DbEventMonitor> autoMocker = new RhinoAutoMocker<DbEventMonitor>(MockMode.AAA);
            autoMocker.Get<IDbClient>().Stub(db => db.ReadEvents(0)).Return(new List<ReceivedEventMessage>());

            autoMocker.ClassUnderTest.UpdateEventsFromDb();
            
            autoMocker.Get<IDbClient>().AssertWasCalled(db => db.ReadLatestEventId());
        }

        [TestMethod]
        public void WhenUpdateEventsFromDb_AndLastIdIsInitialized_ThenItShouldNotCallReadLatestEventId()
        {
            RhinoAutoMocker<DbEventMonitor> autoMocker = new RhinoAutoMocker<DbEventMonitor>(MockMode.AAA);
            autoMocker.Get<IDbClient>().Stub(db => db.ReadEvents(0)).Return(new List<ReceivedEventMessage>());
            autoMocker.ClassUnderTest.LastForwardedEvent = 0;

            autoMocker.ClassUnderTest.UpdateEventsFromDb();
            
            autoMocker.Get<IDbClient>().AssertWasNotCalled(db => db.ReadLatestEventId());
        }


        [TestMethod]
        public void WorkerThreadCallback_WhenDbThrowsExceptions_ShouldEatAndLogIt()
        {
            RhinoAutoMocker<DbEventMonitor> autoMocker = new RhinoAutoMocker<DbEventMonitor>(MockMode.AAA);
            autoMocker.Get<IDbClient>().Stub(db => db.ReadLatestEventId()).Throw(new Exception("MockedError"));
            
            autoMocker.ClassUnderTest.WorkerThreadCallback(null);
            
            autoMocker.Get<IDbClient>().AssertWasCalled<IDbClient>(db => db.ReadLatestEventId());
        }

        [TestMethod]
        public void UpdateEventsFromDb_WhenNewEventsArrive_ShouldForwardToProvider()
        {
            var ev = new ReceivedEventMessage() { Id = 4, Message = new EventMessage() { EventId = Guid.NewGuid(), Parameter = Guid.NewGuid(), RaiserId = Guid.NewGuid(), SequenceNumber = 5, SiteId = "site" } };

            RhinoAutoMocker<DbEventMonitor> autoMocker = new RhinoAutoMocker<DbEventMonitor>(MockMode.AAA);
            autoMocker.Get<IDbClient>().Stub(db => db.ReadEvents(0)).Return(new List<ReceivedEventMessage>(new ReceivedEventMessage[] { ev }));
            
            autoMocker.ClassUnderTest.UpdateEventsFromDb();
            
            autoMocker.Get<IDbEventProvider>().AssertWasCalled(e => e.ForwardReceivedMessage(ev.Message));
        }

        public void Test_Timer()
        {
            RhinoAutoMocker<DbEventMonitor> autoMocker = new RhinoAutoMocker<DbEventMonitor>(MockMode.AAA);
            using(autoMocker.ClassUnderTest)
            {
                Assert.IsNull(autoMocker.ClassUnderTest.Timer);
                autoMocker.ClassUnderTest.Initialize();
                Assert.IsNotNull(autoMocker.ClassUnderTest.Timer);

                try
                {
                    autoMocker.ClassUnderTest.Initialize();
                    Assert.Fail("You should not be able to initialize the timer twice");
                }
                catch (InvalidOperationException)
                {

                }
            }
        }
    }
}
