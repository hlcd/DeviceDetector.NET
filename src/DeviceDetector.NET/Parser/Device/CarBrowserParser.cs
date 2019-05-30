using System.Collections.Generic;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Device;

namespace DeviceDetectorNET.Parser.Device
{
    public class CarBrowserParser : DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>
    {
        public CarBrowserParser()
        {
            FixtureFile = "regexes/device/car_browsers.yml";
            ParserName = "car browser";
            regexList = GetRegexes();
        }
    }
}
