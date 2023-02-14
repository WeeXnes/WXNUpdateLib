using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Application = System.Windows.Forms.Application;

namespace UpdaterLib
{
    public static class Updater
    {
        public static void CheckForUpdate(string githubUrl, string currentVersion)
        {
            try
            {
                using(WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Authorization", "Basic :x-oauth-basic");
                    webClient.Headers.Add("User-Agent","lk-github-clien");
                    var downloadString = webClient.DownloadString(githubUrl);
                    GithubApiResponse apiResponseData = JsonConvert.DeserializeObject<GithubApiResponse>(downloadString);
                    if (apiResponseData.tag_name !=  currentVersion)
                    {
                        Console.WriteLine("Update found");
                        DownloadUpdate(apiResponseData);
                    }
                }  
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void DownloadUpdate(GithubApiResponse res)
        {
            if (File.Exists(res.assets[0].name))
            {
                File.Delete(res.assets[0].name);
            }
            using(WebClient webClient = new WebClient())
            {;
                
                webClient.DownloadFile(
                    res.assets[0].browser_download_url, 
                    res.assets[0].name
                );
            }  
            ExecuteUpdater(res.assets[0].name);
        }
        private static void ExecuteUpdater(string zipFile)
        {
            try
            {
                string path = Application.StartupPath;
                string fileName = Path.GetFileName(Application.ExecutablePath);
                string pid = Process.GetCurrentProcess().Id.ToString();
                string updaterargs = $"\"{fileName}\" \"{pid}\" \"{zipFile}\"";
                Process updateProc = Process.Start("WXNUpdate.exe", updaterargs);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine(ex.Message);
                Console.WriteLine("Couldnt start Updater.exe");
            }
        }
        private class GithubApiResponse
        {
            public string tag_name { get; set; }
            public IList<GithubAsset> assets { get; set; }

            public GithubApiResponse(string tag_name, IList<GithubAsset> assets)
            {
                this.tag_name = tag_name;
                this.assets = assets;
            }
            public override string ToString()
            {
                return this.tag_name + " with " + this.assets.Count;
            }
        }

        public class GithubAsset
        {
            public string browser_download_url { get; set; }
            public string name { get; set; }

            public GithubAsset(string browser_download_url, string name)
            {
                this.browser_download_url = browser_download_url;
                this.name = name;
            }
            public override string ToString()
            {
                return this.name;
            }
        }
    }
}