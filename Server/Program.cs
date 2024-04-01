using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server
{
    public class Program
    {
        public static TcpListener? Listener;
        public static List<User> Users = new List<User>();

        public static List<Shared.Game> Games = new List<Shared.Game>();

        public static void Main(string[] args)
        {
            Listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 12356);

            AcceptLoop();
        }

        public static void AcceptLoop()
        {
            Listener!.Start();

            Console.WriteLine(Listener.LocalEndpoint);

            while (true)
            {
                Socket socket = Listener!.AcceptSocket();

                Console.WriteLine(socket.LocalEndPoint);

                Shared.Packet.Connect packet = new Shared.Packet.Connect()
                {
                    Id = new Random().Next(DateTime.Now.Microsecond),
                };

                string json = JsonSerializer.Serialize(packet);

                User user = new User()
                {
                    Id = packet.Id,
                    Socket = socket,
                    RunGame = false,
                    IsBlack = null,
                };

                socket.Send(Encoding.UTF8.GetBytes(json));

                lock (Users)
                {
                    Users.Add(user);

                    User? waitUser = Users.Find(x => x.RunGame == false && x.Id != packet.Id);

                    if (waitUser != null)
                    {
                        waitUser.RunGame = true;
                        waitUser.IsBlack = true;
                        user.RunGame = true;
                        user.IsBlack = false;

                        Shared.Packet.GameStart start = new Shared.Packet.GameStart()
                        {
                            EnemyId = user.Id,
                            MyColor = Shared.Game.Color.Black,
                        };
                        string str = JsonSerializer.Serialize(start);

                        waitUser.Socket!.Send(Encoding.UTF8.GetBytes(str));

                        start = new Shared.Packet.GameStart()
                        {
                            EnemyId = waitUser.Id,
                            MyColor = Shared.Game.Color.White,
                        }; 
                        str = JsonSerializer.Serialize(start);

                        user.Socket!.Send(Encoding.UTF8.GetBytes(str));

                        Games.Add(new Shared.Game()
                        {
                            BlackId = waitUser.Id,
                            WhiteId = user.Id,
                        });
                    }
                }

                SocketLoop(socket);
            }
        }

        private static void SocketLoop(Socket socket)
        {
            new Thread(() =>
            {
                try
                {
                    while (true)
                    {
                        byte[] buffer = new byte[1024];
                        socket.Receive(buffer);
                        string str = Encoding.UTF8.GetString(buffer).Replace("\0", "");

                        switch (JsonSerializer.Deserialize<Shared.Packet.Base>(str)!.Type)
                        {
                            case Shared.Packet.Type.ClientBehaviour:
                                {
                                    Shared.Packet.ClientBehaviour packet
                                        = JsonSerializer.Deserialize<Shared.Packet.ClientBehaviour>(str)!;

                                    Shared.Game? game = Games.Find(x => x.BlackId == packet.Id || x.WhiteId == packet.Id);

                                    User[] users = Users.FindAll(x => x.Id == game!.BlackId || x.Id == game!.WhiteId).ToArray();

                                    if (packet.Id == game!.BlackId && game.Turn == Shared.Game.Color.Black)
                                    {
                                        if (game.Map[packet.X, packet.Y] != Shared.Game.Color.Empty)
                                        {
                                            packet.Invalid = true;
                                        }
                                        else
                                        {
                                            game.Map[packet.X, packet.Y] = Shared.Game.Color.Black;
                                            game.TurnSwap();
                                            packet.Invalid = false;
                                        }
                                    }
                                    else if (packet.Id == game!.WhiteId && game.Turn == Shared.Game.Color.White)
                                    {
                                        if (game.Map[packet.X, packet.Y] != Shared.Game.Color.Empty)
                                        {
                                            packet.Invalid = true;
                                        }
                                        else
                                        {
                                            game.Map[packet.X, packet.Y] = Shared.Game.Color.White;
                                            game.TurnSwap();
                                            packet.Invalid = false;
                                        }
                                    }

                                    foreach (var user in users)
                                    {
                                        string json2 = JsonSerializer.Serialize(packet);
                                        buffer = Encoding.UTF8.GetBytes(json2);
                                        user.Socket!.Send(buffer);
                                    }
                                }
                                break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);

                    Users.Remove(Users.Find(x => x.Socket == socket)!);
                    socket.Close();
                }
            }).Start();
        }
    }
}
