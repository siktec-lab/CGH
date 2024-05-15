using System;
using System.Collections.Generic;
using System.Drawing;
using CGH_Client.Utility;
using System.Linq;
using System.Windows.Forms;

namespace CGH_Client.Controls
{
    internal class PlayerCard : ScaleablePanel
    {

        public Label header;
        public PictureBox avatar;
        public PictureBox pb;
        public Label yourTurn;
        public Button drawButton;

        public string currentCard = "None";
        
        public PlayerCard(Size parent, string side = "right")
        {
            this.DoubleBuffered = true;
            this.Size = this.CalculateSize(parent, 0.18, 0.55, -1, -1);
            this.Location = this.AlignToParent(
                parent : new Size(
                    (int)(parent.Width / 2), 
                    parent.Height
                ), 
                alignment: "center-" + side, 
                width: this.Width, 
                height: this.Height
            );
            this.BackColor = Color.Transparent;

            //Shift:
            this.Location = new Point() { X = this.Location.X + (int)(parent.Width / 4), Y = this.Location.Y };

            // Main label:
            this.header = new Label()
            {
                AutoSize = false,
                Size = new Size(this.Width - 40, 40),
                Text = "Player",
                Font = new Font("Varela Round", 16F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(40, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                RightToLeft = RightToLeft.Yes,
            };

            // Palyer Avatar:
            this.avatar = new PictureBox()
            {
                Size = new Size(40, 40),
                Location = new Point(0, 0),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent
            };
            
            // Player card:
            this.pb = new PictureBox()
            {
                Size = new Size(this.Width, this.Height - 40 - 40 - 80),
                Location = new Point(0, 40),
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = Color.Transparent,
            };

            // Draw Button:
            int buttonMargin = (int)((this.Width * .20) / 2);
            this.drawButton = new Button()
            {
                Size = new Size(this.Width - buttonMargin * 2, 80 - 20),
                Location = new Point(buttonMargin, this.Height - 40 - 80 + 10),
                Font = new Font("Varela Round", 12F, FontStyle.Bold),
                Text = "שלוף קלף",
                Enabled = false,
                Cursor = Cursors.Hand,
            };

            // Your Turn:
            this.yourTurn = new Label()
            {
                AutoSize = false,
                Size = new Size(this.Width, 40),
                Text = "תורך לשחק",
                Font = new Font("Varela Round", 20F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.OrangeRed,
                Location = new Point(0, this.Height - 40),
                TextAlign = ContentAlignment.MiddleCenter,
                RightToLeft = RightToLeft.Yes,
                Visible = false,
            };

            // Layout debug:
            if (Globals.showLayoutDebug)
            {
                this.BorderStyle          = BorderStyle.FixedSingle;
                this.header.BorderStyle   = BorderStyle.FixedSingle;
                this.avatar.BorderStyle   = BorderStyle.FixedSingle;
                this.pb.BorderStyle       = BorderStyle.FixedSingle;
                this.yourTurn.BorderStyle = BorderStyle.FixedSingle;
            }
            

            this.Controls.Add(this.header);
            this.Controls.Add(this.avatar);
            this.Controls.Add(this.pb);
            this.Controls.Add(this.drawButton);
            this.Controls.Add(this.yourTurn);

            // Initial:
            this.SetCard("None");
        }

        // Set Player:
        public void SetPlayer(Player p, bool onlyWatching = false, bool isTurn = false)
        {
            this.SetPlayer(p.Name, p.ImgCharNum);

            if (onlyWatching)
            {
                this.drawButton.Visible = false;
                this.yourTurn.Text = "ממתין למתמודד";
            }
            else
            {
                this.drawButton.Enabled = true;
                this.yourTurn.Visible = isTurn;
            }
            this.yourTurn.Visible = isTurn;
        }
        
        public void SetPlayer(string name, int avatarImgCharNum = 2)
        {
            this.header.Text = name;
            this.avatar.Image = Image.FromFile(Globals.ServerPathToFile("Assets\\Characters", "char_" + avatarImgCharNum + ".png"));
        }
     
        // Set Player Card:
        public void SetCard(string symbol = "None", string card = "None")
        {
            string folder = symbol.ToLower() == "none" || card.ToLower() == "none" ? "card_back" : symbol.ToLower();

            // Remove trailing s from folder name:
            if (folder.EndsWith("s"))
            {
                folder = folder.Substring(0, folder.Length - 1);
            }
            
            // Set card symbol:
            switch (folder) {
                case "card_back":
                    card = "cardBackGreen.png";
                    break;
                case "club":
                    symbol = "Clubs";
                    card = "cardClubs_" + card.ToUpper() + ".png";
                    break;
                case "diamond":
                    symbol = "Diamonds";
                    card = "cardDiamonds_" + card.ToUpper() + ".png";
                    break;
                case "heart":
                    symbol = "Hearts";
                    card = "cardHearts_" + card.ToUpper() + ".png";
                    break;
                case "spade":
                    symbol = "Spades";
                    card = "cardSpades_" + card.ToUpper() + ".png";
                    break;
            }

            if (this.currentCard != card)
            {
                this.currentCard = card;
                // Set image
                string imagePath = Globals.ServerPathToFile("Assets\\Standard_52_Cards\\" + folder, card);
                this.pb.Image = Image.FromFile(imagePath);
            }
        }
        
        public void SetCard(string card = "None")
        {
            card = card == "None" || card == "" ? "cardBackGreen" : card;
            string folder = "";
            if (card.StartsWith("cardClubs"))
            {
                folder = "club";
            }
            else if (card.StartsWith("cardSpades"))
            {
                folder = "spade";
            }
            else if (card.StartsWith("cardHearts"))
            {
                folder = "heart";
            }
            else if (card.StartsWith("cardDiamonds"))
            {
                folder = "diamond";
            }
            else if (card.StartsWith("cardBack"))
            {
                folder = "card_back";
            }
            // Extension:
            if (!card.EndsWith(".png"))
            {
                card += ".png";
            }
            
            // Set the card:
            if (this.currentCard != card)
            {
                this.currentCard = card;
                // Set image
                string imagePath = Globals.ServerPathToFile("Assets\\Standard_52_Cards\\" + folder, card);
                this.pb.Image = Image.FromFile(imagePath);
            }
        }
        
        // Set turn:
        public void SetTurn(bool enable)
        {
            // Enable the button:
            this.drawButton.Enabled = enable;
            // Show the label:
            this.yourTurn.Visible = enable;
        }
    }
}
