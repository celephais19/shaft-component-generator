using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace InventorShaftGenerator.Infrastructure.Helpers
{
    public static class JsonSerializer
    {
        public static string Serialize<T>(T instance) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, instance);
                return Encoding.Default.GetString(stream.ToArray());
            }
        }

        public static T Deserialize<T>(string json) where T : class
        {
            return Deserialize<T>(Encoding.Default.GetBytes(json));
        }

        public static T Deserialize<T>(byte[] jsonBytes) where T : class 
        {
            using (var stream = new MemoryStream(jsonBytes))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return serializer.ReadObject(stream) as T;
            }
        }
    }
}
