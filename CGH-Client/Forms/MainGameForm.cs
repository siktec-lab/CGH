using CGH_Client.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class MainGameForm : Form
    {

        List<PictureBox> pictureBoxesCards = new List<PictureBox>();
        Panel firstPlayerP, secondPlayerP, thirdPlayerP, fourthPlayerP;
        PictureBox firstPlayerPB, secondPlayerPB, thirdPlayerPB, fourthPlayerPB, firstPlayerCardPB, secondPlayerCardPB, thirdPlayerCardPB, fourthPlayerCardPB, openCardPB, closeBtnPB;
        Label firstPlayerLB, secondPlayerLB, thirdPlayerLB, fourthPlayerLB, openCardLB, currentPlayerTurnLB, currentPlayerTurn;

        public MainGameForm()
        {

            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(1495, 840);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            FormBorderStyle = FormBorderStyle.None;

            firstPlayerP = new Panel()
            {
                Size = new Size(200, 250),
                Location = new Point(322, 295),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(firstPlayerP);

            firstPlayerPB = new PictureBox()
            {
                Size = new Size(50, 50),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[0].ImgCharNum + ".png")
            };
            firstPlayerP.Controls.Add(firstPlayerPB);

            firstPlayerLB = new Label()
            {
                AutoSize = true,
                Location = new Point(70, 17),
                Text = Globals.globalGameRoom.players[0].Name
            };
            Functions.CenterControlHorizontally(firstPlayerP, firstPlayerLB);
            firstPlayerP.Controls.Add(firstPlayerLB);

            firstPlayerCardPB = new PictureBox()
            {
                Size = new Size(175, 175),
                Location = new Point(0, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png")
            };
            Functions.CenterControlHorizontally(firstPlayerP, firstPlayerCardPB);
            firstPlayerP.Controls.Add(firstPlayerCardPB);
            pictureBoxesCards.Add(firstPlayerCardPB);

            secondPlayerP = new Panel()
            {
                Size = new Size(200, 250),
                Location = new Point(973, 295),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(secondPlayerP);

            secondPlayerPB = new PictureBox()
            {
                Size = new Size(50, 50),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[1].ImgCharNum + ".png")
            };
            secondPlayerP.Controls.Add(secondPlayerPB);

            secondPlayerLB = new Label()
            {
                AutoSize = true,
                Location = new Point(70, 17),
                Text = Globals.globalGameRoom.players[1].Name
            };
            Functions.CenterControlHorizontally(secondPlayerP, secondPlayerLB);
            secondPlayerP.Controls.Add(secondPlayerLB);

            secondPlayerCardPB = new PictureBox()
            {
                Size = new Size(175, 175),
                Location = new Point(0, 60),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png")
            };
            Functions.CenterControlHorizontally(secondPlayerP, secondPlayerCardPB);
            secondPlayerP.Controls.Add(secondPlayerCardPB);
            pictureBoxesCards.Add(secondPlayerCardPB);

            openCardLB = new Label()
            {
                AutoSize = true,
                Text = "פתח קלף",
                Location = new Point(1300, 300),
                Font = new Font("Varela Round", 15F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            Controls.Add(openCardLB);

            openCardPB = new PictureBox()
            {
                Size = new Size(175, 175),
                Location = new Point(1262, 365),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png"),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            Controls.Add(openCardPB);
            openCardPB.Click += OpenCardPB_Click;

            currentPlayerTurnLB = new Label()
            {
                AutoSize = true,
                Text = ":התור הנוכחי",
                Location = new Point(0, 300),
                Font = new Font("Varela Round", 15F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            Functions.CenterControlHorizontally(this, currentPlayerTurnLB);
            Controls.Add(currentPlayerTurnLB);

            currentPlayerTurn = new Label()
            {
                AutoSize = true,
                Text = Globals.globalGameRoom.currentPlayerTurn,
                Location = new Point(0, 350),
                Font = new Font("Varela Round", 13F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White
            };
            Functions.CenterControlHorizontally(this, currentPlayerTurn);
            Controls.Add(currentPlayerTurn);

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

            createThirdAndFourthPlayers();

            new Thread(new ThreadStart(updatePlayersCards)).Start();

            this.FormClosed += MainGameForm_FormClosed;

        }

        private void CloseBtnPB_Click(object sender, EventArgs e)
        {
            Globals.ServerConnector.SendMessage(Globals.gameChoosed + "-" + Globals.gameCode, "deleteGame");

            Thread.Sleep(3000);

            Environment.Exit(0);
        }

        private void OpenCardPB_Click(object sender, EventArgs e)
        {
            if (Globals.globalGameRoom.currentPlayerTurn == Globals.charName)
            {
                Globals.ServerConnector.SendMessage(Globals.charName + "{0}" + Globals.gameChoosed + "-" + Globals.gameCode, "changeSelectedCard");
            }

            else
            {
                MessageBox.Show("זה לא התור שלך");
            }
        }

        private void updatePlayersCards()
        {
            while (true)
            {
                Globals.ServerConnector.SendMessage(Globals.globalGameRoom.gameType + "-" + Globals.globalGameRoom.roomCode, "getGameLobby");

                Thread.Sleep(100);

                for (int i = 0; i < Globals.globalGameRoom.players.Count; i++)
                {
                    string selectedCard = Globals.globalGameRoom.players[i].selectedCard;
                    string[] cardInfo = selectedCard.Split('_'); // Split the selected card string

                    string typeOfCard = cardInfo[0]; // Get the type of card

                    switch (typeOfCard)
                    {
                        case "cardClubs":
                            pictureBoxesCards[i].Image = Image.FromFile(Path.Combine(Globals.baseDirectory, @"Assets\Standard_52_Cards\club", selectedCard + ".png"));
                            break;

                        case "cardDiamonds":
                            pictureBoxesCards[i].Image = Image.FromFile(Path.Combine(Globals.baseDirectory, @"Assets\Standard_52_Cards\diamond", selectedCard + ".png"));
                            break;

                        case "cardHearts":
                            pictureBoxesCards[i].Image = Image.FromFile(Path.Combine(Globals.baseDirectory, @"Assets\Standard_52_Cards\heart", selectedCard + ".png"));
                            break;

                        case "cardSpades":
                            pictureBoxesCards[i].Image = Image.FromFile(Path.Combine(Globals.baseDirectory, @"Assets\Standard_52_Cards\spade", selectedCard + ".png"));
                            break;

                        default:
                            pictureBoxesCards[i].Image = Image.FromFile(Path.Combine(Globals.baseDirectory, @"Assets\Standard_52_Cards\card_back\cardBackGreen.png"));
                            break;
                    }
                }

                currentPlayerTurn.Text = Globals.globalGameRoom.currentPlayerTurn;

                Thread.Sleep(100);
            }
        }

        private void createThirdAndFourthPlayers()
        {
            if (Globals.globalGameRoom.players.Count > 2)
            {
                if (Globals.globalGameRoom.players.Count == 3)
                {
                    thirdPlayerP = new Panel()
                    {
                        Size = new Size(200, 250),
                        Location = new Point(0, 40),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    Functions.CenterControlHorizontally(this, thirdPlayerP);
                    Controls.Add(thirdPlayerP);

                    thirdPlayerPB = new PictureBox()
                    {
                        Size = new Size(50, 50),
                        Location = new Point(0, 0),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[2].ImgCharNum + ".png")
                    };
                    thirdPlayerP.Controls.Add(thirdPlayerPB);

                    thirdPlayerLB = new Label()
                    {
                        AutoSize = true,
                        Location = new Point(70, 17),
                        Text = Globals.globalGameRoom.players[2].Name
                    };
                    Functions.CenterControlHorizontally(thirdPlayerP, thirdPlayerLB);
                    thirdPlayerP.Controls.Add(thirdPlayerLB);

                    thirdPlayerCardPB = new PictureBox()
                    {
                        Size = new Size(175, 175),
                        Location = new Point(0, 60),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png")
                    };
                    Functions.CenterControlHorizontally(thirdPlayerP, thirdPlayerCardPB);
                    thirdPlayerP.Controls.Add(thirdPlayerCardPB);
                    pictureBoxesCards.Add(thirdPlayerCardPB);
                }

                if (Globals.globalGameRoom.players.Count == 4)
                {
                    thirdPlayerP = new Panel()
                    {
                        Size = new Size(200, 250),
                        Location = new Point(0, 40),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    Functions.CenterControlHorizontally(this, thirdPlayerP);
                    Controls.Add(thirdPlayerP);

                    thirdPlayerPB = new PictureBox()
                    {
                        Size = new Size(50, 50),
                        Location = new Point(0, 0),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[2].ImgCharNum + ".png")
                    };
                    thirdPlayerP.Controls.Add(thirdPlayerPB);

                    thirdPlayerLB = new Label()
                    {
                        AutoSize = true,
                        Location = new Point(70, 17),
                        Text = Globals.globalGameRoom.players[2].Name
                    };
                    Functions.CenterControlHorizontally(thirdPlayerP, thirdPlayerLB);
                    thirdPlayerP.Controls.Add(thirdPlayerLB);

                    thirdPlayerCardPB = new PictureBox()
                    {
                        Size = new Size(175, 175),
                        Location = new Point(0, 60),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png")
                    };
                    Functions.CenterControlHorizontally(thirdPlayerP, thirdPlayerCardPB);
                    thirdPlayerP.Controls.Add(thirdPlayerCardPB);
                    pictureBoxesCards.Add(thirdPlayerCardPB);

                    fourthPlayerP = new Panel()
                    {
                        Size = new Size(200, 250),
                        Location = new Point(0, 550),
                        BorderStyle = BorderStyle.FixedSingle
                    };
                    Functions.CenterControlHorizontally(this, fourthPlayerP);
                    Controls.Add(fourthPlayerP);

                    fourthPlayerPB = new PictureBox()
                    {
                        Size = new Size(50, 50),
                        Location = new Point(0, 0),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[3].ImgCharNum + ".png")
                    };
                    fourthPlayerP.Controls.Add(fourthPlayerPB);

                    fourthPlayerLB = new Label()
                    {
                        AutoSize = true,
                        Location = new Point(70, 17),
                        Text = Globals.globalGameRoom.players[3].Name
                    };
                    Functions.CenterControlHorizontally(fourthPlayerP, fourthPlayerLB);
                    fourthPlayerP.Controls.Add(fourthPlayerLB);

                    fourthPlayerCardPB = new PictureBox()
                    {
                        Size = new Size(175, 175),
                        Location = new Point(0, 60),
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = Image.FromFile(Globals.baseDirectory + @"\Assets\Standard_52_Cards\card_back\cardBackGreen.png")
                    };
                    Functions.CenterControlHorizontally(fourthPlayerP, fourthPlayerCardPB);
                    fourthPlayerP.Controls.Add(fourthPlayerCardPB);
                    pictureBoxesCards.Add(fourthPlayerCardPB);
                }
            }
        }

        private void MainGameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Globals.ServerConnector.SendMessage(Globals.gameChoosed + "-" + Globals.gameCode, "deleteGame");

            Thread.Sleep(3000);

            Environment.Exit(0);
        }
    }
}
