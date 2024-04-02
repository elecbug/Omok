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

        public bool WinCheck(int x, int y)
        {
            return Horizontal(x, y) || Vertical(x, y) || LeftUp(x, y) || RightUp(x, y);
        }

        private bool Horizontal(int x, int y, int line = 5)
        {
            Color color = Map[x, y];

            int sum = 0;

            try
            {
                for (int i = 0; i < line; i++)
                {
                    if (Map[x + i, y] == color) sum++;
                    else break;
                }
            }
            catch { }
            try
            {
                for (int i = -1; i > -line; i--)
                {
                    if (Map[x + i, y] == color) sum++;
                    else break;
                }
            }
            catch { }

            return sum >= line;
        }

        private bool Vertical(int x, int y, int line = 5)
        {
            Color color = Map[x, y];

            int sum = 0;

            try
            {
                for (int i = 0; i < line; i++)
                {
                    if (Map[x, y + i] == color) sum++;
                    else break;
                }
            }
            catch { }
            try
            {
                for (int i = -1; i > -line; i--)
                {
                    if (Map[x, y + i] == color) sum++;
                    else break;
                }
            }
            catch { }

            return sum >= line;
        }

        private bool LeftUp(int x, int y, int line = 5)
        {
            Color color = Map[x, y];

            int sum = 0;

            try
            {
                for (int i = 0; i < line; i++)
                {
                    if (Map[x + i, y + i] == color) sum++;
                    else break;
                }
            }
            catch { }
            try
            {
                for (int i = -1; i > -line; i--)
                {
                    if (Map[x + i, y + i] == color) sum++;
                    else break;
                }
            }
            catch { }

            return sum >= line;
        }

        private bool RightUp(int x, int y, int line = 5)
        {
            Color color = Map[x, y];

            int sum = 0;

            try
            {
                for (int i = 0; i < line; i++)
                {
                    if (Map[x + i, y - i] == color) sum++;
                    else break;
                }
            }
            catch { }
            try
            {
                for (int i = -1; i > -line; i--)
                {
                    if (Map[x + i, y - i] == color) sum++;
                    else break;
                }
            }
            catch { }

            return sum >= line;
        }

        public bool OpenThree(int x, int y)
        {
            /// 33이 필요해요
            /// 

            return false;
        }
    }
}
