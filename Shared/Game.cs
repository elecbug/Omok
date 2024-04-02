using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
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
        public Color[,] Map { get; set; } = new Color[15, 15];
        public Color Turn { get; set; } = Color.Black;
        public Point LastMove { get; set; } = new Point();

        public void TurnSwap()
        {
            if (Turn == Color.White) Turn = Color.Black;
            else if (Turn == Color.Black) Turn = Color.White;
        }

        public bool WinCheck(int x, int y)
        {
            return Case1(x, y) || Case2(x, y) || Case3(x, y) || Case4(x, y);
        }

        private bool Case1(int x, int y, int line = 5)
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
        private bool Case2(int x, int y, int line = 5)
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
        private bool Case3(int x, int y, int line = 5)
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
        private bool Case4(int x, int y, int line = 5)
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

        public bool FailureMove(int x, int y)
        {
            return false;
        }
        //    Color color = Turn;

        //    if (color == Color.White)
        //    {
        //        return false;
        //    }

        //    bool result;

        //    Map[x, y] = color;

        //    result = AgamottoEye3(color, x, y) >= 2;

        //    Map[x, y] = Color.Empty;

        //    return result;
        //}

        //private int AgamottoEye3(Color color, int xx, int yy)
        //{
        //    int resultSum = 0;
        //    List<Point> usedPoint = new List<Point>();
        //    bool[] flag = { false, false, false, false };

        //    for (int x = Math.Max(0, xx - 3); x < Math.Min(Map.GetLength(0), xx + 4); x++)
        //    {
        //        for (int y = Math.Max(0, yy - 3); y < Math.Min(Map.GetLength(1), yy + 4); y++)
        //        {
        //            int result = 0;

        //            if (Map[x, y] != Color.Empty && usedPoint.FindIndex(p => p.X == x && p.Y == y) == -1)
        //            {
        //                continue;
        //            }

        //            Map[x, y] = color;

        //            List<Point> case1 = new List<Point>();
        //            List<Point> case2 = new List<Point>();
        //            List<Point> case3 = new List<Point>();
        //            List<Point> case4 = new List<Point>();

        //            int point1 = flag[0] ? 0 : Case1_33(x, y, case1);
        //            int point2 = flag[1] ? 0 : Case2_33(x, y, case2);
        //            int point3 = flag[2] ? 0 : Case3_33(x, y, case3);
        //            int point4 = flag[3] ? 0 : Case4_33(x, y, case4);

        //            result += point1 + point2 + point3 + point4;

        //            if (point1 == 1)
        //            {
        //                usedPoint.Concat(case1);
        //                flag[0] = true;
        //            }
        //            if (point2 == 1)
        //            {
        //                usedPoint.Concat(case2);
        //                flag[1] = true;
        //            }
        //            if (point3 == 1)
        //            {
        //                usedPoint.Concat(case3);
        //                flag[2] = true;
        //            }
        //            if (point4 == 1)
        //            {
        //                usedPoint.Concat(case4);
        //                flag[3] = true;
        //            }

        //            Map[x, y] = Color.Empty;

        //            resultSum += result;
        //        }
        //    }

        //    return resultSum;
        //}

        //private int Case1_33(int x, int y, List<Point> points)
        //{
        //    Color color = Map[x, y];

        //    int sum = 0;

        //    int last1x = x + 1, last1y = y;
        //    int last2x = x - 1, last2y = y;

        //    try
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (Map[x + i, y] == color)
        //            {
        //                sum++;
        //                last1x = x + i + 1;
        //                last1y = y;
        //                points.Add(new Point(x + i, y));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }
        //    try
        //    {
        //        for (int i = -1; i > -4; i--)
        //        {
        //            if (Map[x + i, y] == color)
        //            {
        //                sum++;
        //                last2x = x + i - 1;
        //                last2y = y;
        //                points.Add(new Point(x + i, y));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }

        //    if (sum >= 4)
        //    {
        //        int point = 0;

        //        try
        //        {
        //            point += Map[last1x, last1y] == Color.Empty ? 1 : 0;
        //            point += Map[last2x, last2y] == Color.Empty ? 1 : 0;
        //        }
        //        catch { }

        //        return point >= 2 ? 1 : 0;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //private int Case2_33(int x, int y, List<Point> points)
        //{
        //    Color color = Map[x, y];

        //    int sum = 0;

        //    int last1x = x, last1y = y + 1;
        //    int last2x = x, last2y = y - 1;

        //    try
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (Map[x, y + i] == color)
        //            {
        //                sum++;
        //                last1x = x;
        //                last1y = y + i + 1;
        //                points.Add(new Point(x, y + i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }
        //    try
        //    {
        //        for (int i = -1; i > -4; i--)
        //        {
        //            if (Map[x, y + i] == color)
        //            {
        //                sum++;
        //                last2x = x;
        //                last2y = y + i - 1;
        //                points.Add(new Point(x, y + i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }

        //    if (sum >= 4)
        //    {
        //        int point = 0;

        //        try
        //        {
        //            point += Map[last1x, last1y] == Color.Empty ? 1 : 0;
        //            point += Map[last2x, last2y] == Color.Empty ? 1 : 0;
        //        }
        //        catch { }

        //        return point >= 2 ? 1 : 0;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //private int Case3_33(int x, int y, List<Point> points)
        //{
        //    Color color = Map[x, y];

        //    int sum = 0;

        //    int last1x = x + 1, last1y = y + 1;
        //    int last2x = x - 1, last2y = y - 1;

        //    try
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (Map[x + i, y + i] == color)
        //            {
        //                sum++;
        //                last1x = x + i + 1;
        //                last1y = y + i + 1;
        //                points.Add(new Point(x + i, y + i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }
        //    try
        //    {
        //        for (int i = -1; i > -4; i--)
        //        {
        //            if (Map[x + i, y + i] == color)
        //            {
        //                sum++;
        //                last2x = x + i - 1;
        //                last2y = y + i - 1;
        //                points.Add(new Point(x + i, y + i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }

        //    if (sum >= 4)
        //    {
        //        int point = 0;

        //        try
        //        {
        //            point += Map[last1x, last1y] == Color.Empty ? 1 : 0;
        //            point += Map[last2x, last2y] == Color.Empty ? 1 : 0;
        //        }
        //        catch { }

        //        return point >= 2 ? 1 : 0;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //private int Case4_33(int x, int y, List<Point> points)
        //{
        //    Color color = Map[x, y];

        //    int sum = 0;

        //    int last1x = x + 1, last1y = y - 1;
        //    int last2x = x - 1, last2y = y + 1;

        //    try
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            if (Map[x + i, y - i] == color)
        //            {
        //                sum++;
        //                last1x = x + i + 1;
        //                last1y = y - i - 1;
        //                points.Add(new Point(x + i, y - i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }
        //    try
        //    {
        //        for (int i = -1; i > -4; i--)
        //        {
        //            if (Map[x + i, y - i] == color)
        //            {
        //                sum++;
        //                last2x = x + i - 1;
        //                last2y = y - i + 1;
        //                points.Add(new Point(x + i, y - i));
        //            }
        //            else break;
        //        }
        //    }
        //    catch { }

        //    if (sum >= 4)
        //    {
        //        int point = 0;

        //        try
        //        {
        //            point += Map[last1x, last1y] == Color.Empty ? 1 : 0;
        //            point += Map[last2x, last2y] == Color.Empty ? 1 : 0;
        //        }
        //        catch { }

        //        return point >= 2 ? 1 : 0;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
    }
}