using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpectrumLibrary.Interpolation
{
    public static class InterpolationMethods
    {

        public static double InterpolateAt(this List<XYPoint> points, double x)
        {

            var firstX = points.First().X;
            var lastX = points.Last().X;

            if (x <= firstX)
                return points[0].Y;

            if (x >= lastX)
                return points.Last().Y;

            var relativePosition = (x - firstX) / (lastX - firstX);

            var index = (int)Math.Floor(relativePosition * points.Count);
            while (points[index].X > x)
            {
                index--;
            }
            while (points[index + 1].X <= x)
            {
                index++;
            }

            var prevPoint = points[index];
            var nextPoint = points[index + 1];

            var relativePositionBetweenPoints = (x - prevPoint.X) / (nextPoint.X - prevPoint.X);

            return prevPoint.Y + relativePositionBetweenPoints * (nextPoint.Y - prevPoint.Y);

        }

    }
}
