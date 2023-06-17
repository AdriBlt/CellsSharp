using System;
using System.Drawing;
using CellsCore.Maths;

namespace CellsCore.Fractals.Fractals
{
    class Mandelbrot : IFractal
    {
        public Size GetDefaultSize() => new Size(new Complex(-2.0, -1.0), max: new Complex(1.0, 0));

        public int GetMaxIterations() => 100;

        public double GetMaxSquareMod() => 4;


        public FractalResult GetConvergenceResult(Complex z0)
        {
            var z = z0;
            for (var n = 0; n < this.GetMaxIterations(); n++)
            {
                var zz = this.GetIterationComplex(z, z0);

                if (zz.GetSquareModule() >= this.GetMaxSquareMod())
                {
                    return new FractalResult(
                        point: z,
                        iterations: n,
                        status: ConvergenceStatus.Diverged);
                }

                if (z.Equals(zz))
                {
                    return new FractalResult(
                        point: zz,
                        iterations: n,
                        status: ConvergenceStatus.Converged);
                }

                z = zz;
            }

            return new FractalResult(
                point: z,
                iterations: this.GetMaxIterations(),
                status: ConvergenceStatus.Unknown);
        }


        public Complex GetIterationComplex(Complex z, Complex z0)
        {
            return z.GetSquare().Add(z0);
        }

        public Color GetResultColor(FractalResult result)
        {
            switch (result.Status)
            {
                case ConvergenceStatus.Converged:
                case ConvergenceStatus.Unknown:
                    return Color.Black;
                case ConvergenceStatus.Diverged:
                default:
                    var p = Math.Log(1 + result.Iterations) / Math.Log(1 + this.GetMaxIterations());
                    return Utils.Colors.GetColorBetween(Color.DarkBlue, Color.White, p);
            }

        }
    }
}
