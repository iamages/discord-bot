
namespace IamagesDiscordBot.Services.API
{
    using System;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class IamageModel
    {
        [JsonProperty("FileID")]
        public int ImageID;

        [JsonProperty("FileDescription")]
        public string FileDesc;

        [JsonProperty("FileNSFW")]
        public bool FileNSFW;

        [JsonProperty("FilePrivate")]
        public bool FilePrivate;

        [JsonProperty("FileMime")]
        public FileMime FileMime;

        [JsonProperty("FileWidth")]
        public long FileWidth;

        [JsonProperty("FileHeight")]
        public long FileHeight;

        [JsonProperty("FileCreatedDate")]
        public DateTimeOffset created_at;

        [JsonProperty("FileExcludeSearch")]
        public bool FileExcludeSearch;
    }

    public enum FileMime
    { ImageJpeg, ImagePNG, ImageGIF, ImageBMP};

    public partial class IamageModel
    {
        public static IamageModel FromJson(string json) => JsonConvert.DeserializeObject<IamageModel>(json, Converter.settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings settings = new JsonSerializerSettings { 
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            
            Converters =
            {
                FileMimeConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal}
            }
            
        };
    }

    internal class FileMimeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(FileMime) || t == typeof(FileMime?);
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "image/jpeg":
                    return FileMime.ImageJpeg;
                case "image/png":
                    return FileMime.ImagePNG;
                case "image/gif":
                    return FileMime.ImageGIF;
                case "image/bmp":
                    return FileMime.ImageBMP;
            }
            throw new Exception("Cannot unmarshal type FileMime");
        }
        public override void WriteJson(JsonWriter writer, object notTypedvalue, JsonSerializer serializer)
        {
            if (notTypedvalue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (FileMime)notTypedvalue;
            switch (value)
            {
                case FileMime.ImageJpeg:
                    serializer.Serialize(writer, "image/jpeg");
                    return;
                case FileMime.ImagePNG:
                    serializer.Serialize(writer, "image/png");
                    return;
                case FileMime.ImageGIF:
                    serializer.Serialize(writer, "image/gif");
                    return;
                case FileMime.ImageBMP:
                    serializer.Serialize(writer, "image/bmp");
                    return;
            }
            throw new Exception("Cannot marshal type FileMime");
        }
        public static readonly FileMimeConverter Singleton = new FileMimeConverter();
    }
}
 