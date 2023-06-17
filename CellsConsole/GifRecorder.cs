using System;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CellsConsole
{
    class GifRecorder
    {
        private GifBitmapEncoder gEnc;
        public GifRecorder()
        {
            this.gEnc = new GifBitmapEncoder();
        }

        public void AddImage(Bitmap bmpImage)
        {
            var bmp = bmpImage.GetHbitmap();
            var src = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bmp,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            gEnc.Frames.Add(BitmapFrame.Create(src));
            // DeleteObject(bmp); // recommended, handle memory leak
        }

        public void SaveFile(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                gEnc.Save(fs);
            }
        }
    }
}
