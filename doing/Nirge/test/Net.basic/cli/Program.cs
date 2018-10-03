using System;
using log4net;
using log4net.Config;
using System.IO;
using System.Threading;
using Nirge.Core;
using System.Net;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Google.Protobuf;

namespace cli
{
    class Program
    {
        class PKG
        {
            byte[] pkg = new byte[2048];
            Random rng = new Random();
            IMessage[] msgs =
            {
                new G2C_PULSE_GEMON()
                {
                    Charid=1,
                    PulseSlot=1,
                    GemSlot=1,
                    Gemid=1,
                },
                new SPELL_INFO()
                {
                    Id=1,
                    Level = 1,
                    BonusLevel = 1,
                },
                new SGEM_INFO()
                {
                    ResId = 1,
                    ObjId = 1,
                    Pos = 1,
                    IsBind = true,
                    IsLock = true,
                    Level = 1,
                    Exp = 1,
                    TradeCooldown = 1,
                    IsExpensive = true,
                    Attr = new SGEM_ATTR_INFO()
                    {
                        Attr=1,
                        Value=1,
                    },
                },

                new G2C_SYNC_ROLE_DATA_BEGIN()
                {
                    Roleid = 1,
                    Rolename = "dxf",
                    Class = 1,
                    Body = 1,
                    Gender = 1,
                    Level = 1,
                    Exp = 1,
                    StoryProgress = 1,
                    Frame = 1,
                    ServerTime = 1,
                },

            };

            public object GetPkg(int type)
            {
                if (type == 1)
                    return new ArraySegment<byte>(pkg, 0, rng.Next(1, pkg.Length));
                else if (type == 2)
                    return msgs[rng.Next(msgs.Length)];
                return null;
            }
        }

        static void Main(string[] args)
        {
            XmlConfigurator.Configure(LogManager.CreateRepository("cli"), new FileInfo("../../Net.basic.log.cli.xml"));
            var cache = new CTcpClientCache(new CTcpClientCacheArgs(104857600, 104857600), LogManager.Exists("cli", "all"));
            var fill = new CTcpClientPkgFill();
            fill.Register(typeof(byte[]), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            fill.Register(typeof(ArraySegment<byte>), (int)eTcpClientPkgType.ArraySegment, new CTcpClientArraySegment());
            var code = new CTcpClientProtobufCode();
            code.Collect(typeof(G2C_PULSE_GEMON).Assembly);
            fill.Register(typeof(IMessage), (int)eTcpClientPkgType.Protobuf, new CTcpClientProtobuf(code));

            const int gCapacity = 1;
            var pKG = new PKG();

            var clis = new CTcpClient[gCapacity];
            for (var i = 0; i < clis.Length; ++i)
            {
                var cli = new CTcpClient(new CTcpClientArgs(), LogManager.Exists("cli", "all"), cache, fill);
                cli.Connected += Cli_Connected;
                cli.Closed += Cli_Closed;
                cli.Recved += Cli_Recved;
                clis[i] = cli;
                cli.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9527));
            }

            var t = Environment.TickCount;

            while (true)
            {
                for (var i = 0; i < gCapacity; ++i)
                {
                    var cli = clis[i];

                    cli.Exec();
                    switch (cli.State)
                    {
                    case eTcpClientState.Connected:
                        try
                        {
                            cli.Send(pKG.GetPkg(1));
                            cli.Send(pKG.GetPkg(2));
                        }
                        catch (Exception exception)
                        {
                            Console.WriteLine(exception);
                        }
                        break;
                    }
                }

                if (Environment.TickCount > t + 10000)
                {
                    t = Environment.TickCount;
                    Console.WriteLine(cache.Stat);
                }

                Thread.Sleep(100);
            }
        }

        private static void Cli_Recved(object arg1, object pkg)
        {
        }

        private static void Cli_Closed(object sender, CDataEventArgs<CTcpClientCloseArgs> e)
        {
            Console.WriteLine("cli close {0} {1} {2}", e.Arg1.Reason, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }

        private static void Cli_Connected(object sender, CDataEventArgs<CTcpClientConnectArgs> e)
        {
            Console.WriteLine("cli connect {0} {1} {2}", e.Arg1.Result, e.Arg1.SocketError, e.Arg1.Exception != null ? e.Arg1.Exception.ToString() : "");
        }
    }
}
