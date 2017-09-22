using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DecayDataAnalyzer
{
    public class DataParameters
    {

        public bool InferXColumn { get; set; }

        public bool UseXYZ { get; set; }


        public static DataParameters ReadFromFile(string fileFullPath)
        {
            var lines = File.ReadAllLines(fileFullPath);
            DataParameters result = new DataParameters();
            bool isInParamsBlock = false;
            foreach (var line in lines)
            {
                if (line.Equals("[params]", StringComparison.OrdinalIgnoreCase))
                {
                    isInParamsBlock = true;
                }
                else if (isInParamsBlock)
                {
                    var linePieces = line.Split('=');
                    if (linePieces.Length == 2)
                    {
                        if (linePieces[0].Equals("InferXColumn", StringComparison.OrdinalIgnoreCase))
                        {
                            result.InferXColumn = bool.Parse(linePieces[1].Trim());
                        }
                        else if (linePieces[0].Equals(nameof(UseXYZ), StringComparison.OrdinalIgnoreCase))
                        {
                            result.UseXYZ = bool.Parse(linePieces[1].Trim());
                        }
                    }
                }
            }
            return result;
        }

    }
}
