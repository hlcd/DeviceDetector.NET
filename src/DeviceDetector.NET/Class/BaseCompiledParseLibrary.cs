using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace DeviceDetectorNET.Class
{
    public abstract class BaseCompiledParseLibrary : IParseLibrary
    {
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "regex")]
        public string Regex { get; set; }


        private Regex cachedRegex;
        public Regex CompiledRegex
        {
            get
            {
                if (cachedRegex == null)
                {
                    cachedRegex = new Regex(Regex.FixUserAgentRegEx(), RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                return cachedRegex;
            }
        }
    }
}
