using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class
{
    public class Os : BaseCompiledParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
    }
}
