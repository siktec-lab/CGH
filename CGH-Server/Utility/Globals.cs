using CGH_Server.Networking;
using Newtonsoft.Json;
using System;
using System.Security.AccessControl;
using System.Text.RegularExpressions;

namespace CGH_Server.Utility
{
    public static class Globals
    {

        public static string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        // A List of all required Directories
        public static List<string> directories = new List<string> {
            "GameLobbies"
        };


        public static List<TCP> ClientTCPS = new List<TCP>();

        public static bool EnableEncryption = true;
        
        public static Router? RouterServer;

        public static PhoneBook phoneBook = new();
        
        public static List<string> gameCards = new()
        {
            "cardClubs_A", "cardClubs_2", "cardClubs_3", "cardClubs_4", "cardClubs_5", "cardClubs_6", "cardClubs_7", "cardClubs_8", "cardClubs_9", "cardClubs_10", "cardClubs_J", "cardClubs_Q", "cardClubs_K", "cardDiamonds_A", "cardDiamonds_2", "cardDiamonds_3", "cardDiamonds_4", "cardDiamonds_5", "cardDiamonds_6", "cardDiamonds_7", "cardDiamonds_8", "cardDiamonds_9", "cardDiamonds_10", "cardDiamonds_J", "cardDiamonds_Q", "cardDiamonds_K", "cardHearts_A", "cardHearts_2", "cardHearts_3", "cardHearts_4", "cardHearts_5", "cardHearts_6", "cardHearts_7", "cardHearts_8", "cardHearts_9", "cardHearts_10", "cardHearts_J", "cardHearts_Q", "cardHearts_K", "cardSpades_A", "cardSpades_2", "cardSpades_3", "cardSpades_4", "cardSpades_5", "cardSpades_6", "cardSpades_7", "cardSpades_8", "cardSpades_9", "cardSpades_10", "cardSpades_J", "cardSpades_Q", "cardSpades_K"
        };

        public static void ServerDebug(string who = "Router", string message = "Debug Maeesage")
        {
            Console.ResetColor();
            Console.Write("[");
            Console.ForegroundColor = who == "Router" ? ConsoleColor.Yellow : ConsoleColor.Cyan;
            Console.Write(who);
            Console.ResetColor();
            Console.WriteLine($"] {message}");
        }
        /// <summary>
        ///     A function to get a directory path and create the directory if it does not exist
        ///     Only directories that are in the directories list are created
        /// </summary>
        /// <param name="directory">The name name of the directory</param>
        /// <returns>
        ///     empty string when an error happens or the dir path
        /// </returns>
        public static string GetDirectoryPath(string directory)
        {
            // Concatenate the base directory with the directory with the correct path separator
            string path = Path.Combine(baseDirectory, directory);

            if (!Directory.Exists(path))
            {
                if (directories.Contains(directory))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error creating the base directories that are required for the server to run.");
                        Console.WriteLine(e.Message);
                        return "";
                    }
                }
            }
            return path;
        }

        /// <summary>
        /// Return a path to a file while ensuring that the directory exists.
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string ServerPathToFile(string directory, string file)
        {
            string dir = GetDirectoryPath(directory);

            // if empty dir just use the strings:
            return Path.Combine(
                dir == "" ? baseDirectory : dir, file
            );
        }

        public static string FilterGameRoomAndBuildPath(string filename)
        {
            Regex rgx = new("[^a-zA-Z0-9_\\-]");
            string gameTypeFiltered = rgx.Replace(filename, "") + ".json";
            return ServerPathToFile("GameLobbies", gameTypeFiltered);
        }
        public static string FilterGameRoomAndBuildPath(string gameType, int roomCode)
        {
            return FilterGameRoomAndBuildPath(gameType + "-" + roomCode.ToString());
        }

        public static GameRoom? GetGameRoom(string gameType, int roomCode)
        {
            string path = FilterGameRoomAndBuildPath(gameType, roomCode);
            if (!File.Exists(path))
            {
                return null;
            }
            string json = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<GameRoom>(json);
        }
        
        public static GameRoom? GetGameRoom(string fullGamePath)
        {
            if (!File.Exists(fullGamePath))
            {
                return null;
            }
            string json = File.ReadAllText(fullGamePath);
            return JsonConvert.DeserializeObject<GameRoom>(json);
        }
        
    }
}
