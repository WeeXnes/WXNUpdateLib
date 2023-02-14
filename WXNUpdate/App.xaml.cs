using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Windows;
using Application = System.Windows.Forms.Application;

namespace WXNUpdate
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            DoUpdate(
                Application.StartupPath,
                e.Args[0],
                e.Args[1],
                e.Args[2]
            );
        }
        private void DoUpdate(string workingDir, string fileToLaunchPostUpdate, string processIdToKill, string zipFileName)
        {

            try
            {
                Process p = Process.GetProcessById(Convert.ToInt32(processIdToKill));
                p.Kill();
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            ZipArchiveHelper.ExtractToDirectory(zipFileName, workingDir, true);
            if (File.Exists(zipFileName))
            {
                File.Delete(zipFileName);
            }
            Process.Start(fileToLaunchPostUpdate);
            Environment.Exit(0);
        }
    }
    public static class ZipArchiveHelper
    {
        public static void ExtractToDirectory(string archiveFileName, string destinationDirectoryName, bool overwrite)
        {
            
            using (ZipArchive archive = ZipFile.OpenRead(archiveFileName))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entry.ExtractToFile(Path.Combine(destinationDirectoryName, entry.FullName), true);
                }
            }  
        }
    }
}