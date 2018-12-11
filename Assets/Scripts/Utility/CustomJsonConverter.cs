using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
}
