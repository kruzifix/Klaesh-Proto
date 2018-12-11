using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Assets.Scripts.Utility
{
    /// <summary>
    /// this only exists because the StringEnumConverter does not respect the CamelCaseText property...
    /// </summary>
    public class AnnoyingStringEnumConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                //Enum @enum = (Enum)value;
                //string enumText = @enum.ToString("G");
                //if (char.IsNumber(enumText[0]) || enumText[0] == '-')
                //{
                //    writer.WriteValue(value);
                //}
                //else
                //{
                //    string enumName = EnumUtils.ToEnumName(@enum.GetType(), enumText, CamelCaseText);
                //    writer.WriteValue(enumName);
                //}
                var text = value.ToString();
                if (!CamelCaseText)
                    text = text.ToLower();
                writer.WriteValue(text);
            }
        }
    }
}
