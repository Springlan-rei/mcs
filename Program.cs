using DnsClient.Protocol;
using Pingo;
using Pingo.Status;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Net;
using System.Reflection.Emit;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace mcs
{
    class Program
    {
        class Server//服务器地址保存类
        {
            public string Address;
            public string Port;
            public string Label;
            public string ID;
        }
        static List<Server> servers = new List<Server>();
        static void Main() {
            string command;
            check();
            LoadSevers();
            if (!File.Exists("./servers/sevTask/Servers.xml"))//检查xml
            {
                ReWriteXml();//创建初始xml
            }
            Console.WriteLine("=========================================");
            Console.WriteLine("   欢迎使用mcs —— 一个mc服务器小工具   ");
            Console.WriteLine("=========================================");
            Console.WriteLine("tips：不知道输入什么？试试输入help获取帮助");
            int r = View();//把返回值赋值给变量
            if(r == 1)
            {
                Console.WriteLine("请尝试手动输入load指令");
            }
            do
            {
                Console.Write("@mcs:");
                command = Console.ReadLine()?.ToLower();
                switch (command)//类指令集
                {
                    case ("help"):
                        Help();
                        break;
                    case "exit":
                        Console.WriteLine("再见！");
                        break;
                    case "ping":
                        Ping();
                        break;
                    case "detail":
                        Detail();
                        break;
                    case "save":
                        SaveServers();
                        break;
                    case "load":
                        LoadSevers();
                        break;
                    case "view":
                        View(); 
                        break;
                    case "delete":
                        Delete();
                        break;
                    default:
                        Console.WriteLine(command + "是不正确的指令");
                        break;
                    //其他指令以后再说
                }

            } while (command != "exit");
        }

        static void check()//该方法检查xml存放的文件夹
        {
            if (!Directory.Exists("./servers"))
            {
                Directory.CreateDirectory("./servers");
            }
            if (!Directory.Exists("./servers/sevTask/"))
            {
                Directory.CreateDirectory("./servers/sevTask/");
            }
            if (!Directory.Exists("./servers/sevMOTD/"))
            {
                Directory.CreateDirectory("./servers/sevMOTD/");
            }

        }

        static void Help()
        {
            Console.WriteLine("详细指令帮助：");
            Console.WriteLine("help - 打开指令帮助，就像现在");
            Console.WriteLine("ping - ping你要连接的服务器");
            Console.WriteLine("detail - 获取服务器详细信息");
            Console.WriteLine("save - 保存服务器到xml");
            Console.WriteLine("load - 手动加载xml");
            Console.WriteLine("view - 查看当前保存的服务器（请确认你加载了xml)");
            Console.WriteLine("exit - 退出程序");
            Console.WriteLine("注意：指令后面无需加参数，部分指令输入后会提示你输入相关的信息");
            Console.WriteLine("如果更改xml的名字，内容或位置，程序可能无法识别或内容异常");
            Console.WriteLine("其他指令开发中...");

        }

        static void Ping()
        {

            Console.Write("请输入服务器地址（无需带端口）: ");
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
                    Port = (ushort)port //把 int 强制转成 ushort
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

        static void Detail()
        {
            Console.Write("请输入服务器地址（无需带端口）: ");
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
        static void SaveServers()
        {
            Console.Write("请输入服务器地址（无需带端口）: ");
            string address = Console.ReadLine();

            Console.Write("请输入端口 (直接回车默认 25565): ");
            string port = Console.ReadLine();
            if (string.IsNullOrEmpty(port))
            {
                port = "25565";
            }

            Console.Write("请输入此服务器的名称：");
            string label = Console.ReadLine();


            check();//检查xml文件夹
            if (!string.IsNullOrEmpty(address) && !string.IsNullOrEmpty(port) && !string.IsNullOrEmpty(label))
            {
                servers.Add(new Server { Address = address, Port = port, Label = label ,ID = Guid.NewGuid().ToString() });//id就像服务器的身份证，防止label重名，有唯一性
                // 生成 XML
                XmlDocument xml = new XmlDocument();
                XmlElement Servers = xml.CreateElement("Servers");
                xml.AppendChild(Servers);

                foreach (Server s in servers)
                {
                    XmlElement server = xml.CreateElement("Server");
                    Servers.AppendChild(server);

                    XmlElement Label = xml.CreateElement("Label");
                    Label.InnerText = s.Label;
                    server.AppendChild(Label);

                    XmlElement Address = xml.CreateElement("Address");
                    Address.InnerText = s.Address;
                    server.AppendChild(Address);

                    XmlElement Port = xml.CreateElement("Port");
                    Port.InnerText = s.Port;
                    server.AppendChild(Port);

                    XmlElement Id = xml.CreateElement("Id");
                    Id.InnerText = s.ID;
                    server.AppendChild(Id);
                }

                xml.Save("./servers/sevTask/Servers.xml");
                Console.WriteLine("已保存至Servers.xml");
                
            }
            else
            {
                Console.WriteLine("服务器保存失败！");
            }
        }

        static void LoadSevers()
        {
            XmlDocument xml = new XmlDocument();
            if (File.Exists("./servers/sevTask/Servers.xml"))
            {
                servers.Clear();
                Console.WriteLine("正在加载服务器...");
                xml.Load("./servers/sevTask/Servers.xml");
                XmlNodeList nodes = xml.SelectNodes("Servers/Server");

                // 遍历
                foreach (XmlNode node in nodes)
                {
                    string label = node.SelectSingleNode("Label")?.InnerText;
                    string address = node.SelectSingleNode("Address")?.InnerText;
                    string port = node.SelectSingleNode("Port")?.InnerText;
                    string id = node.SelectSingleNode("Id")?.InnerText;

                    servers.Add(new Server { Label = label, Address = address, Port = port ,ID = id });
                }
                Console.WriteLine("加载成功！");

            }
            else
            {
                Console.WriteLine("服务器加载失败！");
            }
        }

        static int View()
        {
            Console.WriteLine("正在加载服务器...");
            bool l = false;
            foreach (Server s in servers)
            {
                Console.WriteLine("---------------------------");
                Console.WriteLine("服务器名：" + s.Label);
                Console.WriteLine("服务器地址：" + s.Address);
                Console.WriteLine("服务器端口：" + s.Port);
                Console.WriteLine("服务器ID：" + s.ID);
                l = true;
            }
            if (l)
            {
                Console.WriteLine("已加载");
                return 0;
            }
            else
            {
                Console.WriteLine("加载失败！");
                return 1;
            }
            
        }
        static void Delete()
        {
            Console.WriteLine("删除选择");
            Console.WriteLine("1 - 删除整个xml");
            Console.WriteLine("2 -  删除单个服务器");
            Console.WriteLine("3 - 不删除");
            Console.Write("请选择：");
            string Dchoose = Console.ReadLine();
            if (!string.IsNullOrEmpty(Dchoose) && int.TryParse(Dchoose, out int dChoose))
            {
                if (dChoose == 1)
                {
                    servers.Clear();
                    File.Delete("./servers/sevTask/Servers.xml");
                }
                else if (dChoose == 2)
                {
                    while (true)
                    {
                        View();  // 列出所有服务器
                        Console.Write("请粘贴要删除的服务器 ID（输入 c 退出）：");
                        string Schoose = Console.ReadLine();

                        if (Schoose == "c")
                        {
                            break;
                        }

                        if (string.IsNullOrEmpty(Schoose))
                        {
                            Console.WriteLine("输个棍木干什么");
                            continue;
                        }

                        // 找匹配的 Server
                        Server target = null;
                        foreach (Server s in servers)
                        {
                            if (s.ID == Schoose)
                            {
                                target = s;
                                break;
                            }
                        }

                        if (target != null)
                        {
                            servers.Remove(target);
                            ReWriteXml();
                            Console.WriteLine("已删除");
                        }
                        else
                        {
                            Console.WriteLine("这个ID没有对应的服务器，看看复制是否正确");
                        }
                    }
                }
                else if (dChoose == 3)
                {
                    Console.WriteLine("已取消！");
                }
            }
            else
            {
                Console.WriteLine("该选择不存在（未删除服务器）");
            }
        }

        static void ReWriteXml()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement Servers = xml.CreateElement("Servers");
            xml.AppendChild(Servers);

            foreach (Server s in servers)
            {
                XmlElement server = xml.CreateElement("Server");
                Servers.AppendChild(server);

                XmlElement Label = xml.CreateElement("Label");
                Label.InnerText = s.Label;
                server.AppendChild(Label);

                XmlElement Address = xml.CreateElement("Address");
                Address.InnerText = s.Address;
                server.AppendChild(Address);

                XmlElement Port = xml.CreateElement("Port");
                Port.InnerText = s.Port;
                server.AppendChild(Port);

                XmlElement Id = xml.CreateElement("Id");
                Id.InnerText = s.ID;
                server.AppendChild(Id);
            }

            xml.Save("./servers/sevTask/Servers.xml");
        }
    }
}