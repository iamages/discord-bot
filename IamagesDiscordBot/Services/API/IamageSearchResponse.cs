using System;
using System.Collections.Generic;
using System.Text;

namespace IamagesDiscordBot.Services.API
{
    using Newtonsoft.Json;

    public partial class IamageSearchResponse
    {
        [JsonProperty("FileDescription")]
        public string searchTag;

        [JsonProperty("FileIDs")]
        public int[] FileIDs;
    }

    public partial class IamageSearchResponse
    {
        public static IamageSearchResponse FromJson(string json) => JsonConvert.DeserializeObject<IamageSearchResponse>(json);
    }

}
