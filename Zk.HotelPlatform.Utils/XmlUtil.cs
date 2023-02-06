using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Zk.HotelPlatform.Utils
{
    public class XmlUtil
    {
        public static T XmlDeserialize<T>(string s, Encoding encoding = null)
        {
            T local;
            if (string.IsNullOrEmpty(s))
            {
                throw new ArgumentNullException("s");
            }
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (MemoryStream stream = new MemoryStream(encoding.GetBytes(s)))
            {
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    local = (T)serializer.Deserialize(reader);
                }
            }
            return local;
        }

        public static string XmlSerialize(object o, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            string str;
            using (MemoryStream stream = new MemoryStream())
            {
                XmlSerializeInternal(stream, o, encoding);
                stream.Position = 0L;
                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    str = reader.ReadToEnd();
                }
            }
            return str;
        }

        private static void XmlSerializeInternal(Stream stream, object o, Encoding encoding)
        {
            if (o == null)
            {
                throw new ArgumentNullException("o");
            }
            if (encoding == null)
            {
                throw new ArgumentNullException("encoding");
            }
            XmlSerializer serializer = new XmlSerializer(o.GetType());
            XmlWriterSettings settings = new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n",
                Encoding = encoding,
                IndentChars = "    "
            };
            using (XmlWriter writer = XmlWriter.Create(stream, settings))
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("", "");
                serializer.Serialize(writer, o, namespaces);
                writer.Close();
            }
        }
    }
}
