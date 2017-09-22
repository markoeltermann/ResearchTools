using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DecayDataAnalyzer
{
    public partial class Form1 : Form
    {

        private int? _DataCompactionRatio;
        public int? DataCompactionRatio
        {
            get { return _DataCompactionRatio; }
            private set { _DataCompactionRatio = value; }
        }

        public int? NumberOfPointsToSkip { get; private set; }

        private bool _ShouldProceed;
        public bool ShouldProceed
        {
            get { return _ShouldProceed; }
            private set { _ShouldProceed = value; }
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void TryParseDataCompactionRatioFromTextBox()
        {
            var text = dataCompactionRatioTextBox.Text;
            DataCompactionRatio = TryParseText(text);
            NumberOfPointsToSkip = TryParseText(pointsToSkipCountTextBox.Text);
        }

        private int? TryParseText(string text)
        {
            int ratio;
            if (int.TryParse(text, out ratio))
            {
                return ratio;
            }
            else
            {
                return null;
            }
        }

        private void dataCompactionRatioTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                TryProceed();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TryProceed();
        }

        private void TryProceed()
        {
            TryParseDataCompactionRatioFromTextBox();
            if (DataCompactionRatio.HasValue)
            {
                ShouldProceed = true;
                this.Close();
            }
        }

        private void pointsToSkipCountTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            dataCompactionRatioTextBox_KeyUp(sender, e);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Focus();
            BringToFront();         
        }

    }
}
