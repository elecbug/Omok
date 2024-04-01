using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;

namespace Omok
{
    public partial class MainForm : Form
    {
        private int Id { get; set; } = 0;
        private Shared.Game.Color MyColor { get; set; }
        private LocationButton[,] Buttons { get; set; } = new LocationButton[15, 15];
        private const int EdgeSize = 50;
        private TcpClient? Client { get; set; }

        public MainForm()
        {
            InitializeComponent();

            ClientSize = new Size(770, 770);

            new Thread(ReceiveLoop).Start();
        }

        private void ReceiveLoop()
        {
            Client = new TcpClient();
            Client.Connect(IPEndPoint.Parse("127.0.0.1:12356"));

            while (true)
            {
                byte[] buffer = new byte[1024];
                Client.GetStream().Read(buffer);

                Shared.Packet.Base json = JsonSerializer.Deserialize<Shared.Packet.Base>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                switch (json.Type)
                {
                    case Shared.Packet.Type.Connect:
                        {
                            Shared.Packet.Connect json2
                                = JsonSerializer.Deserialize<Shared.Packet.Connect>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                            Id = json2.Id;
                        }
                        break;
                    case Shared.Packet.Type.GameStart:
                        {
                            Shared.Packet.GameStart json3
                                = JsonSerializer.Deserialize<Shared.Packet.GameStart>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                            MyColor = json3.MyColor;

                            Invoke(() =>
                            {
                                for (int x = 0; x < Buttons.GetLength(0); x++)
                                {
                                    for (int y = 0; y < Buttons.GetLength(1); y++)
                                    {
                                        Buttons[x, y] = new LocationButton()
                                        {
                                            X = x,
                                            Y = y,
                                            Parent = this,
                                            Visible = true,
                                            Size = new Size(EdgeSize, EdgeSize),
                                            Location = new Point(x * EdgeSize + 10, y * EdgeSize + 10),
                                        };
                                        Buttons[x, y].Click += ButtonClick;
                                    }
                                }
                            });
                        }
                        break;
                    case Shared.Packet.Type.ClientBehaviour:
                        {
                            Shared.Packet.ClientBehaviour json3
                                = JsonSerializer.Deserialize<Shared.Packet.ClientBehaviour>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                            if (json3.Invalid == false)
                            {
                                Buttons[json3.X, json3.Y].BackColor = GetColor(json3.Id);
                            }
                        }
                        break;
                }
            }
        }

        private Color GetColor(int id)
        {
            if (id == Id)
            {
                if (MyColor == Shared.Game.Color.White) return Color.White;
                if (MyColor == Shared.Game.Color.Black) return Color.Black;
            }
            else
            {
                if (MyColor == Shared.Game.Color.White) return Color.Black;
                if (MyColor == Shared.Game.Color.Black) return Color.White;
            }

            return Color.Red;
        }

        private void ButtonClick(object? sender, EventArgs e)
        {
            LocationButton button = (LocationButton)sender!;

            string json = JsonSerializer.Serialize(new Shared.Packet.ClientBehaviour()
            {
                Id = Id,
                X = button.X,
                Y = button.Y,
            });

            byte[] buffer = Encoding.UTF8.GetBytes(json);
            Client!.GetStream().Write(buffer);
        }
    }

    public class LocationButton : Button
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
