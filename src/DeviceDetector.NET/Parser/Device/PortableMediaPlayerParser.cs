using System.Collections.Generic;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Device;

namespace DeviceDetectorNET.Parser.Device
{
    public class PortableMediaPlayerParser : DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>
    {
        public PortableMediaPlayerParser()
        {
            FixtureFile = "regexes/device/portable_media_player.yml";
            ParserName = "portablemediaplayer";
            regexList = GetRegexes();
        }
    }
}