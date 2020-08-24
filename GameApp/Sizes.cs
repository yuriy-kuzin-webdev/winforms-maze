using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp
{
    public static class Sizes
    {
        public const int Divider = 3; // borders thickness
        public const int MinimalCell = 20; // height or width of minimal generated area
        public const int Pass = 30; // size of passes
        public static int Player => Sizes.MinimalCell;
    }
}
