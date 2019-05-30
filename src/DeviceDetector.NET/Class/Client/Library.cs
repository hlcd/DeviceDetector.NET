using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class Library : BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
        [YamlMember(Alias = "url")]
        public string Url { get; set; }
    }
}