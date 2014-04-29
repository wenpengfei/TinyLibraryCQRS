/* Command-Query Responsibility Segregation (CQRS) Architecture Demonstration
 * Built based on Apworks framework (http://apworks.codeplex.com)
 * Copyright (C) 2009-2011, apworks.codeplex.com.
 * Author: daxnet (Sunny Chen, daxnet@live.com)
 * */

using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Xml;

namespace TinyLibraryCQRS.Services.SynchronizationService
{
    public class DomainEventMessageContent
    {
        private Message message;
        private string type = null;
        private DateTime? timestamp = null;
        private DateTime sentTime = DateTime.MinValue;
        private string xml = null;
        private byte[] bytes = null;
        private string messageId;
        private bool isValidMessage;

        public DomainEventMessageContent(Message message)
        {
            this.message = message;
            long len = message.BodyStream.Length;
            bytes = new byte[len];
            message.BodyStream.Read(bytes, 0, (int)len);
            this.xml = Encoding.ASCII.GetString(bytes);
            this.messageId = message.Id;
            this.sentTime = message.SentTime;

            try
            {
                using (MemoryStream readerStream = new MemoryStream(bytes))
                {
                    using (XmlTextReader reader = new XmlTextReader(readerStream))
                    {
                        while (reader.Read())
                        {
                            switch (reader.NodeType)
                            {
                                case XmlNodeType.Element:
                                    if (reader.Name == "AssemblyQualifiedTypeName")
                                        this.type = reader.ReadElementContentAsString();
                                    if (reader.Name == "Timestamp")
                                        this.timestamp = Convert.ToDateTime(reader.ReadElementContentAsDateTime());
                                    break;
                            }
                        }
                        reader.Close();
                    }
                    readerStream.Close();
                }
                if (!string.IsNullOrEmpty(this.type) && this.timestamp != null)
                    this.isValidMessage = true;
                else
                    this.isValidMessage = false;
            }
            catch
            {
                this.type = null;
                this.timestamp = null;
                this.isValidMessage = false;
            }
        }

        public string Type
        {
            get { return this.type; }
        }

        public string Xml
        {
            get { return this.xml; }
        }

        public DateTime? Timestamp
        {
            get { return this.timestamp; }
        }

        public byte[] Bytes
        {
            get { return bytes; }
        }

        public string MessageId
        {
            get { return this.messageId; }
        }

        public override string ToString()
        {
            return this.messageId;
        }

        public bool IsValidMessage
        {
            get { return this.isValidMessage; }
        }

        public DateTime SentTime
        {
            get { return this.sentTime; }
        }
    }
}
