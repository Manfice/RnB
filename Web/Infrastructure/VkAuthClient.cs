using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using Web.Models.Viewmodels;


namespace Web.Infrastructure
{
    public static class VkAuthClient
    {
        private const string AppId = "5749182";
        private const string AppSecret = "t8O6Gq5Pc9KcipsBLmzw";
        private const string CallbackUrl = "https://redblackclub.ru/Auth/GetVkCodeCallback";

        public static VkSecret GetToken(string code)
        {
            var url = GetAccessTokenUrl(code);
            var responseStr = GetRequest(url);
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<VkSecret>(responseStr);
        }

        public static VkUserInfo GetUserDetails(VkSecret token)
        {
            var url = GetUsersDataUrl(token);
            var response = GetRequest(url);
            var serializer = new JavaScriptSerializer();
            return serializer.Deserialize<VkResponse>(response).response[0];
        }

        public static string GetRequest(string url)
        {
            var wr = WebRequest.Create(url);

            var objStream = wr.GetResponse().GetResponseStream();

            if (objStream == null) return string.Empty;
            var objReader = new StreamReader(objStream);

            var sb = new StringBuilder();
            while (true)
            {
                var line = objReader.ReadLine();
                if (line != null) sb.Append(line);

                else
                {

                    return sb.ToString();
                }
            }
        }

        public static string GetLoginUrl(string code, string scope)
        {
            return $"http://oauth.vk.com/authorize?client_id={AppId}&scope={scope}&redirect_uri={CallbackUrl}&response_type={code}";
        }
        public static string GetAccessTokenUrl(string code)
        {
            return $"https://oauth.vk.com/access_token?client_id={AppId}&client_secret={AppSecret}&redirect_uri={CallbackUrl}&code={code}";
        }
        public static string GetUsersDataUrl(VkSecret code)
        {
            const string fields = "bdate,city, photo_200_orig,screen_name,sex,nickname,connections,domain";
            return $"https://api.vk.com/method/users.get?user_ids={code.user_id}&fields={fields}&v=5.60&access_token={code.access_token}";
        }

        internal class VkResponse
        {
            public VkUserInfo[] response { get; set; }
        }
    }
}