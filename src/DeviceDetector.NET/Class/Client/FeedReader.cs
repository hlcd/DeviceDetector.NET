using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class FeedReader : BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
        [YamlMember(Alias = "type")]
        public string Type { get; set; }
    }
}
