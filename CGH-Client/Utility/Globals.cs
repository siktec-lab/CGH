using CGH_Client.Forms;
using CGH_Client.Networking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace CGH_Client.Utility
{
    public static class Globals
    {
        public static bool showLayoutDebug = false;
        public static string baseDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        public static string currentScreenName = "None";
        public static object currentScreen = null;

        public static string gameChoosed = "None"; //TODO : Always set to None when selecting a game screen
        public static string hostOrJoin = "None";
        public static object gameRoom = null;

        public static Image charImgSelected;
        public static int charTagSelected;
        public static string charName;

        public static Client ServerConnector;
        public static string serverIP = "127.0.0.1";

        // A List of all required Directories
        public static List<string> directories = new List<string> {
            
        };

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
    }
}
