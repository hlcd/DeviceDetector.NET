using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceDetectorNET.Class
{
    internal static class RegexUtils
    {
        internal static string FixUserAgentRegEx(this string regex)
        {
            return @"(?:^|[^A-Z0-9\-_]|[^A-Z0-9\-]_|sprd-)(?:" + regex.Replace("/", @"\/").Replace("++", "+").Replace(@"\_", "_") + ")";
        }
    }
}
