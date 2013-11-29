﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using System.Runtime.Serialization;
using log4net;
using System.Globalization;
using EPiServer.Data;
using EPiServer.ServiceLocation;

namespace EPiServer.Events.Database
{
    /// <summary>
    /// Database client used for read and write to the database
    /// </summary>
    [ServiceConfiguration(typeof(IDbClient), Lifecycle=ServiceInstanceScope.Singleton)]
    public class DbClient : IDbClient
    {
        Func<IDatabaseHandler> _handler;
        XmlObjectSerializer _serializer;
        Guid _source;

        public DbClient(Func<IDatabaseHandler> handler, XmlObjectSerializer serializer)
        {
            _handler = handler;
            _serializer = serializer;
            _source = Guid.NewGuid();
        }

        /// <summary>
        /// Read the latest tblRemoteEvents.pkId from the database
        /// </summary>
        /// <returns></returns>
        public int ReadLatestEventId()
        {
            var handler = _handler();
            return handler.Execute(() =>
            {
                var cmd = handler.CreateCommand("SELECT TOP 1 Max(pkId) FROM tblDatabaseEvents", CommandType.Text);
                object latestIdInDbObj = cmd.ExecuteScalar();
                if (latestIdInDbObj != null && latestIdInDbObj!=DBNull.Value)
                {
                    return Convert.ToInt32(latestIdInDbObj, CultureInfo.InvariantCulture);
                }
                return 0;
            });
        }

        /// <summary>
        /// Read all events efter the specified Id
        /// </summary>
        /// <param name="readEventsAfterId"></param>
        /// <returns></returns>
        public IList<ReceivedEventMessage> ReadEvents(int readEventsAfterId, out int? lastReadId)
        {
            var list = new List<ReceivedEventMessage>();
            var handler = _handler();
            lastReadId = handler.Execute(() =>
            {
                var cmd = handler.CreateCommand("SELECT pkId, Source, SerializedMessage FROM tblDatabaseEvents WHERE pkId>@p0 ORDER BY pkId ASC", CommandType.Text, readEventsAfterId);
                int? lastReadIdFromDb = null;
                using (IDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lastReadIdFromDb = reader.GetInt32(0);
                        if (reader.GetGuid(1) == _source)
                        {
                            continue;
                        }
                        list.Add(new ReceivedEventMessage() 
                        { 
                            Id = reader.GetInt32(0), 
                            Message = (EventMessage)_serializer.ReadObject(new MemoryStream((byte[])reader[2])) 
                        });
                    }
                }
                return lastReadIdFromDb;
            });

            return list;;
        }

        /// <summary>
        /// Saves an event down to the tblDatabaseEvents
        /// </summary>
        /// <param name="message"></param>
        public void StoreEvent(EventMessage message)
        {
            var serializedMessage = new MemoryStream();
            _serializer.WriteObject(serializedMessage, message);
            serializedMessage.Seek(0, SeekOrigin.Begin);
            var handler = _handler();
            handler.Execute(() =>
            {
                var cmd = handler.CreateCommand("INSERT INTO tblDatabaseEvents(SerializedMessage) VALUES(@p0)", CommandType.Text, new BinaryReader(serializedMessage).ReadBytes((int)serializedMessage.Length));
                cmd.ExecuteNonQuery();
            });
        }

        /// <summary>
        /// Truncates tblDatabaseEvents, only used for testing
        /// </summary>
        public void TruncateTable()
        {
            var handler = _handler();
            handler.Execute(() =>
            {
                var cmd = handler.CreateCommand("TRUNCATE TABLE tblDatabaseEvents", CommandType.Text);
                cmd.ExecuteNonQuery();
            });
        }
    }
}
