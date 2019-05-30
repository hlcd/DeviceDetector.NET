using System.Collections.Generic;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Device;

namespace DeviceDetectorNET.Parser.Device
{
    public class CameraParser : DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>
    {
        public CameraParser()
        {
            FixtureFile = "regexes/device/cameras.yml";
            ParserName = "camera";
            regexList = GetRegexes();
        }
    }
}