using System;
using System.Windows.Forms;
using System.Drawing;

namespace CGH_Client.Controls
{
    internal class MainMenuItem : Panel
    {
        private Label label;
        private PictureBox pictureBox;
        private float fontSize;
        private int labelHeight;
        public MainMenuItem(string text, string imagePath, float fontSize = 18F)
        {
            this.DoubleBuffered = true;
            
            this.fontSize   = fontSize;
            this.labelHeight = (int)(fontSize * 1.8);
            this.BackColor  = Color.Transparent;
            this.Cursor     = Cursors.Hand;

            // Menu Item Picture Box:
            this.pictureBox = new PictureBox()
            {
                Location    = new Point(0, 0),
                Image       = Image.FromFile(imagePath),
                SizeMode    = PictureBoxSizeMode.Zoom,
                Cursor      = Cursors.Hand,
                BorderStyle = BorderStyle.None
            };

            // Menu Item Label:
            this.label = new Label()
            {
                AutoSize    = false,
                Text        = text,
                Font        = new Font("Varela Round", this.fontSize, System.Drawing.FontStyle.Bold),
                BackColor   = Color.Transparent,
                ForeColor   = Color.White,
                TextAlign   = ContentAlignment.MiddleCenter
            };

            // Add controls to the panel:
            this.Controls.Add(this.pictureBox);
            this.Controls.Add(this.label);
        }

        public void SetSize(int width, int height)
        {
            this.Size               = new Size(width, height);
            this.pictureBox.Size    = new Size(width, height - this.labelHeight);
            this.label.Location     = new Point(0, height - this.labelHeight);
            this.label.Size         = new Size(width, this.labelHeight);
        }

        public event EventHandler ClickAny
        {
            add
            {
                this.Click += value;
                this.pictureBox.Click += value;
                this.label.Click += value;
            }
            remove
            {
                this.Click -= value;
                this.pictureBox.Click -= value;
                this.label.Click -= value;
            }
        }

        public void ShowDebug(bool state)
        {
            if (state)
            {
                this.BorderStyle            = BorderStyle.FixedSingle;
                this.pictureBox.BorderStyle = BorderStyle.FixedSingle;
                this.label.BorderStyle      = BorderStyle.FixedSingle;
            }
            else
            {
                this.BorderStyle            = BorderStyle.None;
                this.pictureBox.BorderStyle = BorderStyle.None;
                this.label.BorderStyle      = BorderStyle.None;
            }
        }
    }
}
