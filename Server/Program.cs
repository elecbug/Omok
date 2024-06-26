﻿using System;
using System.Collections.Generic;
using System.Drawing;
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
            Listener = new TcpListener(IPEndPoint.Parse(Console.ReadLine()!));

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
                                        if (game.Map[packet.X, packet.Y] != Shared.Game.Color.Empty
                                            || game.FailureMove(packet.X, packet.Y))
                                        {
                                            packet.Invalid = true;
                                        }
                                        else
                                        {
                                            game.Map[packet.X, packet.Y] = Shared.Game.Color.Black;
                                            game.Canceled = false;
                                            game.LastMove = new Point(packet.X, packet.Y);

                                            bool win = game.WinCheck(packet.X, packet.Y);
                                            packet.Invalid = false;

                                            if (win)
                                            {
                                                packet.WinColor = game.Turn;
                                            }

                                            game.TurnSwap();
                                        }
                                    }
                                    else if (packet.Id == game!.WhiteId && game.Turn == Shared.Game.Color.White)
                                    {
                                        if (game.Map[packet.X, packet.Y] != Shared.Game.Color.Empty
                                            || game.FailureMove(packet.X, packet.Y))
                                        {
                                            packet.Invalid = true;
                                        }
                                        else
                                        {
                                            game.Map[packet.X, packet.Y] = Shared.Game.Color.White;
                                            game.Canceled = false;
                                            game.LastMove = new Point(packet.X, packet.Y);

                                            bool win = game.WinCheck(packet.X, packet.Y);
                                            packet.Invalid = false;

                                            if (win)
                                            {
                                                packet.WinColor = game.Turn;
                                            }

                                            game.TurnSwap();
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
                            case Shared.Packet.Type.ReDo:
                                {
                                    Shared.Packet.ReDo packet
                                        = JsonSerializer.Deserialize<Shared.Packet.ReDo>(str)!;

                                    Shared.Game? game = Games.Find(x => x.BlackId == packet.Id || x.WhiteId == packet.Id);

                                    if (game!.Canceled == true)
                                    {
                                        break;
                                    }

                                    User[] users = Users.FindAll(x => x.Id == game!.BlackId || x.Id == game!.WhiteId).ToArray();

                                    Shared.Packet.ReDoOk packet2 = new Shared.Packet.ReDoOk()
                                    {
                                        Id = packet.Id,
                                        Cancel = true,
                                    };

                                    if (packet.Id == game!.BlackId && game.Turn == Shared.Game.Color.White)
                                    {
                                        packet2.Cancel = false;
                                        game!.TurnSwap();
                                        game!.Canceled = true;
                                        game!.Map[game!.LastMove!.Value.X, game!.LastMove!.Value.Y] 
                                            = Shared.Game.Color.Empty;
                                        game!.LastMove = null;
                                    }
                                    else if (packet.Id == game!.WhiteId && game.Turn == Shared.Game.Color.Black)
                                    {
                                        packet2.Cancel = false;
                                        game!.TurnSwap();
                                        game!.Canceled = true;
                                        game!.Map[game!.LastMove!.Value.X, game!.LastMove!.Value.Y] 
                                            = Shared.Game.Color.Empty;
                                        game!.LastMove = null;
                                    }

                                    foreach (var user in users)
                                    {
                                        string json2 = JsonSerializer.Serialize(packet2);
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

                    User user = Users.Find(x => x.Socket == socket)!;
                    Users.Remove(user);

                    Shared.Game? game = Games.Find(x => x.BlackId == user.Id || x.WhiteId == user.Id);

                    if (game != null)
                    {
                        int enemyId = game.BlackId == user.Id ? game.WhiteId : game.BlackId;

                        User enemy = Users.Find(x => x.Id == enemyId)!;

                        string json = JsonSerializer.Serialize(new Shared.Packet.EnemyFailure());
                        byte[] buffer = Encoding.UTF8.GetBytes(json);
                        enemy.Socket!.Send(buffer);

                        Users.Remove(enemy);
                        Games.Remove(game);
                    }

                    socket.Close();
                }
            }).Start();
        }
    }
}
