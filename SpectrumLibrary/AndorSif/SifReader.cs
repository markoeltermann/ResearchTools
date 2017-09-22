using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.AndorSif
{
    public class SifReader
    {

        public static List<XYPoint> ReadSignalFromSifFile(string filePath)
        {
            SifResult result = SifResult.None;

            result = (SifResult)SifMethods.ReadFromFile(filePath);
            ThrowOnError(result);

            var resultList = ReadSifFileContents();

            SifMethods.CloseFile();

            return resultList;
        }


        private static List<XYPoint> ReadSifFileContents()
        {
            SifResult result = SifResult.None;

            int isDataSourcePresent = 0;
            result = (SifResult)SifMethods.IsDataSourcePresent(SifDataSource.Signal, out isDataSourcePresent);
            ThrowOnError(result);

            if (isDataSourcePresent == 1)
            {
                uint frameCount;
                result = (SifResult)SifMethods.GetNumberFrames(SifDataSource.Signal, out frameCount);
                ThrowOnError(result);

                if (frameCount >= 1)
                {
                    uint pixelCount;
                    result = (SifResult)SifMethods.GetFrameSize(SifDataSource.Signal, out pixelCount);
                    ThrowOnError(result);

                    if (pixelCount > 0)
                    {
                        float[] pixelValues = new float[pixelCount];
                        result = (SifResult)SifMethods.GetFrame(SifDataSource.Signal, 0, pixelValues, pixelCount);
                        ThrowOnError(result);

                        List<XYPoint> resultList = new List<XYPoint>((int)pixelCount);

                        for (int i = 0; i < pixelCount; i++)
                        {
                            double xValue;
                            result = (SifResult)SifMethods.GetPixelCalibration(SifDataSource.Signal, SifCalibrationAxis.CalibX, i + 1, out xValue);
                            ThrowOnError(result);
                            resultList.Add(new XYPoint(xValue, pixelValues[i]));
                        }

                        return resultList;
                    }
                    else
                    {
                        return new List<XYPoint>();
                    }

                }
                else
                {
                    throw new SifException("Fail ei sisalda ühtegi signaali kaadrit");
                }
            }
            else
            {
                throw new SifException("Fail ei sisalda signaali andmeid");
            }
        }

        private static void ThrowOnError(SifResult result)
        {
            if (result != SifResult.Success)
                throw new SifException(GetSifResultDescription(result));
        }

        public static string GetSifResultDescription(SifResult result)
        {

            switch (result)
            {
                case SifResult.SifFormatError:
                    return "Fail on vigane";

                case SifResult.NoSifLoaded:
                    return "Sif fail on sisse lugemata";

                case SifResult.FileNotFound:
                    return "Faili ei leitud";

                case SifResult.FileAccessError:
                    return "Failile ei õnnestu ligi pääseda";

                case SifResult.DataNotPresent:
                    return "Andmed puuduvad";

                case SifResult.P1Invalid:
                case SifResult.P2Invalid:
                case SifResult.P3Invalid:
                case SifResult.P4Invalid:
                case SifResult.P5Invalid:
                case SifResult.P6Invalid:
                case SifResult.P7Invalid:
                case SifResult.P8Invalid:
                default:
                    return "";
            }

        }


    }
}
