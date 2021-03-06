﻿using System;
using System.Collections.Generic;
using System.Net;
using RestSharp;
using System.Text;
using Newtonsoft.Json;

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

        #region GeneralIamageGET

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
        public string getImgThumb(int FileId)
        {
            string endpoint = baseReferenceEndpoint + "thumb/" + FileId;
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

        #endregion

        #region iamagesSearch
        public static string searchResponse = "";
        public IamageSearchResponse postSearch(string searchTag, string username = null, string password = null)
        {
            string requestString =
                "{\"UserName\": \"" + username + "\"," +
                "\"UserPassword\": \"" + password + "\"," +
                "\"FileDescription\": \"" + searchTag + "\"}";

            var client = new RestClient();
            IRestRequest request = new RestRequest()
            {
                Resource = baseReferenceEndpoint + "search"
            };
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("Accept", "application/json");
            request.AddJsonBody(requestString);
            var response = client.Post(request);

            if (response.StatusCode == HttpStatusCode.OK) //will always be true if the convo is successful
            {
                Console.WriteLine("found!");
                var output = IamageSearchResponse.FromJson(response.Content);
                return output;
            }
            else
            {
                return null;
            }   
        }

        #endregion
    }
}
