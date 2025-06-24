using System.IO;
using System.Text;
using System.Xml.Serialization;

public class Xml
{
	public static string Serialize(object obj)
	{
		StringBuilder stringBuilder = new StringBuilder();
		XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
		using (TextWriter textWriter = new StringWriter(stringBuilder))
		{
			xmlSerializer.Serialize(textWriter, obj);
		}
		return stringBuilder.ToString();
	}

	public static T Deserialize<T>(string xml)
	{
		T val = default(T);
		XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
		using (TextReader textReader = new StringReader(xml))
		{
			return (T)xmlSerializer.Deserialize(textReader);
		}
	}
}
