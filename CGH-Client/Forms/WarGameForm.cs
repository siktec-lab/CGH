using CGH_Client.Controls;
using CGH_Client.Networking.Messages;
using CGH_Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class WarGameForm : BaseMovableForm
    {

        ScreenHeader header;
        MainMenuContainer footerMenu;
        PlayerCard player1, player2;
        WarGameRoom room;
        Player me;
        Player enemy;

        public WarGameForm() : base(
            parent: null,
            name: "WarGameForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.7,
            movable: true
        ) {

            // Set the room:
            this.room = (WarGameRoom)Globals.gameRoom;

            // Set me:
            this.me = room.GetMyPlayer();
            this.enemy = room.GetOpponents(1)[0];
            
            // Screen header:
            this.header = new ScreenHeader(
                parent: this.Size,
                text: this.PrepareHeaderText("משחקים", Globals.gameChoosed),
                fontSize: 20F
            );
            this.Controls.Add(this.header);

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

            // Player 1:
            player1 = new PlayerCard(
                parent: this.Size,
                side: "right"
            );
            this.Controls.Add(player1);
            this.player1.drawButton.Click += this.DrawButton_Click;
            // Player 2:
            player2 = new PlayerCard(
                parent: this.Size,
                side: "left"
            );
            this.Controls.Add(player2);

            // Players:
            this.SetPlayers();
            
            // Form closing event:
            this.FormClosing += WarGameForm_FormClosing;
        }

        private string PrepareHeaderText(string headerTxt, string nameTxt)
        {
            // Concatenate the two and return
            return headerTxt + ": \"" + nameTxt + "\"";
        }
        
        public void PerfomTurn(WarRoundResponse round)
        {
            // Update players:
            bool changedPlayers = false;
            if (round.enemyName != "" && this.enemy.Name != round.enemyName)
            {
                this.enemy = this.room.GetPlayer(round.enemyName);
                changedPlayers = true;
            }
            if (round.yourName != "" && this.me.Name != round.yourName)
            {
                this.me = this.room.GetPlayer(round.yourName);
                changedPlayers = true;
            }
            if (changedPlayers)
            {
                this.SetPlayers();
            }

            // Update Cards:
            this.player1.SetCard(round.yourCard);
            this.player2.SetCard(round.enemyCard);

            // If the round is over:
            if (!round.isRoundOver)
            {
                if (round.yourCard != "")
                {
                    this.player1.SetTurn(false);
                } 
                else
                {
                    this.player1.SetTurn(true);
                }
                if (round.enemyCard != "")
                {
                    this.player2.SetTurn(false);
                } 
                else
                {
                    this.player2.SetTurn(true);
                }
            }
            else
            {
                Task.Delay(3000).ContinueWith(t =>
                {
                    this.player1.SetTurn(true);
                    this.player2.SetTurn(true);
                    this.player1.SetCard("None");
                    this.player2.SetCard("None");

                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            //TODO: update scores:
            
            //TODO: Game finished:
            
        }
        public void SetPlayers()
        {
            player1.SetPlayer(this.me, false, true);
            player2.SetPlayer(this.enemy, true, true);
        }

        public void DrawButton_Click(object sender, EventArgs e)
        {
            this.player1.SetTurn(false);
            GameRoundMessage message = new GameRoundMessage(){
                playerName = this.me.Name,
                gameType   = this.room.gameType,
                gameCode   = this.room.roomCode,
                action     = "turn"
            };
            Globals.ServerConnector.SendMessage(JsonConvert.SerializeObject(message), "gameTurn");
        }
        
        
        private void WarGameForm_FormClosing(object sender, EventArgs e)
        {
            if (Globals.gameRoom is BaseGameRoom room)
            {
                MessageBox.Show("Closing????");
                room.RemoveFromGame();
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            this.CloseAndBack();
        }
    }
}
