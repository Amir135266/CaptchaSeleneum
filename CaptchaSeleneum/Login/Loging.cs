using IronOcr;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Net;
using System.Threading;

namespace CaptchaSeleneum.Login
{
    [TestFixture]
    public class Loging
    {
        IWebDriver webDriver = null;
        IWebElement webElement = null;
        readonly int milliSeconds = 2000;

        [SetUp]
        public void Openbrwser()
        {
            webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl("https://eg-task.buzz/login.php");
        }

        [Test]
        public void SendAndClick()
        {
            webElement = webDriver.FindElement(By.XPath("//*[@id='email']"));
            webElement.SendKeys("amirhossineedalat2020@gmail.com");

            webElement = webDriver.FindElement(By.XPath("//*[@id='password']"));
            webElement.SendKeys("amir1352");

            webElement = webDriver.FindElement(By.XPath("//*[@id='login-form']/div[4]/button"));
            webElement.Click();
            Thread.Sleep(milliSeconds);

            webDriver.FindElement(By.LinkText("Challenge typing speed")).Click();
            Thread.Sleep(milliSeconds);

            //var done = webDriver.FindElement(By.Id("btnSubmitId"));
            //done.Click();
            //Thread.Sleep(milliSeconds);

            var framename = webDriver.FindElement(By.Id("ccc"));
            String ImageUrl = framename.GetAttribute("src");
            string ImageName = framename.GetAttribute("alt");

            var ocr = new IronTesseract();
            using (var input = new OcrInput(@"C:\Users\hassa\Desktop\New folder\CaptchaSeleneum\CaptchaSeleneum\imging\C1.png"))
            {
                var result = ocr.Read(input);

                result.SaveAsTextFile(@"C:\Users\hassa\Desktop\New folder\CaptchaSeleneum\CaptchaSeleneum\imging\CAP.txt");
            }


            //webDriver.FindElement(By.XPath("//*[@id='ccc']")).Click();

            //webElement.SendKeys(framename);

            webElement = webDriver.FindElement(By.TagName("input"));
            String title = webElement.GetAttribute("src");
        }
        
        [TearDown]
        public void TearDown()
        {
            TestContext.WriteLine("Closed!");
        }
    }
}