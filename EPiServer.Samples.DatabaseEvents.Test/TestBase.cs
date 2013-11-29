using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Repository;
using log4net.Repository.Hierarchy;
using log4net.Core;
using log4net.Appender;

namespace EPiServer.Events.Database.Test
{
    /// <summary>
    /// Summary description for TestBase
    /// </summary>
    public class TestBase
    {
        protected bool CompareBytes(byte[] b1, byte[] b2)
        {
            if (b1.Length != b2.Length)
            {
                return false;
            }
            for (int i = 0; i < b1.Length; i++)
            {
                if (b1[i] != b2[i])
                {
                    return false;
                }
            }
            return true;
        }

        protected void TurnOnLogging()
        {
            DebugAppender a = new log4net.Appender.DebugAppender();
            a.Threshold = Level.All;
            a.ActivateOptions();
            log4net.Config.BasicConfigurator.Configure(a);
        }

    }
}
