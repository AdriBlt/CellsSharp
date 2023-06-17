using System.Drawing;
using CellsCore.Maths;

namespace CellsCore.Fractals
{
    struct Size
    {
        public Complex Min;
        public Complex Max;

        public Size(Complex min, Complex max) : this()
        {
            this.Min = min;
            this.Max = max;
        }
    }

    interface IFractal
    {
        int GetMaxIterations();
        double GetMaxSquareMod();
        Size GetDefaultSize();
        Complex GetIterationComplex(Complex z, Complex z0);
        FractalResult GetConvergenceResult(Complex z0);
        Color GetResultColor(FractalResult result);
    }

    enum Directions
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }

    enum ConvergenceStatus
    {
        Unknown,
        Converged,
        Diverged,
    }

    struct FractalResult
    {
        public Complex Point;
        public int Iterations;
        public ConvergenceStatus Status;

        public FractalResult(Complex point, int iterations, ConvergenceStatus status)
        {
            Point = point;
            Iterations = iterations;
            Status = status;
        }
    }
}
