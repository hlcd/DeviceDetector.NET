using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class.Device
{
    public class Model: BaseCompiledParseLibrary, IDeviceParseLibrary
    {
        [YamlMember(Alias = "model")]
        public string Name { get; set; }
        [YamlMember(Alias = "device")] //mobile
        public string Device { get; set; }
        [YamlMember(Alias = "brand")] //mobile
        public string Brand { get; set; }
    }
}