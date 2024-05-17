using CGH_Client.Utility;
using CGH_Client.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class GameLobbyForm : BaseMovableForm
    {
        ScreenHeader header;
        MainMenuContainer footerMenu;
        Label lobbyCodeLB, codeLB;
        ListView joinedPlayers;
        Button startGameB;

        public bool isRefreshing = true;

        public GameLobbyForm(object parent) : base(
            parent: parent,
            name: "GameLobbyForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.7,
            movable: true
        )
        {
            // Screen header:
            this.header = new ScreenHeader(
                parent: this.Size,
                text: this.PrepareHeaderText("המשחק הנבחר", Globals.gameChoosed),
                fontSize: 20F
            );
            this.Controls.Add(this.header);

            // Lobby code label:
            this.lobbyCodeLB = new Label()
            {
                AutoSize = true,
                Text = ":הקוד ללובי",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 100)
            };
            Functions.CenterControlHorizontally(this, this.lobbyCodeLB);
            Controls.Add(this.lobbyCodeLB);

            // Lobby code value:
            this.codeLB = new Label()
            {
                AutoSize = true,
                Text = "",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 140)
            };
            Functions.CenterControlHorizontally(this, this.codeLB);
            this.Controls.Add(this.codeLB);

            // Players list:
            this.joinedPlayers = new ListView()
            {
                Size = new Size(300, 450),
                Location = new Point(0, 180),
                BorderStyle = BorderStyle.FixedSingle,
                View = View.Details,
                SmallImageList = new ImageList()
            };
            Functions.CenterControlHorizontally(this, this.joinedPlayers);
            this.Controls.Add(joinedPlayers);

            this.joinedPlayers.Columns.Add("Image", 150);
            this.joinedPlayers.Columns.Add("Player Name", 150);
            this.joinedPlayers.SmallImageList.ImageSize = new Size(100, 100);


            // Start game button:
            this.startGameB = new Button()
            {
                Size = new Size(300, 50),
                Location = new Point(0, 655),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Varela Round", 12F, FontStyle.Bold)
            };
            Functions.CenterControlHorizontally(this, this.startGameB);
            this.Controls.Add(this.startGameB);
            this.startGameB.Click += this.StartGameB_Click;

            // Footer Menu:
            this.footerMenu = new MainMenuContainer(
                parent: new Size(this.Width, this.Height - 15),
                alignment: "bottom-left",
                horizontalRatio: 0.3,
                fixedHeight: 80,
                totalItems: 5,
                itemPadding: 0
            );
            this.Controls.Add(this.footerMenu);

            // Back button:
            MainMenuItem backMenuItem = new MainMenuItem(
                text: "יציאה",
                imagePath: Globals.ServerPathToFile("Assets\\Icons", "homeIcon.png"),
                fontSize: 16F
            );
            backMenuItem.ClickAny += this.BackButton_Click;
            this.footerMenu.AddItem(backMenuItem, 0);

            // Form level events:
            this.FormClosing += this.GameLobbyForm_FormClosing;

            // Initial Refresh:
            this.RefreshLobby();

        }
        private string PrepareHeaderText(string headerTxt, string nameTxt)
        {
            // Concatenate the two and return
            return headerTxt + ": \"" + nameTxt + "\"";
        }
        
        private void StartGameB_Click(object sender, EventArgs e)
        {
            if (Globals.gameRoom is BaseGameRoom room)
            {
                if (Globals.hostOrJoin == "HOST" && room.IsGameReady())
                {
                    // Send start game request to server
                    Globals.ServerConnector.SendMessage(room.gameType + "-" + room.roomCode.ToString(), "startGameRoom");
                }
                else
                {
                    MessageBox.Show("רק מנהל הלובי יכול להתחיל את המשחק", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        public void RefreshLobby()
        {
            // Refresh Code:
            this.RefreshCode();
            // Refresh Players:
            this.RefreshPlayersList();
            // Refresh State:
            this.StartGameState();
        }

        public void RefreshPlayersList()
        {
            this.joinedPlayers.Items.Clear();
            if (Globals.gameRoom is BaseGameRoom room)
            {
                foreach (Player player in room.players)
                {
                    ListViewItem item = new ListViewItem();

                    // Load the image
                    Image image = Image.FromFile(Globals.ServerPathToFile("Assets\\Characters", "char_" + player.ImgCharNum + ".png"));
                    item.ImageIndex = joinedPlayers.SmallImageList.Images.Count;
                    this.joinedPlayers.SmallImageList.Images.Add(image);

                    // Add the player's name as the second sub-item
                    item.SubItems.Add(player.Name);

                    // Add the new item to the ListView
                    this.joinedPlayers.Invoke((MethodInvoker)delegate { this.joinedPlayers.Items.Add(item); });
                }
            }
        }

        public void RefreshCode()
        {
            if (Globals.gameRoom is BaseGameRoom room)
            {
                this.codeLB.Text = room.roomCode.ToString();
                Functions.CenterControlHorizontally(this, this.codeLB);
            }
        }

        private void StartGameState()
        {
            if (Globals.gameRoom is BaseGameRoom room)
            {
                if (!room.IsGameReady())
                {
                    this.startGameB.Enabled = false;
                    this.startGameB.Text = "ממתין לשחקנים נוספים...";

                } 
                else 
                {
                    Player me = room.GetPlayer(room.myPlayerIndex);
                    if (me.Name != "" && me.isHost)
                    {
                        this.startGameB.Enabled = true;
                        this.startGameB.Text = "הכל מוכן, התחל משחק";
                    } 
                    else
                    {
                        this.startGameB.Enabled = false;
                        this.startGameB.Text = "ממתין למארח שיתחיל את המשחק";
                    }
                }
                Functions.CenterControlHorizontally(this, this.startGameB);
            }
        }

        private void GameLobbyForm_FormClosing(object sender, EventArgs e)
        {
            if (Globals.gameRoom is BaseGameRoom room && !room.isGameStarted)
            {
                Globals.hostOrJoin = "None";
                room.RemoveFromGame();
            }
        }
        
        private void BackButton_Click(object sender, EventArgs e)
        {
            this.CloseAndBack();
        }
    }
}
