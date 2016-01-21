using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonkyMonky
{
          


    /// <summary>
    /// Main Form
    /// </summary>
    public partial class Form1 : Form
    {
        //path to the config file
        string cfgFile = /*Application.StartupPath + */".\\config.cfg";

        //a list of directories, which are all scanned
        List<DirectoryInfo> watchDirs = new List<DirectoryInfo>();

        //this one is a bit tricky: a Array of dictionaries of Files.
        //This will serve as our database
        //Allows us to have all filenames of all directories in one place
        Dictionary<string, FileInfo>[] fileLists;

        List<string> filesToAddToDictionary = new List<string>();
        List<string> filesToRemoveFromDictionary = new List<string>();

        Timer timer;
        public Form1()
        {
            InitializeComponent();

            //Greet user
            label1.Text = "MonkyMonky Version 0.2.0";
            label2.Text = "Looking for configuration file " + cfgFile;

            //Check Cfg File
            if (File.Exists(cfgFile))
            {
                label2.Text += "... OK!";
                label2.ForeColor = Color.DarkGreen;

                //read configuration file into program
                string[] fileLines = File.ReadAllLines(cfgFile);

                //loop over all read lines
                foreach(string line in fileLines)
                {
                    //ignore comments
                    if(!line.StartsWith("#") && line != string.Empty)
                    { 
                        //if the current line is a valid directory
                        if(Directory.Exists(line))
                        {
                            //add it to our watchlist
                            watchDirs.Add(new DirectoryInfo(line));
                        }
                    }
                }

                //now we can instantiate the fileLists as we know the number of dictionaries
                fileLists = new Dictionary<string, FileInfo>[watchDirs.Count];
                
                
                //now process all found directories for the first time
                //use a for loop this time, as we must access the array of dictionaries by index
                //i represents the index of the directory, which is currently being processed
                for (int i = 0; i < watchDirs.Count; i++)
                {
                    //create a new dictionary for this directory
                    fileLists[i] = new Dictionary<string, FileInfo>();

                    //get files in this directory
                    FileInfo[] files = watchDirs[i].GetFiles("*", SearchOption.TopDirectoryOnly);

                    //now loop over all files
                    //foreach - pretty familiar by now?
                    foreach (FileInfo file in files)
                    {
                        //add the file to our list
                        fileLists[i].Add(file.FullName, file);
                    }
                }

                //now, we have setup our initial list. 
                //From now on, we want to periodically check for changes in the directories
                //we use a timer for that.
                timer = new Timer();
                timer.Interval = 800;
                timer.Tick += timer_Tick; //call this function, when the timer ticks (see below)
                timer.Start();
            }
            else
            {
                label2.Text += "... FAIL!";
                label2.ForeColor = Color.Red;
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //first of all, we need to pause the timer
            timer.Stop();

            //loop over all directories to watch
            //again, i is the running index of the current directory
            for (int i = 0; i < watchDirs.Count; i++)
            {
                FileInfo[] filesInDir = watchDirs[i].GetFiles();
                int fileCountInDir = filesInDir.Length;
                int fileCountInList = fileLists[i].Count;

                //compare number of entries
                if(fileCountInDir > fileCountInList)
                {
                    //number of files in the dir are bigger.
                    //look for the new files

                    //this is the big moment, find the new files!
                    //for this, we simply loop over all files in the directory
                    //and check if it is present in our list
                    foreach(FileInfo file in filesInDir)
                    {
                            
                        //this is why we use a dictionary instead of an ordinary list
                        //this expression is true when the currently reviewed file is not present in the dictionary
                        if(!fileLists[i].ContainsKey(file.FullName))
                        {
                            //we found it!

                            //too bad, we now have to loop over the files again, so an ordinary list would have been just as good
                            //but too lazy to change it back
                            foreach(KeyValuePair<string, FileInfo> dictEntry in fileLists[i])
                            {
                                //get the filename (with path and without)
                                string originalNewFileName = file.FullName;
                                string originalNewFileNameShort = file.Name;

                                //I've got the feeling, that this is a terrible waste of resources
                                //in case a copy of an existing file is made, windows adds a suffix to it.
                                //BUT: this suffix is dependant on the version of windows.
                                //what we do instead of looking for the suffix: we compare the beginning of the filenames

                                //before that. we have to remove the extension of the file, else the extensions is bad for the "newFileName.StartsWith() function"
                                //basically we look for the position of the "." which marks the extension and remove it an all other characters afterwards
                                string foundFileName = dictEntry.Key.Remove(dictEntry.Key.LastIndexOf("."), dictEntry.Key.Length - dictEntry.Key.LastIndexOf("."));
                                    
                                //do we have the right file
                                if (originalNewFileName.StartsWith(foundFileName))
                                {
                                    //this is it!

                                    //remove the extension of the file just like above for convenience
                                    string fileExtension = dictEntry.Value.Extension;
                                    string filePath = dictEntry.Value.Directory.FullName;
                                    originalNewFileNameShort = originalNewFileNameShort.Remove(originalNewFileNameShort.LastIndexOf("."), originalNewFileNameShort.Length - originalNewFileNameShort.LastIndexOf("."));

                                    //open a dialog and ask the user to name it
                                    //this is a small form from a different file.
                                    FormNewFileDialog dialog = new FormNewFileDialog(originalNewFileNameShort);
                                    DialogResult result = dialog.ShowDialog();
                                    if(result == DialogResult.OK)
                                    {
                                        //get the result from the dialog
                                        string newNewFileName = dialog.newFileName;
                                            
                                        //remove all but the filename so we know what to replace in the file
                                        string foundFileNameShort = dictEntry.Value.Name.Remove(dictEntry.Value.Name.LastIndexOf("."), dictEntry.Value.Name.Length - dictEntry.Value.Name.LastIndexOf("."));

                                        string createdFile = filePath + "\\" + newNewFileName + fileExtension;

                                        //in case the user has entered a file that already exists, we sould issue a warning
                                        bool skipFile = false;   //as long as the file does not exist and the user doesnt decide against over writing, we dont skip the file
                                        if(File.Exists(createdFile))
                                        {
                                            DialogResult overwriteResult = MessageBox.Show(
                                                    "The file '" + newNewFileName + fileExtension + "' already exists." + 
                                                    "\nDo you want to overwrite the file?" +
                                                    "\nIf you decide 'no', the file '" + originalNewFileNameShort + "' will be deleted.",
                                                "File already exists!",
                                                MessageBoxButtons.YesNo,
                                                MessageBoxIcon.Warning,
                                                MessageBoxDefaultButton.Button2
                                                );

                                            //when the user wants to NOT overwrite the file, we skip it
                                            if(overwriteResult == DialogResult.No)
                                            {
                                                skipFile = true;
                                            }
                                        }

                                        //when the file is not flagged for skipping, we procede
                                        if(!skipFile)
                                        { 
                                            //now create a new dummyfile with the function
                                            ReplaceTextInFile(originalNewFileName, createdFile, foundFileNameShort, newNewFileName);

                                            //Delete the original file - with " - Copy" suffix
                                            File.Delete(originalNewFileName);

                                            //now, add the new file to the directory
                                            //we cannot directly add the new entry into the dictionary
                                            //since we are currently looping over it. 
                                            //Instead, remember the file and add afterwards
                                            filesToAddToDictionary.Add(createdFile);
                                        }
                                        else
                                        {
                                            //Delete the original file - with " - Copy" suffix
                                            File.Delete(originalNewFileName);
                                        }
                                        //break from loop to save computing power, there wont be any more matches
                                        break;
                                    }
                                }
                            }

                            //remember that file? now add it!
                            foreach (string createdFile in filesToAddToDictionary)
                            {
                                //only add the file if it is not already present in the dictionary
                                if (!fileLists[i].ContainsKey(createdFile))
                                { 
                                    fileLists[i].Add(createdFile, new FileInfo(createdFile));
                                }
                            }
                            //now delete all items from this list so it can be populated again next round
                            filesToAddToDictionary.Clear();

                        }
                    }
                    
                }
                else if (fileCountInDir < fileCountInList)
                {
                    //a file has been removed. 
                    //delete if from the list

                    //previously we looped over the dictionary and looked for the first entry that is not in the dictionary
                    //now we do it the other way round: look in the dictionary for the first entry that does not exist as a file
                    foreach (KeyValuePair<string, FileInfo> dictEntry in fileLists[i])
                    {
                        //this is the file, we are looking for in this pass
                        string searchFile = dictEntry.Key;
                        bool presentInDir = false;

                        //look for this file in the directory
                        for(int j = 0; j< filesInDir.Length;j++)
                        {
                            //do to names match?
                            if(filesInDir[j].FullName == searchFile)
                            {
                                //yes? then this is not the file we are looking for
                                //http://www.nurbilder.com/pics/179eab-341.jpg
                                presentInDir = true;
                                break;
                            }
                        }

                        //after having a look at all the files, did we find it?
                        if (!presentInDir)
                        {
                            //no? then this must be the deleted file
                            //we now delete it from the dictionary.
                            //no directly. we remember to remove it later since we are processing the dictionary
                            //and cannot remove items from it while we do so
                            filesToRemoveFromDictionary.Add(searchFile);
                        }
                    }

                    //now we are done processing the dictionary and we can remove the entries
                    foreach (string deleteFile in filesToRemoveFromDictionary)
                    {
                        fileLists[i].Remove(deleteFile);
                    }
                    //now delete all items from this list so it can be populated again next round
                    filesToRemoveFromDictionary.Clear();
                }
                else
                {
                    //nothing changed
                }
            }

            //dont forget to restart the timer
            timer.Start();
        }

        
        private static void ReplaceTextInFile(string originalFile, string outputFile, string searchTerm, string replaceTerm)
        {
            // found on http://stackoverflow.com/questions/1915632/open-a-file-and-replace-strings-in-c-sharp

            string tempLineValue;
            using (FileStream inputStream = File.OpenRead(originalFile))
            {
                using (StreamReader inputReader = new StreamReader(inputStream, Encoding.Default))
                {
                    try //try executing the code
                    {   
                        using (StreamWriter outputWriter = File.CreateText(outputFile))
                        {
                            inputReader.Peek();
                            while (null != (tempLineValue = inputReader.ReadLine()))
                            {
                                outputWriter.WriteLine(tempLineValue.Replace(searchTerm, replaceTerm));
                            }
                        }
                    }
                    catch (IOException e) //catch every error
                    {
                        MessageBox.Show("File names must be changed!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
