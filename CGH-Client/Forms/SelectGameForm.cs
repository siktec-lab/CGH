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
    public class SelectGameForm : Form
    {

        Label mainLB, unoLB, warLB, cheatLB;
        PictureBox unoPB, warPB, cheatPB, closeBtnPB;
        Panel unoPanel, warPanel, cheatPanel;

        public SelectGameForm()
        {

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1495, 840);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            BackgroundImageLayout = ImageLayout.Center;
            FormBorderStyle = FormBorderStyle.None;

            mainLB = new Label()
            {
                AutoSize = true,
                Text = "מועדון משחקי הקלפים",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 25)
            };
            Functions.CenterControlHorizontally(this, mainLB);
            Controls.Add(mainLB);

            unoPanel = new Panel()
            {
                Size = new Size(164, 290),
                Location = new Point(334, 270),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(unoPanel);
            unoPanel.Click += UnoPanel_Click;

            unoPB = new PictureBox()
            {
                Size = new Size(164, 255),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Uno\wild\wild_card.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            unoPanel.Controls.Add(unoPB);
            unoPB.Click += UnoPanel_Click;

            unoLB = new Label()
            {
                AutoSize = true,
                Text = "אונו",
                Font = new Font("Varela Round", 25F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 250),
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(unoPanel, unoLB);
            unoPanel.Controls.Add(unoLB);
            unoLB.Click += UnoPanel_Click;

            warPanel = new Panel()
            {
                Size = new Size(164, 290),
                Location = new Point(668, 270),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(warPanel);
            warPanel.Click += WarPanel_Click;

            warPB = new PictureBox()
            {
                Size = new Size(164, 255),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\heart\cardHearts_K.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            warPanel.Controls.Add(warPB);
            warPB.Click += WarPanel_Click;

            warLB = new Label()
            {
                AutoSize = true,
                Text = "מלחמה",
                Font = new Font("Varela Round", 25F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 250),
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(warPanel, warLB);
            warPanel.Controls.Add(warLB);
            warLB.Click += WarPanel_Click;

            cheatPanel = new Panel()
            {
                Size = new Size(164, 290),
                Location = new Point(1002, 270),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(cheatPanel);
            cheatPanel.Click += CheatPanel_Click;

            cheatPB = new PictureBox()
            {
                Size = new Size(164, 255),
                Location = new Point(0, 0),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\spade\cardSpades_A.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            cheatPanel.Controls.Add(cheatPB);
            cheatPB.Click += CheatPanel_Click;

            cheatLB = new Label()
            {
                AutoSize = true,
                Text = "צ'יט",
                Font = new Font("Varela Round", 25F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 250),
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(cheatPanel, cheatLB);
            cheatPanel.Controls.Add(cheatLB);
            cheatLB.Click += CheatPanel_Click;

            closeBtnPB = new PictureBox()
            {
                Size = new Size(40, 40),
                Location = new Point(50, 50),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Icons\powerIcon.gif"),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Controls.Add(closeBtnPB);
            closeBtnPB.Click += CloseBtnPB_Click;

        }

        private void CloseBtnPB_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void CheatPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "Cheat";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm();
            this.Hide();
            hostOrJoinForm.Show();
        }

        private void WarPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "War";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm();
            this.Hide();
            hostOrJoinForm.Show();
        }

        private void UnoPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "Uno";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm();
            this.Hide();
            hostOrJoinForm.Show();
        }
    }
}
