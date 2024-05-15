using CGH_Client.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Controls
{
    internal class ScreenHeader : ScaleablePanel
    {

        public int innerHeight = 35;
        
        public Label headerLb;
        
        public ScreenHeader(Size parent, string text, Single fontSize = 20F, int padding = 20)
        {
            
            this.DoubleBuffered = true;
            
            this.Size = this.CalculateSize(parent, 0.5, 0, -1, this.innerHeight + (padding * 2));
            this.Location = this.AlignToParent(parent, "top-center", this.Width, this.Height);
            this.BackColor = Color.Transparent;

            // Main label:
            headerLb = new Label()
            {
                AutoSize = false,
                Size = new Size(this.Width - (padding * 2), this.innerHeight),
                Text = text,
                Font = new Font("Varela Round", fontSize, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(padding, padding),
                TextAlign = ContentAlignment.MiddleCenter,
                RightToLeft = RightToLeft.Yes,
            };

            // Layout debug:
            if (Globals.showLayoutDebug)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.headerLb.BorderStyle = BorderStyle.FixedSingle;
            }
            
            this.Controls.Add(headerLb);
        }
    }
}
