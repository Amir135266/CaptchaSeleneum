using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptchaSeleneum
{
    public class ImageTypersAPI
    {
        private static string CAPTCHA_ENDPOINT = "http://captchatypers.com/Forms/UploadFileAndGetTextNEW.ashx";
        private static string RECAPTCHA_SUBMIT_ENDPOINT = "http://captchatypers.com/captchaapi/UploadRecaptchaV1.ashx";
        private static string RECAPTCHA_ENTERPRISE_SUBMIT_ENDPOINT = "http://captchatypers.com/captchaapi/UploadRecaptchaEnt.ashx";
        private static string BALANCE_ENDPOINT = "http://captchatypers.com/Forms/RequestBalance.ashx";
        private static string BAD_IMAGE_ENDPOINT = "http://captchatypers.com/Forms/SetBadImage.ashx";
        private static string GEETEST_SUBMIT_ENDPOINT = "http://captchatypers.com/captchaapi/UploadGeeTest.ashx";
        private static string GEETEST_V4_SUBMIT_ENDPOINT = "http://www.captchatypers.com/captchaapi/UploadGeeTestV4.ashx";
        private static string HCAPTCHA_ENDPOINT = "http://captchatypers.com/captchaapi/UploadHCaptchaUser.ashx";
        private static string CAPY_ENDPOINT = "http://captchatypers.com/captchaapi/UploadCapyCaptchaUser.ashx";
        private static string TIKTOK_ENDPOINT = "http://captchatypers.com/captchaapi/UploadTikTokCaptchaUser.ashx";
        private static string FUNCAPTCHA_ENDPOINT = "http://captchatypers.com/captchaapi/UploadFunCaptcha.ashx";
        private static string RETRIEVE_JSON_ENDPOINT = "http://captchatypers.com/captchaapi/GetCaptchaResponseJson.ashx";
        private static string CAPTCHA_ENDPOINT_CONTENT_TOKEN = "http://captchatypers.com/Forms/UploadFileAndGetTextNEWToken.ashx";
        private static string CAPTCHA_ENDPOINT_URL_TOKEN = "http://captchatypers.com/Forms/FileUploadAndGetTextCaptchaURLToken.ashx";
        private static string RECAPTCHA_SUBMIT_ENDPOINT_TOKEN = "http://captchatypers.com/captchaapi/UploadRecaptchaToken.ashx";
        //private static string BALANCE_ENDPOINT_TOKEN = "http://captchatypers.com/Forms/RequestBalanceToken.ashx";
        private static string BAD_IMAGE_ENDPOINT_TOKEN = "http://captchatypers.com/Forms/SetBadImageToken.ashx";
        private static string GEETEST_SUBMIT_ENDPOINT_TOKEN = "http://captchatypers.com/captchaapi/UploadGeeTestToken.ashx";
        private static string USER_AGENT = "csharpAPI1.0";      // user agent used in requests
        private string _access_token;
        private string _username = "";
        private string _password = "";
        private string _affiliateid;
        private int _timeout;


        public ImageTypersAPI(string access_token, string affiliate_id = "0", int timeout = 120)
        {
            this._access_token = access_token;
            this._affiliateid = affiliate_id;
            this._timeout = timeout * 1000;
        }

        public string submit_image(string captcha, Dictionary<string, string> optional_parameters)
        {
            string url, image_data = "";
            // use name value collection in this case
            var d = new Dictionary<string, string>();
            d.Add("action", "UPLOADCAPTCHA");

            // optional parameters
            if (optional_parameters.Count > 0)
            {
                // add optional parameters up
                foreach (var k in optional_parameters.Keys) d.Add(k, optional_parameters[k]);
            }

            if (!string.IsNullOrWhiteSpace(this._username))
            {
                // legacy way
                d.Add("username", this._username);
                d.Add("password", this._password);
                url = CAPTCHA_ENDPOINT;
                // check if file/captcha image exists
                if (!File.Exists(captcha))
                {
                    throw new Exception(string.Format("captcha image file does not exist: {0}", captcha));
                }

                image_data = Utils.read_local_captcha(captcha);
            }
            else
            {
                // token way
                if (captcha.ToLower().StartsWith("http"))
                {
                    url = CAPTCHA_ENDPOINT_URL_TOKEN;
                    image_data = captcha;
                }
                else
                {
                    url = CAPTCHA_ENDPOINT_CONTENT_TOKEN;
                    if (!File.Exists(captcha))
                    {
                        throw new Exception(string.Format("captcha image file does not exist: {0}", captcha));
                    }
                    image_data = Utils.read_local_captcha(captcha);
                }
                d.Add("token", this._access_token);
            }

            // affiliate id
            if (!string.IsNullOrWhiteSpace(this._affiliateid) && this._affiliateid.ToString() != "0")
            {
                d.Add("affiliateid", this._affiliateid);
            }

            d.Add("file", image_data);

            // string fro dict
            var post_data = "";
            foreach (var e in d)
            {
                post_data += string.Format("{0}={1}&", e.Key, e.Value);
            }

            post_data = post_data.Substring(0, post_data.Length - 1);

            string response = Utils.POST(url, post_data, USER_AGENT, this._timeout);       // make request
            if (response.Contains("ERROR:"))
            {
                var response_err = response.Split(new string[] { "ERROR:" }, StringSplitOptions.None)[1].Trim();
                throw new Exception(response_err);
            }

            return response.Split('|')[0];
        }

        public Dictionary<string, string> retrieve_response(string captcha_id)
        {
            string url = "";
            var d = new Dictionary<string, string>();
            d.Add("action", "GETTEXT");
            d.Add("captchaid", captcha_id);
            if (!string.IsNullOrWhiteSpace(this._username))
            {
                d.Add("username", this._username);
                d.Add("password", this._password);
            }
            else d.Add("token", this._access_token);

            // affiliate id
            if (!string.IsNullOrWhiteSpace(this._affiliateid) && this._affiliateid.ToString() != "0") d.Add("affiliateid", this._affiliateid);

            var post_data = Utils.list_to_params(d);        // transform dict to params
            string response = Utils.POST(RETRIEVE_JSON_ENDPOINT, post_data, USER_AGENT, this._timeout);       // make request
            if (response.Contains("ERROR:"))
            {
                var response_err = response.Split(new string[] { "ERROR:" }, StringSplitOptions.None)[1].Trim();
                response_err = response_err.Split('"')[0].Trim();
                throw new Exception(response_err);
            }
            response = response.Substring(1, response.Length - 2);
            var data = Newtonsoft.Json.JsonConvert.DeserializeAnonymousType(response, new Dictionary<string, string>());
            // not solved yet
            if (data["Status"] == "Pending") return null;
            // set as recaptcha [id] response and return
            return data;
        }

        #region others
        /// <summary>
        /// Get account balance
        /// </summary>
        /// <returns></returns>
        public string account_balance()
        {
            // create dict with params
            string url = "https://eg-task.buzz/verifycode.php";
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("action", "REQUESTBALANCE");
            d.Add("submit", "Submit");

            if (!string.IsNullOrWhiteSpace(this._username))
            {
                d.Add("amirhossineedalat2020@gmail.com", this._username);
                d.Add("amir1352", this._password);
                url = BALANCE_ENDPOINT;
            }
            else
            {
                //d.Add("", this._access_token);
                //url = BALANCE_ENDPOINT_TOKEN;
            }

            var post_data = Utils.list_to_params(d);        // transform dict to params
            string response = Utils.POST(url, post_data, USER_AGENT, this._timeout);       // make request
            if (response.Contains("ERROR:"))
            {
                var response_err = response.Split(new string[] { "ERROR:" }, StringSplitOptions.None)[1].Trim();
                throw new Exception(response_err);
            }

            return string.Format("${0}", response);        // return response/balance
        }

        /// <summary>
        /// Set captcha as bad
        /// </summary>
        /// <param name="captcha_id"></param>
        /// <returns></returns>
        public string set_captcha_bad(string captcha_id)
        {
            // check if captcha is OK
            if (string.IsNullOrWhiteSpace(captcha_id))
            {
                throw new Exception("catpcha ID is null or empty");
            }

            string url = "";
            // create dict with params
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("action", "SETBADIMAGE");
            d.Add("imageid", captcha_id);
            d.Add("submit", "Submissssst");

            if (!string.IsNullOrWhiteSpace(this._username))
            {
                d.Add("username", this._username);
                d.Add("password", this._password);
                url = BAD_IMAGE_ENDPOINT;
            }
            else
            {
                d.Add("token", this._access_token);
                url = BAD_IMAGE_ENDPOINT_TOKEN;
            }

            var post_data = Utils.list_to_params(d);        // transform dict to params
            string response = Utils.POST(url, post_data, USER_AGENT, this._timeout);       // make request
            if (response.Contains("ERROR:"))
            {
                var response_err = response.Split(new string[] { "ERROR:" }, StringSplitOptions.None)[1].Trim();
                throw new Exception(response_err);
            }

            return response;
        }
        #endregion

        #region misc
        /// <summary>
        /// Set timeout (in seconds)
        /// </summary>
        /// <param name="timeout"></param>
        public void set_timeout(int timeout)
        {
            this._timeout = timeout * 1000;     // requests timeout is in milliseconds, multiply by 1k
        }
        #endregion
    }
}
