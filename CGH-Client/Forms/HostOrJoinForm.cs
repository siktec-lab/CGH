using CGH_Client.Controls;
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
    public class HostOrJoinForm : BaseMovableForm
    {

        ScreenHeader header;
        MainMenuContainer mainMenu;
        MainMenuContainer footerMenu;
        
        public HostOrJoinForm(object parent, string GameNameDisplay = "לא ידוע") : base(
            parent: parent,
            name: "HostOrJoinForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.7,
            movable: true
        )
        {

            // Screen header:
            this.header = new ScreenHeader(
                parent      : this.Size,
                text        : this.PrepareHeaderText("המשחק הנבחר", GameNameDisplay),
                fontSize    : 20F
            );
            this.Controls.Add(this.header);

            // Main Menu of mini games:
            this.mainMenu = new MainMenuContainer(
                parent: this.Size,
                alignment: "center",
                horizontalRatio: 0.5,
                verticalRatio: 0.25,
                totalItems: 2,
                itemPadding: 60
            );

            // Join a game panel:
            MainMenuItem joinMenuItem = new MainMenuItem(
                text: "הצטרף לחדר",
                imagePath: Globals.ServerPathToFile("Assets\\Icons", "joinIcon.png")
            );
            joinMenuItem.ClickAny += JoinPanel_Click;
            this.mainMenu.AddItem(joinMenuItem, 0);
            this.Controls.Add(this.mainMenu);

            // Create a game panel:
            MainMenuItem createMenuItem = new MainMenuItem(
                text: "צור חדר",
                imagePath: Globals.ServerPathToFile("Assets\\Icons", "createIcon.png")
            );
            createMenuItem.ClickAny += CreatePanel_Click;
            this.mainMenu.AddItem(createMenuItem, 1);

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
                text: "חזור",
                imagePath: Globals.ServerPathToFile("Assets\\Icons", "homeIcon.png"),
                fontSize: 16F
            );
            backMenuItem.ClickAny += BackButton_Click;
            this.footerMenu.AddItem(backMenuItem, 0);

        }

        private string PrepareHeaderText(string headerTxt, string nameTxt)
        {
            // Concatenate the two and return
            return headerTxt + ": \"" + nameTxt + "\"";
        }
        
        private void JoinPanel_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "JOIN";
            EnterRoomCodeForm enterRoomCodeForm = new EnterRoomCodeForm(this);
            this.Hide();
            enterRoomCodeForm.Show();
        }

        private void CreatePanel_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "HOST";
            ChooseNameForm chooseNameForm = new ChooseNameForm(this);
            this.Hide();
            chooseNameForm.Show();
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Globals.hostOrJoin = "";
            this.CloseAndBack();
        }
    }
}
