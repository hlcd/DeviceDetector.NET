using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Client
{
    public class MediaPlayer: BaseCompiledParseLibrary, IClientParseLibrary
    {
        [YamlMember(Alias = "version")]
        public string Version { get; set; }
    }
}