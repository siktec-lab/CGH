using CGH_Client.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CGH_Client.Controls
{
    internal class ScoreCounter : ScaleablePanel
    {

        public Tuple<int, int> score { get; set; }
        public Label scoreLabel;

        public ScoreCounter(Size parent, int fixedHeight = 80, int offsetTop = 80)
        {
            this.BackColor = Color.Transparent;
            this.ResetScore();
            this.Size = this.CalculateSize(parent, 0.4, 0, -1, fixedHeight);
            Point position =  this.AlignToParent(parent, "top-center", this.Width, this.Height);
            // Adjust:
            position = new Point() { X = position.X, Y = position.Y + offsetTop };
            this.Location = position;

            // Main label:
            this.scoreLabel = new Label()
            {
                AutoSize = false,
                Size = new Size(this.Width, this.Height),
                Text = "0 : 0",
                Font = new Font("Varela Round", 30F, FontStyle.Bold),
                BackColor = Color.Transparent,
                ForeColor = Color.Yellow,
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter,
                RightToLeft = RightToLeft.Yes,
            };
            this.Controls.Add(this.scoreLabel);
            
            // Layout debug:
            if (Globals.showLayoutDebug)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
                this.scoreLabel.BorderStyle = BorderStyle.FixedSingle;
            }
        }

        public void AddScore(int side, int amount)
        {
            if (side == 0)
            {
                this.score = new Tuple<int, int>(this.score.Item1 + amount, this.score.Item2);
            }
            else
            {
                this.score = new Tuple<int, int>(this.score.Item1, this.score.Item2 + amount);
            }
        }

        public void RefreshScore()
        {
            this.scoreLabel.Text = this.score.Item1 + " : " + this.score.Item2;
        }
        
        public void ResetScore(bool refresh = false)
        {
            this.score = new Tuple<int, int>(0, 0);
            if (refresh)
            {
                this.RefreshScore();
            }
        }

        public void SetScore(int side0, int side1, bool refresh = false)
        {
            this.SetScore(new Tuple<int, int>(side0, side1), refresh);
        }
        
        public void SetScore(Tuple<int, int> score, bool refresh = false)
        {
            this.score = score;
            if (refresh)
            {
                this.RefreshScore();
            }
        }

        public Tuple<int, int> GetScore()
        {
            return this.score;
        }
    }
}
