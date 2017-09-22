using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.XYData
{
    public interface IXYDataSetFactory<TXYDataSet>
    {

        TXYDataSet CreateDataSet(XYPoint[] points, string dataSetName, string sourceFileFullPath);

    }
}
