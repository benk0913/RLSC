using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorHexConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value.GetType().IsGenericType)
        {
            var jArray = new JArray();
            IEnumerable<Color> objectList = (IEnumerable<Color>)value;
            foreach (var objectValue in objectList)
            {
                JObject obj = JObject.Parse("{ \"name\" : \"#" + ColorUtility.ToHtmlStringRGB(objectValue) + "\" }");
                jArray.Add(obj);
            }
            jArray.WriteTo(writer);
        }
        else
        {
            Color objectValue = (Color)value;
            JObject obj = JObject.Parse("{ \"name\" : \"#" + ColorUtility.ToHtmlStringRGB(objectValue) + "\" }");
            obj.WriteTo(writer);
        }
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return null;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Color) || objectType == typeof(List<Color>);
    }
}