using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace RestApiTest
{
    [TestClass]
    public class Browserstack //TODO Find API for Screenshots & Videos AND Rerun some tests on new trial account for videos/screenshots
    {
        private string bsJobID = "8dad3583ea4b83e8017f8521a58220ccb1b9ba32";
        public void browserstackAuthentication(HttpWebRequest HttpWReq)//TODO Method to edit username/password (Interface?)
        {
            HttpWReq.ContentType = "application/json";
            HttpWReq.ContentLength = 0;
            String username = "jackbroughton1";
            String password = "NcRCWGxhpysLA2EfMYzp";
            String encoded =
                System.Convert.ToBase64String(
                    System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes(username + ":" + password));
            HttpWReq.Headers.Add("Authorization", "Basic " + encoded);
        }
        [TestMethod]
        public void BrowserStackGetJobs()
        {
            HttpWebRequest HttpWReq =(HttpWebRequest)WebRequest.Create("https://www.browserstack.com/automate/builds/d24ac11a1da95642e7bdb0dc380f85499b0b98f3/sessions.json");
            browserstackAuthentication(HttpWReq);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            string json;
            using (var sr = new StreamReader(HttpWResp.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }
            HttpWResp.Close();
        }
        [TestMethod]
        public void BrowserStackGetSession()
        {
            HttpWebRequest HttpWReq = (HttpWebRequest)WebRequest.Create("https://www.browserstack.com/automate/sessions/"+ bsJobID+ ".json");
            browserstackAuthentication(HttpWReq);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            string json;
            using (var sr = new StreamReader(HttpWResp.GetResponseStream()))
            {
                json = sr.ReadToEnd();
            }
            HttpWResp.Close();
        }

        [TestMethod]
        public void BrowserStackGetLogs()
        {
            System.IO.Directory.CreateDirectory(bsJobID);
            HttpWebRequest HttpWReq =
                (HttpWebRequest)
                    WebRequest.Create("https://www.browserstack.com/automate/builds/<build-id>/sessions/"+ bsJobID + "/logs");
            browserstackAuthentication(HttpWReq);
            HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse();

            Stream stream = HttpWResp.GetResponseStream();
            StreamReader oReader = new StreamReader(stream, Encoding.ASCII);
            StreamWriter oWriter = new StreamWriter(bsJobID + "/selenium-server.log");
            oWriter.Write(oReader.ReadToEnd());
            oWriter.Close();
            oReader.Close();
            HttpWResp.Close();
        }

        [TestMethod]
        public void BrowserStack_DownloadVideo()
        {
            System.IO.Directory.CreateDirectory(bsJobID);
            HttpWebRequest HttpWReq =(HttpWebRequest)WebRequest.Create("https://bs-video-logs-euw.s3-eu-west-1.amazonaws.com/ad087cb0180cae61c5e7add4251160663abb7282/video-ad087cb0180cae61c5e7add4251160663abb7282.mp4?AWSAccessKeyId=AKIAJRYJ4U7UXXFIHWLQ&Expires=1465714410&Signature=tOWYT8zSR%2BlEk4UUiHSnGCjj7kQ%3D&response-content-disposition=attachment&response-content-type=video%2Fmp4");
            String lsResponse = string.Empty;
            using (HttpWebResponse HttpWResp = (HttpWebResponse)HttpWReq.GetResponse())
            {
                using (BinaryReader reader = new BinaryReader(HttpWResp.GetResponseStream()))
                {
                    Byte[] lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                    using (FileStream lxFS = new FileStream(bsJobID + "/video.flv", FileMode.Create))
                    {
                        lxFS.Write(lnByte, 0, lnByte.Length);
                        HttpWResp.Close();
                    }
                }
            }
        }
    }
}