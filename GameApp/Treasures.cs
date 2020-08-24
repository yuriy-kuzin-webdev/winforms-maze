using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameApp
{
    public class Treasures : IEnumerable<PictureBox>
    {
        #region fields
        public List<PictureBox> treasuresCollection;
        public int treasuresCount;
        Image sapphireImage;
        GraphicsPath treasuresPath;
        Region treasuresRegion;
        #endregion fields

        #region ctor
        public Treasures(int count)
        {
            treasuresPath = new GraphicsPath();
            treasuresCollection = new List<PictureBox>();
            treasuresCount = count;

            Image image = Image.FromFile(@"sapphire.png");
            sapphireImage = new Bitmap(image, Sizes.Player, Sizes.Player);

            InitializeCollection();
        }
        #endregion ctor

        #region initializer
        private void InitializeCollection()
        {
            while (treasuresCollection.Count != treasuresCount)
            {
                PictureBox pBox = new PictureBox();
                pBox.Size = new Size(Sizes.Player, Sizes.Player);
                pBox.SizeMode = PictureBoxSizeMode.CenterImage;
                pBox.Image = this.sapphireImage;
                pBox.LocationChanged += PBox_LocationChanged;
                treasuresCollection.Add(pBox);
            }
        }
        #endregion initializer

        private void PBox_LocationChanged(object sender, EventArgs e)
        {
            PictureBox pBox = (PictureBox)sender;
            Rectangle area = new Rectangle(pBox.Location, pBox.Size);
            treasuresPath.AddRectangle(area);
            treasuresRegion = new Region(treasuresPath);
        }

        public bool isTaken(Rectangle rect)
            => treasuresRegion.IsVisible(rect.X + Sizes.Player/2,rect.Y + Sizes.Player/2);
        public void Remove(PictureBox treasure)
        {
            treasuresRegion.Exclude(treasure.Bounds);
            treasuresCollection.Remove(treasure);
            treasuresPath.Reset();
            if (treasuresCollection.Count != 0) 
                treasuresPath.AddRectangles(treasuresCollection.Select(tr => tr.Bounds).ToArray());
        }
        #region interface
        IEnumerator IEnumerable.GetEnumerator()
        {
            return treasuresCollection.GetEnumerator();
        }

        public IEnumerator<PictureBox> GetEnumerator()
        {
            return treasuresCollection.GetEnumerator();
        }
        #endregion interface
    }
}
