using Newtonsoft.Json;
using System;
using UnityEngine;

public class ObjectNameConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        ScriptableObject objectValue = (ScriptableObject)value;
        writer.WriteStartObject();
        writer.WritePropertyName("name");
        writer.WriteValue(objectValue.name);
        writer.WriteEndObject();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return serializer.Deserialize<ScriptableObject>(reader);
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ScriptableObject);
    }
}