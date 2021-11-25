using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jbh
{
    internal static class AppManager
    {
        /// <summary>
        /// 
        /// Add the following commented-out section to the App.Xaml.cs file (it assumes the startup window will be called MainWindow)
        /// 
        /// Then add 
        /// Startup="Application_Startup" Exit="Application_Exit"
        /// to the App.xaml file in place of "StartupUri=...
        /// 
        /// </summary>

        // NOTE Only edit AppManager.cs in the Launchpad application. The Launchpad/My Applications screen will show which apps have an out-of-date AppManager file
        // using the Launchpad's AppManager file as the definitive version
        // This file edited 14-08-2021

        /*
         private void Application_Startup(object sender, StartupEventArgs e)
        {
            bool OkToLaunch = true;
            string titl = Jbh.AppManager.AppName;
            if (string.IsNullOrWhiteSpace(Jbh.AppManager.DataPath))
            {
                MessageBox.Show("The folder for the " + titl + " application data within the Databank\\AppData directory has not been found.\n\nThe application will now be shut down.", titl, MessageBoxButton.OK, MessageBoxImage.Error);
                OkToLaunch = false;
            }
            else
            {
                DateTime? launched = Jbh.AppManager.RunStart;
                if (launched.HasValue)
                {

                    MessageBoxResult answer = MessageBox.Show(titl + " appears to be already running.\n\nLaunched at " + launched.Value.ToShortTimeString() + " on " + launched.Value.ToShortDateString() + "\n\nOnly continue if you are sure that " + titl + " is not currently running.\n\nContinue?", titl, MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.OK)
                    {
                        OkToLaunch = false;
                    }
                }
            }
            if (OkToLaunch)
            {
                Jbh.AppManager.SetRunStart();
                MainWindow w = new MainWindow();
                w.Show();
            }
            else
            {
                App.Current.Shutdown();
            }
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Jbh.AppManager.DeleteRuntimeFile();
        }
        */

        private static string _dataPath;

        internal static string DataPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_dataPath)) { _dataPath = UsualDataPath; }
                return _dataPath;
            }
        }

        internal static string AppName
        {
            get
            {
                string apNam = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
                if (apNam.EndsWith(".vshost")) { apNam = apNam.Substring(0, apNam.Length - 7); } // in case running in IDE
                return apNam;
            }
        }

        internal static DateTime? RunStart
        {
            get
            {
                DateTime? retVal = null;
                if (System.IO.File.Exists(RunTimeFile))
                {
                    string s = System.IO.File.ReadAllText(RunTimeFile);
                    DateTime runTimeStart = DateTime.Parse(s);
                    retVal = runTimeStart;
                }
                return retVal;
            }
        }

        private static string RunTimeFile
        {
            get
            {
                return Path.Combine(DataPath, "_runtime.txt");
            }
        }

        internal static void SetRunStart()
        {
            DateTime start = DateTime.Now;
            File.WriteAllText(RunTimeFile, start.ToString());
        }

        internal static void DeleteRuntimeFile()
        {
            File.Delete(RunTimeFile);
        }

        private static string UsualDataPath
        {
            // Revised Àugust 2021
            // Looks for a directory on the C: drive of this computer matching the path specified in the 'JBH' user environment variable
            // Then appends "AppData" and the name of this application.
            // OR if running on USB drive, returns the applicable path

            // Assumes that on the home (C) drive the location of the Jbh.Info folder which contains the AppData folder is recorded in the JBH environment variable
            // And that in the case of my travelling USB drive the path is [Root]\Jbh.Portable\Jbh.Info
            get
            {
                string myPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
                DriveInfo di = new DriveInfo(myPath);
                string RunLocationMessage;
                string AppDataPath;
                if (di.RootDirectory.Name.StartsWith("C", StringComparison.InvariantCultureIgnoreCase))
                {
                    // We are installed on the current machine, or run from an EXE on the current machine
                    RunLocationMessage = "Running on C drive";
                    AppDataPath = Path.Combine(di.RootDirectory.FullName, "Jbh.Original", "Jbh.Info", "AppData", AppName);
                }
                else
                {
                    // We are running on my travelling USB drive
                    RunLocationMessage = $"Running on travelling drive: {di.VolumeLabel}";
                    AppDataPath = Path.Combine(di.RootDirectory.FullName, "Jbh.Portable", "Jbh.Info", "AppData", AppName);
                }
                if (!string.IsNullOrEmpty(AppDataPath) && Directory.Exists(AppDataPath)) { return AppDataPath; }
                System.Windows.MessageBox.Show($"{RunLocationMessage}\n\nFailed to locate the AppData path: {AppDataPath}", AppName, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error); return string.Empty;
            }
        }

        internal static void CreateBackupDataFile(string DataFileSpec)
        {
            string Extn = Path.GetExtension(DataFileSpec);
            FileInfo fi = new FileInfo(DataFileSpec);
            string TimeStamp = $"{fi.LastWriteTimeUtc:yyyy-MM-dd-HH-mm-ss})";
            string BackupPath = System.IO.Path.Combine(_dataPath, $"Backup-{TimeStamp}{Extn}");
            if (File.Exists(BackupPath)) { File.Delete(BackupPath); }
            if (File.Exists(DataFileSpec)) { File.Copy(DataFileSpec, BackupPath); }
        }

        internal static void PurgeOldBackups(string FileExtension, int MinimumDaysToKeep, int MinimumFilesToKeep)
        {
            if (!FileExtension.StartsWith(".", StringComparison.InvariantCultureIgnoreCase)) { FileExtension = "." + FileExtension; }
            List<string> PreviousFiles = System.IO.Directory.GetFiles(_dataPath, "Backup-????-??-??-??-??-??" + FileExtension, System.IO.SearchOption.TopDirectoryOnly).ToList();
            if (PreviousFiles.Count <= MinimumFilesToKeep) { return; }

            int OverAged;
            do
            {
                OverAged = 0;
                string oldest = string.Empty;
                DateTime oldestDate = DateTime.MaxValue;
                foreach (string s in PreviousFiles)
                {
                    System.IO.FileInfo fi = new System.IO.FileInfo(s);
                    TimeSpan FileAge = DateTime.UtcNow - fi.LastWriteTimeUtc;
                    if (FileAge.TotalDays > MinimumDaysToKeep)
                    {
                        OverAged++;
                        if (fi.LastWriteTimeUtc < oldestDate) { oldestDate = fi.LastWriteTimeUtc; oldest = s; }
                    }
                }
                if (!string.IsNullOrWhiteSpace(oldest)) { System.IO.File.Delete(oldest); PreviousFiles.Remove(oldest); OverAged--; }
            } while ((PreviousFiles.Count > MinimumFilesToKeep) && (OverAged > 0));
        }

        public static string VersionString()
        {
            Version versionInfo = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            return $"{versionInfo.Major}.{versionInfo.Minor}";
        }

    }
}

