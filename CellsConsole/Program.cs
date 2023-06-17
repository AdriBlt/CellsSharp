using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CellsCore.Fractals;
using CellsCore.Maths;

namespace CellsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            int nbImages = 99;
            SaveImages(new Complex(-0.170337, -1.06506), nbImages);
            SaveImages(new Complex(0.42884, -0.231345), nbImages);
            SaveImages(new Complex(-1.62917, -0.0203968), nbImages);
            SaveImages(new Complex(-0.761574, -0.0847596), nbImages);
        }

        private static void SaveImages(Complex point, int nbImages)
        {
            var width = 1200;
            var height = 800;
            var pad = (int)Math.Ceiling(Math.Log10(nbImages + 1));
            var sketch = new FractalSketch(width, height);

            //var recorder = new GifRecorder();
            //recorder.AddImage(sketch.GetBitmap());
            //for (int i = 0; i < nbImages; i++)
            //{
            //    System.Console.WriteLine((i + 1) + " out of " + nbImages);
            //    sketch.ZoomInOnPoint(point);
            //    recorder.AddImage(sketch.GetBitmap());
            //}

            //recorder.SaveFile(@"C:\dev\CellsSharp\mandelbrot.gif");

            var basePath = @"C:\dev\CellsSharp\export\mandelbrot_" + point.ToString() + "_";
            Action<int> save = (int i) => sketch.GetBitmap().Save(basePath + i.ToString().PadLeft(pad, '0') + ".jpg");
            save(0);
            for (int i = 1; i <= nbImages; i++)
            {
                sketch.ZoomInOnPoint(point);
                save(i);
            }
        }
    }
}
