using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class Browser : BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
        [YamlMember(Alias = "engine")]
        public Engine Engine { get; set; }
    }
}
