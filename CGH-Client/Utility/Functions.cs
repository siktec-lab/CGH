using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using CGH_Client.Forms;

namespace CGH_Client.Utility
{
    public static class Functions
    {
        public static void CenterControlHorizontally(Form form, Control control)
        {
            Graphics g = form.CreateGraphics();
            if (control.GetType() == typeof(Label))
            {
                Size labelSize = g.MeasureString(control.Text, control.Font).ToSize();
                int formWidth = form.Width;
                control.Location = new Point((formWidth - labelSize.Width) / 2, control.Location.Y);
            }

            else
            {
                int controlSize = control.Width;
                int formWidth = form.Width;
                control.Location = new Point((formWidth - controlSize) / 2, control.Location.Y);
            }
        }

        public static void CenterControlHorizontally(Panel p, Control control)
        {
            Graphics g = p.CreateGraphics();
            if (control.GetType() == typeof(Label))
            {
                Size labelSize = g.MeasureString(control.Text, control.Font).ToSize();
                int panelWidth = p.Width;
                control.Location = new Point((panelWidth - labelSize.Width) / 2, control.Location.Y);
            }

            else
            {
                int controlSize = control.Width;
                int panelWidth = p.Width;
                control.Location = new Point((panelWidth - controlSize) / 2, control.Location.Y);
            }
        }

        public static void CenterControlVertically(Form form, Control control)
        {
            Graphics g = form.CreateGraphics();
            if (control.GetType() == typeof(Label))
            {
                Size labelSize = g.MeasureString(control.Text, control.Font).ToSize();
                int formHeight = form.Height;
                control.Location = new Point(control.Location.X, (formHeight - labelSize.Height) / 2);
            }

            else
            {
                int controlSize = control.Height;
                int formHeight = form.Height;
                control.Location = new Point(control.Location.X, (formHeight - controlSize) / 2);
            }
        }

        public static void CenterControlVertically(Panel p, Control control)
        {
            Graphics g = p.CreateGraphics();
            if (control.GetType() == typeof(Label))
            {
                Size labelSize = g.MeasureString(control.Text, control.Font).ToSize();
                int panelHeight = p.Height;
                control.Location = new Point(control.Location.X, (panelHeight - labelSize.Height) / 2);
            }

            else
            {
                int controlSize = control.Height;
                int panelHeight = p.Height;
                control.Location = new Point(control.Location.X, (panelHeight - controlSize) / 2);
            }
        }

        public static bool isNameAvailable(string name)
        {
            return true;
        }

        public static int createGameCode()
        {
            Random random = new Random();

            return random.Next(100000, 1000000);
        }

    }
}
