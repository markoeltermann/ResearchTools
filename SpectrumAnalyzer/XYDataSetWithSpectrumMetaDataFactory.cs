using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpectrumLibrary.XYData;
using SpectrumLibrary;
using System.IO;
using SpectrumLibrary.Spectrum;

namespace SpectrumAnalyzer
{
    public class XYDataSetWithSpectrumMetaDataFactory : IXYDataSetFactory<XYDataSet<SpectrumMetadata>>
    {

        #region IXYDataSetFactory<XYDataSet<SpectrumRegions>> Members

        public XYDataSet<SpectrumMetadata> CreateDataSet(XYPoint[] points, string dataSetName, string sourceFileFullPath)
        {
            string path = sourceFileFullPath.Substring(0, sourceFileFullPath.LastIndexOf('.'));
            path += "_" + dataSetName + "_params.txt";
            SpectrumMetadata metaData = null;
            if (File.Exists(path))
            {
                metaData = SpectrumMetadata.ReadFromFile(path);
            }

            return new XYDataSet<SpectrumMetadata>(points, dataSetName) { MetaData = metaData };
        }

        #endregion
    }
}
