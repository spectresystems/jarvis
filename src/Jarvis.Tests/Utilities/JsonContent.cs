using System.IO;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace Jarvis.Tests.Utilities
{
    public class JsonContent : StringContent
    {
        private const string ApplicationJson = "application/json";

        public JsonContent(object content, JsonSerializer serializer = null)
            : base(Serialize(content, serializer), Encoding.UTF8, ApplicationJson) { }

        private static string Serialize(object content, JsonSerializer serializer)
        {
            if (serializer == null)
            {
                return JsonConvert.SerializeObject(content);
            }

            using (var stringWriter = new StringWriter())
            using (var jsonWriter = new JsonTextWriter(stringWriter))
            {
                serializer.Serialize(jsonWriter, content);
                stringWriter.Flush();
                return stringWriter.ToString();
            }
        }
    }
}
