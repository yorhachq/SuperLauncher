using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SuperLauncher
{
    internal class Program
    {
        // 获取自定义配置节，并转换为NameValueCollection
        readonly static NameValueCollection superLauncherMessages = ConfigurationManager.GetSection("SuperLauncherMessages") as NameValueCollection;
        static void Main(string[] args)
        {
            // 将控制台的输出编码设置为UTF-8
            Console.OutputEncoding = Encoding.UTF8;

            // 设置控制台窗口的宽度和高度
            Console.SetWindowSize(80, 30);
            //Console.WindowWidth = 80;
            //Console.WindowHeight = 30;

            // 读取配置信息
            if (superLauncherMessages != null)
            {
                string exitAfterCountDown = ConfigurationManager.AppSettings["ExitAfterCountDown"];
                string commandPrompt = superLauncherMessages["CommandPrompt"];
                string invalidCommandMessage = superLauncherMessages["InvalidCommandMessage"];
                string notProvideAppNameMessage = superLauncherMessages["NotProvideAppNameMessage"];
                string notProvideURLMessage = superLauncherMessages["NotProvideURLMessage"];
                string goodByeMessage = superLauncherMessages["GoodByeMessage"];

                // 显示欢迎页
                ShowWelcomePage();

                bool running = true;

                while (running)
                {
                    // 显示命令提示符
                    Console.Write(commandPrompt);
                    string input = Console.ReadLine().Trim(); // 获取用户输入并去除首尾空格

                    if (!string.IsNullOrEmpty(input))
                    {
                        string[] argsArr = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        string command = argsArr[0].ToLower(); // 获取第一个参数，并转换为小写

                        // 处理命令
                        switch (command)
                        {
                            case "-s":
                            case "--start":
                            case "start":
                            case "startup":
                            case "launch":
                                if (argsArr.Length < 2)
                                {
                                    HighLightMsgType(notProvideAppNameMessage,ConsoleColor.Red);
                                }
                                else
                                {
                                    string appName = argsArr[1].ToLower(); // 获取启动的应用名称，并转换为小写
                                    StartOptions(appName);
                                }
                                break;

                            case "-u":
                            case "--update":
                            case "update":
                                if (argsArr.Length < 2)
                                {
                                    HighLightMsgType(notProvideAppNameMessage,ConsoleColor.Red);
                                }
                                else
                                {
                                    string appName = argsArr[1].ToLower(); // 获取要更新的应用名称，并转换为小写
                                    UpdateGame(appName);
                                }
                                break;

                            case "-n":
                            case "--name":
                            case "namelist":
                                ShowNameList();
                                break;

                            case "-l":
                            case "--link":
                            case "link":
                                if (argsArr.Length < 2)
                                {
                                    HighLightMsgType(notProvideURLMessage,ConsoleColor.Red);
                                }
                                else
                                {
                                    string host = argsArr[1].ToLower(); // 获取启动的网站，并转换为小写
                                    OpenWebSite(host);
                                }
                                break;

                            case "-v":
                            case "--version":
                            case "version":
                            case "ver":
                                ShowVersion();
                                break;
                                
                            case "-c":
                            case "-cg":
                            case "--changelog":
                            case "changelog":
                                ShowChangelog();
                                break;

                            case "-h":
                            case "--help":
                            case "help":
                            case "/?":
                                ShowHelpMessage();
                                break;

                            case "-q":
                            case "--quit":
                            case "exit":
                            case "goodbye":
                            case "byebye":
                            case "bye":
                            case "886":
                            case "88":
                                running = false;
                                Console.WriteLine(goodByeMessage);
                                if ("true".Equals(exitAfterCountDown))
                                {
                                    ShowCountdown(3);
                                }
                                if ("false".Equals(exitAfterCountDown))
                                {
                                    //Console.Write("Press any key to continue.");
                                    //Console.ReadKey();
                                    //Console.Write("\r                          \r");
                                }
                                break;

                            default:
                                HighLightMsgType(invalidCommandMessage, ConsoleColor.Red);
                                ShowHelpMessage();
                                break;
                        }
                    }
                }

            }
            else
            {
                HighLightMsgType("[Error] Configuration Loading Failed.\n",ConsoleColor.Red);
            }
        }

        // 显示启动欢迎页
        private static void ShowWelcomePage()
        {
            //Console.WriteLine("\r\n███████╗██╗   ██╗██████╗ ███████╗██████╗     ██╗      █████╗ ██╗   ██╗███╗   ██╗ ██████╗██╗  ██╗███████╗██████╗ \r\n██╔════╝██║   ██║██╔══██╗██╔════╝██╔══██╗    ██║     ██╔══██╗██║   ██║████╗  ██║██╔════╝██║  ██║██╔════╝██╔══██╗\r\n███████╗██║   ██║██████╔╝█████╗  ██████╔╝    ██║     ███████║██║   ██║██╔██╗ ██║██║     ███████║█████╗  ██████╔╝\r\n╚════██║██║   ██║██╔═══╝ ██╔══╝  ██╔══██╗    ██║     ██╔══██║██║   ██║██║╚██╗██║██║     ██╔══██║██╔══╝  ██╔══██╗\r\n███████║╚██████╔╝██║     ███████╗██║  ██║    ███████╗██║  ██║╚██████╔╝██║ ╚████║╚██████╗██║  ██║███████╗██║  ██║\r\n╚══════╝ ╚═════╝ ╚═╝     ╚══════╝╚═╝  ╚═╝    ╚══════╝╚═╝  ╚═╝ ╚═════╝ ╚═╝  ╚═══╝ ╚═════╝╚═╝  ╚═╝╚══════╝╚═╝  ╚═╝\r\n                                                                                                                \r\n");
            //Console.WriteLine("\r\n  ____                              _                                _                 \r\n / ___|  _   _  _ __    ___  _ __  | |     __ _  _   _  _ __    ___ | |__    ___  _ __ \r\n \\___ \\ | | | || '_ \\  / _ \\| '__| | |    / _` || | | || '_ \\  / __|| '_ \\  / _ \\| '__|\r\n  ___) || |_| || |_) ||  __/| |    | |___| (_| || |_| || | | || (__ | | | ||  __/| |   \r\n |____/  \\__,_|| .__/  \\___||_|    |_____|\\__,_| \\__,_||_| |_| \\___||_| |_| \\___||_|   \r\n               |_|                                                                     \r\n");

            //Console.WriteLine("\r\n   _____                          __                           __             \r\n  / ___/__  ______  ___  _____   / /   ____ ___  ______  _____/ /_  ___  _____\r\n  \\__ \\/ / / / __ \\/ _ \\/ ___/  / /   / __ `/ / / / __ \\/ ___/ __ \\/ _ \\/ ___/\r\n ___/ / /_/ / /_/ /  __/ /     / /___/ /_/ / /_/ / / / / /__/ / / /  __/ /    \r\n/____/\\__,_/ .___/\\___/_/     /_____/\\__,_/\\__,_/_/ /_/\\___/_/ /_/\\___/_/     \r\n          /_/                                                                 \r\n");
            //Console.WriteLine("{0,78}", "Developer:YoRHa_CHQ");
            //Console.WriteLine("{0,78}", "Create:2023/8/2");
            //Console.WriteLine("{0,78}", "Update:2023/8/2");
            //Console.WriteLine();

            string welcomeMessage = superLauncherMessages["WelcomeMessage"];
            Console.WriteLine(welcomeMessage);
            Console.WriteLine();
        }

        // 显示版本信息
        static void ShowVersion()
        {
            string version = superLauncherMessages["Version"];
            //通过设置光标位置将版本号居中输出
            Console.SetCursorPosition((Console.WindowWidth - version.Length) / 2, Console.CursorTop);
            HighLight(version+'\n', ConsoleColor.DarkYellow);

            ShowWelcomePage();
        }

        // 显示帮助信息
        private static void ShowHelpMessage()
        {
            string cmdTips = superLauncherMessages["CmdTips"];
            Console.WriteLine(cmdTips);
        }

        // 显示所有应用程序的所有可用名称
        private static void ShowNameList()
        {
            string nameList = superLauncherMessages["NameList"];
            Console.WriteLine(nameList);
        }

        // 显示更新日志
        private static void ShowChangelog()
        {
            // 获取当前程序集
            Assembly assembly = Assembly.GetExecutingAssembly();

            // 获取资源文件流
            using (Stream stream = assembly.GetManifestResourceStream("SuperLauncher.Changelog.txt"))
            {
                if (stream != null)
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        // 读取资源文件内容并显示在控制台中
                        Console.WriteLine(reader.ReadToEnd());
                    }
                }
                else
                {
                    HighLightMsgType("[Error] Changelog not found.", ConsoleColor.Red);
                }
            }
        }
        // 显示退出前倒计时
        static void ShowCountdown(int count)
        {
            for (int i = count; i > 0; i--)
            {
                Console.Write($"\rThe program will exit after ");
                HighLight(i.ToString(), ConsoleColor.Yellow);
                Console.Write(" seconds.");
                Thread.Sleep(1000);
            }
            Console.Write("\r                                       \r"); // 清除倒计时文本
        }

        // 处理start命令的参数
        static void StartOptions(string appName)
        {
            appName = appName.ToLower();

            switch (appName)
            {
                case "ark":
                case "arknights":
                case "mrfz":
                case "明日方舟":
                    LaunchSingleApplication("Arknights");
                    break;

                case "gi":
                case "genshinimpact":
                case "genshin":
                case "ys":
                case "原神":
                    LaunchSingleApplication("GenshinImpact");
                    break;

                case "sr":
                case "star":
                case "starrail":
                case "xt":
                case "星铁":
                case "星穹铁道":
                    LaunchSingleApplication("StarRail");
                    break;

                case "meoasstgui":
                case "maa":
                case "牛牛":
                    LaunchSingleApplication("MAA");
                    break;

                case "all":
                    // 启动列表中所有游戏
                    List<string> gamesLaunchList = new List<string>
                    {
                        "Arknights",
                        "MAA",
                        //"GenshinImpact",
                        "StarRail"
                    };
                    LaunchMultipleApplications(gamesLaunchList);
                    break;

                case "cyberpunk2077":
                case "cyberpunk":
                case "2077":
                    LaunchSingleApplication("Cyberpunk2077");
                    break;

                case "chatgpt":
                case "gpt":
                case "ai":
                    LaunchSingleApplication("ChatGPT");
                    break;

                case "tim":
                case "qq":
                    LaunchSingleApplication("TIM");
                    break;

                case "wechat":
                case "wx":
                case "微信":
                    LaunchSingleApplication("WeChat");
                    break;

                default:
                    string appNotFoundMessage = superLauncherMessages["AppNotFoundMessage"];
                    HighLightMsgType(appNotFoundMessage,ConsoleColor.Red);
                    break;
            }
        }

        // 启动单个应用
        static void LaunchSingleApplication(string appName)
        {
            // 明日方舟(通过BlueStacks)
            if (appName.Equals("Arknights"))
            {
                string bluestacksPath = ConfigurationManager.AppSettings["BlueStacksPath"];
                string bluestacksExe = "HD-Player.exe";
                string instanceName = "Nougat32";
                string command = "launchApp";
                string package = "com.hypergryph.arknights";

                string arguments = $"--instance {instanceName} --cmd {command} --package \"{package}\"";

                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = $"{bluestacksPath}{bluestacksExe}",
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    };

                    using (Process process = new Process())
                    {
                        process.StartInfo = startInfo;
                        process.Start();
                    }

                    HighLightMsgType("[Success] Arknights Is Running.\n",ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    HighLightMsgType("[Error] An Error Occurred During Startup (Arknights):" + ex.Message,ConsoleColor.Red);
                    Console.WriteLine();
                }
            }
            else
            {
                // 其他可通过EXE文件直接启动的应用          
                string appPath = string.Empty;

                try
                {
                    if (appName.Equals("GenshinImpact"))
                    {
                        appPath = ConfigurationManager.AppSettings["GenshinImpactPath"];
                        appName = "Genshin Impact";
                    }
                    else if (appName.Equals("GenshinImpactLauncher"))
                    {
                        appPath = ConfigurationManager.AppSettings["GenshinImpactLauncherPath"];
                        appName = "Genshin Impact Launcher";
                    }
                    else if (appName.Equals("StarRail"))
                    {
                        appPath = ConfigurationManager.AppSettings["StarRailPath"];
                        appName = "Honkai: Star Rail";
                    }
                    else if (appName.Equals("StarRailLauncher"))
                    {
                        appPath = ConfigurationManager.AppSettings["StarRailLauncherPath"];
                        appName = "Star Rail Launcher";
                    }
                    else if (appName.Equals("MAA"))
                    {
                        // 检查是否存在 adb.exe 进程，残留的该进程将影响MAA正常工作
                        Process[] processes = Process.GetProcessesByName("adb");
                        if (processes.Length > 0)
                        {
                            // 存在 adb.exe 进程，尝试关闭它
                            foreach (Process process in processes)
                            {
                                try
                                {
                                    process.Kill();
                                    HighLightMsgType("[Info] The redundant adb.exe process is has been closed.",ConsoleColor.Cyan);
                                }
                                catch (Exception ex)
                                {
                                    HighLightMsgType($"[Error] An Error Occurred During Kill adb.exe:" + ex.Message,ConsoleColor.Red);
                                    Console.WriteLine();
                                }
                            }
                        }
                        appPath = ConfigurationManager.AppSettings["MAAPath"];
                        appName = "MaaAssistantArknights";
                    }
                    else if (appName.Equals("Cyberpunk2077"))
                    {
                        appPath = ConfigurationManager.AppSettings["Cyberpunk2077Path"];
                        appName = "Cyberpunk2077";
                    }
                    else if (appName.Equals("ChatGPT"))
                    {
                        appPath = ConfigurationManager.AppSettings["ChatGPTPath"];
                        appName = "ChatGPT Desktop";
                    }
                    else if (appName.Equals("TIM"))
                    {
                        appPath = ConfigurationManager.AppSettings["TIMPath"];
                        appName = "TIM";
                    }
                    else if (appName.Equals("WeChat"))
                    {
                        appPath = ConfigurationManager.AppSettings["WeChatPath"];
                        appName = "WeChat";
                    }
                    Process.Start(appPath);
                    HighLightMsgType($"[Success] {appName} Is Running.\n",ConsoleColor.Green);
                }
                catch (Exception ex)
                {
                    HighLightMsgType($"[Error] An Error Occurred During Startup ({appName}):" + ex.Message, ConsoleColor.Red);
                    Console.WriteLine();
                }
            }
        }

        // 启动多个应用
        static void LaunchMultipleApplications(List<string> appLaunchList)
        {
            foreach (string appName in appLaunchList)
            {
                LaunchSingleApplication(appName);
            }
        }

        // 更新游戏版本
        static void UpdateGame(string appName)
        {

            appName = appName.ToLower();

            switch (appName)
            {
                case "gi":
                case "genshinimpact":
                case "genshin":
                case "ys":
                case "原神":
                    LaunchSingleApplication("GenshinImpactLauncher");
                    break;

                case "sr":
                case "star":
                case "starrail":
                case "xt":
                case "星铁":
                case "星穹铁道":
                    LaunchSingleApplication("StarRailLauncher");
                    break;

                default:
                    string appNotFoundMessage = superLauncherMessages["AppNotFoundMessage"];
                    HighLightMsgType(appNotFoundMessage, ConsoleColor.Red);
                    break;
            }
        }

        // 访问网站
        static void OpenWebSite(string url)
        {
            try
            {
                if (!CheckUrlIsValid(url))
                {
                    switch (url)
                    {
                        case "bilibili":
                        case "bili":
                        case "哔哩哔哩":
                        case "哔哩":
                        case "b站":
                            url = "https://www.bilibili.com/";
                            break;

                        case "arknights":
                        case "ark":
                        case "明日方舟":
                            url = "https://ak.hypergryph.com/";
                            break;

                        case "monster-siren":
                        case "siren":
                        case "塞壬唱片":
                        case "塞壬":
                            url = "https://monster-siren.hypergryph.com/";
                            break;

                        case "mys":
                        case "米游社":
                            url = "https://bbs.mihoyo.com/sr/wiki/?bbs_presentation_style=no_header";
                            break;

                        case "prts":
                        case "普瑞赛斯":
                            url = "https://prts.wiki/w/%E9%A6%96%E9%A1%B5";
                            break;

                        case "github":
                            url = "https://github.com/";
                            break;

                        case "leetcode":
                        case "力扣":
                            url = "https://leetcode.cn/";
                            break;

                        case "taobao":
                        case "tb":
                        case "淘宝":
                            url = "https://www.taobao.com/";
                            break;

                        case "jongdong":
                        case "jd":
                        case "京东":
                            url = "https://www.jd.com/";
                            break;

                        case "dangdang":
                        case "当当":
                            url = "https://www.dangdang.com/";
                            break;

                        case "youtube":
                        case "yt":
                        case "油管":
                            url = "https://www.youtube.com/";
                            break;

                        case "pornhub":
                        case "porn":
                        case "fuck":
                        case "ph":
                            url = "https://www.pornhub.com/";
                            break;

                        case "missav":
                        case "ma":
                            url = "https://missav.com/cn";
                            break;

                        case "netflav":
                        case "flav":
                            url = "https://www.netflav.com/";
                            break;

                        case "pixiv":
                            url = "https://www.pixiv.net/";
                            break;

                        case "google":
                        case "谷歌":
                            url = "https://www.google.com/";
                            break;

                        case "gmail":
                        case "谷歌邮箱":
                            url = "https://mail.google.com/";
                            break;

                        case "baidu":
                        case "百度":
                            url = "https://www.baidu.com/";
                            break;

                        case "bing":
                        case "必应":
                            url = "https://www.bing.com/";
                            break;

                        case "openai":
                        case "ai":
                            url = "https://openai.com/";
                            break;

                        case "chat-gpt":
                        case "chatgpt":
                        case "gpt":
                            url = "https://chat.openai.com/";
                            break;

                        case "aliiyunoss":
                        case "aliiyun":
                        case "阿里云oss":
                        case "阿里云对象存储":
                        case "阿里云对象存储oss":
                            url = "https://oss.console.aliyun.com/overview";
                            break;

                        case "ipv6":
                        case "ipv6test":
                        case "testipv6":
                            url = "https://www.test-ipv6.com/";
                            break;

                        case "speed":
                        case "speedtest":
                        case "st":
                            url = "https://www.speedtest.net/";
                            break;

                        case "thuspeedtest":
                        case "tsinghuaspeedtest":
                        case "tst":
                            url = "https://iptv.tsinghua.edu.cn/st/";
                            break;

                        case "163":
                        case "163mail":
                        case "163邮箱":
                        case "网易邮箱":
                            url = "https://mail.163.com/";
                            break;

                        case "qqmail":
                        case "qq邮箱":
                            url = "https://mail.qq.com/";
                            break;

                        case "sakura-quicken":
                        case "sakura":
                        case "ssr":
                            url = "https://ssr.gr/";
                            break;

                        case "ruijie":
                        case "router":
                        case "x32-pro":
                        case "x32pro":
                        case "x32":
                        case "路由器":
                        case "路由":
                            url = "http://192.168.110.1/";
                            break;

                        case "cloudflare":
                        case "cf":
                            url = "https://dash.cloudflare.com/";
                            break;

                        default:
                            string webSiteNotFoundMessage = superLauncherMessages["WebSiteNotFoundMessage"];
                            HighLightMsgType(webSiteNotFoundMessage, ConsoleColor.Red);
                            break;
                    }
                }
                Process.Start(url);
                HighLightMsgType($"[Success] {url} Is Open.\n",ConsoleColor.Green);
            }
            catch (Exception ex)
            {
                HighLightMsgType($"[Error] An Error Occurred During Link To ({url}):" + ex.Message, ConsoleColor.Red);
                Console.WriteLine();
            }

        }

        /// <summary>
        /// 检测链接是否为合法的网址格式
        /// </summary>
        /// <param name="uri">待检测的链接</param>
        /// <returns></returns>
        public static bool CheckUrlIsValid(string uri)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(uri))
                    return false;

                var regex = @"(http://)?([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?";
                Regex re = new Regex(regex);
                return re.IsMatch(uri);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        /// <summary>
        ///  高亮输出提示信息的类型
        /// </summary>
        /// <param name="msg">格式:[Type] content</param>
        /// <param name="newColor">ConsoleColor</param>
        public static void HighLightMsgType(string msg, ConsoleColor newColor)
        {
            string[] splitStr = msg.Split(']');
            ConsoleColor oldColor = Console.ForegroundColor; // 获取当前控制台前景色
            Console.ForegroundColor = newColor; // 将控制台前景色设置为指定的高亮颜色
            Console.Write($"{splitStr[0]}]");
            Console.ForegroundColor = oldColor;// 恢复控制台原本的前景色
            Console.Write($"{splitStr[1]}\n");
        }

        // 高亮输出文本
        public static void HighLight(string str, ConsoleColor newColor)
        {
            ConsoleColor oldColor = Console.ForegroundColor; // 获取当前控制台前景色
            Console.ForegroundColor = newColor; // 将控制台前景色设置为指定的高亮颜色
            Console.Write(str);
            Console.ForegroundColor = oldColor;// 恢复控制台原本的前景色
        }
    }

}
