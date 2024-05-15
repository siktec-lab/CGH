using CGH_Client.Controls;
using CGH_Client.Utility;
using CGH_Client.Networking.Messages;
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
    public class ChooseNameForm : BaseMovableForm
    {
        ScreenHeader header;
        MainMenuContainer footerMenu;
        TextBox chooseNameTB;
        PictureBox choosePlayerCardPB;
        Button chooseNameB;

        public int withCode { get; set; } = 0;

        public ChooseNameForm(object parent) : base(
            parent: parent,
            name: "ChooseNameForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.25,
            movable: true
        )
        {

            // Screen header:
            this.header = new ScreenHeader(
                parent: this.Size,
                text: "בחר שם ותמונה",
                fontSize: 20F
            );
            this.Controls.Add(this.header);
            
            // Name Input:
            chooseNameTB = new TextBox()
            {
                Size = new Size(this.Width / 2, 100),
                Location = new Point(0, 75),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Varela Round", 16F, FontStyle.Bold),
            };
            Functions.CenterControlHorizontally(this, chooseNameTB);
            Controls.Add(chooseNameTB);

            // Button for avater selection:
            choosePlayerCardPB = new PictureBox()
            {
                Size = new Size(50, 50),
                Location = new Point(0, 125),
                Image = Image.FromFile(Globals.ServerPathToFile("Assets\\Icons", "addIcon.png")),
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
                Text = Globals.hostOrJoin == "HOST" ? "צור משחק" : "הצטרף למשחק",
                Font = new Font("Varela Round", 12F, FontStyle.Bold)
            };
            Functions.CenterControlHorizontally(this, chooseNameB);
            Controls.Add(chooseNameB);
            chooseNameB.Click += ChooseNameB_Click;

            this.SetPalyerAvatarSelection();

            // Footer Menu:
            this.footerMenu = new MainMenuContainer(
                parent: new Size(this.Width, this.Height - 15),
                alignment: "top-left",
                horizontalRatio: 0.24,
                fixedHeight: 55,
                totalItems: 2,
                itemPadding: 0
            );
            this.Controls.Add(this.footerMenu);

            // Back button:
            MainMenuItem backMenuItem = new MainMenuItem(
                text: "חזור",
                imagePath: Globals.ServerPathToFile("Assets\\Icons", "homeIcon.png"),
                fontSize: 11F
            );
            backMenuItem.ClickAny += BackButton_Click;
            this.footerMenu.AddItem(backMenuItem, 0);

            // Destruct data before leaving:
            this.FormClosing += (s, args) => {
                this.withCode = 0;
            };
        }

        private void SetPalyerAvatarSelection()
        {
            if (Globals.charImgSelected == null)
            {
                choosePlayerCardPB.Image = Image.FromFile(Globals.ServerPathToFile("Assets\\Icons", "addIcon.png"));
            }
            else
            {
                choosePlayerCardPB.Image = Globals.charImgSelected;
            }
        }
        
        private void ChooseNameB_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            bool isNameOk = true;
            bool isPictureOk = true;

            Regex nameRegex = new Regex("([A-Za-z])\\w+");

            if (!nameRegex.IsMatch(chooseNameTB.Text) && Functions.isNameAvailable(chooseNameTB.Text))
            {
                sb.AppendLine("שם לא תקין, השם צריך להיות רציף וללא רווחים, סימני פיסוק או שהשם תפוס");
                isNameOk = false;
            }

            if (Globals.charImgSelected == null)
            {
                sb.AppendLine("לא בחרת תמונה לשחקן");
                isPictureOk = false;
            }
            
            if (isNameOk && isPictureOk)
            {
                if (Globals.hostOrJoin == "HOST")
                {
                    CreateGameMessage createGameMessage = new CreateGameMessage(){
                        gameType        = Globals.gameChoosed,
                        playerName      = chooseNameTB.Text,
                        playerAvatar    = Globals.charTagSelected
                    };
                    
                    Globals.ServerConnector.SendMessage(
                        msg     : JsonConvert.SerializeObject(createGameMessage), 
                        purpose : "createGameLobby"
                    );
                    Thread.Sleep(100);
                }
                else if (Globals.hostOrJoin == "JOIN" && this.withCode > 0)
                {

                    JoinGameMessage joinGameMessage = new JoinGameMessage()
                    {
                        gameType        = Globals.gameChoosed,
                        playerName      = chooseNameTB.Text,
                        playerAvatar    = Globals.charTagSelected,
                        gameCode        = this.withCode
                    };

                    Globals.ServerConnector.SendMessage(
                        msg     : JsonConvert.SerializeObject(joinGameMessage),
                        purpose : "joinPlayerToGame"
                    );
                    Thread.Sleep(100);
                } else {

                    // Show Error message:
                    MessageBox.Show("אירעה שגיאה אנא נסה שנית לאחר אתחול המשחק", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.CloseAndBack();
                }

                //Globals.gameLobbyForm = new GameLobbyForm();
                //this.Hide();
                //Globals.gameLobbyForm.Show();
            }
            else
            {
                MessageBox.Show(sb.ToString(), "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                sb.Clear();
            }
        }

        private void ChoosePlayerCardPB_Click(object sender, EventArgs e)
        {
            ChoosePlayerCardForm choosePlayerCardForm = new ChoosePlayerCardForm();
            choosePlayerCardForm.FormClosed += (s, args) =>
            {
                this.SetPalyerAvatarSelection();
                this.Show();
            };
            this.Hide();
            choosePlayerCardForm.Show();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.CloseAndBack();
        }
    }
}
