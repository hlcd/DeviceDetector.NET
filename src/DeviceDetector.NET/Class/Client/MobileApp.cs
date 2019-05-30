using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class MobileApp: BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
    }
}