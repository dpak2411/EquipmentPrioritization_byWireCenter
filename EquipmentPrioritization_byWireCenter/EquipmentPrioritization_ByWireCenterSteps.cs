using System;
using System.Collections.Generic;
using AlteryxGalleryAPIWrapper;
using AnalogStoreAnalysis;
using HtmlAgilityPack;
using Newtonsoft.Json;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace EquipmentPrioritization_byWireCenter
{
    [Binding]
    public class EquipmentPrioritization_ByWireCenterSteps
    {
        public string alteryxurl;
        public string _sessionid;
        private string _appid;
        private string _userid;
        private string _appName;
        private string jobid;
        private string outputid;
        private string validationId;

        // public delegate void DisposeObject();
        //private Client Obj = new Client("https://devgallery.alteryx.com/api/");
        Client Obj = new Client("https://gallery.alteryx.com/api");
        RootObject jsString = new RootObject();


        [Given(@"alteryx running at ""(.*)""")]
        public void GivenAlteryxRunningAt(string url)
        {
            alteryxurl = url;
        }
        
        [Given(@"I am logged in using ""(.*)"" and ""(.*)""")]
        public void GivenIAmLoggedInUsingAnd(string user, string password)
        {
            _sessionid = Obj.Authenticate(user, password).sessionId;
        }
        
        [When(@"I run analog store analysis with Average Number of Employees per Business on avgEmployee ""(.*)"" avgBandWidth  ""(.*)"" voiceBandwidth ""(.*)"" dataBandwidth ""(.*)""")]
        public void WhenIRunAnalogStoreAnalysisWithAverageNumberOfEmployeesPerBusinessOnAvgEmployeeAvgBandWidthVoiceBandwidthDataBandwidth(string avgEmployee, string avgBandWidth, string voiceBandwidth, string dataBandwidth)
        {
            string response = Obj.SearchApps("Equipment Prioritization");
            var appresponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(response);
            int count = appresponse["recordCount"];
            _userid = appresponse["records"][0]["owner"]["id"];
            _appName = appresponse["records"][0]["primaryApplication"]["fileName"];
            _appid = appresponse["records"][0]["id"];
            jsString.appPackage.id = _appid;
            jsString.userId = _userid;
            jsString.appName = _appName;
            string appinterface = Obj.GetAppInterface(_appid);
            dynamic interfaceresp = JsonConvert.DeserializeObject(appinterface);
            //List<Jsonpayload.Question> questionAnsls = new List<Jsonpayload.Question>();
            //questionAnsls.Add(new Jsonpayload.Question("AvgEmployeer", avgEmployee));
            //questionAnsls.Add(new Jsonpayload.Question("AvgBandwidth", avgBandWidth));
            //questionAnsls.Add(new Jsonpayload.Question("VoiceBandwidth", voiceBandwidth));
            //questionAnsls.Add(new Jsonpayload.Question("DataBandwidth", dataBandwidth));
            //jsString.jobName = "Checks";
            List<Jsonpayload.Question> questionAnsls = new List<Jsonpayload.Question>();
            questionAnsls.Add(new Jsonpayload.Question("AvgEmployeer", avgEmployee));
            questionAnsls.Add(new Jsonpayload.Question("AvgBandwidth", avgBandWidth));
            questionAnsls.Add(new Jsonpayload.Question("VoiceBandwidth", voiceBandwidth));
            questionAnsls.Add(new Jsonpayload.Question("DataBandwidth", dataBandwidth));
           // questionAnsls.Add(new Jsonpayload.Question("Maximum Search Distance", maxSearchDistance));
            jsString.questions.AddRange(questionAnsls);
            jsString.jobName = "Job Name";
            var postData = new System.Web.Script.Serialization.JavaScriptSerializer().Serialize(jsString);
            string postdata = postData.ToString();
            string resjobqueue = Obj.QueueJob(postdata);
            var jobqueue = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(resjobqueue);
            jobid = jobqueue["id"];

            int counts = 0;
            string status = "";

        CheckValidate:
            System.Threading.Thread.Sleep(1000);
            if (status == "Completed" && counts < 15)
            {
                //string disposition = validationStatus.disposition;
            }
            else if (counts < 15)
            {
                string jobstatusresp = Obj.GetJobStatus(jobid);
                var statusResponse = new System.Web.Script.Serialization.JavaScriptSerializer().Deserialize<Dictionary<string, dynamic>>(jobstatusresp);
                status = statusResponse["status"];
                goto CheckValidate;
            }

            else
            {
                throw new Exception("Complete Status Not found");

            }
 
        }

        [Then(@"I see the EquipmentPrioritizatiobyWireCenter result ""(.*)""")]
        public void ThenISeeTheEquipmentPrioritizatiobyWireCenterResult(string result)
        {
            string getmetadata = Obj.GetOutputMetadata(jobid);
            dynamic metadataresp = JsonConvert.DeserializeObject(getmetadata);
            int count = metadataresp.Count;
            for (int j = 0; j <= count - 1; j++)
            {
                outputid = metadataresp[j]["id"];
            }
            string getjoboutput = Obj.GetJobOutput(jobid, outputid, "html");
            string htmlresponse = getjoboutput;
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlresponse);
            string output = doc.DocumentNode.SelectSingleNode("//span[@class='DefaultNumericText']").InnerText;
            StringAssert.Contains(result, output);
        }

        
    }
}
