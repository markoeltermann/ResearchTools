using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpectrumLibrary.AndorSif;
using System.IO;
using System.Globalization;

namespace SifFileConverter
{
    static class Program
    {
        private static NumberFormatInfo NumberFormat = CultureInfo.InvariantCulture.NumberFormat;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Andor sif files (*.sif)|*.sif";
                ofd.Multiselect = true;

                var dlgResult = ofd.ShowDialog();
                if (dlgResult != DialogResult.OK)
                    return;

                foreach (var file in ofd.FileNames)
                {
                    TryConvertSifFile(file);
                }
            }

            //Application.Run(new Form1());
        }

        private static void TryConvertSifFile(string sifFileName)
        {
            try
            {
                var data = SifReader.ReadSignalFromSifFile(sifFileName);
                string outputFile;
                if (sifFileName.EndsWith(".sif", StringComparison.OrdinalIgnoreCase))
                {
                    outputFile = sifFileName.Substring(0, sifFileName.Length - 4) + ".txt";
                }
                else
                {
                    outputFile = sifFileName + ".txt";
                }

                if (File.Exists(outputFile))
                {
                    var owDlgResult = MessageBox.Show("Fail nimega " + outputFile + " eksisteerib. Kas kirjutan üle?", "", MessageBoxButtons.YesNoCancel);
                    if (owDlgResult == DialogResult.Yes)
                    {
                        File.Delete(outputFile);
                    }
                    else
                    {
                        return;
                    }
                }

                using (StreamWriter sw = new StreamWriter(outputFile, false))
                {
                    foreach (var point in data)
                    {
                        sw.WriteLine(point.X.ToString(NumberFormat) + "\t" + point.Y.ToString(NumberFormat));
                    }
                }

            }
            catch (IOException ioe)
            {
                MessageBox.Show("Failide lugemisel/kirjutamisel tekkis viga." + Environment.NewLine + ioe.Message);
            }
            catch (SifException e)
            {
                string message = "Faili konverteerimine ebaõnnestus.";
                if (!string.IsNullOrEmpty(e.Message))
                {
                    message += Environment.NewLine + e.Message + ".";
                }

                MessageBox.Show(message);
            }
            catch (Exception)
            {
                MessageBox.Show("Faili konverteerimine ebaõnnestus, tekkis tundmatu viga.");
            }
        }

    }
}
