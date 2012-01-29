using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Storage;
using System.IO;

namespace EatMyDust
{
    public static class HighScoreManager
    {
        public static StorageDevice storageDevice;// used for saving/loading games
        public static IAsyncResult syncResult;  // to asynchronously get a storage device
        public static bool deviceRequested; // to coordinate so the EndShowSelector only gets called once. 
        public static List<KeyValuePair<int, string>> highScoreList = new List<KeyValuePair<int, string>>(); // high score list that is in memory
        public static char[] tempInitialsOne = { 'A', 'A', 'A' }; //starting initials for high score entry
        public static char[] tempInitialsTwo = { 'A', 'A', 'A' }; //starting initials for high score entry

        /// <summary>
        /// Stores a reference to the active storage device.
        /// </summary>
        public static void GetStorageDevice()
        {
            storageDevice = StorageDevice.EndShowSelector(syncResult);
        }

        /// <summary>
        /// Saves the high score entry to the storage device
        /// </summary>
        /// <param name="initials">Player's initials</param>
        /// <param name="highScore">Player's score</param>
        public static void SaveHighScoreEntry(String initials, int highScore)
        {
            // if storage device is disconnected, we're SOL
            if (storageDevice != null && storageDevice.IsConnected)
            {
                DoLoadGame(storageDevice);//load the scores from disk to memory
                AddHighScoreToList(initials, highScore);//add current score to memory
                DoSaveGame(storageDevice);// save back to disk
            }
        }

        /// <summary>
        /// Adds a high score to the list in memory.
        /// </summary>
        /// <param name="initials">The initials of the player</param>
        /// <param name="highScore">The score of the player</param>
        private static void AddHighScoreToList(String initials, int highScore)
        {
            highScoreList.Add(new KeyValuePair<int, string>(highScore, initials));
            highScoreList.Sort(CompareScores);
            if (highScoreList.Count > 30)
            {
                highScoreList.RemoveAt(30);
            }
        }

        /// <summary>
        /// This method serializes a data object into
        /// the StorageContainer for this game.
        /// </summary>
        /// <param name="device">the device</param>
        /// <param name="initials">the initials representing the player</param>
        /// <param name="highScore">the score</param>
        private static void DoSaveGame(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("HighScores", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (container.FileExists(filename))
                // Delete it so that we can create one fresh.
                container.DeleteFile(filename);

            // Create the file.
            Stream stream = container.CreateFile(filename);

            using (StreamWriter sw = new StreamWriter(stream))
            {
                for (int i = 0; i < highScoreList.Count; i++)
                {
                    sw.WriteLine("{0}\t{1}", highScoreList[i].Value, highScoreList[i].Key);
                }
                sw.Close();
            }

            // Close the file.
            stream.Close();

            // Dispose the container, to commit changes.
            container.Dispose();
        }

        /// <summary>
        /// Loads the high scores from the storage device
        /// </summary>
        /// <returns></returns>
        public static void LoadHighScores()
        {
            if (storageDevice != null && storageDevice.IsConnected)
            {
                DoLoadGame(storageDevice);
            }
        }

        /// <summary>
        /// This method loads a serialized data object
        /// from the StorageContainer for this game.
        /// </summary>
        /// <param name="device"></param>
        private static void DoLoadGame(StorageDevice device)
        {
            // Open a storage container.
            IAsyncResult result = device.BeginOpenContainer("HighScores", null, null);

            // Wait for the WaitHandle to become signaled.
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            // Close the wait handle.
            result.AsyncWaitHandle.Close();

            string filename = "savegame.sav";

            // Check to see whether the save exists.
            if (!container.FileExists(filename))
            {
                // If not, dispose of the container and return.
                container.Dispose();
                return;
            }

            // Open the file.
            Stream stream = container.OpenFile(filename, FileMode.Open);

            // read all of the scores from the file and add them to the list in memory
            using (StreamReader sr = new StreamReader(stream))
            {
                String input;
                highScoreList.Clear();
                while ((input = sr.ReadLine()) != null)
                {
                    char separator = '\t';
                    String initials = input.Split(separator)[0];
                    int score = int.Parse(input.Split(separator)[1]);
                    highScoreList.Add(new KeyValuePair<int, string>(score, initials));
                }
                highScoreList.Sort(CompareScores);
                sr.Close();
            }

            // Close the file.
            stream.Close();

            // Dispose the container.
            container.Dispose();
        }

        /// <summary>
        /// Sorts scores by score: highest to lowest, then by initials alphabetically.
        /// </summary>
        /// <param name="score1">First score</param>
        /// <param name="score2">Second score</param>
        /// <returns>-1 if first is less than second, 0 if equal, 1 if first is greater than second</returns>
        private static int CompareScores(KeyValuePair<int, string> score1, KeyValuePair<int, string> score2)
        {
            if (score1.Key == score2.Key)
            {
                return score1.Value.CompareTo(score2.Value);
            }
            else
            {
                return -score1.Key.CompareTo(score2.Key);
            }
        }
    }
}
