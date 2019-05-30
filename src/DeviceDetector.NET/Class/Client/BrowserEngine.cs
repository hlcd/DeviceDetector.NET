using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class BrowserEngine : BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlIgnore]//@todo:change logic
        public string Version { get; set; }
    }
}
