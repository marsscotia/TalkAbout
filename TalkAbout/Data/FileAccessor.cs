using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace TalkAbout.Data
{
    /// <summary>
    /// 
    /// Class reads and writes files to local app storage.
    /// 
    /// </summary>
    public class FileAccessor
    {

        /// <summary>
        /// 
        /// Method accesses the specified file, and returns
        /// the json string within.  
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public async Task<string> getJsonString(string filename)
        {
            string result = "";

            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            try
            {
                StorageFile jsonFile = await localFolder.GetFileAsync(filename);
                result = await FileIO.ReadTextAsync(jsonFile);
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine("The" + filename + " file was not found.");
                
            }

            return result;
        }

        public async void writeJsonFile(string filename, string jsonString)
        {
            StorageFolder localFolder = ApplicationData.Current.LocalFolder;

            StorageFile jsonFile = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(jsonFile, jsonString);
        }

        
    }
}
