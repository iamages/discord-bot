using System;
using System.Collections.Generic;
using System.Text;

namespace IamagesDiscordBot.Services.API
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    public partial class IamageSearchResponse
    {
        [JsonProperty("FileDescription")]
        public string searchTag;

        [JsonProperty("FileIDs")]
        public int[] FileIDs;
    }

}
