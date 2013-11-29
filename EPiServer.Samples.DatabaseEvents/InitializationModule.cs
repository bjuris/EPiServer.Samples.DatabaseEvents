using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Reflection;
using log4net;
using StructureMap;
using StructureMap.Attributes;
using System.Configuration;
using EPiServer.Framework;
using EPiServer.ServiceLocation;
using EPiServer.Events;

namespace EPiServer.Samples.DatabaseEvents
{
    /// <summary>
    /// Init module that sets up dependencies
    /// </summary>
    [ModuleDependency(typeof(EPiServer.Events.EventsInitialization))]
    public class InitializationModule : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            if (context == null)
            {
                return;
            }
            context.Container.Configure(c =>
                {
                    var serializerFactory = new Func<IContext, XmlObjectSerializer>((ctx) => new DataContractBinarySerializer(typeof(EventMessage), context.Container.GetInstance<EventsServiceKnownTypesLookup>().KnownTypes));
                    c.For<IDbClient>().Use<DbClient>().Ctor<XmlObjectSerializer>().Is(serializerFactory);
                    c.For<IDbEventProvider>().Singleton().Use(()=>DbEventProvider.Instance);
                });
        }

        public void Initialize(Framework.Initialization.InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }

        public void Uninitialize(Framework.Initialization.InitializationEngine context)
        {   
        }
    }
}
