using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestApiTest
{
    public class SauceLabsMethods
    {
        //TODO - SPLIT THE HTTP Request building into separate class and add into separate project 
        public void saucelabsAuthentication(HttpWebRequest HttpWReq) //TODO Method to edit username/password (Interface?)
        {
            HttpWReq.ContentType = "application/json";
            HttpWReq.ContentLength = 0;
            String username = "JackBTest";
            String password = "9988f55c-336a-4177-883e-52c347933be4";
            String encoded =
                System.Convert.ToBase64String(
                    System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpWReq.Headers.Add("Authorization", "Basic " + encoded);
        }

        public SauceScreenshots SauceLabs_GetScreenshotAssets(string jobsID)
        {
            //TODO - blend auth into actual request builder - use constructors
            HttpWebRequest HttpWReq =
                (HttpWebRequest)WebRequest.Create("http://saucelabs.com/rest/v1/JackBTest/jobs/" + jobsID + "/assets");
            saucelabsAuthentication(HttpWReq);

            //TODO - separate Response handling into separate class
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            string json;
            using (var sr = new StreamReader(HttpWResp.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }
            SauceScreenshots sauceScreenshots = JsonConvert.DeserializeObject<SauceScreenshots>(json);
            return sauceScreenshots;
        }

        //TODO - Make methods more focused (GetLastJobRef & GetAllJobRef etc)
        public List<string> SauceLabs_LastJobsFunction(int jobNumber)
        {
            HttpWebRequest HttpWReq =
                (HttpWebRequest)WebRequest.Create("http://saucelabs.com/rest/v1/JackBTest/jobs?limit=" + jobNumber.ToString());
            saucelabsAuthentication(HttpWReq);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            string json;
            using (var sr = new StreamReader(HttpWResp.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }

            //TODO - Update this to use LINQ and select token for less code (see API test solution for examples). Give me a shout if you want some steer
            //You can cut the following down to just 2 lines of code ;)

            JArray jobs = JArray.Parse(json);
            List<string> jobList = Enumerable.Repeat("", jobNumber).ToList();
            jobList.Capacity = jobNumber;

            for (int i = 0; i < jobs.Count; i++)
            {
                jobList[i] = jobs[i].ToString().Substring(12, 32);
            }
            return jobList;
        }

        public void SauceLabs_DownloadLogs(string jobsID)
        {
            System.IO.Directory.CreateDirectory(jobsID);
            HttpWebRequest HttpWReq =
                (HttpWebRequest)
                    WebRequest.Create("http://saucelabs.com/rest/JackBTest/jobs/" + jobsID +
                                      "/results/selenium-server.log");
            saucelabsAuthentication(HttpWReq);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            Stream stream = HttpWResp.GetResponseStream();
            StreamReader oReader = new StreamReader(stream, Encoding.ASCII);
            StreamWriter oWriter = new StreamWriter(jobsID + "/selenium-server.log");
            oWriter.Write(oReader.ReadToEnd());
            oWriter.Close();
            oReader.Close();
            HttpWResp.Close();
        }

        public void SauceLabs_DownloadVideo(string jobsID)
        {
            System.IO.Directory.CreateDirectory(jobsID);
            HttpWebRequest HttpWReq =
                (HttpWebRequest)
                    WebRequest.Create(
                        "http://saucelabs.com/rest/JackBTest/jobs/" + jobsID + "/results/video.flv");
            saucelabsAuthentication(HttpWReq);
            String lsResponse = string.Empty;
            using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    using (FileStream lxFS = new FileStream(jobsID + "/Video.flv", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                        HttpWResp.Close();
                    }
                }
            }
        }
        public void SauceLabs_DownloadScreenShot(string jobsID)
        {
            System.IO.Directory.CreateDirectory(jobsID);
            HttpWebRequest HttpWReq =
                (HttpWebRequest)
                    WebRequest.Create("http://saucelabs.com/rest/JackBTest/jobs/" + jobsID +
                                      "/results/final_screenshot.png");
            saucelabsAuthentication(HttpWReq);
            String lsResponse = string.Empty;
            using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    using (FileStream lxFS = new FileStream(jobsID + "/final_screenshot.jpg", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                        HttpWResp.Close();
                    }
                }
            }
        }
        public void SauceLabs_DownloadALLScreenShot(string jobsID) //Get JSON of get assets and check the highest screenshot number
        {
            SauceScreenshots sauceScreenshots = SauceLabs_GetScreenshotAssets(jobsID);
            for (int i = 0; i < sauceScreenshots.screenshots.Count; i++)
            {
                System.IO.Directory.CreateDirectory(jobsID);
                HttpWebRequest HttpWReq =
                    (HttpWebRequest)
                        WebRequest.Create("http://saucelabs.com/rest/JackBTest/jobs/" + jobsID +
                                          "/results/" + sauceScreenshots.screenshots[i]);
                saucelabsAuthentication(HttpWReq);
                String lsResponse = string.Empty;
                using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                    {
                        Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                        using (FileStream lxFS = new FileStream(jobsID + "/" + sauceScreenshots.screenshots[i] + ".jpg",
                                FileMode.Create))
                        {
                            lxFS.Write(lnByte, 0, lnByte.Length);
                            HttpWResp.Close();
                        }
                    }
                }
            }
        }
        public void SauceLabs_DownloadAllJobAssets(int jobNumber)
        {
            var jobList = SauceLabs_LastJobsFunction(jobNumber);

            for (int j = 0; j < jobList.Count; j++)
            {
                SauceLabs_DownloadLogs(jobList[j]); //download logs from joblist location
                SauceLabs_DownloadVideo(jobList[j]); //download video from joblist location
                SauceScreenshots sauceScreenshots = SauceLabs_GetScreenshotAssets(jobList[j]);
                for (int i = 0; i < sauceScreenshots.screenshots.Count; i++)
                {
                    System.IO.Directory.CreateDirectory(jobList[j]);
                    HttpWebRequest HttpWReq =
                        (HttpWebRequest)
                            WebRequest.Create("http://saucelabs.com/rest/JackBTest/jobs/" + jobList[j] +
                                              "/results/" + sauceScreenshots.screenshots[i]);
                    saucelabsAuthentication(HttpWReq);
                    String lsResponse = string.Empty;
                    using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
                    {
                        using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                        {
                            Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                            using (
                                FileStream lxFS = new FileStream(jobList[j] + "/" + sauceScreenshots.screenshots[i] + ".jpg",
                                    FileMode.Create))
                            {
                                lxFS.Write(lnByte, 0, lnByte.Length);
                                HttpWResp.Close();
                            }
                        }
                    }
                }
            }
        }
    }
}
