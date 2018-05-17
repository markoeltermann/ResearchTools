using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace GasProgramGenerator
{
    public class JsonScriptLine
    {

        public static byte[] Serialize(IList<JsonScriptLine> lines)
        {
            return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(lines, Formatting.Indented));
        }

        public static IList<JsonScriptLine> Deserialize(byte[] bytes)
        {
            return JsonConvert.DeserializeObject<IList<JsonScriptLine>>(Encoding.UTF8.GetString(bytes));
        }

        public double? Sp1 { get; set; }
        public double? Sp2 { get; set; }
        public double? Sp3 { get; set; }
        public TimeSpan Time { get; set; }

    }
}
