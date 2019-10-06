using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LOMN.Menu
{
    public class FileImporter : EditorWindow
    {
        public enum VerisonType { None, Vanilla, Alpha, Beta, Rebuilt };
        private enum PathFieldType { Folder, File };

        VerisonType version;
        string sourcePath;
        bool deleteOldFiles;

        string blkFilePath;
        string xFilePath;

        const string charactersFolder = "characters";
        const string levelsFolder = "levels";

        readonly string[] exceptionFilesForDelete = new string[] { "placeholder.txt", "placeholder.txt.meta" };

        void OnEnable()
        {
            blkFilePath = Path.Combine(Application.dataPath, "..", "Outresources", "BlkConverter", "blkfile.py");
            xFilePath = Path.Combine(Application.dataPath, "..", "Outresources", "XFileConverter", "LOMNTool.exe");
        }

        void OnGUI()
        {
            GUILayout.Label("File Import", EditorStyles.boldLabel);

            version= (VerisonType)EditorGUILayout.EnumPopup("Version:", version);

            GUILayout.BeginHorizontal(GUILayout.Height(30));
            sourcePath = EditorGUILayout.TextField(" Source Path", sourcePath, GUILayout.MaxWidth(400));
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(75)))
                sourcePath = EditorUtility.OpenFolderPanel("Add Source Path", "", "");
            GUILayout.EndHorizontal();

            deleteOldFiles = EditorGUILayout.Toggle("Delete Old Files", deleteOldFiles);

            if (GUILayout.Button("Import"))
                Import();
        }

        void Import()
        {
            if(version == VerisonType.None)
            {
                Debug.LogError("Version can't be None.");
                return;
            }

            if (!IsFileReady("Blk File Converter", blkFilePath) || !IsFileReady("X File Converter", xFilePath) ||
                !IsVersionEmpty() || !IsGamePathReady())
                return;

            ImportFiles();
            ConvertBlkFiles();
            ConvertXFiles();

            Debug.Log(version.ToString() + " is ready");
        }

        void ImportFiles()
        {
            string versionName = version.ToString().ToLower();
            string baseTargetPath = Path.Combine(Application.dataPath, "Resources", versionName);
            string sourceCharacterPath = Path.Combine(sourcePath, charactersFolder);
            string sourceLevelsPath = Path.Combine(sourcePath, levelsFolder);

            List<string> files = GetFiles(sourceCharacterPath);
            files.AddRange(GetFiles(sourceLevelsPath));
            var progressBar = new ProgressBar(files.Count, versionName + "'s files import", "Start");

            CopyFolder(sourceCharacterPath, Path.Combine(baseTargetPath, charactersFolder), progressBar);
            CopyFolder(sourceLevelsPath, Path.Combine(baseTargetPath, levelsFolder), progressBar);

            progressBar.Hide();
            Debug.Log(versionName + "'s files is imported");
        }

        void CopyFolder(string sourcePath, string targetPath, ProgressBar progressBar)
        {
            Directory.CreateDirectory(targetPath);
            string[] files = Directory.GetFiles(sourcePath);
            foreach (string file in files)
            {
                progressBar.Info = file;
                progressBar.Next();
                progressBar.Refresh();
                File.Copy(file, Path.Combine(targetPath, Path.GetFileName(file)), true);
            }

            string[] folders = Directory.GetDirectories(sourcePath);
            foreach (string folder in folders)
            {
                string folderName = folder.Substring(sourcePath.Length + 1);
                CopyFolder(folder, Path.Combine(targetPath, folderName), progressBar);
            }
        }

        void ConvertBlkFiles()
        {
            string versionName = version.ToString().ToLower();
            string baseTargetPath = Path.Combine(Application.dataPath, "Resources", versionName);

            List<string> files = GetFiles(Path.Combine(baseTargetPath, charactersFolder),"*.blk");
            files.AddRange(GetFiles(Path.Combine(baseTargetPath, levelsFolder), "*.blk"));

            var progressBar = new ProgressBar(files.Count, versionName + "'s blk files convert.", "Start");

            var process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            foreach(string file in files)
            {
                progressBar.Info = file;
                progressBar.Next();
                progressBar.Refresh();
                startInfo.Arguments = "/C "+ blkFilePath + " -c "+ file;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                File.Delete(file);
            }

            progressBar.Hide();
            Debug.Log(versionName + "'s blk files is converted.");
        }

        void ConvertXFiles()
        {
            string versionName = version.ToString().ToLower();
            string baseTargetPath = Path.Combine(Application.dataPath, "Resources", versionName);

            List<string> files = GetFiles(Path.Combine(baseTargetPath, charactersFolder), "*.x");
            files.AddRange(GetFiles(Path.Combine(baseTargetPath, levelsFolder), "*.x"));

            var progressBar = new ProgressBar(files.Count, versionName + "'s x files concert.", "Start");

            var process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";

            foreach (string file in files)
            {
                progressBar.Info = file;
                progressBar.Next();
                progressBar.Refresh(); 
                startInfo.Arguments = "/C " + xFilePath + " " + file;
                process.StartInfo = startInfo;
                process.Start();
                process.WaitForExit();

                File.Delete(file);
            }

            progressBar.Hide();
            Debug.Log(versionName + "'s x files is converted.");
        }

        bool IsVersionEmpty()
        {
            string versionName = version.ToString().ToLower();
            string baseTargetPath = Path.Combine(Application.dataPath, "Resources", versionName);
            string targetCharacterPath = Path.Combine(baseTargetPath, charactersFolder);
            string targetLevelsPath = Path.Combine(baseTargetPath, levelsFolder);


            var folders = new List<string>(Directory.GetDirectories(targetCharacterPath));
            folders.AddRange(Directory.GetDirectories(targetLevelsPath));

            if(!deleteOldFiles && folders.Count > 0)
            {
                Debug.LogError(versionName + " has folders.");
                return false;
            }


            var files = new List<string>(Directory.GetFiles(targetCharacterPath));
            files.AddRange(Directory.GetFiles(targetLevelsPath));
            foreach (string file in exceptionFilesForDelete)
            {
                files.Remove(Path.Combine(targetCharacterPath, file));
                files.Remove(Path.Combine(targetLevelsPath, file));
            }

            if(!deleteOldFiles && files.Count > 0)
            {
                Debug.LogError(versionName + " has files.");
                return false;
            }


            if (deleteOldFiles)
            {
                var progressBar = new ProgressBar(folders.Count + files.Count, versionName + "'s files delete", "Start");

                foreach (var folder in folders)
                {
                    progressBar.Info = folder;
                    progressBar.Next();
                    progressBar.Refresh();
                    Directory.Delete(folder, true);
                }

                foreach (var file in files)
                {
                    progressBar.Info = file;
                    progressBar.Next();
                    progressBar.Refresh();
                    File.Delete(file);
                }

                progressBar.Hide();
                Debug.Log(versionName + "'s files is deleted");
            }

            return true;
        }

        bool IsGamePathReady()
        {
            string versionName = version.ToString().ToLower();

            if (String.IsNullOrEmpty(sourcePath))
            {
                Debug.LogWarning(versionName + " path is empty, that's why it will be skiped.");
                return false;
            }

            if (!Directory.Exists(sourcePath))
            {
                Debug.LogError(versionName + " directory doesn't exist.");
                return false;
            }

            string[] folders = Directory.GetDirectories(sourcePath);
            bool foundCharacters = false;
            bool foundLevels = false;

            foreach (string folderPath in folders)
            {
                string folder = folderPath.Substring(sourcePath.Length + 1);
                if (folder == charactersFolder)
                    foundCharacters = true;

                if (folder == levelsFolder)
                    foundLevels = true;

                if (foundCharacters && foundLevels)
                    break;
            }


            if (!foundCharacters || !foundLevels)
            {
                Debug.LogError(versionName + " directory hasn't characters and levels folders.");
                return false;
            }

            return true;
        }

        bool IsFileReady(string name, string path)
        {
            if (String.IsNullOrEmpty(path))
            {
                Debug.LogError(name + " path is empty.");
                return false;
            }

            if (!File.Exists(path))
            {
                Debug.LogError(name + " file doesn't exist.");
                return false;
            }

            return true;
        }

        List<string> GetFiles(string path, string searchPattern="*")
        {
            var list = new List<string>(Directory.GetFiles(path, searchPattern));

            string[] folders = Directory.GetDirectories(path);
            foreach (string folder in folders)
                list.AddRange(GetFiles(folder, searchPattern));

            return list;
        }
    }
}