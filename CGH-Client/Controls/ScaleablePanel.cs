using System.Drawing;
using System.Windows.Forms;

namespace CGH_Client.Controls
{
    internal class ScaleablePanel : Panel
    {
        public ScaleablePanel() : base() { 
        }

        protected Size CalculateSize(Size parent, double horizontalRatio, double verticalRatio, int fixedWidth, int fixedHeight)
        {
            int width   = fixedWidth != -1 ? fixedWidth : (int)(parent.Width * horizontalRatio);
            int height  = fixedHeight != -1 ? fixedHeight : (int)(parent.Height * verticalRatio);
            return new Size(width, height);
        }
        public Point AlignToParent(Size parent, string alignment, int width, int height)
        {
            switch (alignment)
            {
                case "center":
                    return new Point((parent.Width - width) / 2, (parent.Height - height) / 2);
                case "top-left":
                    return new Point(0, 0);
                case "top-right":
                    return new Point(parent.Width - width, 0);
                case "bottom-left":
                    return new Point(0, parent.Height - height);
                case "bottom-right":
                    return new Point(parent.Width - width, parent.Height - height);
                case "top-center":
                    return new Point((parent.Width - width) / 2, 0);
                case "bottom-center":
                    return new Point((parent.Width - width) / 2, parent.Height - height);
                case "center-left":
                    return new Point(0, (parent.Height - height) / 2);
                case "center-right":
                    return new Point(parent.Width - width, (parent.Height - height) / 2);
                default:
                    return new Point(0, 0);
            }
        }
    }
}
