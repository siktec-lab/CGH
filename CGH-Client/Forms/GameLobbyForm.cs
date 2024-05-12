using CGH_Client.Utility;
using CRS_Client.Networking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class GameLobbyForm : Form
    {

        Label mainLB, selectedGameLB, lobbyCodeLB, codeLB;
        ListView joinedPlayers;
        Button startGameB;

        public bool isRefreshing = true;

        public GameLobbyForm()
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

            lobbyCodeLB = new Label()
            {
                AutoSize = true,
                Text = ":הקוד ללובי",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 100)
            };
            Functions.CenterControlHorizontally(this, lobbyCodeLB);
            Controls.Add(lobbyCodeLB);

            codeLB = new Label()
            {
                AutoSize = true,
                Text = "",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 140)
            };
            codeLB.Text = Globals.gameCode.ToString();
            Functions.CenterControlHorizontally(this, codeLB);
            Controls.Add(codeLB);

            joinedPlayers = new ListView()
            {
                Size = new Size(300, 450),
                Location = new Point(0, 180),
                BorderStyle = BorderStyle.FixedSingle,
                View = View.Details,
                SmallImageList = new ImageList()
            };
            Functions.CenterControlHorizontally(this, joinedPlayers);
            Controls.Add(joinedPlayers);

            joinedPlayers.Columns.Add("Image", 150);
            joinedPlayers.Columns.Add("Player Name", 150);
            joinedPlayers.SmallImageList.ImageSize = new Size(100, 100);


            startGameB = new Button()
            {
                Size = new Size(100, 50),
                Location = new Point(0, 655),
                Text = "התחל משחק",
                FlatStyle = FlatStyle.Flat
            };
            Functions.CenterControlHorizontally(this, startGameB);
            Controls.Add(startGameB);
            startGameB.Hide();
            startGameB.Click += StartGameB_Click;

            new Thread(new ThreadStart(keepRefreshingLobby)).Start();

            this.FormClosed += GameLobbyForm_FormClosed;

        }

        private void StartGameB_Click(object sender, EventArgs e)
        {
            Globals.ServerConnector.SendMessage(Globals.globalGameRoom.gameType + "-" + Globals.globalGameRoom.roomCode, "gameStarted");
        }

        private void keepRefreshingLobby()
        {
            List<string> playerNames = new List<string>();
            List<string> playerNamesDisconnected = new List<string>();
            bool hasShown = false;

            while (isRefreshing)
            {
                Globals.ServerConnector.SendMessage(Globals.globalGameRoom.gameType + "-" + Globals.globalGameRoom.roomCode, "getGameLobby");

                Thread.Sleep(100);

                for (int i = 0; i < Globals.globalGameRoom.players.Count; i++)
                {
                    ListViewItem item = null;
                    Invoke((MethodInvoker)delegate
                    {
                        item = joinedPlayers.FindItemWithText(Globals.globalGameRoom.players[i].Name);
                    });

                    if (item == null)
                    {
                        // If the item doesn't exist, create a new item with an image and the player's name
                        ListViewItem newItem = new ListViewItem();

                        // Load the image
                        Image image = Image.FromFile(Globals.baseDirectory + @"\Assets\Characters\char_" + Globals.globalGameRoom.players[i].ImgCharNum + ".png");

                        // Set the image as the first sub-item
                        newItem.ImageIndex = joinedPlayers.SmallImageList.Images.Count;
                        joinedPlayers.SmallImageList.Images.Add(image);

                        // Add the player's name as the second sub-item
                        newItem.SubItems.Add(Globals.globalGameRoom.players[i].Name);

                        // Add the new item to the ListView
                        joinedPlayers.Invoke((MethodInvoker)delegate { joinedPlayers.Items.Add(newItem); });

                        playerNames.Add(Globals.globalGameRoom.players[i].Name);
                    }
                    else
                    {
                        // If the item exists, update its information
                        if (Globals.globalGameRoom.players[i].isDisconnected)
                        {
                            // Update the player's name
                            Invoke((MethodInvoker)delegate { item.SubItems[1].Text = Globals.globalGameRoom.players[i].Name + " (Disconnected)"; });

                            // Add the player's name to the list of player names if it's not already present
                            if (!playerNamesDisconnected.Contains(Globals.globalGameRoom.players[i].Name))
                            {
                                playerNamesDisconnected.Add(Globals.globalGameRoom.players[i].Name);
                            }
                        }
                        else
                        {
                            // Update the player's name
                            Invoke((MethodInvoker)delegate { item.SubItems[1].Text = Globals.globalGameRoom.players[i].Name; });

                            // Remove the player's name from the list of player names if it's present
                            if (playerNamesDisconnected.Contains(Globals.globalGameRoom.players[i].Name))
                            {
                                playerNamesDisconnected.Remove(Globals.globalGameRoom.players[i].Name);
                            }
                        }
                    }
                }

                // Show or hide the start game button based on the count of player names
                switch (Globals.hostOrJoin)
                {
                    case "HOST":
                        if (playerNames.Count > 1 && playerNamesDisconnected.Count == 0 && !hasShown)
                        {
                            Invoke((MethodInvoker)delegate { startGameB.Show(); });
                            hasShown = true;
                        }
                        else if (playerNamesDisconnected.Count >= 1 && hasShown)
                        {
                            Invoke((MethodInvoker)delegate { startGameB.Hide(); });
                            hasShown = false;
                        }
                        break;
                }


                Thread.Sleep(100);
            }
        }

        private void GameLobbyForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Player tempPlayer = new Player();
            tempPlayer.Name = Globals.charName;
            tempPlayer.ImgCharNum = Globals.charTagSelected;
            tempPlayer.gameID = Globals.globalGameRoom.gameType + "-" + Globals.globalGameRoom.roomCode;
            tempPlayer.isDisconnected = true;

            switch (Globals.hostOrJoin)
            {
                case "HOST":

                    tempPlayer.isHost = true;

                    break;

                case "JOIN":

                    tempPlayer.isHost = false;

                    break;
            }

            string msgToSend = JsonConvert.SerializeObject(tempPlayer);

            Globals.ServerConnector.SendMessage(msgToSend, "removeFromGame");
            Environment.Exit(0);
        }
    }
}
