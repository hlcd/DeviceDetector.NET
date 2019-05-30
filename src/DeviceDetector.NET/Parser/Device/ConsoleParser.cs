using System.Collections.Generic;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Device;

namespace DeviceDetectorNET.Parser.Device
{
    public class ConsoleParser : DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>
    {
        public ConsoleParser()
        {
            FixtureFile = "regexes/device/consoles.yml";
            ParserName = "consoles";
            regexList = GetRegexes();
        }
    }
}