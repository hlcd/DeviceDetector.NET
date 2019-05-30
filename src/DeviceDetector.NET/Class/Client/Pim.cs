using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class Pim : BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
    }
}