using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Diagnostics;

namespace Omok
{
    public partial class MainForm : Form
    {
        private int Id { get; set; } = 0;
        private Shared.Game.Color MyColor { get; set; }
        private LocationButton[,] Buttons { get; set; } = new LocationButton[15, 15];
        private const int EdgeSize = 50;
        private TcpClient? Client { get; set; }
        private LocationButton? LastMove { get; set; }

        public MainForm()
        {
            InitializeComponent();

            ClientSize = new Size(770, 770);

            new Thread(ReceiveLoop).Start();

            this.Closed += MainFormClosed;
        }

        private void MainFormClosed(object? sender, EventArgs e)
        {
            Client!.Close();
        }

        private void ReceiveLoop()
        {
            Client = new TcpClient();
            Client.Connect(IPEndPoint.Parse(new StreamReader("ip-port").ReadToEnd()));

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    Client.GetStream().Read(buffer);

                    Shared.Packet.Base json = JsonSerializer.Deserialize<Shared.Packet.Base>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                    switch (json.Type)
                    {
                        case Shared.Packet.Type.Connect:
                            {
                                Shared.Packet.Connect packet
                                    = JsonSerializer.Deserialize<Shared.Packet.Connect>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                                Id = packet.Id;
                            }
                            break;
                        case Shared.Packet.Type.GameStart:
                            {
                                Shared.Packet.GameStart packet
                                    = JsonSerializer.Deserialize<Shared.Packet.GameStart>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                                MyColor = packet.MyColor;

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
                                                Image = Resources.Image.Cross,
                                                SizeMode = PictureBoxSizeMode.Zoom,
                                                BackColor = Color.Wheat,
                                            };
                                            Buttons[x, y].Click += ButtonClick;
                                        }
                                    }
                                });
                            }
                            break;
                        case Shared.Packet.Type.ClientBehaviour:
                            {
                                Shared.Packet.ClientBehaviour packet
                                    = JsonSerializer.Deserialize<Shared.Packet.ClientBehaviour>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                                if (packet.Invalid == false)
                                {
                                    if (LastMove != null)
                                    {
                                        // LastMove.BorderStyle = BorderStyle.None;
                                    }

                                    LastMove = Buttons[packet.X, packet.Y];
                                    // LastMove.BorderStyle = BorderStyle.Fixed3D;

                                    Buttons[packet.X, packet.Y].Image = GetImage(packet.Id);
                                }

                                if (packet.WinColor != Shared.Game.Color.Empty)
                                {
                                    MessageBox.Show(packet.WinColor.ToString() + " is win!");

                                    Invoke(() => { Close(); });
                                }
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);

                    break;
                }
            }
        }

        private Image GetImage(int id)
        {
            if (id == Id)
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.White;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.Black;
            }
            else
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.Black;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.White;
            }

            return Resources.Image.Cross;
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

    public class LocationButton : PictureBox
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}
