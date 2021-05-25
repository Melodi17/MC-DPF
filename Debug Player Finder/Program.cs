using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ConsoleUIRenderer;

namespace Debug_Player_Finder
{
    class Program
    {
        static Window mainWindow;
        static ListBox listBox;
        static void Main(string[] args)
        {
            Run();
        }

        static void Run()
        {
            Console.Clear();
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
            Console.Title = "Debug Player Finder        Control+H for help";

            mainWindow = new Window();
            mainWindow.position = new Point(0, 0);
            mainWindow.size = new Size(Console.WindowWidth, Console.WindowHeight);
            mainWindow.foregroundColor = ConsoleColor.White;
            mainWindow.backgroundColor = ConsoleColor.DarkBlue;
            mainWindow.text = "Debug Player Finder";
            mainWindow.Render();

            listBox = new ListBox();
            listBox.position = new Point(mainWindow.position.x + 1, mainWindow.position.y + 1);
            listBox.size = new Size(mainWindow.size.width - 3, mainWindow.size.height - 3);
            listBox.foregroundColor = mainWindow.foregroundColor;
            listBox.backgroundColor = mainWindow.backgroundColor;
            listBox.activeForegroundColor = mainWindow.backgroundColor;
            listBox.activeBackgroundColor = mainWindow.foregroundColor;
            listBox.selectedIndex = 0;
            listBox.items.Add("Crash Minecraft");
            listBox.items.Add("Parse Latest Debug Log");
            listBox.items.Add("Quit");
            listBox.Render();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                    if (consoleKeyInfo.Modifiers == ConsoleModifiers.Control)
                    {
                        if (consoleKeyInfo.Key == ConsoleKey.Q)
                        {
                            Process.GetCurrentProcess().Kill();
                        }
                        if (consoleKeyInfo.Key == ConsoleKey.B)
                        {
                            Run();
                        }
                        else if (consoleKeyInfo.Key == ConsoleKey.H)
                        {
                            Window popupWindow = new Window();
                            popupWindow.foregroundColor = ConsoleColor.Black;
                            popupWindow.backgroundColor = ConsoleColor.DarkGray;
                            popupWindow.size = new Size((mainWindow.size.width / 3) * 2, (mainWindow.size.height / 3) * 2);
                            popupWindow.position = new Point(popupWindow.size.width / 4, popupWindow.size.height / 4);
                            popupWindow.text = "Help Menu";
                            popupWindow.Render();

                            Label popupText = new Label();
                            popupText.position = new Point(popupWindow.position.x + 1, popupWindow.position.y + 1);
                            popupText.size = new Size(popupWindow.size.width - 3, popupWindow.size.height - 3);
                            popupText.foregroundColor = popupWindow.foregroundColor;
                            popupText.backgroundColor = popupWindow.backgroundColor;
                            popupText.text = "----- Help -----\n";
                            popupText.text += "This is a small command-line utility for extracting coodinates from a multiplayer server's crash logs\n";
                            popupText.text += "\n";
                            popupText.text += "----- Credits -----\n";
                            popupText.text += "Made by Melodi\n";
                            popupText.text += "Github: https://github.com/Melodi17/MC-DPF\n";
                            popupText.text += "\n";
                            popupText.text += "----- Shortcuts -----\n";
                            popupText.text += "Control+H - Help\n";
                            popupText.text += "Control+B - Restart and re-render screen\n";
                            popupText.text += "Control+Q - Quit\n";
                            popupText.text += "Esc       - Close menu (Only available in menus)\n";
                            popupText.Render();

                            while (true)
                            {
                                if (Console.KeyAvailable)
                                {
                                    ConsoleKeyInfo tunnelConsoleKeyInfo = Console.ReadKey();
                                    if (tunnelConsoleKeyInfo.Key == ConsoleKey.Escape)
                                    {
                                        mainWindow.Render();
                                        listBox.Render();
                                        break;
                                    }
                                    if (tunnelConsoleKeyInfo.Modifiers == ConsoleModifiers.Control)
                                    {
                                        if (tunnelConsoleKeyInfo.Key == ConsoleKey.Q)
                                        {
                                            Process.GetCurrentProcess().Kill();
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (consoleKeyInfo.Key == ConsoleKey.UpArrow)
                        {
                            if (listBox.selectedIndex > 0)
                            {
                                listBox.selectedIndex--;
                                listBox.Render();
                            }
                        }
                        else if (consoleKeyInfo.Key == ConsoleKey.DownArrow)
                        {
                            if (listBox.selectedIndex < listBox.items.Count - 1)
                            {
                                listBox.selectedIndex++;
                                listBox.Render();
                            }
                        }
                        else if (consoleKeyInfo.Key == ConsoleKey.Enter)
                        {
                            string selectedItem = listBox.items[listBox.selectedIndex];
                            if (selectedItem == "Quit")
                            {
                                Process.GetCurrentProcess().Kill();
                            }
                            else if (selectedItem == "Parse Latest Debug Log")
                            {
                                string roaming = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                                string mcdir = Path.Join(roaming, ".minecraft");
                                string logs = Path.Join(mcdir, "logs");
                                string debugFile = Path.Join(logs, "debug.log");
                                if (Directory.Exists(mcdir))
                                {
                                    if (Directory.Exists(logs))
                                    {
                                        if (File.Exists(debugFile))
                                        {
                                            try
                                            {
                                                string[] file = File.ReadAllLines(debugFile);
                                                int indexOfError = file.IndexOf("A detailed walkthrough of the error, its code path and all known details is as follows:");

                                                if (indexOfError != -1)
                                                {
                                                    int indexOfCoods = file.SoftIndexOf("	All players: ");
                                                    if (indexOfCoods != -1)
                                                    {
                                                        string unparsedLine = file[indexOfCoods];
                                                        try
                                                        {
                                                            string startParsedLine = unparsedLine.Substring(unparsedLine.IndexOf("[") + 1);
                                                            startParsedLine = startParsedLine.Remove(startParsedLine.Length - 1);
                                                            string[] playersAsString = startParsedLine.Split("], ");
                                                            List<Player> players = new List<Player>();
                                                            foreach (string item in playersAsString)
                                                            {
                                                                players.Add(Player.Parse(item));
                                                            }

                                                            string output = "";
                                                            foreach (Player item in players)
                                                            {
                                                                output += item.ToString() + "\n";
                                                            }

                                                            Window popupWindow = new Window();
                                                            popupWindow.foregroundColor = ConsoleColor.Black;
                                                            popupWindow.backgroundColor = ConsoleColor.DarkGray;
                                                            popupWindow.size = new Size((mainWindow.size.width / 3) * 2, (mainWindow.size.height / 3) * 2);
                                                            popupWindow.position = new Point(popupWindow.size.width / 4, popupWindow.size.height / 4);
                                                            popupWindow.text = "Data Retrieved";
                                                            popupWindow.Render();

                                                            Label popupText = new Label();
                                                            popupText.position = new Point(popupWindow.position.x + 1, popupWindow.position.y + 1);
                                                            popupText.size = new Size(popupWindow.size.width - 3, popupWindow.size.height - 4);
                                                            popupText.foregroundColor = popupWindow.foregroundColor;
                                                            popupText.backgroundColor = popupWindow.backgroundColor;
                                                            popupText.text = output;
                                                            popupText.Render();

                                                            Button exportButton = new Button();
                                                            exportButton.position = new Point(popupWindow.position.x + 1, popupWindow.position.y + popupWindow.size.height - 2);
                                                            exportButton.foregroundColor = popupWindow.foregroundColor;
                                                            exportButton.backgroundColor = popupWindow.backgroundColor;
                                                            exportButton.activeForegroundColor = popupWindow.backgroundColor;
                                                            exportButton.activeBackgroundColor = popupWindow.foregroundColor;
                                                            exportButton.active = true;
                                                            exportButton.text = "Export";
                                                            exportButton.Render();

                                                            Button rawButton = new Button();
                                                            rawButton.position = new Point(popupWindow.position.x + 1 + 10, popupWindow.position.y + popupWindow.size.height - 2);
                                                            rawButton.foregroundColor = popupWindow.foregroundColor;
                                                            rawButton.backgroundColor = popupWindow.backgroundColor;
                                                            rawButton.activeForegroundColor = popupWindow.backgroundColor;
                                                            rawButton.activeBackgroundColor = popupWindow.foregroundColor;
                                                            rawButton.active = false;
                                                            rawButton.text = "Raw";
                                                            rawButton.Render();

                                                            while (true)
                                                            {
                                                                if (Console.KeyAvailable)
                                                                {
                                                                    ConsoleKeyInfo tunnelConsoleKeyInfo = Console.ReadKey();
                                                                    if (tunnelConsoleKeyInfo.Key == ConsoleKey.Escape)
                                                                    {
                                                                        mainWindow.Render();
                                                                        listBox.Render();
                                                                        break;
                                                                    }
                                                                    else if (tunnelConsoleKeyInfo.Key == ConsoleKey.LeftArrow)
                                                                    {
                                                                        exportButton.active = true;
                                                                        rawButton.active = false;

                                                                        exportButton.Render();
                                                                        rawButton.Render();
                                                                    }
                                                                    else if (tunnelConsoleKeyInfo.Key == ConsoleKey.RightArrow)
                                                                    {
                                                                        exportButton.active = false;
                                                                        rawButton.active = true;

                                                                        exportButton.Render();
                                                                        rawButton.Render();
                                                                    }
                                                                    else if (tunnelConsoleKeyInfo.Key == ConsoleKey.Enter)
                                                                    {
                                                                        if (exportButton.active)
                                                                        {
                                                                            string fl = Guid.NewGuid().ToString() + ".txt";
                                                                            try
                                                                            {
                                                                                File.WriteAllText(fl, output);
                                                                                mainWindow.Render();
                                                                                listBox.Render();
                                                                                Popup("Export", "Export successful, file saved as " + fl);
                                                                            }
                                                                            catch (Exception)
                                                                            {
                                                                                mainWindow.Render();
                                                                                listBox.Render();
                                                                                Popup("Error", "Export unsuccessful, due to error");
                                                                            }
                                                                            break;
                                                                        }
                                                                        else if (rawButton.active)
                                                                        {
                                                                            Console.Clear();
                                                                            Console.SetCursorPosition(0, 0);
                                                                            Console.WriteLine(output);
                                                                            while (true)
                                                                            {
                                                                                if (Console.ReadKey().Key == ConsoleKey.Escape)
                                                                                {
                                                                                    break;
                                                                                }
                                                                            }
                                                                            Process.GetCurrentProcess().Kill();
                                                                        }
                                                                    }
                                                                    else if (tunnelConsoleKeyInfo.Modifiers == ConsoleModifiers.Control)
                                                                    {
                                                                        if (tunnelConsoleKeyInfo.Key == ConsoleKey.Q)
                                                                        {
                                                                            Process.GetCurrentProcess().Kill();
                                                                        }
                                                                    }
                                                                }
                                                            }

                                                        }
                                                        catch (Exception)
                                                        {
                                                            Popup("Error", "Parse Error");
                                                        }
                                                    }
                                                    else
                                                    {
                                                        Popup("Error", "Minecraft debug.log playerdata missing");
                                                    }
                                                }
                                                else
                                                {
                                                    Popup("Error", "Minecraft debug.log exception missing");
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                Popup("Error", "Minecraft is already running, press and hold F3+C for 10 seconds on your minecraft client to crash it");
                                            }
                                        }
                                        else
                                        {
                                            Popup("Error", "Minecraft debug.log file missing");
                                        }
                                    }
                                    else
                                    {
                                        Popup("Error", "Minecraft log folder missing");
                                    }
                                }
                                else
                                {
                                    Popup("Error", "Minecraft not installed");
                                }
                            }
                            else if (selectedItem == "Crash Minecraft")
                            {
                                Popup("Crash Minecraft", "To raise an exception, switch to your minecraft window, press and hold F3+C for 10 seconds");
                            }
                        }
                    }
                }
            }
        }

        static void Popup(string title, string content)
        {
            Window popupWindow = new Window();
            popupWindow.foregroundColor = ConsoleColor.Black;
            popupWindow.backgroundColor = ConsoleColor.DarkGray;
            popupWindow.size = new Size((mainWindow.size.width / 3) * 2, (mainWindow.size.height / 3) * 2);
            popupWindow.position = new Point(popupWindow.size.width / 4, popupWindow.size.height / 4);
            popupWindow.text = title;
            popupWindow.Render();

            Label popupText = new Label();
            popupText.position = new Point(popupWindow.position.x + 1, popupWindow.position.y + 1);
            popupText.size = new Size(popupWindow.size.width - 3, popupWindow.size.height - 3);
            popupText.foregroundColor = popupWindow.foregroundColor;
            popupText.backgroundColor = popupWindow.backgroundColor;
            popupText.text = content;
            popupText.Render();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo tunnelConsoleKeyInfo = Console.ReadKey();
                    if (tunnelConsoleKeyInfo.Key == ConsoleKey.Escape)
                    {
                        mainWindow.Render();
                        listBox.Render();
                        break;
                    }
                    if (tunnelConsoleKeyInfo.Modifiers == ConsoleModifiers.Control)
                    {
                        if (tunnelConsoleKeyInfo.Key == ConsoleKey.Q)
                        {
                            Process.GetCurrentProcess().Kill();
                        }
                    }
                }
            }
        }
    }
    static class EXT
    {
        public static int IndexOf(this string[] arr, string match)
        {
            int i = 0;
            foreach (string item in arr)
            {
                if (item == match)
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public static int SoftIndexOf(this string[] arr, string match)
        {
            int i = 0;
            foreach (string item in arr)
            {
                if (item.StartsWith(match))
                {
                    return i;
                }
                i++;
            }
            return -1;
        }
        public static string SkipString(this string str, string match)
        {
            if (str.StartsWith(match) && match != "")
            {
                return str.Substring(match.Length - 1);
            }
            else
            {
                return str;
            }
        }
    }
    class Player
    {
        public string name;
        public double x;
        public double y;
        public double z;
        public static Player Parse(string str)
        {
            Player player = new Player();

            string leftover = str.Replace("RemoteClientPlayerEntity['", "").Replace("ClientPlayerEntity['", "");
            if (leftover[^1] == ']')
            {
                leftover = leftover.Remove(leftover.Length - 1);
            }
            player.name = leftover.Substring(0, leftover.IndexOf("'"));
            leftover = leftover.SkipString(player.name + "'");
            player.x = Convert.ToDouble(leftover.Split(", ")[2].Split("=")[1]);
            player.y = Convert.ToDouble(leftover.Split(", ")[3].Split("=")[1]);
            player.z = Convert.ToDouble(leftover.Split(", ")[4].Split("=")[1]);

            return player;
        }
        public override string ToString()
        {
            return $"name:{name} x:{x} y:{y} z:{z}";
        }
    }
}
