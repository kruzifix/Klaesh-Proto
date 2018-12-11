using System;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Klaesh.Utility
{
    public interface IJsonConverter
    {
        T DeserializeObject<T>(string text);
        string SerializeObject(object o);
        string SerializeObject(object o, Formatting f);
    }

    public class CustomJsonConverter : IJsonConverter
    {
        private JsonConverter[] _converter;

        public CustomJsonConverter()
        {
            _converter = new JsonConverter[]
            {
                new ColorConverter(),
                new AnnoyingStringEnumConverter { AllowIntegerValues = false, CamelCaseText = false }
            };
        }

        public T DeserializeObject<T>(string text)
        {
            return JsonConvert.DeserializeObject<T>(text, _converter);
        }

        public string SerializeObject(object o)
        {
            return SerializeObject(o, Formatting.None);
        }

        public string SerializeObject(object o, Formatting f)
        {
            return JsonConvert.SerializeObject(o, f, _converter);
        }
    }

    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        /// <summary>
        /// Create an instance of objectType, based properties in the JSON object
        /// </summary>
        /// <param name="objectType">type of object expected</param>
        /// <param name="jObject">
        /// contents of JSON object that will be deserialized
        /// </param>
        /// <returns></returns>
        protected abstract T Create(Type objectType, JObject jObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader,
                                        Type objectType,
                                         object existingValue,
                                         JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                return null;
            }

            // Load JObject from stream
            JObject jObject = JObject.Load(reader);

            // Create target object based on JObject
            T target = Create(objectType, jObject);

            // Populate the object properties
            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}
