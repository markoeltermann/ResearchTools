using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SpectrumLibrary.Spectrum
{
    public class SpectrumMetadata
    {

        public SpectrumMetadata()
        {
            FittingRegionA1 = 510;
            FittingRegionA2 = 550;
            FittingRegionB1 = 710;
            FittingRegionB2 = 750;
            ReRegionStart = 565;
            ReRegionEnd = 675;
            FileExtension = "txt";
            FittingOrder = 1;
            IsBaselineSubtractionEnabled = true;
        }

        public static SpectrumMetadata FullRegions
        {
            get
            {
                SpectrumMetadata sr = new SpectrumMetadata();
                sr.FittingRegionA1 = double.MinValue;
                sr.FittingRegionA2 = double.MaxValue;
                sr.FittingRegionB1 = double.MinValue;
                sr.FittingRegionB2 = double.MaxValue;
                return sr;
            }
        }

        [JsonProperty("a1")]
        public double FittingRegionA1 { get; set; }

        [JsonProperty("a2")]
        public double FittingRegionA2 { get; set; }

        [JsonProperty("b1")]
        public double FittingRegionB1 { get; set; }

        [JsonProperty("b2")]
        public double FittingRegionB2 { get; set; }

        [JsonProperty("r1")]
        public double ReRegionStart { get; set; }

        [JsonProperty("r2")]
        public double ReRegionEnd { get; set; }

        [JsonProperty("ext")]
        public string FileExtension { get; set; }

        [JsonProperty("order")]
        public int FittingOrder { get; set; }

        [JsonProperty("d1")]
        public double DefectRegionStart { get; set; }

        [JsonProperty("d2")]
        public double DefectRegionEnd { get; set; }

        [JsonProperty("fixcomma")]
        public bool UseCommaFix { get; set; }

        [JsonProperty("isbaselineenabled")]
        public bool IsBaselineSubtractionEnabled { get; set; }

        public bool IsNoiseFilteringEnabled { get; set; }

        public static SpectrumMetadata ReadFromFile(string filePath)
        {
            SpectrumMetadata sr = null;

            try
            {
                try
                {
                    var fileText = File.ReadAllText(filePath);
                    sr = JsonConvert.DeserializeObject<SpectrumMetadata>(fileText);
                    return sr;
                }
                catch (Exception) { }

                sr = new SpectrumMetadata();
                var lines = File.ReadAllLines(filePath);

                foreach (var line in lines)
                {
                    if (line.StartsWith("a1", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.FittingRegionA1 = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("a2", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.FittingRegionA2 = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("b1", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.FittingRegionB1 = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("b2", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.FittingRegionB2 = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("r1", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.ReRegionStart = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("r2", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.ReRegionEnd = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("ext", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                            sr.FileExtension = parts[1];
                    }
                    else if (line.StartsWith("order", StringComparison.OrdinalIgnoreCase))
                    {
                        var parts = line.Split('=');
                        if (parts.Length == 2)
                        {
                            int order;
                            if (int.TryParse(parts[1], out order))
                                sr.FittingOrder = order;
                        }
                    }
                    else if (line.StartsWith("d1", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.DefectRegionStart = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("d2", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.DefectRegionEnd = TryParseLineValue(line);
                    }
                    else if (line.StartsWith("fixcomma", StringComparison.OrdinalIgnoreCase))
                    {
                        sr.UseCommaFix = TryParseLineValueBool(line);
                    }
                }

            }
            catch (IOException)
            {
            }
            if (sr == null)
                return new SpectrumMetadata();
            else
                return sr;
        }

        public void WriteToFile(string filePath)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(filePath, json);
            }
            catch (Exception) { }
        }

        private static double TryParseLineValue(string line)
        {
            var parts = line.Split('=');
            if (parts.Length == 2)
            {
                double result = Utilities.TryParseDouble(parts[1]).GetValueOrDefault();
                return result;
            }
            return 0;
        }

        private static bool TryParseLineValueBool(string line)
        {
            var parts = line.Split('=');
            if (parts.Length == 2)
            {
                bool result = false;
                bool.TryParse(parts[1], out result);
                return result;
            }
            return false;
        }

    }
}
