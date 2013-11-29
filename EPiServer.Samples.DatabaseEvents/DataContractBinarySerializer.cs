using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml;

namespace EPiServer.Events.Database
{
    /// <summary>
    /// Replica of the DataContractBinarySerializer in the Microsoft.ServiceBus assembly
    /// that supports passing in a list of known types to the serializer.
    /// </summary>
    public sealed class DataContractBinarySerializer : XmlObjectSerializer
    {
        private readonly DataContractSerializer _dataContractSerializer;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public DataContractBinarySerializer(Type type)
            : this(type, null) { }

        public DataContractBinarySerializer(Type type, IEnumerable<Type> knownTypes)
        {
            _dataContractSerializer = new DataContractSerializer(type, knownTypes ?? Enumerable.Empty<Type>());
        }

        public override object ReadObject(Stream stream)
        {
            return ReadObject(XmlDictionaryReader.CreateBinaryReader(stream, XmlDictionaryReaderQuotas.Max));
        }

        public override void WriteObject(Stream stream, object graph)
        {
            var binaryWriter = XmlDictionaryWriter.CreateBinaryWriter(stream, null, null, false);
            WriteObject(binaryWriter, graph);
            binaryWriter.Flush();
        }

        public override void WriteObject(XmlDictionaryWriter writer, object graph)
        {
            _dataContractSerializer.WriteObject(writer, graph);
        }

        public override bool IsStartObject(XmlDictionaryReader reader)
        {
            return _dataContractSerializer.IsStartObject(reader);
        }

        public override object ReadObject(XmlDictionaryReader reader, bool verifyObjectName)
        {
            return _dataContractSerializer.ReadObject(reader, verifyObjectName);
        }

        public override void WriteEndObject(XmlDictionaryWriter writer)
        {
            _dataContractSerializer.WriteEndObject(writer);
        }

        public override void WriteObjectContent(XmlDictionaryWriter writer, object graph)
        {
            _dataContractSerializer.WriteObjectContent(writer, graph);
        }

        public override void WriteStartObject(XmlDictionaryWriter writer, object graph)
        {
            _dataContractSerializer.WriteStartObject(writer, graph);
        }
    }

}
