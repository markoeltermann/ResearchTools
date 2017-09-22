using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.XYData
{
    public class XYAsciiFileWriter
    {

        //public static void WriteDataToAsciiFile<T>(string outputPath, IList<T> data, Func<string, T> captionFunction, Func<)
        //{
        //    using (StreamWriter sw = new StreamWriter(outputPath + "\\SimplifiedData.txt", false))
        //    {
        //        int counter = 0;
        //        string lineToWrite = "slot nr\t";
        //        for (int i = 0; i < simplifiedDataSets.Count; i++)
        //        {
        //            if (i < simplifiedDataSets.Count - 1)
        //            {
        //                lineToWrite += simplifiedDataSets[i].SourceFileName + '\t';
        //            }
        //            else
        //            {
        //                lineToWrite += simplifiedDataSets[i].SourceFileName;
        //            }
        //        }
        //        sw.WriteLine(lineToWrite);

        //        while (counter < simplifiedDataSets[0].Points.Length)
        //        {
        //            lineToWrite = counter.ToString() + '\t';

        //            for (int i = 0; i < simplifiedDataSets.Count; i++)
        //            {
        //                if (i < simplifiedDataSets.Count - 1)
        //                {
        //                    lineToWrite += simplifiedDataSets[i].Points[counter].Y.ToString(NumberFormatToUse) + '\t';
        //                }
        //                else
        //                {
        //                    lineToWrite += simplifiedDataSets[i].Points[counter].Y.ToString(NumberFormatToUse);
        //                }
        //            }
        //            counter++;
        //            sw.WriteLine(lineToWrite);
        //        }
        //    }
        //}

    }
}
