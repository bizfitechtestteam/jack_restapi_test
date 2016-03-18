using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RestApiTest
{
    public class SauceScreenshots
    {
        public List<string> screenshots { get; set; }
    }
    public class SauceLogs
    {
        public List<string> screenshots { get; set; }
    }
    public class RootObject
    {
        public string id { get; set; }
    }

    [TestClass]
    public class ApiTests
    {
        private string jobID = "eed6028fd1d54cd6bba36e462f328289";
        SauceLabsMethods sauceMethods = new SauceLabsMethods();
        
        [TestMethod]
        public void SauceLabs_Last10JobID()
        {
            sauceMethods.SauceLabs_LastJobsFunction(2);
        }

        [TestMethod]
        public void SauceLabs_DownloadScreenShot()
        {
            var jobsID = sauceMethods.SauceLabs_LastJobsFunction(0);
            sauceMethods.SauceLabs_DownloadScreenShot(jobsID[0]);
        }

        [TestMethod]
        public void SauceLabs_DownloadALLScreenShot()
        {
            var jobsID = sauceMethods.SauceLabs_LastJobsFunction(1);
            sauceMethods.SauceLabs_DownloadALLScreenShot(jobsID[0]);
        }

        [TestMethod]
        public void SauceLabs_DownloadLogs()
        {
            sauceMethods.SauceLabs_DownloadLogs(jobID);
        }

        [TestMethod]
        public void SauceLabs_DownloadVideo()
        {
           sauceMethods.SauceLabs_DownloadVideo(jobID);
        }

        [TestMethod]
        public void SauceLabs_DownloadAllJobAssets()
        {
            sauceMethods.SauceLabs_DownloadAllJobAssets(2);
        }
    }
}