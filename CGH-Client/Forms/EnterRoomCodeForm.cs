using CGH_Client.Controls;
using CGH_Client.Utility;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class EnterRoomCodeForm : BaseMovableForm
    {
        
        ScreenHeader header;
        MainMenuContainer footerMenu;
        TextBox enterRoomCodeTB;
        Button enterRoomCodeB;
        
        public int selectedRoomCode { get; private set; }
        
        public EnterRoomCodeForm(object parent) : base(
            parent: parent,
            name: "EnterRoomCodeForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.25,
            movable: true
        )
        {

            // Screen header:
            this.header = new ScreenHeader(
                parent: this.Size,
                text: "הזן קוד משחק",
                fontSize: 20F
            );
            this.Controls.Add(this.header);

            // Code Input:
            enterRoomCodeTB = new TextBox()
            {
                Size = new Size(this.Width / 2, 100),
                Location = new Point(0, 75),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Varela Round", 16F, FontStyle.Bold),
            };
            Functions.CenterControlHorizontally(this, enterRoomCodeTB);
            Controls.Add(enterRoomCodeTB);

            // Login Button:
            enterRoomCodeB = new Button()
            {
                Size = new Size(100, 50),
                Location = new Point(0, 150),
                Text = "הצטרף",
                Font = new Font("Varela Round", 12F, FontStyle.Bold),
            };
            Functions.CenterControlHorizontally(this, enterRoomCodeB);
            Controls.Add(enterRoomCodeB);
            enterRoomCodeB.Click += EnterRoomCodeB_Click;

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
                this.selectedRoomCode = 0;
            };
        }

        private int ValidateCode(string code)
        {
            // Code should be at least 4 characters long, Code should contain only numbers:
            if (code.Length < 4)
            {
                MessageBox.Show("קוד חייב להיות באורך של 4 תווים לפחות", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            else if (!Regex.IsMatch(code, @"^[0-9]+$"))
            {
                MessageBox.Show("קוד יכול להכיל רק מספרים", "שגיאה", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            // Code is valid return the code as an integer:
            return int.Parse(code);
        }
        private void EnterRoomCodeB_Click(object sender, EventArgs e)
        {

            this.selectedRoomCode = this.ValidateCode(enterRoomCodeTB.Text);
            if (this.selectedRoomCode == 0)
            {
                return;
            }

            // Send availabilty check:
            Globals.ServerConnector.SendMessage(Globals.gameChoosed + "-" + this.selectedRoomCode, "isRoomAvailable");
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "None";
            this.CloseAndBack();
        }
    }
}
