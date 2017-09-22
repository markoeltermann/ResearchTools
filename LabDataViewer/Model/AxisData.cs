using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LabDataViewer.Model
{
    public class AxisData
    {

        public AxisData(string axisName, double minimum, double maximum)
        {
            this.AxisName = axisName;
            this.Minimum = minimum;
            this.Maximum = maximum;
        }

        public string AxisName { get; set; }

        public double Minimum { get; set; }

        public double Maximum { get; set; }

    }
}
