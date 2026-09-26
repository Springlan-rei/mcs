using Pingo;
using Pingo.Status;
using System;
using System.IO;//以后存服务器用
using System.Net;

namespace mcs
{
    class Program
    {
        static List<string> servers = new List<string>();//以后存服务器用
        static void Main() {
            string command;

            Console.WriteLine("=========================================");
            Console.WriteLine("   欢迎使用mcs —— 一个mc服务器小工具   ");
            Console.WriteLine("=========================================");
            Console.WriteLine("tips：不知道输入什么？试试输入help获取帮助");
            do
            {
                Console.Write("@mcs:");
                command = Console.ReadLine()?.ToLower();
                switch (command)
                {
                    case ("help"):
                        Help();
                        break;
                    case "exit":
                        Console.WriteLine("再见！");
                        break;
                    case "ping":
                        Console.WriteLine("该指令开发中");
                        Ping();
                        break;
                    case "detail":
                        Console.WriteLine("该指令开发中");
                        Detail();
                        break;
                    default:
                        Console.WriteLine(command + "是不正确的指令");
                        break;
                    //其他指令以后再说
                }

            } while (command != "exit");
        }

        static void Help()
        {
            Console.WriteLine("详细指令帮助：");
            Console.WriteLine("help - 打开指令帮助，就像现在");
            Console.WriteLine("ping - ping你要连接的服务器（开发中）");
            Console.WriteLine("detail - 获取服务器详细信息（开发中）");
            Console.WriteLine("exit - 退出程序");
            Console.WriteLine("注意：指令后面无需加参数，部分指令输入后会提示你输入相关的信息");
            Console.WriteLine("其他指令开发中...");

        }

        static void Ping()//%80Deepseek,20%Springlan
        {

            Console.Write("请输入服务器地址: ");
            string address = Console.ReadLine();
            
            IPAddress[] ips = Dns.GetHostAddresses(address);//DNS解析

            Console.Write("请输入端口 (直接回车默认 25565): ");
            string portInput = Console.ReadLine();

            int port = 25565;


            if (!string.IsNullOrEmpty(portInput))
            {
                int.TryParse(portInput, out port);
            }

            try
            {
                var options = new MinecraftPingOptions
                {
                    Address = address,
                    Port = (ushort)port // 👈 把 int 强制转成 ushort
                };

                var status = Minecraft.PingAsync(options).Result;

                Console.WriteLine($"服务器 {address}:{port} 在线");
                Console.WriteLine($"延迟测速开发中");
            }
            catch (Exception)
            {
                Console.WriteLine($"服务器 {address}:{port} 离线或无法连接。");
            }
        }

        static void Detail()//%95Deepseek,5%Springlan折叠区域
        {
            Console.Write("请输入服务器地址: ");
            string address = Console.ReadLine();

            Console.Write("请输入端口 (直接回车默认 25565): ");
            string portInput = Console.ReadLine();

            int port = 25565;
            if (!string.IsNullOrEmpty(portInput))
            {
                int.TryParse(portInput, out port);
            }

            try
            {
                var options = new MinecraftPingOptions
                {
                    Address = address,
                    Port = (ushort)port
                };

                var status = Minecraft.PingAsync(options).Result;

                // 转成 JavaStatus 才能访问详细属性
                if (status is JavaStatus javaStatus)
                {
                    Console.WriteLine("=========================================");
                    Console.WriteLine($"  服务器: {address}:{port}");
                    Console.WriteLine("=========================================");
                    Console.WriteLine($"  版本: {javaStatus.Name}");
                    Console.WriteLine($"  协议: {javaStatus.Protocol}");
                    Console.WriteLine($"  玩家: {javaStatus.OnlinePlayers}/{javaStatus.MaximumPlayers}");
                    Console.WriteLine($"  MOTD: {string.Join(" ", javaStatus.MessagesOfTheDay)}");
                    Console.WriteLine("=========================================");
                }
                else
                {
                    Console.WriteLine("这不是一个 Java 版服务器。");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"服务器 {address}:{port} 离线或无法连接。");
            }
        }
    }
}