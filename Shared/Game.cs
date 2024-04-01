using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Game
    {
        public enum Color
        {
            Empty = 0,
            Black,
            White,
        }

        public int BlackId { get; set; } = 0;
        public int WhiteId { get; set; } = 0;
        public Color[,] Map = new Color[15, 15];
        public Color Turn = Color.Black;

        public void TurnSwap()
        {
            if (Turn == Color.White) Turn = Color.Black;
            else if (Turn == Color.Black) Turn = Color.White;
        }
    }
}
