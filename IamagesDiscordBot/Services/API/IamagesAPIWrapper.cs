using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace IamagesDiscordBot.Services.API
{
    public class IamagesAPIWrapper
    {
        private string baseReferenceEndpoint = "https://iamages.uber.space/iamages/api/";
        private string infoBaseEndpoint = "https://iamages.uber.space/iamages/api/info/";

        public IamagesAPIWrapper()
        {
            GetConfiguration();
        }

        private void GetConfiguration()
        {
            // create config to do
        }

        public string getImgEmbed(int FileId)
        {
            string endpoint = baseReferenceEndpoint + "embed/" + FileId;
            return endpoint;
        }

        public string getImage(int FileId)
        {
            string endpoint = baseReferenceEndpoint + "img/" + FileId;
            return endpoint;
        }

        public IamageModel getImgInfo(int FileId)
        {
            string endpoint = infoBaseEndpoint + FileId;
            IamageModel imageInfo = IamageModel.FromJson(GetJsonFromEndpoint(endpoint));
            return imageInfo;
        }
   
        public IamageModel getRandom()
        {
            string endpoint = baseReferenceEndpoint + "random";
            IamageModel imageInfo = IamageModel.FromJson(GetJsonFromEndpoint(endpoint));
            return imageInfo;
        }

        private string GetJsonFromEndpoint(string uri)
        {
            string result = ""; // json == string
            using (WebClient wc = new WebClient())
            {
                result = wc.DownloadString(uri);
            }
            return result;
        }
    }
}
