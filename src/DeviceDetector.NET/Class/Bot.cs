using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class
{
    public class Bot : BaseCompiledParseLibrary
    {
        [YamlMember(Alias = "category")]
        public string Category { get; set; }
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
        [YamlMember(Alias = "producer")]
        public Producer Producer { get; set; }
    }
}
