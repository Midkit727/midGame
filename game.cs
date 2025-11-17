namespace game
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Timers;
    using System.Threading;
    using System.IO.Compression;
    using System.Threading.Tasks;
    using System.Collections.Generic;
    class Program
    {
        private static bool Win10;
        public static int Score = 0;
        public static int FrameRate;
        private static int Elapsed = 0;
        public static int[] Resolution;
        public static int LastRoadStart;
        private static int HighScore = 0;
        public static bool Playing = false;
        public static Road Road = new Road();
        public static Player Player = new Player();
        private static string HighScoreName = "n/a";
        public static readonly List<string> PresetList = new List<string>();
        public static List<string> SettingsList = new List<string>();
        private static readonly List<string> Replay = new List<string>();
        public static System.Timers.Timer Timer = new System.Timers.Timer();
        private static IDictionary<int, string> Scores = new Dictionary<int, string>();
        private static void Main()
        {
            SetUp(false);
            MainMenu();
        }
        public static void MainMenu()
        {
            Console.CursorVisible = false;
            LoadSettings(false, false, -1);
            LoadScores();
            reDo:
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("highscore: " + HighScore + ", by " + HighScoreName);
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine();
            Console.WriteLine("press Enter to start the game");
            Console.WriteLine("press S to open the settings menu");
            Console.WriteLine("press L to show local scores");
            Console.WriteLine("press H to see a help menu");
            Console.WriteLine("press R to import a replay");
            Console.WriteLine("press Q to exit the game");
            var key = Console.ReadKey(true).Key;
            Console.Clear();
            switch (key)
            {
                case ConsoleKey.Enter:
                    RunGame();
                    break;
                case ConsoleKey.S:
                    Settings();
                    break;
                case ConsoleKey.L:
                    ShowScores();
                    break;
                case ConsoleKey.H:
                    HelpMenu();
                    break;
                case ConsoleKey.R:
                    ImportReplay();
                    break;
                case ConsoleKey.Q:
                    Quit();
                    break;
                default:
                    goto reDo;
            }
        }
        public static void Settings()
        {
            LoadSettings(false, false, -1);
            reDo:
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(new string('-', Console.WindowWidth));
            Console.WriteLine();
            Console.WriteLine("1, player icon: " + Player.Icon);
            Console.WriteLine("2, obstacle icon: " + Road.Icon);
            Console.WriteLine("3, fog icon: " + Road.FogIcon);
            Console.WriteLine("4, road changes (chars/line): " + Road.RoadChange);
            Console.WriteLine("5, road width (chars): " + Road.RoadWidth);
            Console.WriteLine("6, road width decrease rate (chars): " + Road.RoadWidthDcrC);
            Console.WriteLine("7, road width decrease rate (lines): " + Road.RoadWidthDcrL[0] + ";" + Road.RoadWidthDcrL[1]);
            Console.WriteLine("8, fog of war (lines): " + Road.FogOfWar);
            Console.WriteLine("9, delete all scores");
            Console.WriteLine("S, reset settings");
            Console.WriteLine("R, change resolution: " + Resolution[0] + " x " + Resolution[1]);
            Console.WriteLine("P, preset manager");
            Console.WriteLine("F, refresh rate: " + FrameRate);
            Console.WriteLine("W, toggle performance mode: " + Win10);
            Console.WriteLine("M, toggle midbot: " + MidBot.Toggled);
            Console.WriteLine("G, toggle godmode: " + Player.GodMode);
            Console.WriteLine("Enter to go back");
            var key = Console.ReadKey(true).Key;
            Console.CursorVisible = true;
            Console.Clear();
            try
            {
                switch (key)
                {
                    case ConsoleKey.D1:
                        ChangeIcon(0);
                        break;
                    case ConsoleKey.D2:
                        ChangeIcon(1);
                        break;
                    case ConsoleKey.D3:
                        ChangeIcon(2);
                        break;
                    case ConsoleKey.D4:
                        Road.ChangeRoadChange();
                        break;
                    case ConsoleKey.D5:
                        Road.ChangeRoadWidth();
                        break;
                    case ConsoleKey.D6:
                        Road.ChangeRoadWidthDcrC();
                        break;
                    case ConsoleKey.D7:
                        Road.ChangeRoadWidthDcrL();
                        break;
                    case ConsoleKey.D8:
                        Road.ChangeFogOfWar();
                        break;
                    case ConsoleKey.D9:
                        ResetFiles(0, true);
                        break;
                    case ConsoleKey.S:
                        ResetFiles(1, true);
                        break;
                    case ConsoleKey.R:
                        ChangeResolution();
                        break;
                    case ConsoleKey.P:
                        PresetManager();
                        break;
                    case ConsoleKey.F:
                        ChangeFps();
                        break;
                    case ConsoleKey.W:
                        ToggleWin10Mode();
                        break;
                    case ConsoleKey.M:
                        MidBot.Toggle();
                        break;
                    case ConsoleKey.G:
                        Player.ToggleGodMode();
                        break;
                    case ConsoleKey.Enter:
                        MainMenu();
                        break;
                    default:
                        goto reDo;
                }
            }
            catch
            {
                Console.Clear();
                Console.WriteLine("an error occured, no settings were changed");
                Console.WriteLine("press Enter...");
                Console.ReadKey();
                MainMenu();
            }
        }
        private static void Quit()
        {
            Environment.Exit(0);
        }
        private static void HelpMenu()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(string.Concat(Enumerable.Repeat("-", Console.WindowWidth)));
            Console.WriteLine();
            Console.WriteLine("1, changes the player icon");
            Console.WriteLine("2, changes the obstacles icon");
            Console.WriteLine("3, changes the fog icon");
            Console.WriteLine("4, the amount of characters the road move to the left or right");
            Console.WriteLine("5, the amount of characters the road is wide");
            Console.WriteLine("6, the amount of characters by which the road width decreases");
            Console.WriteLine("7, two values that dictate when the road width decreases");
            Console.WriteLine("8, the amount of invisible lines at the top of the screen");
            Console.WriteLine("9, deletes the local score file and creates a new empty one");
            Console.WriteLine("S, resets all settings to their default values");
            Console.WriteLine("R, changes the console width and height (240 x 63 is the maximum on 1920 x 1080 monitors)");
            Console.WriteLine("P, lets you create/delete and load different presets of settings");
            Console.WriteLine("F, how many times the display is updated each second (more frames => more obstacle generation)");
            Console.WriteLine("W, changes the way certain things are displayed to work on less powerful consolehosts");
            Console.WriteLine("M, toggles midbot on/off, midbot is very powerful, but she cant do everything");
            Console.WriteLine("G, \"'til death do us apart, true or false? are we dancin at the edge of life and death together?\"");
            Console.WriteLine();
            Console.WriteLine("press Enter...");
            Console.ReadKey();
            MainMenu();
        }
        private static void SetUp(bool save)
        {
            //Environment
            Program.Win10 = false;
            Program.FrameRate = 15;
            Program.Timer.Elapsed += Timer_Elapsed;
            int[] x = { 100, 30 }; Program.Resolution = x;
            Console.WindowWidth = Program.Resolution[0];
            Console.WindowHeight = Program.Resolution[1];
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\scores.txt"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\scores.txt").Close();
            }
            //Player
            Player.Icon = '^';
            Player.yLocation = 1; //base is 1
            Player.Crashed = false;
            Player.GodMode = false;
            Player.Left = ConsoleKey.LeftArrow;
            Player.Right = ConsoleKey.RightArrow;
            //Road
            Road.Icon = '.';
            Road.FogOfWar = 0;
            Road.FogIcon = '#';
            Road.RoadChange = 4;
            Road.RoadWidth = 30;
            Road.RoadWidthDcrC = 1;
            int[] y = { 250, 2000 }; Road.RoadWidthDcrL = y;
            //Save 
            if (save == true || !File.Exists(Directory.GetCurrentDirectory() + "\\settings.txt"))
            {
                Program.SettingsList.Clear();
                Program.SettingsList.Add(Player.Icon.ToString());
                Program.SettingsList.Add(Road.Icon.ToString());
                Program.SettingsList.Add(Road.RoadChange.ToString());
                Program.SettingsList.Add(Road.RoadWidth.ToString());
                Program.SettingsList.Add(Road.RoadWidthDcrC.ToString());
                Program.SettingsList.Add(Road.RoadWidthDcrL[0].ToString() + ";" + Road.RoadWidthDcrL[1].ToString());
                Program.SettingsList.Add(Road.FogOfWar.ToString());
                Program.SettingsList.Add(Program.FrameRate.ToString());
                Program.SettingsList.Add(Player.GodMode.ToString());
                Program.SettingsList.Add(Program.Resolution[0].ToString() + ";" + Program.Resolution[1].ToString());
                Program.SettingsList.Add(Road.FogIcon.ToString());
                Program.SettingsList.Add(Program.Win10.ToString());
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\settings.txt", Program.SettingsList);
            }
        }
        public static void SaveSettings(string statChange)
        {
            var info = statChange.Split(';');
            string allInfo = "";
            for (int i = 1; i < info.Length; i++)
            {
                allInfo += info[i];
                if (info.Length - i != 1)
                {
                    allInfo += ";";
                }
            }
            SettingsList.Insert(Convert.ToInt32(info[0]), allInfo);
            SettingsList.RemoveAt(Convert.ToInt32(info[0]) + 1);
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\settings.txt", SettingsList);
        }
        private static void PresetManager()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\presets"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\presets");
            }
            reDo:
            Console.CursorVisible = false;
            Console.Clear();
            Console.WriteLine("Press C to save current settings as a new preset");
            Console.WriteLine("Press D to delete an old preset");
            Console.WriteLine("Press L to load an old preset");
            Console.WriteLine("Press B to go back");
            var key = Console.ReadKey(true).Key;
            Console.CursorVisible = true;
            Console.Clear();
            switch (key)
            {
                case ConsoleKey.C:
                    CreatePreset();
                    break;
                case ConsoleKey.D:
                    DisplayPresets();
                    DeletePreset();
                    break;
                case ConsoleKey.L:
                    DisplayPresets();
                    LoadPreset();
                    break;
                case ConsoleKey.B:
                    Settings();
                    break;
                default:
                    goto reDo;
            }
        }
        private static void DisplayPresets()
        {
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine("current presets: ");
            Console.WriteLine();
            PresetList.Clear();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\presets");
            for (int i = 0; i < files.Length; i++)
            {
                if (GetFileExt(files[i]) == "txt")
                {
                    Console.WriteLine((i + 1) + ". " + files[i].Split('\\')[files[i].Split('\\').Length - 1]);
                    PresetList.Add(files[i]);
                }
            }
            Console.WriteLine();
        }
        private static void CreatePreset()
        {
            Console.Clear();
            Console.Write("preset name: ");
            var presetName = Console.ReadLine();
            SettingsList = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\settings.txt").ToList();
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\presets\\" + presetName + ".txt", SettingsList);
            PresetManager();
        }
        private static void DeletePreset()
        {
            Console.WriteLine("Enter index to delete");
            Console.WriteLine("note: file is irrecoverable");
            Console.Write("index: ");
            var index = ConvertToInt32(Console.ReadLine()) - 1;
            File.Delete(PresetList[index]);
            PresetManager();
        }
        private static void LoadPreset()
        {
            Console.WriteLine("Enter index to load");
            Console.WriteLine("note: current settings will be lost");
            Console.Write("index: ");
            var index = ConvertToInt32(Console.ReadLine()) - 1;
            LoadSettings(true, true, index);
        }
        private static void LoadSettings(bool manual, bool preset, int index)
        {
            try
            {
                //preset handling
                if (preset == true)
                {
                    File.Delete(Directory.GetCurrentDirectory() + "\\settings.txt");
                    File.Copy(PresetList[index], Directory.GetCurrentDirectory() + "\\settings.txt");
                }
                SettingsList = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\settings.txt").ToList();
                //Environment
                Win10 = Convert.ToBoolean(SettingsList[11]);
                FrameRate = Convert.ToInt32(SettingsList[7]);
                var a = SettingsList[9].Split(';'); int[] b = { Convert.ToInt32(a[0]), Convert.ToInt32(a[1]) }; Program.Resolution = b;
                Console.WindowWidth = Program.Resolution[0];
                Console.WindowHeight = Program.Resolution[1];
                Console.CursorVisible = false;
                //Player
                Player.Icon = SettingsList[0].ToCharArray()[0];
                Player.GodMode = Convert.ToBoolean(SettingsList[8]);
                //Road
                Program.LastRoadStart = 1;
                Road.Icon = SettingsList[1].ToCharArray()[0];
                Road.FogIcon = SettingsList[10].ToCharArray()[0];
                Road.RoadChange = Convert.ToInt32(SettingsList[2]);
                Road.RoadWidth = Convert.ToInt32(SettingsList[3]);
                Road.RoadWidthDcrC = Convert.ToInt32(SettingsList[4]);
                var y = SettingsList[5].Split(';'); int[] x = { Convert.ToInt32(y[0]), Convert.ToInt32(y[1]) }; Road.RoadWidthDcrL = x;
                Road.FogOfWar = Convert.ToInt32(SettingsList[6]);
                if (manual == true)
                {
                    Settings();
                }
            }
            catch
            {
                reDo:
                Console.Clear();
                Console.WriteLine("something went wrong while trying to load the settings file");
                Console.WriteLine("close the Program (Q), fix any errors and restart the program or press (D)");
                Console.WriteLine("note: pressing D will reset all settings to the default");
                var answer = Console.ReadKey(true).Key;
                switch (answer)
                {
                    case ConsoleKey.Q:
                        Quit();
                        break;
                    case ConsoleKey.D:
                        ResetFiles(1, false);
                        LoadScores();
                        break;
                    default:
                        Console.Clear();
                        goto reDo;
                }
            }
        }
        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Elapsed = 1;
        }
        private static async void RunGame()
        {
            Player.xLocation = Console.WindowWidth / 2; //base is 50
            Program.LastRoadStart = Road.Random.Next(1, Convert.ToInt32(Player.xLocation) - 10); //make sure player isnt spawn killed
            Replay.Clear();
            SetUp(false);
            LoadSettings(false, false, -1);
            Console.Clear();
            Player.OldLines.Clear();
            Playing = true;
            Player.Crashed = false;
            Score = 0;
            Road.GenerateStartScreen();
            if (MidBot.Toggled == true)
            {
                var Bot = Task.Run(() => MidBot.RunBot());

            }
            var Background = Task.Run(() => ListenToPlayerMovements());
            do
            {
                Road.Generate(false);
                Player.ShowPlayer();
                Player.OldLines.Add(Player.NewLine.ToString());
                Player.OldLines.RemoveAt(0);
                Player.NewLine.Clear();
                Score += 1;
                //await TimerStart(1000 / FrameRate);
                await UpdateDisplay();
            } while (Player.Crashed == false);
            //await TimerStart(1000 / 10);
            GameOver();
        }
        public static async Task<int> UpdateDisplay()
        {
            try
            {
                //Player.ClearPlayerTrail();
                var frame = "";
                if (Win10 != true)
                {
                    for (int i = 0; i < Road.FogOfWar; i++)
                    {
                        frame += new string(Road.FogIcon, Console.WindowWidth);
                        frame += "\n";
                    }
                }
                for (int i = Player.OldLines.Count - 1; i >= 0; i--)
                {
                    frame += Player.OldLines[i] + "\n";
                }
                if (Playing == true)
                {
                    frame += "score: " + Score;
                }
                frame += string.Concat(Enumerable.Repeat(" ", Console.WindowWidth - (("score: " + Score).Length + "press End to quit".Length)));
                frame += "press End to quit"; 
                Console.SetCursorPosition(0, 0);
                Console.Write(frame);
                //Console.WriteLine(frame);
                RecordReplay(Road.FrameInfo);
                //await Task.Delay(1000 / FrameRate);
                await TimerStart(1000 / FrameRate);
                return 0;
            }
            catch (DivideByZeroException)
            {
                FrameRate = 1;
                SaveSettings("7;" + FrameRate);
                return 0;
            }
        }
        public static void RecordReplay(string frameinfo)
        {
            Replay.Add(frameinfo);
            Road.FrameInfo = "";
        }
        private static void SaveReplay(string nameofPlayer)
        {
            Console.CursorVisible = true;
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\temp2.txt", Replay);
            Console.Write("replay name: ");
            var replayName = Console.ReadLine();
            Console.CursorVisible = false;
            reTry:
            if (File.Exists(Directory.GetCurrentDirectory() + "\\replays\\" + replayName + ".kitty"))
            {
                replayName += "_";
                goto reTry;
            }
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\replays"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\replays");
            }
            Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\replays\\" + replayName);
            var replayinfo = SettingsList;
            replayinfo.Add(Score.ToString());
            replayinfo.Add(nameofPlayer);
            replayinfo.Add("replayinfo");
            File.WriteAllLines(Directory.GetCurrentDirectory() + "\\replays\\" + replayName + "\\" + replayName + ".txt", replayinfo);
            var frames = File.ReadAllText(Directory.GetCurrentDirectory() + "\\temp2.txt");
            File.AppendAllText(Directory.GetCurrentDirectory() + "\\replays\\" + replayName + "\\" + replayName + ".txt", frames);
            File.Delete(Directory.GetCurrentDirectory() + "\\temp2.txt");
            ZipFile.CreateFromDirectory(Directory.GetCurrentDirectory() + "\\replays\\" + replayName, Directory.GetCurrentDirectory() + "\\replays\\" + replayName + ".kitty");
            File.Delete(Directory.GetCurrentDirectory() + "\\replays\\" + replayName + "\\" + replayName + ".txt");
            Directory.Delete(Directory.GetCurrentDirectory() + "\\replays\\" + replayName);
        }
        private static void ImportReplay()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + "\\replays"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + "\\replays");
            }
            Console.WriteLine("replays:");
            Console.WriteLine();
            var files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\replays");
            for (int i = 0; i < files.Length; i++)
            {
                if (GetFileExt(files[i]) == "kitty")
                {
                    Console.WriteLine((i + 1) + ". " + files[i].Split('\\')[files[i].Split('\\').Length - 1]);
                }
            }
            Console.WriteLine();
            Console.CursorVisible = true;
            Console.Write("replay name: ");
            var replayName = Console.ReadLine();
            Console.CursorVisible = false;
            Console.Clear();
            var extension = GetFileExt(replayName);
            if (extension != "kitty")
            {
                replayName += ".kitty";
            }
            if (File.Exists(Directory.GetCurrentDirectory() + "\\replays\\" + replayName))
            {
                DecodeReplayFile(replayName);
            }
            else
            {
                if (!string.IsNullOrEmpty(RemoveExtension(replayName)))
                {
                    Console.WriteLine("couldnt find replay \"" + replayName + "\"");
                    Console.WriteLine("make sure the replay is in " + Directory.GetCurrentDirectory() + "\\replays");
                    Console.ReadKey();
                }
                MainMenu();
            }
        }
        private static void DecodeReplayFile(string replayName)
        {
            var replayLocation = Directory.GetCurrentDirectory() + "\\replays\\" + RemoveExtension(replayName);
            var folderLocation = replayLocation + ".zip";
            File.Copy(replayLocation + ".kitty", folderLocation);
            ZipFile.ExtractToDirectory(replayLocation + ".zip", replayLocation);
            File.Delete(replayLocation + ".zip");
            ReadReplay(replayName, replayLocation);
        }
        private static void ReadReplay(string replayName, string replayLocation)
        {
            var replayFile = replayLocation + "\\" + RemoveExtension(replayName) + ".txt";
            var replay = File.ReadAllLines(replayFile).ToList();
            File.Delete(replayFile);
            Directory.Delete(replayLocation);
            var infoIndex = replay.IndexOf("replayinfo");
            var info = new List<string>();
            for (int i = 0; i < infoIndex; i++)
            {
                info.Add(replay[i]);
            }
            var a = info[9].Split(';'); int[] b = { Convert.ToInt32(a[0]), Convert.ToInt32(a[1]) }; Resolution = b;
            replay.RemoveRange(0, infoIndex + 1);
            askAgain:
            Console.WriteLine("save imported score in local savefile (Y/N)");
            var key = Console.ReadKey(true).Key;
            Console.Clear();
            switch (key)
            {
                case ConsoleKey.Y:
                    SaveScore(Convert.ToInt32(info[12]), info[13], true);
                    break;
                case ConsoleKey.N:
                    goto reDo;
                default:
                    goto askAgain;
            }
            reDo:
            Console.Clear();
            Console.WriteLine("replay info: ");
            Console.WriteLine("score: " + info[12]);
            Console.WriteLine("played by: " + info[13]);
            Console.WriteLine("resolution: " + Resolution[0] + " x " + Resolution[1]);
            Console.WriteLine("player icon: " + info[0]);
            Console.WriteLine("obstacle icon: " + info[1]);
            Console.WriteLine("fog icon: " + info[10]);
            Console.WriteLine("roadchange: " + info[2]);
            Console.WriteLine("road width: " + info[3]);
            Console.WriteLine("road width decrease rate (chars): " + info[4]);
            Console.WriteLine("road width decrease rate (lines): " + info[5]);
            Console.WriteLine("fog of war: " + info[6]);
            Console.WriteLine("framerate: " + info[7]);
            Console.WriteLine("performance mode: " + info[11]);
            Console.WriteLine("godmode: " + info[8]);
            Console.WriteLine();
            Console.WriteLine("watch replay (Y/N)");
            var answer = Console.ReadKey(true).Key;
            switch (answer)
            {
                case ConsoleKey.Y:
                    askOnceAgain:
                    Console.Clear();
                    Console.WriteLine("run replay at original frame rate and resolution (Y/N)");
                    var decision = Console.ReadKey(true).Key;
                    bool ogFps;
                    switch (decision)
                    {
                        case ConsoleKey.Y:
                            ogFps = true;
                            break;
                        case ConsoleKey.N:
                            ogFps = false;
                            break;
                        default:
                            Console.Clear();
                            goto askOnceAgain; //lole
                    }
                    SetUpReplay(replay, info, replayName, ogFps);
                    break;
                case ConsoleKey.N:
                    MainMenu();
                    break;
                default:
                    Console.Clear();
                    goto reDo;
            }
        }
        private static void SetUpReplay(List<string> replay, List<string> settings, string replayName, bool ogFps)
        {
            Console.CursorVisible = false;
            Console.Clear();
            if (ogFps == true)
            {
                FrameRate = Convert.ToInt32(settings[7]);
                Console.WindowWidth = Resolution[0];
                Console.WindowHeight = Resolution[1];
            }
            Player.Icon = Convert.ToChar(settings[0]);
            Road.Icon = Convert.ToChar(settings[1]);
            Road.FogIcon = Convert.ToChar(settings[10]);
            Road.FogOfWar = ConvertToInt32(settings[6]);
            PlayReplay(replay, settings, replayName);
        }
        private static async void PlayReplay(List<string> replay, List<string> settings, string replayName)
        {
            Score = 0;
            var lines = new List<string>();
            for (int i = 0; i < Console.WindowHeight + 1 - Road.FogOfWar; i++)
            {
                var lineInfos = replay[i].Split(';');
                var roadStart = Convert.ToInt32(lineInfos[0]);
                var roadEnd = Convert.ToInt32(lineInfos[1]);
                var newLine = GenerateReplayLine(roadStart, roadEnd);
                lines.Add(newLine);
            }
            await RenderReplay(lines);
            replay.RemoveRange(0, Console.WindowHeight + 1 - Road.FogOfWar);
            for (int i = 0; i < replay.Count; i++)
            {
                var lineInfos = replay[i].Split(';');
                var roadStart = Convert.ToInt32(lineInfos[0]);
                var roadEnd = Convert.ToInt32(lineInfos[1]);
                var playerxLocation = Convert.ToInt32(lineInfos[2]);
                var newLine = GenerateReplayLine(roadStart, roadEnd);
                lines = Program.ShowPlayer(playerxLocation, lines);
                lines.Add(newLine);
                lines.RemoveAt(0);
                Score += 1;
                await RenderReplay(lines);
            }
            Score = Convert.ToInt32(settings[12]);
            await RenderReplay(lines);
            RewatchReplay(replayName);
        }
        private static string GenerateReplayLine(int roadStart, int roadEnd)
        {
            var MapWidth = Console.WindowWidth - 1;
            var newLine = new StringBuilder();
            for (int i = 0; i < roadStart; i++)
            {
                newLine.Insert(i, Road.Icon.ToString());
            }
            for (int i = roadStart; i < roadEnd; i++)
            {
                newLine.Insert(i, " ");
            }
            for (int i = roadEnd; i < MapWidth; i++)
            {
                newLine.Insert(i, Road.Icon.ToString());
            }
            return newLine.ToString();
        }
        private static List<string> ShowPlayer(int playerxLocation, List<string> lines)
        {
            var playerLine = new StringBuilder(lines[Player.yLocation]);
            playerLine.Insert(playerxLocation, Player.Icon);
            playerLine.Remove(playerxLocation + 1, 1);
            lines.Insert(Player.yLocation, playerLine.ToString());
            lines.RemoveAt(Player.yLocation + 1);
            return lines;
        }
        private static async Task<int> RenderReplay(List<string> lines)
        {
            var frame = "";
            if (Win10 != true)
            {
                for (int i = 0; i < Road.FogOfWar; i++)
                {
                    frame += new string(Road.FogIcon, Console.WindowWidth); new string(Road.FogIcon, Console.WindowWidth);
                    frame += "\n";
                }
            }
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                frame += lines[i] + "\n";
            }
            frame += "score: " + Score;
            if (!string.IsNullOrEmpty(frame))
            {
                Console.SetCursorPosition(0, 0);
                Console.Write(frame);
            }
            await TimerStart(1000 / FrameRate);
            return 0;
        }
        private static void RewatchReplay(string replayName)
        {
            reDo:
            Console.WriteLine();
            Console.WriteLine("watch again (Y/N)");
            var key = Console.ReadKey(true).Key;
            LoadSettings(false, false, -1);
            Console.Clear();
            switch (key)
            {
                case ConsoleKey.Y:
                    DecodeReplayFile(replayName);
                    break;
                case ConsoleKey.N:
                    MainMenu();
                    break;
                default:
                    goto reDo;
            }
        }
        private static string RemoveExtension(string filename)
        {
            var name = filename.Split('.');
            var FullName = "";
            for (int i = 0; i < name.Length - 1; i++)
            {
                FullName += name[i];
            }
            return FullName;
        }
        private static string GetFileExt(string filename)
        {
            var fileextension = filename.Split('.');
            return fileextension[fileextension.Length - 1];
        }
        public static int ConvertToInt32(string convertible)
        {
            try
            {
                if (string.IsNullOrEmpty(convertible))
                {
                    MainMenu();
                    return 0;
                }
                var convertedInt = Convert.ToInt32(convertible);
                return convertedInt;
            }
            catch
            {
                Console.WriteLine("invalid character, press Enter...");
                Console.ReadKey();
                MainMenu();
                return 0;
            }
        }
        public static async Task<int> TimerStart(double interval)
        {
            try
            {
                Timer.Interval = interval;
                Elapsed = 0;
                Timer.Start();
                while (Elapsed == 0)
                { /*wait*/ }
                Timer.Stop();
                return 0;
            }
            catch
            {
                var timer = new System.Timers.Timer();
                timer.Interval = 1000;
                Elapsed = 0;
                timer.Start();
                while (Elapsed == 0)
                { /*wait*/ }
                timer.Stop();
                return 0;
            }
        }
        private static void ListenToPlayerMovements()
        {
            do
            {
                Player.Move();
            } while (Player.Crashed == false && Playing == true);
        }
        private static void GameOver()
        {
            Playing = false;
            MidBot.Running = false;
            Player.Crashed = false;
            Console.CursorVisible = true;
            Console.WriteLine();
            Console.WriteLine("game over");
            Console.WriteLine("play again (Y/N), save score (S)");
            reDo:
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Y:
                    RunGame();
                    break;
                case ConsoleKey.N:
                    MainMenu();
                    break;
                    case ConsoleKey.Enter:
                    RunGame();
                    break;
                case ConsoleKey.Escape:
                    MainMenu();
                    break;
                case ConsoleKey.S:
                    string playerName;
                    if (MidBot.Toggled != true)
                    {
                        Console.Write("enter name: ");
                        playerName = Console.ReadLine();
                    }
                    else
                    {
                        playerName = "midbot";
                    }
                    SaveScore(Score, playerName, false);
                    break;
                default:
                    goto reDo;
            }
        }
        private static void ChangeIcon(int PoRoF)
        {
            Console.WriteLine("input new icon: ");
            var newIcon = Console.ReadLine().ToCharArray();
            var statChange = "";
            Console.WriteLine("new icon is: " + newIcon[0].ToString());
            if (PoRoF == 0)
            {
                statChange += (0 + ";" + newIcon[0]).ToString();
                Player.Icon = newIcon[0];
                Program.SaveSettings(statChange);
            }
            else if (PoRoF == 1)
            {
                statChange += (1 + ";" + newIcon[0]).ToString();
                Road.Icon = newIcon[0];
                Program.SaveSettings(statChange);
            }
            else if (PoRoF == 2)
            {
                statChange += (10 + ";" + newIcon[0]).ToString();
                Road.FogIcon = newIcon[0];
                Program.SaveSettings(statChange);
            }
            Console.CursorVisible = false;
            Console.WriteLine("press Enter to go back");
            Console.ReadKey();
            Settings();
        }
        private static void ChangeFps()
        {
            Console.WriteLine("input new amount of frames per second (integer): ");
            try
            {
                var newRefreshRate = ConvertToInt32(Console.ReadLine());
                var statChange = 7 + ";" + newRefreshRate;
                Console.WriteLine("new refresh rate is: " + newRefreshRate);
                FrameRate = newRefreshRate;
                Program.SaveSettings(statChange);
                Console.CursorVisible = false;
                Console.WriteLine("press Enter to go back");
                Console.ReadKey();
                Program.Settings();
            }
            catch
            {
                Console.WriteLine("invalid character, press Enter...");
                Console.ReadKey();
                Program.MainMenu();
            }
        }
        private static void ChangeResolution()
        {
            Console.WriteLine("input new resolution (X;Y): ");
            try
            {
                var newResolution = Console.ReadLine();
                var statChange = 9 + ";" + newResolution;
                Console.WriteLine("new resolution is: " + newResolution);
                var y = newResolution.Split(';'); int[] x = { Convert.ToInt32(y[0]), Convert.ToInt32(y[1]) };
                if (x[0] < 30)
                {
                    x[0] = 30;
                    Console.WriteLine("minimum resolution is 30;10");
                }
                if (x[1] < 10)
                {
                    x[1] = 10;
                    Console.WriteLine("minimum resolution is 30;10");
                }
                Resolution = x;
                Program.SaveSettings(statChange);
                Console.CursorVisible = false;
                Console.WriteLine("press Enter to go back");
                Console.ReadKey();
                Program.Settings();
            }
            catch
            {
                Console.WriteLine("invalid character, press Enter...");
                Console.ReadKey();
                Program.MainMenu();
            }
        }
        public static void ToggleWin10Mode()
        {
            var statChange = "";
            if (Win10 != true)
            {
                statChange += 11 + ";" + true;
            }
            else
            {
                statChange += 11 + ";" + false;
            }
            Program.SaveSettings(statChange);
            Program.Settings();
        }
        private static void ResetFiles(int SoS, bool manual)
        {
            if (manual != true)
            {
                if (SoS == 0)
                {
                    DeleteScoreFile();
                }
                else
                {
                    DeleteSettingsFile();
                }
            }
            else
            {
                reDo:
                Console.Clear();
                Console.CursorVisible = false;
                Console.WriteLine("are you sure? the file will not be recoverable");
                Console.WriteLine("(Y/N)");
                var answer = Console.ReadKey(true).Key;
                switch (answer)
                {
                    case ConsoleKey.Y:
                        if (SoS == 0)
                        {
                            DeleteScoreFile();
                        }
                        else
                        {
                            DeleteSettingsFile();
                        }
                        break;
                    case ConsoleKey.N:
                        MainMenu();
                        break;
                    default:
                        goto reDo;
                }
                Console.WriteLine("file deleted, press Enter...");
                Console.ReadKey();
                Settings();
            }
        }
        private static void DeleteScoreFile()
        {
            File.Delete(Directory.GetCurrentDirectory() + "\\scores.txt"); //score savefile path
            File.Create(Directory.GetCurrentDirectory() + "\\scores.txt").Close();
            HighScore = 0;
            HighScoreName = "n/a";
        }
        private static void DeleteSettingsFile()
        {
            File.Delete(Directory.GetCurrentDirectory() + "\\settings.txt"); //settings savefile path
            File.Create(Directory.GetCurrentDirectory() + "\\settings.txt").Close();
            SetUp(true);
        }
        private static void SaveScore(int score, string playerName, bool replay)
        {
            try
            {
                LoadScores();
                if (Scores.ContainsKey(score))
                {
                    reDo:
                    Console.CursorVisible = true;
                    Console.WriteLine("a score with the same value is already saved");
                    Console.WriteLine("do you want to overwrite it (Y/N)");
                    var answer = Console.ReadKey(true).Key;
                    Console.CursorVisible = false;
                    switch (answer)
                    {
                        case ConsoleKey.Y:
                            ResetFiles(0, false);
                            Scores.Remove(score);
                            break;
                        case ConsoleKey.N:
                            goto skip;
                        default:
                            Console.Clear();
                            goto reDo;
                    }
                }
                Scores.Add(score, playerName);
                var tempList = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\scores.txt").ToList();
                for (int i = 0; i < Scores.Count; i++)
                {
                    if (!tempList.Contains(Scores.Keys.ElementAt(i) + ";" + Scores.Values.ElementAt(i)))
                    {
                        tempList.Add(Scores.Keys.ElementAt(i) + ";" + Scores.Values.ElementAt(i));
                    }
                }
                File.WriteAllLines(Directory.GetCurrentDirectory() + "\\scores.txt", tempList);
                Scores.Remove(score);
                skip:
                if (replay == false)
                {
                    reDo:
                    Console.CursorVisible = true;
                    Console.WriteLine("save replay (Y/N)");
                    var answer = Console.ReadKey(true).Key;
                    Console.CursorVisible = false;
                    switch (answer)
                    {
                        case ConsoleKey.Y:
                            SaveReplay(playerName);
                            MainMenu();
                            break;
                        case ConsoleKey.N:
                            File.Delete(Directory.GetCurrentDirectory() + "\\temp2.txt");
                            MainMenu();
                            break;
                        default:
                            Console.Clear();
                            goto reDo;
                    }
                    MainMenu();
                }
            }
            catch //cant ever happen
            {
                if (replay == false)
                {
                    reDo:
                    Console.WriteLine("score already set by you or another person");
                    Console.WriteLine("but you can still save the replay (Y/N)");
                    var answer = Console.ReadKey(true).Key;
                    switch (answer)
                    {
                        case ConsoleKey.Y:
                            SaveReplay(playerName);
                            MainMenu();
                            break;
                        case ConsoleKey.N:
                            File.Delete(Directory.GetCurrentDirectory() + "\\temp.txt");
                            MainMenu();
                            break;
                        default:
                            Console.Clear();
                            goto reDo;
                    }
                }
            }
        }
        private static void LoadScores()
        {
            try
            {
                Scores = File.ReadAllLines(Directory.GetCurrentDirectory() + "\\scores.txt").Select(line => line.Split(';')).ToDictionary(split => Convert.ToInt32(split[0]), split => split[1]);
                for (int i = 0; i < Scores.Count; i++)
                {
                    if (Scores.Keys.ElementAt(i) > HighScore)
                    {
                        HighScore = Scores.Keys.ElementAt(i);
                        HighScoreName = Scores[HighScore];
                    }
                }
            }
            catch
            {
                reDo:
                Console.Clear();
                Console.WriteLine("a duplicate score was found in the score file");
                Console.WriteLine("close the Program (Q), remove it and restart the program or press (D)");
                Console.WriteLine("note: pressing D will reset all scores");
                var answer = Console.ReadKey(true).Key;
                switch (answer)
                {
                    case ConsoleKey.Q:
                        Quit();
                        break;
                    case ConsoleKey.D:
                        ResetFiles(0, false);
                        LoadScores();
                        break;
                    default:
                        Console.Clear();
                        goto reDo;
                }
            }
        }
        private static void ShowScores()
        {
            HighScore = 0;
            HighScoreName = "n/a";
            if (!File.Exists(Directory.GetCurrentDirectory() + "\\scores.txt"))
            {
                File.Create(Directory.GetCurrentDirectory() + "\\scores.txt").Close();
            }
            Console.Clear();
            Console.WriteLine();
            LoadScores();
            var sortList = new List<int>();
            for (int i = 0; i < Scores.Count; i++)
            {
                sortList.Add(Scores.Keys.ElementAt(i));
            }
            sortList.Sort();
            var j = 1;
            for (int i = sortList.Count - 1; i >= 0; i--)
            {
                var score = new StringBuilder();
                if (j.ToString().Length < sortList.Count.ToString().Length)
                {
                    score.Insert(0, " ", sortList.Count.ToString().Length - j.ToString().Length);
                }
                score.Append(j + ". ");
                if (sortList[i].ToString().Length < HighScore.ToString().Length)
                {
                    score.Insert(score.Length - 1, " ", HighScore.ToString().Length - sortList[i].ToString().Length);
                }
                score.Append(sortList[i] + ", by " + Scores[sortList[i]]);
                Console.WriteLine(score);
                j++;
            }
            Console.WriteLine();
            Console.WriteLine("press Enter...");
            Console.ReadKey();
            MainMenu();
        }
    }
    class Player
    {     
        public static char Icon;
        public static bool Crashed;
        public static bool GodMode;
        public static int xLocation;
        public static int yLocation;
        //public static ConsoleKey Up;
        //public static ConsoleKey Down;
        public static ConsoleKey Left;
        public static ConsoleKey Right;
        public static List<string> OldLines = new List<string>();
        public static StringBuilder NewLine = new StringBuilder("");
        public static void ShowPlayer()
        {
            var PlayerLine = new StringBuilder(Player.OldLines[Player.yLocation]);
            if (PlayerLine[Player.xLocation] == Road.Icon && Player.GodMode != true)
            {
                Player.Crashed = true;
            }
            else
            {
                PlayerLine.Insert(Player.xLocation, Player.Icon);
                PlayerLine.Remove(Player.xLocation + 1, 1);
                Player.OldLines.Insert(Player.yLocation, PlayerLine.ToString());
                Player.OldLines.RemoveAt(Player.yLocation + 1);
            }
        }
        public static void ClearPlayerTrail()
        {
            for (int i = 0; i < OldLines.Count; i++)
            {
                var tailLine = new StringBuilder(Player.OldLines[i]);
                tailLine.Replace(Player.Icon, ' ');
                Player.OldLines.Insert(i, tailLine.ToString());
                Player.OldLines.RemoveAt(i + 1);
            }
            Player.ShowPlayer();
        }
        public static void Move()
        {
            var key = Console.ReadKey(true).Key;
            if (key == Left && Player.xLocation > 1)
            {
                Player.xLocation -= 1;
            }
            else if (key == Right && Player.xLocation < (Console.WindowWidth - 2))
            {
                Player.xLocation += 1;
            }
            else if (key == ConsoleKey.End)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("you quit");
                Player.Crashed = true;
            }
            /*
            else if (key == Down && Player.yLocation > 1)
            {
                Player.yLocation -= 1;
            }
            else if (key == Up && Player.yLocation < (Console.WindowHeight - 2 - FogOfWar))
            {
                Player.yLocation += 1;
            }
            */
        }
        public static void ToggleGodMode()
        {
            var statChange = "";
            if (GodMode != true)
            {
                statChange += 8 + ";" + true;
            }
            else
            {
                statChange += 8 + ";" + false;
            }
            Program.SaveSettings(statChange);
            Program.Settings();
        }
    }
    class Road
    {
        public static char Icon;
        public static bool Right;
        public static char FogIcon;
        public static int FogOfWar;
        public static int RoadWidth;
        public static int RoadChange;
        public static string FrameInfo;
        public static int RoadWidthDcrC;
        public static int[] RoadWidthDcrL;
        public static int GenerationSpeed = 5;
        public static Random Random = new Random();
        public static async void GenerateStartScreen()
        {
            var MapHeight = Console.WindowHeight - FogOfWar;
            for (int i = 0; i < MapHeight; i++)
            {
                Generate(true);
                Console.WriteLine(Player.NewLine.ToString());
                Player.NewLine.Clear();
                await Program.UpdateDisplay();
            }
        }
        public static void Generate(bool firstEx)
        {
            if (RoadWidth > (3 * RoadChange) && Program.Score % RoadWidthDcrL[0] == 0 && Program.Score != 0 && firstEx != true || RoadWidth > (2 * RoadChange) && Program.Score % RoadWidthDcrL[1] == 0 && Program.Score != 0 && firstEx != true)
            {
                RoadWidth -= RoadWidthDcrC;
            }
            var MapWidth = Console.WindowWidth;
            var RoadStart = Program.LastRoadStart;
            if (RoadStart < (2 * RoadChange))
            {
                RoadStart += RoadChange;
                Right = true;
            }
            else if  (RoadStart > MapWidth - RoadWidth - RoadChange)
            {
                RoadStart -= RoadChange;
                Right = false;
            }
            var RoadEnd = RoadStart + Road.RoadWidth;
            for (int i = 0; i < RoadStart; i++)
            {
                Player.NewLine.Insert(i, Road.Icon.ToString());
            }
            if (RoadStart >= 50 && firstEx == true && Player.OldLines.Count < 10 && RoadStart + (RoadWidth / 2) < RoadEnd)
            {
                Player.xLocation = RoadStart + (RoadWidth / 4);
            }
            for (int i = RoadStart; i < RoadEnd; i++)
            {
                Player.NewLine.Insert(i, " ");
            }
            if (RoadEnd < Player.xLocation + 10 && firstEx == true)
            {
                do
                {
                  Player.NewLine.Append(' ', 1);
                  RoadEnd++;
                } while (RoadEnd < Player.xLocation + 10);
            }
            for (int i = RoadEnd; i < MapWidth; i++)
            {
                Player.NewLine.Insert(i, Road.Icon.ToString());
            }
            SaveFrameInfo(RoadStart.ToString(), RoadEnd.ToString(), Player.xLocation.ToString());
            if (Right == true)
            {
                var leftright = Random.Next(0, 3);
                if (leftright == 0)
                {
                    RoadStart -= RoadChange;
                    Right = false;
                }
                else
                {
                    RoadStart += RoadChange;
                }
            }
            else
            {
                var leftright = Random.Next(0, 3);
                if (leftright == 0)
                {
                    RoadStart += RoadChange;
                    Right = true;
                }
                else
                {
                    RoadStart -= RoadChange;
                }
            }
            Program.LastRoadStart = RoadStart;
            if (firstEx == true)
            {
                Player.OldLines.Add(Player.NewLine.ToString());
                Player.NewLine.Clear();
            }
            VerifyMap();
            for (int i = 0; i < FogOfWar; i++)
            {
                Console.WriteLine(" ");
            }
        }
        private static void SaveFrameInfo(string RoadStart, string RoadEnd, string PlayerLocation)
        {
            FrameInfo += RoadStart;
            FrameInfo += ";" + RoadEnd;
            FrameInfo += ";" + PlayerLocation;
        }
        private static void VerifyMap()
        {
            for (int i = 0; i < Player.OldLines.Count - 1; i++)
            {
                if (string.IsNullOrEmpty(Player.OldLines[i]) || string.IsNullOrWhiteSpace(Player.OldLines[i]))
                {
                    Player.OldLines.Insert(i, Player.OldLines[i - 1]);
                    Player.OldLines.RemoveAt(i + 1);
                }
            }
        }
        public static void ChangeRoadChange()
        {
            Console.WriteLine("input new road change (integer): ");
            var newChange = Program.ConvertToInt32(Console.ReadLine());
            var statChange = 2 + ";" + newChange;
            Console.WriteLine("new road change is: " + newChange);
            RoadChange = newChange;
            Program.SaveSettings(statChange);
            Console.CursorVisible = false;
            Console.WriteLine("press Enter to go back");
            Console.ReadKey();
            Program.Settings();
        }
        public static void ChangeRoadWidth()
        {
            Console.WriteLine("input new road width (integer): ");
            var newWidth = Program.ConvertToInt32(Console.ReadLine());
            var statChange = 3 + ";" + newWidth;
            Console.WriteLine("new road width is: " + newWidth);
            RoadWidth = newWidth;
            Program.SaveSettings(statChange);
            Console.CursorVisible = false;
            Console.WriteLine("press Enter to go back");
            Console.ReadKey();
            Program.Settings();
        }
        public static void ChangeRoadWidthDcrC()
        {
            Console.WriteLine("input new road width decrease rate (integer): ");
            var newRoadWidthDcrC = Program.ConvertToInt32(Console.ReadLine());
            var statChange = 4 + ";" + newRoadWidthDcrC;
            Console.WriteLine("new road width decrease rate is: " + newRoadWidthDcrC);
            RoadWidthDcrC = newRoadWidthDcrC;
            Program.SaveSettings(statChange);
            Console.CursorVisible = false;
            Console.WriteLine("press Enter to go back");
            Console.ReadKey();
            Program.Settings();
        }
        public static void ChangeRoadWidthDcrL()
        {
            Console.WriteLine("input new road width decrease rate (X;Y): ");
            try
            {
                var newRoadWidthDcrL = Console.ReadLine();
                var statChange = 5 + ";" + newRoadWidthDcrL;
                Console.WriteLine("new road width decrease rate is: " + newRoadWidthDcrL);
                var y = newRoadWidthDcrL.Split(';'); int[] x = { Convert.ToInt32(y[0]), Convert.ToInt32(y[1]) }; RoadWidthDcrL = x;
                Program.SaveSettings(statChange);
                Console.CursorVisible = false;
                Console.WriteLine("press Enter to go back");
                Console.ReadKey();
                Program.Settings();
            }
            catch
            {
                Console.CursorVisible = false;
                Console.WriteLine("invalid character, press Enter...");
                Console.ReadKey();
                Program.MainMenu();
            }
        }
        public static void ChangeFogOfWar()
        {
            Console.WriteLine("input fog of war (integer): ");
            var newFogOfWar = Program.ConvertToInt32(Console.ReadLine());
            if (newFogOfWar > 27)
            {
                newFogOfWar = 27;
                Console.WriteLine("fog of war maximum value is 27");
            }
            var statChange = 6 + ";" + newFogOfWar;
            Console.WriteLine("new fog of war is: " + newFogOfWar);
            FogOfWar = newFogOfWar;
            Program.SaveSettings(statChange);
            Console.CursorVisible = false;
            Console.WriteLine("press Enter to go back");
            Console.ReadKey();
            Program.Settings();
        }
    }
    class MidBot
    {
        public static bool Toggled = false;
        public static bool Running = false;
        public static async void RunBot()
        {
            if (Toggled == true)
            {
                Running = true;
            }
            if (Running == true)
            {
                var topLine = new StringBuilder(Player.OldLines[Player.yLocation + 1]);
                int road = 0;
                for (int i = 0; i < topLine.Length; i++)
                {
                    if (topLine[i] == ' ')
                    {
                        road = i + (Road.RoadWidth / 2);
                        i = topLine.Length;
                    }
                }
                while (Player.xLocation > road && Player.xLocation > 1 && Player.xLocation < (Console.WindowWidth - 2))
                {
                    Player.xLocation -= 1;
                }
                while (Player.xLocation < road && Player.xLocation > 1 && Player.xLocation < (Console.WindowWidth - 2))
                {
                    Player.xLocation += 1;
                }
                await Program.TimerStart(1000 / Program.FrameRate);
                RunBot();
            }
        }
        public static void Toggle()
        {
            if (Toggled == false)
            {
                Toggled = true;
            }
            else
            {
                Toggled = false;
            }
            Program.Settings();
        }
    }
}