using CGH_Client.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CGH_Client.Controls;
using System.Runtime.CompilerServices;

namespace CGH_Client.Forms
{
    public class SelectGameForm : BaseMovableForm
    {
        
        ScreenHeader header;
        
        public SelectGameForm() : base(
            parent: null,
            name: "SelectGameForm",
            backgroundImagePath: Globals.ServerPathToFile("Assets\\Backgrounds", "background_2.png"),
            scale: 0.7,
            movable: true
        )
        {
            // Main label:
            header = new ScreenHeader(this.Size, "מועדון משחקי הקלפים", 20F);
            
            // Main Menu of mini games:
            MainMenuContainer mainMenu = new MainMenuContainer(
                this.Size,
                alignment: "center",
                horizontalRatio: 0.6,
                verticalRatio: 0.4,
                totalItems: 3,
                itemPadding: 30
            );

            // UNO Game menu item:
            MainMenuItem gameUnoMenuItem = new MainMenuItem(
                text : "אונו",
                imagePath : Globals.ServerPathToFile("Assets\\Uno\\wild", "wild_card.png")
            );
            gameUnoMenuItem.ClickAny += UnoPanel_Click;
            mainMenu.AddItem(gameUnoMenuItem, 0);

            // WAR Game menu item:
            MainMenuItem gameWarMenuItem = new MainMenuItem(
                text: "מלחמה",
                imagePath: Globals.ServerPathToFile("Assets\\Standard_52_Cards\\heart", "cardHearts_K.png")
            );
            gameWarMenuItem.ClickAny += WarPanel_Click;
            mainMenu.AddItem(gameWarMenuItem, 1);

            // CHEAT Game menu item:
            MainMenuItem gameCheatMenuItem = new MainMenuItem(
                text: "צ'יט",
                imagePath: Globals.ServerPathToFile("Assets\\Standard_52_Cards\\spade", "cardSpades_A.png")
            );
            gameCheatMenuItem.ClickAny += CheatPanel_Click;
            mainMenu.AddItem(gameCheatMenuItem, 2);

            // Add all to the form:
            this.Controls.Add(header);
            this.Controls.Add(mainMenu);
        }
        
        private void CheatPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "Cheat";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm(this, "צ'יטים");
            this.Hide();
            hostOrJoinForm.Show();
        }

        private void WarPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "War";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm(this, "מלחמת קלפים");
            this.Hide();
            hostOrJoinForm.Show();
        }

        private void UnoPanel_Click(object sender, EventArgs e)
        {
            Globals.gameChoosed = "Uno";
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm(this, "אונו");
            this.Hide();
            hostOrJoinForm.Show();
        }
    }
}
