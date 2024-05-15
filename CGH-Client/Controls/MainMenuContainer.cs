using System.Windows.Forms;
using System.Drawing;
using CGH_Client.Utility;

namespace CGH_Client.Controls
{
    internal class MainMenuContainer : ScaleablePanel
    {

        // Number of items in the main menu
        private int itemCount = 0;

        // Allowed number of items in the main menu
        private int maxItems = 4;

        private int itemWidth = 200;

        private int itemHeight = 200;

        private int itemPadding = 20;
        
        public MainMenuContainer(
            Size parent,
            string alignment = "center",
            double horizontalRatio = 0.7, 
            double verticalRatio = 0.3,
            int fixedWidth = -1,
            int fixedHeight = -1,
            int totalItems = 4, 
            int itemPadding = 20
        )
        {

            this.DoubleBuffered = true;
            
            // Set the size of the main menu container
            this.Size           = this.CalculateSize(parent, horizontalRatio, verticalRatio, fixedWidth, fixedHeight);
            this.Location       = this.AlignToParent(parent, alignment, this.Width, this.Height);
            this.BackColor      = Color.Transparent;
            this.maxItems       = totalItems;
            this.itemPadding    = itemPadding;

            // Adjust the item width and height based on the number of items
            this.itemWidth  = (this.Width - (this.maxItems - 1) * this.itemPadding) / this.maxItems;
            this.itemHeight = this.Height;

            // Layout debug:
            if (Globals.showLayoutDebug)
            {
                this.ShowDebug(true);
            }
        }

        public void AddItem(MainMenuItem item,  int slot)
        {
            if (this.itemCount < this.maxItems)
            {
                //item.Size = new Size(this.itemWidth, this.itemHeight);
                item.SetSize(this.itemWidth, this.itemHeight);
                item.Location = new Point(
                    x: slot * (this.itemWidth + this.itemPadding),
                    y: 0
                );
                
                if (Globals.showLayoutDebug)
                {
                    item.ShowDebug(true);
                }
                
                this.Controls.Add(item);
                this.itemCount++;
            }
        }

        public void ShowDebug(bool state)
        {
            if (state)
            {
                this.BorderStyle = BorderStyle.FixedSingle;
            }
            foreach (MainMenuItem item in this.Controls)
            {
                item.ShowDebug(state);
            }
        }
    }
}
