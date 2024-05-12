using CGH_Client.Utility;
using System;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace CGH_Client.Forms
{
    public class EnterRoomCodeForm : Form
    {

        Label enterRoomCodeLB;
        TextBox enterRoomCodeTB;
        Button enterRoomCodeB;

        public EnterRoomCodeForm()
        {
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(300, 225);
            BackgroundImage = Image.FromFile(Globals.baseDirectory + @"\Assets\Backgrounds\background_2.png");
            BackgroundImageLayout = ImageLayout.Center;
            FormBorderStyle = FormBorderStyle.None;

            enterRoomCodeLB = new Label()
            {
                AutoSize = true,
                Text = "הזן קוד משחק",
                Font = new Font("Varela Round", 18F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Location = new Point(0, 25)
            };
            Functions.CenterControlHorizontally(this, enterRoomCodeLB);
            Controls.Add(enterRoomCodeLB);

            enterRoomCodeTB = new TextBox()
            {
                Size = new Size(150, 50),
                Location = new Point(0, 75)
            };
            Functions.CenterControlHorizontally(this, enterRoomCodeTB);
            Controls.Add(enterRoomCodeTB);

            enterRoomCodeB = new Button()
            {
                Size = new Size(100, 50),
                Location = new Point(0, 150),
                Text = "להיכנס"
            };
            Functions.CenterControlHorizontally(this, enterRoomCodeB);
            Controls.Add(enterRoomCodeB);
            enterRoomCodeB.Click += EnterRoomCodeB_Click;

            this.FormClosed += EnterRoomCodeForm_FormClosed;
        }

        private void EnterRoomCodeB_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(enterRoomCodeTB.Text, "\\d{6}"))
            {
                MessageBox.Show("Room code not valid.");
            }

            else
            {
                Globals.gameCode = int.Parse(enterRoomCodeTB.Text);
                Globals.ServerConnector.SendMessage(Globals.gameChoosed + "-" + Globals.gameCode, "isRoomAvailable");

                while (true)
                {
                    if (Globals.isRoomAvailable == "True")
                    {
                        ChooseNameForm chooseNameForm = new ChooseNameForm();
                        this.Hide();
                        chooseNameForm.Show();
                        break;
                    }

                    if (Globals.isRoomAvailable == "False")
                    {
                        MessageBox.Show("Room is not exists");
                        break;
                    }

                    else
                    {
                        continue;
                    }
                }

            }
        }

        private void EnterRoomCodeForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            HostOrJoinForm hostOrJoinForm = new HostOrJoinForm();
            this.Hide();
            hostOrJoinForm.Show();
        }
    }
}
