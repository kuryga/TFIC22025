using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Utilities
{
    public static class Serializer
    {
        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        public static string serialize(object obj)
            => JsonSerializer.Serialize(obj, Options);

        public static void serializeAndSave(string path, object ob)
            => File.WriteAllText(path, serialize(ob));
    }
}