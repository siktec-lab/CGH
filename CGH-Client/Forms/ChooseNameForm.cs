using CGH_Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class ChooseNameForm : Form
    {

        Label chooseNameLB;
        TextBox chooseNameTB;
        PictureBox choosePlayerCardPB;
        Button chooseNameB;

        public ChooseNameForm()
        {

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(300, 275);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            BackgroundImageLayout = ImageLayout.Center;
            FormBorderStyle = FormBorderStyle.None;

            chooseNameLB = new Label()
            {
                AutoSize = true,
                Text = "בחר שם",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 25)
            };
            Functions.CenterControlHorizontally(this, chooseNameLB);
            Controls.Add(chooseNameLB);

            chooseNameTB = new TextBox()
            {
                Size = new Size(150, 50),
                Location = new Point(0, 75)
            };
            Functions.CenterControlHorizontally(this, chooseNameTB);
            Controls.Add(chooseNameTB);

            choosePlayerCardPB = new PictureBox()
            {
                Size = new Size(50, 50),
                Location = new Point(0, 125),
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Icons\addIcon.png"),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };
            Functions.CenterControlHorizontally(this, choosePlayerCardPB);
            Controls.Add(choosePlayerCardPB);
            choosePlayerCardPB.Click += ChoosePlayerCardPB_Click;

            chooseNameB = new Button()
            {
                Size = new Size(100, 50),
                Location = new Point(0, 200),
                Text = "להיכנס"
            };
            Functions.CenterControlHorizontally(this, chooseNameB);
            Controls.Add(chooseNameB);
            chooseNameB.Click += ChooseNameB_Click;

            if (Globals.charImgSelected == null)
            {
                choosePlayerCardPB.Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Icons\addIcon.png");
            }

            else
            {
                choosePlayerCardPB.Image = Globals.charImgSelected;
            }

            FormClosed += ChooseNameForm_FormClosed;

        }

        private void ChooseNameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm();
            this.Hide();
            hostOrJoinForm.Show();
        }

        private void ChooseNameB_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            bool isNameOk = false;
            bool isPictureOk = false;

            Regex nameRegex = new Regex("([A-Za-z])\\w+");

            if (nameRegex.IsMatch(chooseNameTB.Text) && Functions.isNameAvailable(chooseNameTB.Text))
                isNameOk = true;

            else
            {
                sb.AppendLine("שם לא תקין, השם צריך להיות רציף וללא רווחים או סימנים או שהשם תפוס");
                isNameOk = false;
            }

            if (Globals.charImgSelected == null)
            {
                sb.AppendLine("לא בחרת תמונה לשחקן");
                isPictureOk = false;
            }

            else
                isPictureOk = true;

            if (isNameOk && isPictureOk)
            {
                Globals.charName = chooseNameTB.Text;
                Player player = new Player();
                player.Name = Globals.charName;
                player.ImgCharNum = Globals.charTagSelected;
                player.isDisconnected = false;
                player.selectedCard = "";

                if (Globals.hostOrJoin == "HOST")
                {
                    //Globals.globalGameRoom.gameType = Globals.gameChoosed;
                    //Globals.globalGameRoom.roomCode = Functions.createGameCode();
                    //Globals.gameCode = Globals.globalGameRoom.roomCode;
                    //Globals.globalGameRoom.cardPlayed = "";
                    //Globals.globalGameRoom.players = new List<Player>();
                    //Globals.globalGameRoom.currentPlayerTurn = player.Name;
                    player.isHost = true;
                    Globals.ServerConnector.SendMessage(Globals.gameChoosed + "{0}" + player, "createGameLobby");
                }

                if (Globals.hostOrJoin == "JOIN")
                {
                    player.isHost = false;
                    Globals.ServerConnector.SendMessage(Globals.gameChoosed + "-" + Globals.gameCode + "{0}" + player, "joinPlayerToGame");
                }
            }

            else
            {
                MessageBox.Show(sb.ToString());
                sb.Clear();
            }

            while (true)
            {
                if (Globals.isGameCreated)
                {
                    Globals.gameLobbyForm = new GameLobbyForm();
                    this.Hide();
                    Globals.gameLobbyForm.Show();
                    break;
                }
            }
        }

        private void ChoosePlayerCardPB_Click(object sender, EventArgs e)
        {
            ChoosePlayerCardForm choosePlayerCardForm = new ChoosePlayerCardForm();
            this.Hide();
            choosePlayerCardForm.Show();
        }
    }
}
