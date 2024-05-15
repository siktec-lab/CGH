using CGH_Client.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class ChoosePlayerCardForm : Form
    {

        PictureBox char1, char2, char3, char4, char5, char6, char7, char8, char9, char10, char11, char12, char13;

        public ChoosePlayerCardForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(300, 300);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            BackgroundImageLayout = ImageLayout.Center;
            FormBorderStyle = FormBorderStyle.None;

            char1 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_1.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "1"
            };
            Controls.Add(char1);
            char1.Click += Char_Click;

            char2 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(75, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_2.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "2"
            };
            Controls.Add(char2);
            char2.Click += Char_Click;

            char3 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(150, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_3.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "3"
            };
            Controls.Add(char3);
            char3.Click += Char_Click;

            char4 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(225, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_4.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "4"
            };
            Controls.Add(char4);
            char4.Click += Char_Click;

            char5 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(0, 75),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_5.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "5"
            };
            Controls.Add(char5);
            char5.Click += Char_Click;

            char6 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(75, 75),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_6.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "6"
            };
            Controls.Add(char6);
            char6.Click += Char_Click;

            char7 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(150, 75),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_7.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "7"
            };
            Controls.Add(char7);
            char7.Click += Char_Click;

            char8 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(225, 75),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_8.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "8"
            };
            Controls.Add(char8);
            char8.Click += Char_Click;

            char9 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(0, 150),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_9.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "9"
            };
            Controls.Add(char9);
            char9.Click += Char_Click;

            char10 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(75, 150),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_10.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "10"
            };
            Controls.Add(char10);
            char10.Click += Char_Click;

            char11 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(150, 150),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_11.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "11"
            };
            Controls.Add(char11);
            char11.Click += Char_Click;

            char12 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(225, 150),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_12.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "12"
            };
            Controls.Add(char12);
            char12.Click += Char_Click;

            char13 = new PictureBox()
            {
                Size = new Size(75, 75),
                Location = new Point(0, 225),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_13.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                Tag = "13"
            };
            Functions.CenterControlHorizontally(this, char13);
            Controls.Add(char13);
            char13.Click += Char_Click;

        }

        private void Char_Click(object sender, EventArgs e)
        {
            var pb = sender as PictureBox;
            Globals.charTagSelected = int.Parse(pb.Tag.ToString());
            Globals.charImgSelected = pb.Image;
            //ChooseNameForm chooseNameForm = new ChooseNameForm(this);
            this.Close();
            //chooseNameForm.Show();
        }
    }
}
