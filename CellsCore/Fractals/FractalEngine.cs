using System;
using System.Collections.Generic;
using System.Drawing;
using CellsCore.Maths;

namespace CellsCore.Fractals
{
    struct Rectangle
    {
        public int MinI;
        public int MaxI;
        public int MinJ;
        public int MaxJ;

        public Rectangle(int minI, int maxI, int minJ, int maxJ)
        {
            this.MinI = minI;
            this.MaxI = maxI;
            this.MinJ = minJ;
            this.MaxJ = maxJ;
        }
    }

    class FractalEngine
    {
        private int width;
        private int height;
        private IFractal fractal;
        private int antiAliasingSize;
        private Complex min;
        private Complex max;

        private FractalResult[][][] results = new FractalResult[0][][];

        public FractalEngine(
            int width,
            int height,
            IFractal fractal,
            int antiAliasingSize = 2)
        {
            this.width = width;
            this.height = height;
            this.fractal = fractal;
            this.antiAliasingSize = antiAliasingSize;

            this.ResetFractal();
        }

        public void SetFractal(IFractal fractal) {
            this.fractal = fractal;
            this.ResetFractal();
        }

        public void SetAntiAliasing(int aaSize) {
            this.antiAliasingSize = aaSize;
        }

        public void ResetFractal()
        {
            this.results = Utils.List.CreateDefaultMatrix(this.width, this.height, (i, j) => ( new FractalResult[0]));
            var defaultSize = this.fractal.GetDefaultSize();
            this.min = defaultSize.Min;
            this.max = defaultSize.Max;
            this.CheckScales();
        }

