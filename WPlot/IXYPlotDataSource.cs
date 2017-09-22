using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPlot
{
    public interface IXYPlotDataSource
    {

        int PointCount { get; }

        event EventHandler Changed;

        double GetX(int i);

        double GetY(int i);

    }
}
