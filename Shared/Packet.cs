namespace Shared
{
    public class Packet
    {
        public enum Type
        {
            None,
            Connect,
            GameStart,
            ClientBehaviour,
            ReDo,
            ReDoOk,
        }

        public class Base
        {
            public Type Type { get; set; }
        }

        public class ClientBehaviour : Base
        {
            public ClientBehaviour()
            {
                Type = Type.ClientBehaviour;
            }

            public int Id { get; set; } = 0;

            public int X { get; set; } = -1;
            public int Y { get; set; } = -1;

            public bool Invalid { get; set; } = true;
            public Game.Color WinColor { get; set; } = Game.Color.Empty;
        }

        public class Connect : Base
        {
            public Connect()
            {
                Type = Type.Connect;
            }

            public int Id { get; set; } = 0;
        }

        public class GameStart : Base
        {
            public GameStart()
            {
                Type = Type.GameStart;
            }

            public int EnemyId { get; set; } = 0;
            public Game.Color MyColor { get; set; } = Game.Color.Empty;
        }

        public class ReDo: Base
        {
            public ReDo()
            {
                Type = Type.ReDo;
            }

            public int Id { get; set; } = 0;
        }

        public class ReDoOk : Base
        {
            public ReDoOk()
            {
                Type = Type.ReDoOk;
            }

            public int Id { get; set; } = 0;
            public bool Cancel { get; set; } = true;
        }
    }
}
