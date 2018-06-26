using Newtonsoft.Json;

namespace Windows.DeviceClientProgram
{
    public class AzureIoTCommand
    {
        [JsonProperty("methodName")]
        public string MethodName { get; set; }

        [JsonProperty("responseTimeoutInSeconds")]
        public long ResponseTimeoutInSeconds { get; set; }

        [JsonProperty("playLoad")]
        public PlayLoad PlayLoad { get; set; }
    }


    public class PlayLoad
    {
        [JsonProperty("command")]
        public IoTCommandTypeEnum Command { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }
}
