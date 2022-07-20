using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace CaptchaSeleneum.Login
{
    [TestFixture]
    [Parallelizable]
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

            var done = webDriver.FindElement(By.Id("btnSubmitId"));
            done.Click();
            Thread.Sleep(milliSeconds);

            String framename = webDriver.FindElement(By.XPath("//*[@id='ccc']")).GetAttribute("src");
            webDriver.SwitchTo().Frame(framename);
            //webDriver.FindElement(By.XPath("//*[@id='ccc']")).Click();
            webElement.SendKeys(framename);

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