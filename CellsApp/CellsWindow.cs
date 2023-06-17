using System.Drawing;
using System.Windows.Forms;
using CellsCore.Fractals;

namespace CellsApp
{
    class CellsWindow: Form
    {
        private FractalSketch sketch;

        public CellsWindow()
        {
            var screen = this.GetScreen();
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            this.sketch = new FractalSketch(screen.Width, screen.Height);
            this.Paint += new PaintEventHandler(this.PaintHandler);
            
        }
        public Rectangle GetScreen()
        {
            return Screen.FromControl(this).Bounds;
        }

        private void PaintHandler(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawImage(this.sketch.GetBitmap(), new PointF(0, 0));
        }
    }
}
