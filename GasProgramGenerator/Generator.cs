using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasProgramGenerator
{
    public class Generator
    {

        private Random random;

        public Generator()
        {
            random = new Random();
        }

        public void Generate(GeneratorArgs args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));

            var totalDuration = TimeSpan.Zero;

            

        }

    }
}
