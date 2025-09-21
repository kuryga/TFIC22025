using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace Utilities
{
    public static class Serializer
    {
        public static string serialize(object obj)
        {
            if (obj == null) return "null";

            var type = obj.GetType();
            var settings = new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
                EmitTypeInformation = EmitTypeInformation.Never
            };

            var ser = new DataContractJsonSerializer(type, settings);

            using (var ms = new MemoryStream())
            {
                ser.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        public static void serializeAndSave(string path, object ob)
        {
            File.WriteAllText(path, serialize(ob), Encoding.UTF8);
        }
    }
}
