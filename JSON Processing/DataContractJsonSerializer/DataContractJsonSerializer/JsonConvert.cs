using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace CSharpDataContractJsonSerializer
{
    public class JsonConvert
    {
        public static string SerializeObject<T>(T obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());

            using (MemoryStream stream = new MemoryStream())
            {
                serializer.WriteObject(stream, obj);
                string result = Encoding.UTF8.GetString(stream.ToArray());
                return result;
            }
        }

        public static T DeserializeObject<T>(string jsonString)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            byte[] buffer = Encoding.UTF8.GetBytes(jsonString);

            using (MemoryStream stream = new MemoryStream(buffer))
            {
                T result = (T)serializer.ReadObject(stream);
                return result;
            }
        }
    }
}
