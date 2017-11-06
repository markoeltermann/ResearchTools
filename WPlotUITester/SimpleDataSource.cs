using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPlot;

namespace WPlotUITester
{
    public class SimpleDataSource : IXYPlotDataSource
    {
        public int PointCount
        {
            get
            {
                return 4;
            }
        }

        public event EventHandler Changed { add { } remove { } }

        public double GetX(int i)
        {
            switch (i)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 3;
                case 3:
                    return 4;

                default:
                    return 4;
            }
        }

        public double GetY(int i)
        {
            switch (i)
            {
                case 0:
                    return 1;
                case 1:
                    return 3;
                case 2:
                    return 3.8;
                case 3:
                    return 2;

                default:
                    return 4;
            }
        }

    }
}
