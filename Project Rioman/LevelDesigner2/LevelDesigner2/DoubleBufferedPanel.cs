using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public class DoubleBufferedPanel : Panel
    {
        public DoubleBufferedPanel()
        {

            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.UserPaint, true);
        }
    }
}
