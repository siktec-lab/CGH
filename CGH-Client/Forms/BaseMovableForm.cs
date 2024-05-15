using System.Drawing;
using System.Windows.Forms;
using CGH_Client.Utility;

namespace CGH_Client.Forms
{
    public class BaseMovableForm : Form
    {
        public object parent;
        private string formName;
        private bool mouseDown;
        private System.Drawing.Point lastLocation;

        public BaseMovableForm(
            object parent,
            string name,
            string backgroundImagePath = "",
            Point? size = null,
            double scale = 0.7,
            Point? location = null,
            bool movable = true
        )
        {
            //Optimization:
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.UserPaint |
              ControlStyles.AllPaintingInWmPaint |
              ControlStyles.ResizeRedraw |
              ControlStyles.ContainerControl |
              ControlStyles.OptimizedDoubleBuffer |
              ControlStyles.SupportsTransparentBackColor
              , true);

            // Set form name:
            this.parent = parent;
            this.formName = name;

            // Set basic sizes:
            this.Size = size != null ? (Size)size : this.GetSizeBasedOnScreenPercentage(scale);
            this.StartPosition = FormStartPosition.CenterScreen; //TODO: Change to CenterParent
            this.FormBorderStyle = FormBorderStyle.None;
            if (location != null)
            {
                this.Location = (Point)location;
            }
            if (backgroundImagePath != "")
            {
                this.BackgroundImage = Image.FromFile(backgroundImagePath);
                this.BackgroundImageLayout = ImageLayout.Center;
            }

            // Attach events:
            if (movable)
            {
                this.MouseDown += this.Form_MouseDown;
                this.MouseMove += this.Form_MouseMove;
                this.MouseUp += this.Form_MouseUp;
            }

            // Always report which screen is visible 
            this.VisibleChanged += (s, args) =>
            {
                if (this.Visible)
                {
                    Globals.currentScreenName = this.formName;
                    Globals.currentScreen = this;
                }
            };
        }

        private Size GetSizeBasedOnScreenPercentage(double perc = 0.7)
        {
            // Get screen size:
            int screenWidth = Screen.PrimaryScreen.Bounds.Width;
            int screenHeight = Screen.PrimaryScreen.Bounds.Height;

            // Calculate size:
            int width = (int)(screenWidth * perc);
            int height = (int)(screenHeight * perc);

            return new Size(width, height);
        }

        private void Form_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Form_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new System.Drawing.Point(
                    (this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);

                this.Update();
            }
        }

        private void Form_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            mouseDown = false;
        }

        public void ParentShow()
        {
            if (this.parent != null)
            {
                ((Form)this.parent).Show();
            }
        }

        public void ParentHide()
        {
            if (this.parent != null)
            {
                ((Form)this.parent).Hide();
            }
        }

        public void CloseAndBack()
        {
            this.FormClosed += (s, args) =>
            {
                this.ParentShow();
            };
            this.Close();
        }
        
        public void SwitchToForm(object form)
        {
            if (form is BaseMovableForm form1)
            {
                form1.parent = this.parent;
            }
            this.FormClosed += (s, args) =>
            {
                if (form is Form form2)
                {
                    form2.Show();
                }
            };
            this.Close();
        }
    }
}
