using System;
using System.Drawing;
using CellsCore.Fractals.Fractals;
using CellsCore.Maths;

namespace CellsCore.Fractals
{
    public class FractalSketch
    {
        const int antiAliasingSize = 2;
        private int width;
        private int height;
        private IFractal fractal;
        private FractalEngine engine;
        private Bitmap bitmap;

        public FractalSketch(int width, int height)
        {
            this.width = width;
            this.height = height;
            this.fractal = new Mandelbrot();
            this.engine = new FractalEngine(this.width, this.height, fractal, antiAliasingSize);
            this.bitmap = new Bitmap(this.width, this.height);
                
            this.ComputeFractal();
        }

        public void ZoomInOnPoint(Complex point)
        {
            this.engine.ZoomOnPoint(point, true);
            this.UpdateBitmap();
        }

        public Bitmap GetBitmap() => this.bitmap;

        private void ComputeFractal()
        {
            this.engine.ComputeFractal();
            this.UpdateBitmap();
        }

        private void UpdateBitmap()
        {
            for (int i = 0; i < this.height; i++)
            {
                for (int j = 0; j < this.width; j++)
                {
                    this.bitmap.SetPixel(j, i, this.engine.GetColor(i, j));
                }
            }
        }
    }
}
