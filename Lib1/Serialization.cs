using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lib1
{

	public class Serialization<T>
	{
		public Serialization()
        {

        }

		public void SerializeToXML(T Obj, String path)
		{
			Type type = typeof(T);
			XmlSerializer serializer = new XmlSerializer(type);
			TextWriter textWriter = new StreamWriter(path);
			XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
			ns.Add("", "");
			serializer.Serialize(textWriter, Obj, ns);
			textWriter.Close();
		}

		public T DeserializeFromXML(String xmlPath)
		{
			Object valueObj = null;
			Type type = typeof(T);
			XmlSerializer deserializer = new XmlSerializer(type);
			TextReader textReader = new StreamReader(xmlPath);
			if (typeof(T) == typeof(Tester))
			{
				Tester systemConfiguration;
				systemConfiguration = (Tester)deserializer.Deserialize(textReader);
				valueObj = systemConfiguration;
			}
			
			textReader.Close();

			return (T)valueObj;
		}
	}
}
