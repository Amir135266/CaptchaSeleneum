using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Drawing;
using System.IO;
using System.Threading;
using Tesseract;

namespace CaptchaSeleneum.Login
{
    [TestFixture]
    public class Loging
    {
        IWebDriver webDriver = null;
        IWebElement webElement = null;
        readonly string filePath = "";
        readonly int milliSeconds = 3000;

        [SetUp]
        public void Openbrwser()
        {
            webDriver = new ChromeDriver();
            webDriver.Manage().Window.Maximize();
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

            var remElement = webDriver.FindElement(By.Id("ccc"));
            Point location = remElement.Location;

            var screenshot = (webDriver as ChromeDriver).GetScreenshot();
            using (MemoryStream stream = new(screenshot.AsByteArray))
            {
#pragma warning disable CA1416 // Validate platform compatibility
                using (Bitmap bitmap = new(stream))
#pragma warning restore CA1416 // Validate platform compatibility
                {
                    RectangleF part = new(location.X, location.Y, remElement.Size.Width, remElement.Size.Height);
#pragma warning disable CA1416 // Validate platform compatibility
                    using (Bitmap bn = bitmap.Clone(part, bitmap.PixelFormat))
#pragma warning restore CA1416 // Validate platform compatibility
                    {
#pragma warning disable CA1416 // Validate platform compatibility
                        bn.Save(filePath + "CaptchImage.png", System.Drawing.Imaging.ImageFormat.Png);
#pragma warning restore CA1416 // Validate platform compatibility
                    }
                }
            }



            //reading text from images
            using (var engine = new TesseractEngine(@"C:\Users\a.edalati\Downloads\CaptchaSeleneum\CaptchaSeleneum\tessdata", "png", EngineMode.Default))
            {
                //engine.SetVariable("tessedit_char_whitelist", "");
                Tesseract.Page ocrPage = engine.Process(Pix.LoadFromFile(filePath + "CaptchImage.png"), PageSegMode.AutoOnly);
                var captchatext = ocrPage.GetText();
            }


            //webDriver.SwitchTo().Frame(webDriver
            //    .FindElement(By.XPath("//*[@id='ccc']")));

            //webDriver.SwitchTo().DefaultContent();

            //webDriver.FindElement(By.ClassName("center-row")).Click();

            //webDriver.SwitchTo().DefaultContent();
            //IList<IWebElement> images = webDriver.FindElements(By.TagName("img"));
            //webDriver.SwitchTo().Frame(webDriver.FindElements(By.TagName("iframe"))[1]);
            //images = webDriver.FindElements(By.CssSelector("img"));

            //Thread.Sleep(milliSeconds);

            //webDriver.SwitchTo().Frame(0);

            //new WebDriverWait(webDriver, TimeSpan.FromSeconds(3))
            //    .Until(ExpectedConditions.FrameToBeAvailableAndSwitchToIt(By.XPath("//iframe[@title='reCAPTCHA']")));
            //Screenshot ss = ((ITakesScreenshot)webDriver).GetScreenshot();
            //ss.SaveAsFile("C:/Users/a.edalati/Downloads/CaptchaSeleneum/CaptchaSeleneum/Image1.jpg");
            //ss.ToString();



            //var done = webDriver.FindElement(By.Id("btnSubmitId"));
            //done.Click();
            //Thread.Sleep(milliSeconds);

            //var framename = webDriver.FindElement(By.Id("ccc"));
            //String ImageUrl = framename.GetAttribute("src");
            //string ImageName = framename.GetAttribute("alt");

            //var ocr = new IronTesseract();
            //using (var input = new OcrInput(@"C:\Users\hassa\Desktop\New folder\CaptchaSeleneum\CaptchaSeleneum\imging\C1.png"))
            //{
            //    var result = ocr.Read(input);

            //    result.SaveAsTextFile(@"C:\Users\hassa\Desktop\New folder\CaptchaSeleneum\CaptchaSeleneum\imging\CAP.txt");
            //}


            ////webDriver.FindElement(By.XPath("//*[@id='ccc']")).Click();

            ////webElement.SendKeys(framename);

            //webElement = webDriver.FindElement(By.TagName("input"));
            //String title = webElement.GetAttribute("src");

            //webElement = webDriver.FindElement(By.CssSelector("//*[@id='verify-code']")).SendKeys(ss.AsBase64EncodedString);

        }


        [TearDown]
        public void TearDown()
        {
            TestContext.WriteLine("Closed!");
        }
    }
}