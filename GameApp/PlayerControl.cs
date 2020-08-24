using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace GameApp
{
    public partial class PlayerControl : PictureBox
    {
        public PlayerControl() : base()
        {
            InitializeComponent();
            //additional image manipulation
            Image image = Image.FromFile(@"pacman.png");
            Image = new Bitmap(image, Sizes.Player, Sizes.Player);
            DoubleBuffered = true;
            SizeMode = PictureBoxSizeMode.CenterImage;

            GraphicsPath playerPath;
            playerPath = new GraphicsPath();
            playerPath.AddEllipse(DisplayRectangle);
            Region = new Region(playerPath);

        }
        public void updateLocation(Point move)
        {
            Location = new Point(Left + move.X, Top + move.Y);
            Region.Translate(move.X, move.Y);
        }
    }
}
