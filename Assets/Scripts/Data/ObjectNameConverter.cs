using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectNameConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value.GetType().IsGenericType)
        {
            var jArray = new JArray();
            IEnumerable<ScriptableObject> objectList = (IEnumerable<ScriptableObject>)value;
            foreach (var objectValue in objectList)
            {
                JObject obj = JObject.Parse("{ \"name\" : \"" + objectValue.name + "\" }");
                jArray.Add(obj);
            }
            jArray.WriteTo(writer);
        }
        else
        {
            ScriptableObject objectValue = (ScriptableObject)value;
            JObject obj = JObject.Parse("{ \"name\" : \"" + objectValue.name + "\" }");
            obj.WriteTo(writer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(ScriptableObject) || objectType == typeof(List<ScriptableObject>);
    }
}