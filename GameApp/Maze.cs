using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp
{
    //Дополнительный класс для удобства конфигурации 
    //отказ от перечисления для лучшей читаемости кода
    //Используется в контроле плеера и в мейз билдере
    class Maze
    {
        Random rand;
        public GraphicsPath Path; // walls
        public Region Walls;
        #region ctors
        private Maze()
        {
            rand = new Random(DateTime.Now.Millisecond);
        }
        public Maze(Rectangle area) : this()
        {
            Path = new GraphicsPath();
            Path.FillMode = FillMode.Winding;
            Path.AddRectangles(new[]{ 
                Rectangle.FromLTRB(area.Left, area.Top,area.Right, area.Top + Sizes.Divider),
                Rectangle.FromLTRB(area.Right - Sizes.Divider, area.Top,area.Right, area.Bottom),
                Rectangle.FromLTRB(area.Left, area.Top, area.Left + Sizes.Divider, area.Bottom),
                Rectangle.FromLTRB(area.Left, area.Bottom - Sizes.Divider, area.Right, area.Bottom)
            });
            Walls = new Region(Path);
        }
        #endregion ctors
        #region mazeBuilder
        public void RecursiveDivideRect(Rectangle rect)
        {
            //Конец рекурсии при отсутствии дальнейшего разделения
            if (rect.Width > rect.Height)
                foreach (Rectangle divided in this.DivideRectByWidth(rect, Sizes.Divider))
                    RecursiveDivideRect(divided);
            else
                foreach (Rectangle divided in this.DivideRectByHeight(rect,Sizes.Divider))
                    RecursiveDivideRect(divided);
        }
        private IEnumerable<Rectangle> DivideRectByHeight(Rectangle rect,int dividerThickness)      // vertical divider
        {
            if (rect.Height <= 2 * Sizes.MinimalCell + Sizes.Divider)
                yield break;

            //Координата y для начала разделения
            int dividerTop = rand.Next(
                rect.Top + Sizes.MinimalCell, 
                rect.Bottom - Sizes.MinimalCell - Sizes.Divider);
            //Разделитель 
            Rectangle divider = new Rectangle(rect.Left, dividerTop, rect.Width, dividerThickness); //
            this.UpdatePath(this.DivideRectByWidth(divider, Sizes.Pass));
                

            //свободные для дальнейших итераций
            yield return Rectangle.FromLTRB(rect.Left, rect.Top, divider.Right, divider.Top);       // from rect LeftTop corner to divider RightTop corner
            yield return Rectangle.FromLTRB(divider.Left, divider.Bottom, rect.Right, rect.Bottom); // from divider LeftBottom corner to rect RightBottom corner
        }
        private IEnumerable<Rectangle> DivideRectByWidth(Rectangle rect,int dividerThickness)       // horizontal dividing
        {
            if (rect.Width <= 2 * Sizes.MinimalCell + dividerThickness)
                yield break;

            //Координата х для начала разделителя
            int dividerLeft = rand.Next(
                rect.Left + Sizes.MinimalCell, 
                rect.Right - Sizes.MinimalCell - dividerThickness);
            //Разделитель
            Rectangle divider = new Rectangle(dividerLeft, rect.Top, dividerThickness, rect.Height);// 
            this.UpdatePath(this.DivideRectByHeight(divider, Sizes.Pass));

            //Свободные для дальнейших итераций
            yield return Rectangle.FromLTRB(rect.Left, rect.Top, divider.Left, divider.Bottom);     // from rect LeftTop corner to divider LeftBottom corner
            yield return Rectangle.FromLTRB(divider.Right, divider.Top, rect.Right, rect.Bottom);   // from divider RightTop corner to rect RightBottom  
        }
        private void UpdatePath(IEnumerable<Rectangle> rects)
        {
            //Исключаем момент попадания разделителя на проход
            foreach (Rectangle rect in rects)
                if (!isClear(rect))
                    Path.AddRectangle(rect);
            Walls = new Region(Path);
        }
        #endregion mazeBuilder
        #region public methods
        public Point GetLocation(Size size,Rectangle area)
        {
            Rectangle pos;
            do
            {
                pos = new Rectangle(
                    rand.Next(area.Left + size.Width, area.Right - size.Width),
                    rand.Next(area.Top + size.Height, area.Bottom - size.Height),
                    size.Width,size.Height
                    );
            } while (!this.isClear(pos));
            return pos.Location;
        }
        public bool isClear(Rectangle rect)
            => !Walls.IsVisible(Rectangle.FromLTRB(rect.Left - 1, rect.Top - 1, rect.Right + 1, rect.Bottom + 1));
        #endregion public methods
    }
}
