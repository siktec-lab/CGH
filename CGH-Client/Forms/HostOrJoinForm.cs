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
    public class HostOrJoinForm : Form
    {

        Panel createPanel, joinPanel;
        PictureBox createPB, joinPB;
        Label createLB, joinLB, mainLB, selectedGameLB;

        public HostOrJoinForm()
        {

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1495, 840);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            BackgroundImageLayout = ImageLayout.Center;
            FormBorderStyle = FormBorderStyle.None;

            mainLB = new Label()
            {
                AutoSize = true,
                Text = ":המשחק הנבחר",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 25)
            };
            Functions.CenterControlHorizontally(this, mainLB);
            Controls.Add(mainLB);

            selectedGameLB = new Label()
            {
                AutoSize = true,
                Text = "",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 65)
            };
            selectedGameLB.Text = Globals.gameChoosed;
            Functions.CenterControlHorizontally(this, selectedGameLB);
            Controls.Add(selectedGameLB);

            joinPanel = new Panel()
            {
                Size = new Size(192, 230),
                Location = new Point(363, 270),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(joinPanel);
            joinPanel.Click += JoinPanel_Click;

            joinPB = new PictureBox()
            {
                Size = new Size(192, 192),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Icons\joinIcon.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            joinPanel.Controls.Add(joinPB);
            joinPB.Click += JoinPanel_Click;

            joinLB = new Label()
            {
                AutoSize = true,
                Text = "הצטרפות",
                Font = new Font("Varela Round", 25F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 180),
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(joinPanel, joinLB);
            joinPanel.Controls.Add(joinLB);
            joinLB.Click += JoinPanel_Click;

            createPanel = new Panel()
            {
                Size = new Size(192, 230),
                Location = new Point(939, 270),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(createPanel);
            createPanel.Click += CreatePanel_Click;

            createPB = new PictureBox()
            {
                Size = new Size(192, 192),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Icons\createIcon.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            createPanel.Controls.Add(createPB);
            createPB.Click += CreatePanel_Click;

            createLB = new Label()
            {
                AutoSize = true,
                Text = "יצירה",
                Font = new Font("Varela Round", 25F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 180),
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(createPanel, createLB);
            createPanel.Controls.Add(createLB);
            createLB.Click += CreatePanel_Click;

            this.FormClosed += HostOrJoinForm_FormClosed;

        }

        private void JoinPanel_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "JOIN";
            EnterRoomCodeForm enterRoomCodeForm = new EnterRoomCodeForm();
            this.Hide();
            enterRoomCodeForm.Show();
        }

        private void CreatePanel_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "HOST";
            ChooseNameForm chooseNameForm = new ChooseNameForm();
            this.Hide();
            chooseNameForm.Show();
        }

        private void HostOrJoinForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            SelectGameForm selectGameForm = new SelectGameForm();
            this.Hide();
            selectGameForm.Show();
        }
    }
}
