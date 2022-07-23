using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CaptchaSeleneum.Captcha
{
    [TestFixture]
    [Parallelizable]
    public class ImageCh
    {
        [Test]
        public static void run()
        {
            //string token = Config.TOKEN;
            ImageTypersAPI i = new("","",2000);

            // balance
            string balance = i.account_balance();
            Console.WriteLine(string.Format("Balance: {0}", balance));

            // optional parameters dict
            Dictionary<string, string> image_params = new();
            //image_params.Add("iscase", "true");         // case sensitive captcha
            //image_params.Add("isphrase", "true");       // text contains at least one space (phrase)
            //image_params.Add("ismath", "true");         // instructs worker that a math captcha has to be solved
            //image_params.Add("alphanumeric", "1");      // 1 - digits only, 2 - letters only
            //image_params.Add("minlength", "2");         // captcha text length (minimum)
            //image_params.Add("maxlength", "5");         // captcha text length (maximum)

            Console.WriteLine("Waiting for captcha to be solved...");
            string captcha_id = i.submit_image("captcha.jpg", image_params);
            Dictionary<string, string> response = i.retrieve_response(captcha_id);
            Utils.print_response(response);
        }
    }
}