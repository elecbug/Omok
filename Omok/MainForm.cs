using System.Net.Sockets;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Diagnostics;

namespace Omok
{
    public partial class MainForm : Form
    {
        private LoginForm LoginForm { get; set; }
        private int Id { get; set; } = 0;
        private Shared.Game.Color MyColor { get; set; }
        private LocationButton[,] Buttons { get; set; } = new LocationButton[15, 15];
        private const int EdgeSize = 50;
        private TcpClient? Client { get; set; }
        private Tuple<LocationButton, Shared.Game.Color>? LastMove { get; set; }
        private Tuple<LocationButton, Shared.Game.Color>? LastLastMove { get; set; }

        public MainForm(LoginForm form)
        {
            InitializeComponent();

            ClientSize = new Size(770, 770);
            AutoSizeMode = AutoSizeMode.GrowAndShrink;

            LoginForm = form;

            Closed += MainFormClosed;
            KeyDown += MainFormKeyDown;
            Load += MainFormLoad;
        }

        private void MainFormLoad(object? sender, EventArgs e)
        {
            new Thread(ReceiveLoop).Start();
        }

        private void MainFormKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Z)
            {
                string json = JsonSerializer.Serialize(new Shared.Packet.ReDo() 
                {
                    Id = Id,
                });
                byte[] buffer = Encoding.UTF8.GetBytes(json);
                Client!.GetStream().Write(buffer);
            }
        }

        private void MainFormClosed(object? sender, EventArgs e)
        {
            Client!.Close();
            LoginForm.Show();
        }

        private void ReceiveLoop()
        {
            try
            {
                Client = new TcpClient();
                Client.Connect(IPEndPoint.Parse(new StreamReader("ip-port").ReadToEnd()));
            }
            catch
            {
                Invoke(Close);
            }

            while (true)
            {
                try
                {
                    byte[] buffer = new byte[1024];
                    Client!.GetStream().Read(buffer);

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
                                        LastLastMove = Tuple.Create(LastMove.Item1, LastMove.Item2);
                                        LastMove.Item1.Image = GetImage(LastMove.Item2);
                                    }

                                    LastMove = Tuple.Create(Buttons[packet.X, packet.Y], GetColor(packet.Id));
                                    Buttons[packet.X, packet.Y].Image = GetMoveImage(packet.Id);
                                }

                                if (packet.WinColor != Shared.Game.Color.Empty)
                                {
                                    MessageBox.Show(packet.WinColor.ToString() + " is win!");

                                    Invoke(Close);
                                }
                            }
                            break;
                        case Shared.Packet.Type.ReDoOk:
                            {
                                Shared.Packet.ReDoOk packet
                                    = JsonSerializer.Deserialize<Shared.Packet.ReDoOk>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                                if (packet.Cancel == true)
                                {
                                    break;
                                }
                                else
                                {
                                    if (LastMove != null)
                                    {
                                        LastMove.Item1.Image = Resources.Image.Cross;

                                        LastMove = LastLastMove;
                                        LastLastMove = null;

                                        LastMove!.Item1.Image = GetReverseMoveImage(packet.Id);
                                    }
                                }
                            }
                            break;
                        case Shared.Packet.Type.EnemyFailure:
                            {
                                Shared.Packet.EnemyFailure packet
                                    = JsonSerializer.Deserialize<Shared.Packet.EnemyFailure>(Encoding.UTF8.GetString(buffer).Replace("\0", ""))!;

                                MessageBox.Show("Enemy has left!");

                                Invoke(Close);
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

        private Image GetReverseMoveImage(int id)
        {
            if (id != Id)
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.White_export;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.Black_export;
            }
            else
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.Black_export;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.White_export;
            }

            return Resources.Image.Cross;
        }

        private Shared.Game.Color GetColor(int id)
        {
            if (id == Id)
            {
                if (MyColor == Shared.Game.Color.White) return Shared.Game.Color.White;
                if (MyColor == Shared.Game.Color.Black) return Shared.Game.Color.Black;
            }
            else
            {
                if (MyColor == Shared.Game.Color.White) return Shared.Game.Color.Black;
                if (MyColor == Shared.Game.Color.Black) return Shared.Game.Color.White;
            }

            return Shared.Game.Color.Empty;

        }

        private Image GetMoveImage(int id)
        {
            if (id == Id)
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.White_export;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.Black_export;
            }
            else
            {
                if (MyColor == Shared.Game.Color.White) return Resources.Image.Black_export;
                if (MyColor == Shared.Game.Color.Black) return Resources.Image.White_export;
            }

            return Resources.Image.Cross;
        }

        private Image GetImage(Shared.Game.Color color)
        {
            if (color == Shared.Game.Color.Black)
            {
                return Resources.Image.Black;
            }
            else if (color == Shared.Game.Color.White)
            {
                return Resources.Image.White;
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
