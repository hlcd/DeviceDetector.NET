using System.Collections.Generic;
using System.Text.RegularExpressions;
using DeviceDetectorNET.Class;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Device;

namespace DeviceDetectorNET.Parser.Device
{
    public class HbbTvParser : DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>
    {
        public HbbTvParser()
        {
            FixtureFile = "regexes/device/televisions.yml";
            ParserName = "tv";
            regexList = GetRegexes();
        }

        public override ParseResult<DeviceMatchResult> Parse()
        {
            var result = new ParseResult<DeviceMatchResult>();

            // only parse user agents containing hbbtv fragment
            if (!IsHbbTv()) return result;

            // always set device type to tv, even if no model/brand could be found
            deviceType = DeviceType.DEVICE_TYPE_TV;

            result = base.Parse();
            if (!result.Success)
            {
                result.Add(new DeviceMatchResult { Brand = "", Name = "", Type = deviceType.Value });
            }
            return result;
        }

        private static readonly Regex HbbTvRegex = 
            new Regex(@"HbbTV/([1-9]{1}(?:\.[0-9]{1}){1,2})".FixUserAgentRegEx(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
        public bool IsHbbTv()
        {
            return IsMatchUserAgent(HbbTvRegex);
        }

        public string[] HbbTv()
        {
            return MatchUserAgent(HbbTvRegex);
        }
    }
}