        public void Move(Directions dir) {
            var dirXW = 0;
            var dirYH = 0;
            switch (dir) {
                case Directions.DOWN:
                    dirYH = 1;
                    break;
                case Directions.LEFT:
                    dirXW = -1;
                    break;
                case Directions.RIGHT:
                    dirXW = 1;
                    break;
                case Directions.UP:
                    dirYH = -1;
                    break;
                default:
                    break;
            }

            var deltaW = this.width / 3;
            var deltaH = this.height / 3;

            var deltaX = this.DeltaX;
            var deltaY = this.DeltaY;

            var moveX = dirXW * deltaX * deltaW;
            var moveY = dirYH * deltaY * deltaH;

            this.min = this.min.Add(new Complex(moveX, moveY));
            this.max = this.max.Add(new Complex(moveX, moveY));

            if (dir == Directions.DOWN)
            {
                for (int i = 0; i < this.height - deltaH; i++)
                {
                    this.results[i] = this.results[i + deltaH];
                }

                this.ComputeFractal(iMin: this.height - deltaH);
            }
            else if (dir == Directions.UP)
            {
                for (int i = this.height - 1; i >= deltaH; i--)
                {
                    this.results[i] = this.results[i - deltaH];
                }
                this.ComputeFractal(iMax: deltaH);
            }
            else if (dir == Directions.RIGHT)
            {
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = 0; j < this.width - deltaW; j++)
                    {
                        this.results[i][j] = this.results[i][j + deltaW];
                    }
                }
                this.ComputeFractal(jMin: this.width - deltaW);
            }
            else if (dir == Directions.LEFT)
            {
                for (int i = 0; i < this.height; i++)
                {
                    for (int j = this.width - 1; j >= deltaW; j--)
                    {
                        this.results[i][j] = this.results[i][j - deltaW];
                    }
                }
                this.ComputeFractal(jMax: deltaW);
            }
        }

        public void ZoomOnPoint(int i, int j, bool isZoomIn) {
            var z0 = this.GetPointOnScreen(i, j);
            this.ZoomOnPoint(z0, isZoomIn);
        }

        public void ZoomOnPoint(Complex z0, bool isZoomIn)
        {
            var w = this.max.X - this.min.X;
            var h = this.max.Y - this.min.Y;
            var coef = 1.3;
            var deltaW = (isZoomIn ? w / coef : w * coef) / 2;
            var deltaH = (isZoomIn ? h / coef : h * coef) / 2;
            this.min = new Complex(z0.X - deltaW, z0.Y - deltaH);
            this.max = new Complex(z0.X + deltaW, z0.Y + deltaH);

            this.CheckScales();
            // TODO optimize, save 1/4th of computation?
            this.ComputeFractal();
        }

        public void ZoomOnRectangle(Rectangle rectangle)
        {
            var min = this.GetPointOnScreen(rectangle.MinI, rectangle.MinJ);
            var max = this.GetPointOnScreen(rectangle.MaxI, rectangle.MaxJ);
            this.min = min;
            this.max = max;
            this.CheckScales();
            this.ComputeFractal();
        }

        public void ComputeFractal(
            int? iMin = null,
            int? iMax = null,
            int? jMin = null,
            int? jMax = null)
        {
            var indexMinI = iMin ?? 0;
            var indexMaxI = iMax ?? this.height;
            var indexMinJ = jMin ?? 0;
            var indexMaxJ = jMax ?? this.width;

            for (int i = indexMinI; i < indexMaxI; i++)
            {
                var ii = i;
                new System.Threading.Thread(() =>
                {
                    for (int j = indexMinJ; j < indexMaxJ; j++)
                    {
                        var jj = j;
                        this.results[ii][jj] = this.ComputeResult(ii, jj);
                    }
                }).Start();
            }
        }

        public Color GetColor(int i, int j)
        {
            return this.GetMeanColorFromAA(this.results[i][j]);
        }

        private Complex GetPointOnScreen(int i, int j)
        {
            return new Complex(
                this.min.X + j * this.DeltaX,
                this.min.Y + i * this.DeltaY);
        }

        private double DeltaX => (this.max.X - this.min.X) / this.width;
        private double DeltaY => (this.max.Y - this.min.Y) / this.height;

        private FractalResult[] ComputeResult(int i, int j)
        {
            var z0 = this.GetPointOnScreen(i, j);
            
            var result = new List<FractalResult>();
            for (int jj = 0; jj < this.antiAliasingSize; jj++)
            {
                var x = z0.X + jj * this.DeltaX / this.antiAliasingSize;
                for (int ii = 0; ii < this.antiAliasingSize; ii++)
                {
                    var y = z0.Y + 1.0 * ii * this.DeltaY / this.antiAliasingSize;
                    result.Add(this.fractal.GetConvergenceResult(new Complex(x, y)));
                }
            }

            return result.ToArray();
        }

        private void CheckScales()
        {
            var sizeX = this.max.X - this.min.X;
            var sizeY = this.max.Y - this.min.Y;
            var ratioWidth = this.width / sizeX;
            var ratioHeight = this.height / sizeY;

            if (ratioWidth > ratioHeight)
            {
                var newDeltaX = this.width / ratioHeight;
                var marge = (newDeltaX - sizeX) / 2;
                this.min = this.min.AddRe(-marge);
                this.max = this.max.AddRe(marge);
            }

            if (ratioWidth < ratioHeight)
            {
                var newDeltaY = this.height / ratioWidth;
                var marge = (newDeltaY - sizeY) / 2;
                this.min = this.min.AddIm(-marge);
                this.max = this.max.AddIm(marge);
            }
        }

        private Color GetMeanColorFromAA(FractalResult[] resultsAA)
        {
            var r = 0;
            var g = 0;
            var b = 0;
            foreach (FractalResult result in resultsAA) {
                var c = this.fractal.GetResultColor(result);
                r += c.R;
                g += c.G;
                b += c.B;
            }
            var sizeAA = resultsAA.Length;
            if (sizeAA > 1)
            {
                r /= sizeAA;
                g /= sizeAA;
                b /= sizeAA;
            }
            return Utils.Colors.GetColor(r, g, b);
        }
    }
}
