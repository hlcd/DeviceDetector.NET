using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Class;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Yaml;

namespace DeviceDetectorNET.Parser
{
    public abstract class ParserAbstract<T, TResult>: IParserAbstract
        where T : class, IEnumerable
//, IParseLibrary
        where TResult : class, IMatchResult, new()
    {
        /// <summary>
        /// Holds the path to the yml file containing regexes
        /// </summary>
        public string FixtureFile { get; protected set; }

        /// <summary>
        /// Holds the internal name of the parser
        /// Used for caching
        /// </summary>
        public string ParserName { get; protected set; }

        /// <summary>
        /// Holds the user agent the should be parsed
        /// </summary>
        public string UserAgent { get; private set; }

        /// <summary>
        ///
        /// </summary>
        protected string[] globalMethods;

        /// <summary>
        /// Holds an array with regexes to parse, if already loaded
        /// </summary>
        protected T regexList;

        /// <summary>
        /// Indicates how deep versioning will be detected
        /// if $maxMinorParts is 0 only the major version will be returned
        /// </summary>
        protected static int maxMinorParts = -1;

        /// <summary>
        /// Versioning constant used to set max versioning to major version only
        /// Version examples are: 3, 5, 6, 200, 123, ...
        /// </summary>
        const int VERSION_TRUNCATION_MAJOR = 0;

        /// <summary>
        /// Versioning constant used to set max versioning to minor version
        /// Version examples are: 3.4, 5.6, 6.234, 0.200, 1.23, ...
        /// </summary>
        const int VERSION_TRUNCATION_MINOR = 1;

        /// <summary>
        /// Versioning constant used to set max versioning to path level
        /// Version examples are: 3.4.0, 5.6.344, 6.234.2, 0.200.3, 1.2.3, ...
        /// </summary>
        const int VERSION_TRUNCATION_PATCH = 2;

        /// <summary>
        /// Versioning constant used to set versioning to build number
        /// Version examples are: 3.4.0.12, 5.6.334.0, 6.234.2.3, 0.200.3.1, 1.2.3.0, ...
        /// </summary>
        const int VERSION_TRUNCATION_BUILD = 3;

        /// <summary>
        /// Versioning constant used to set versioning to unlimited (no truncation)
        /// </summary>
        public const int VERSION_TRUNCATION_NONE = -1;

        protected ICache Cache;

        protected IParser<T> YamlParser;

        public abstract ParseResult<TResult> Parse();

        protected ParserAbstract()
        {
            //regexList = new IEnumerable<T>();
        }

        protected ParserAbstract(string ua = "")
        {
            if (string.IsNullOrEmpty(ua)) throw new ArgumentNullException(nameof(ua));
            UserAgent = ua;
            //regexList = new List<T>();
        }

        /// <summary>
        /// Set how DeviceDetector should return versions
        /// </summary>
        /// <param name="type">Any of the VERSION_TRUNCATION_* constants</param>
        public static void SetVersionTruncation(int type)
        {
            var versions = new List<int> {
                VERSION_TRUNCATION_BUILD,
                VERSION_TRUNCATION_NONE,
                VERSION_TRUNCATION_MAJOR,
                VERSION_TRUNCATION_MINOR,
                VERSION_TRUNCATION_PATCH
            };
            if (versions.Contains(type))
            {
                maxMinorParts = type;
            }
        }

        /// <summary>
        /// Sets the user agent to parse
        /// </summary>
        /// <param name="ua"></param>
        public virtual void SetUserAgent(string ua)
        {
            if (string.IsNullOrEmpty(ua)) throw new ArgumentNullException(nameof(ua));
            UserAgent = ua;
        }

        /// <summary>
        /// Returns the internal name of the parser
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return ParserName;
        }

        /// <summary>
        /// Returns the result of the parsed yml file defined in $fixtureFile
        /// </summary>
        /// <returns></returns>
        protected T GetRegexes()
        {
            if (regexList.Any())
            {
                return regexList;
            }

            var cacheKey = "DeviceDetector-" + DeviceDetector.VERSION + "regexes-" + GetName();
            cacheKey = Regex.Replace(cacheKey, "/([^a-z0-9_-]+)/i", "");
            var regexListCache = GetCache().Fetch(cacheKey);
            if (regexListCache != null)
            {
                regexList = (T)regexListCache;
            }
            if (regexList.Any())
            {
                return regexList;
            }

            regexList = GetYamlParser().ParseFile(
                GetRegexesDirectory() + FixtureFile
            );
            GetCache().Save(cacheKey, regexList);
            return regexList;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        protected string GetRegexesDirectory()
        {
            return DeviceDetectorSettings.RegexesDirectory;
        }

        /// <summary>
        /// Matches the useragent against the given regex
        /// </summary>
        /// <param name="regex"></param>
        /// <returns></returns>
        protected bool IsMatchUserAgent(Regex regex)
        {
            // only match if useragent begins with given regex or there is no letter before it
            return regex.IsMatch(UserAgent);
        }

        protected string[] MatchUserAgent(Regex regex)
        {
            // only match if useragent begins with given regex or there is no letter before it
            var match = regex.Matches(UserAgent);
            return match.Cast<Match>().SelectMany(m => m.Groups.Cast<Group>().Select(g=>g.Value)).ToArray();
        }



        protected string BuildByMatch(string item, string[] matches)
        {
            for (var nb = 1; nb <= Math.Min(matches.Length-1, 3); nb++)
            {
                if (!item.Contains("$" + nb))
                {
                    continue;
                }

                var replace = matches[nb] ?? "";
                item = item.Replace("$"+nb, replace).Trim();
            }
            return item;
        }

        /// <summary>
        /// Builds the version with the given $versionString and $matches
        /// Example:
        /// $versionString = 'v$2'
        /// $matches = array('version_1_0_1', '1_0_1')
        /// return value would be v1.0.1
        /// </summary>
        /// <param name="versionString"></param>
        /// <param name="matches"></param>
        protected string BuildVersion(string versionString, string[] matches)
        {
            versionString = BuildByMatch(versionString ?? "", matches);
            versionString = versionString.Replace("_", ".").TrimEnd('.');

            var versionParts = versionString.Split('.');

            if (-1 == maxMinorParts || versionParts.Length - 1 <= maxMinorParts) return versionString;
            var newVersionParts = new string[1 + maxMinorParts];
            Array.Copy(versionParts, 0, newVersionParts, 0, newVersionParts.Length);
            versionString = string.Join(".", newVersionParts);
            return versionString;
        }

        /// <summary>
        /// Sets the Cache class
        /// </summary>
        /// <param name="cacheProvider"></param>
        public void SetCache(ICache cacheProvider)
        {
            Cache = cacheProvider;
        }

        /// <summary>
        /// Returns Cache object
        /// </summary>
        /// <returns></returns>
        public ICache GetCache()
        {
            if (Cache != null)
            {
                return Cache;
            }
            Cache = new DictionaryCache();
            return Cache;
        }

        public void SetYamlParser(IParser<T> yaml)
        {
            if (!(yaml is YamlParser<T>)) throw new Exception("Yaml Parser not supported");
            YamlParser = yaml;
        }

        public IParser<T> GetYamlParser()
        {
            return YamlParser ?? new YamlParser<T>();
        }

    }
}