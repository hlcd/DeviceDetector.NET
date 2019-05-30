using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DeviceDetectorNET.Class;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Parser.Device;
using DeviceDetectorNET.Results;

namespace DeviceDetectorNET.Parser
{
    public class VendorFragmentParser : ParserAbstract<Dictionary<string,string[]>, VendorFragmentResult>
    {
        private readonly Dictionary<string, Regex> compiledRegexList = new Dictionary<string, Regex>();
        public VendorFragmentParser()
        {
            FixtureFile = "regexes/vendorfragments.yml";
            ParserName = "vendorfragments";
            regexList = GetRegexes();
        }

        public override ParseResult<VendorFragmentResult> Parse()
        {
            var result = new ParseResult<VendorFragmentResult>();
            foreach (var brands in regexList)
            {
                foreach (var brand in brands.Value)
                {
                    Regex brandRegex = GetCachedRegex(brands.Key, brand);
                    if (IsMatchUserAgent(brandRegex))
                    {
                        result.Add(new VendorFragmentResult
                        {
                            Name = brands.Key,
                            Brand = DeviceParserAbstract<IDictionary<string, DeviceModel>, VendorFragmentResult>.DeviceBrands
                                .FirstOrDefault(d => d.Value.Equals(brands.Key)).Key
                        });
                    }
                }
            }
            return result;
        }

        private Regex GetCachedRegex(string key, string brand)
        {
            string cacheKey = $"{key}_!!_{brand}";
            if (!compiledRegexList.TryGetValue(cacheKey, out Regex result))
            {
                string regexString = (brand + "[^a-z0-9]+").FixUserAgentRegEx();
                result = new Regex(regexString, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                compiledRegexList[cacheKey] = result;
            }

            return result;
        }
    }
}
