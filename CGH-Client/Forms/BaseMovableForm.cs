using System.Drawing;
using System.Windows.Forms;
using CGH_Client.Utility;

namespace CGH_Client.Forms
{
    public class BaseMovableForm : Form
    {
        public object parent;
        public Point screenLocationFallback = new Point(0, 0);
        public bool sameLocationAsParent = true;
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
            bool movable = true,
            bool sameSizeAsParent = false,
            bool sameLocationAsParent = true
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
            this.sameLocationAsParent = sameLocationAsParent;

            // Set basic sizes:
            if (sameSizeAsParent && parent is Form parentForSize)
            {
                this.Size = parentForSize.Size;
            }
            else
            {
                this.Size = size != null ? (Size)size : this.GetSizeBasedOnScreenPercentage(scale);
            }

            // Set basic locations:
            this.UpdateFormInitialLocation();

            // Border style:
            this.FormBorderStyle = FormBorderStyle.None;

            // Override location:
            if (location != null)
            {
                this.Location = (Point)location;
            }

            // Set background image:
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

        public void UpdateFormInitialLocation()
        {
            // Set basic locations:
            if (this.sameLocationAsParent && parent is Form parentFormLocation)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = this.GetParentCenterLocation(parentFormLocation);
            }
            else if (this.sameLocationAsParent && this.screenLocationFallback != new Point(0, 0))
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = this.screenLocationFallback;
            }
            else
            {
                this.StartPosition = FormStartPosition.CenterScreen;
            }
        }
        
        private Point GetParentCenterLocation(Form form)
        {
            return new Point(
                form.Location.X + (form.Width - this.Width) / 2,
                form.Location.Y + (form.Height - this.Height) / 2
            );
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

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            mouseDown = true;
            lastLocation = e.Location;
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (mouseDown)
            {
                this.Location = new Point((this.Location.X - lastLocation.X) + e.X, (this.Location.Y - lastLocation.Y) + e.Y);
                this.Update();
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
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

        public void HideAndShow(object form)
        {            
            if (form is BaseMovableForm formToshow)
            {
                formToshow.parent = this;
                formToshow.screenLocationFallback = this.GetParentCenterLocation(this);
                formToshow.UpdateFormInitialLocation();
            }
            this.Hide();
            ((Form)form).Show();
        }
            
        public void SwitchToForm(object form)
        {
            if (form is BaseMovableForm form1)
            {
                form1.parent = this.parent;
                form1.screenLocationFallback = this.GetParentCenterLocation((Form)this.parent);
                form1.UpdateFormInitialLocation();
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
