using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text.RegularExpressions;
using DeviceDetectorNET.Cache;
using DeviceDetectorNET.Class.Client;
using DeviceDetectorNET.Class.Device;
using DeviceDetectorNET.Parser;
using DeviceDetectorNET.Parser.Client;
using DeviceDetectorNET.Parser.Device;
using DeviceDetectorNET.Results;
using DeviceDetectorNET.Results.Client;
using DeviceDetectorNET.Results.Device;
using YamlDotNet.Core;

namespace DeviceDetectorNET
{
    /// <summary>
    /// Provides user agent detection methods in a form o static extension
    /// method
    /// </summary>
    public static class UserAgentExtensions
    {
        /// <summary>
        /// Operating system families that are known as desktop only
        /// </summary>
        private static readonly string[] desktopOsArray =
        {
            "AmigaOS",
            "IBM",
            "GNU/Linux",
            "Mac",
            "Unix",
            "Windows",
            "BeOS",
            "Chrome OS"
        };

        /// <summary>
        /// Constant used as value for unknown browser / os
        /// </summary>
        public const string UNKNOWN = "UNK";
        /// <summary>
        /// Holds the cache class used for caching the parsed yml-Files
        /// </summary>
        private static readonly ICache cache;
        private static readonly IParser yamlParser;
        private static readonly List<IClientParserAbstract> clientParsers = new List<IClientParserAbstract>();
        private static readonly List<DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>> deviceParsers = 
            new List<DeviceParserAbstract<IDictionary<string, DeviceModel>, DeviceMatchResult>>();
        private static readonly OperatingSystemParser osParser = new OperatingSystemParser();
        
        static UserAgentExtensions()
        {
            AddStandardDevicesParser();
            AddStandardClientsParser();
        }


        private static void AddStandardClientsParser()
        {
            clientParsers.Add(ClientType.Browser.Client);
            clientParsers.Add(ClientType.MobileApp.Client);
            clientParsers.Add(ClientType.FeedReader.Client);
            clientParsers.Add(ClientType.MediaPlayer.Client);
            clientParsers.Add(ClientType.PIM.Client);
            clientParsers.Add(ClientType.Library.Client);
        }

        private static void AddStandardDevicesParser()
        {
            deviceParsers.Add(new MobileParser());
            deviceParsers.Add(new HbbTvParser());
            deviceParsers.Add(new ConsoleParser());
            deviceParsers.Add(new CarBrowserParser());
            deviceParsers.Add(new CameraParser());
            deviceParsers.Add(new PortableMediaPlayerParser());
        }

        private static readonly List<int> MobileDeviceTypes = new List<int>
        {
            DeviceType.DEVICE_TYPE_FEATURE_PHONE,
            DeviceType.DEVICE_TYPE_SMARTPHONE,
            DeviceType.DEVICE_TYPE_TABLET,
            DeviceType.DEVICE_TYPE_PHABLET,
            DeviceType.DEVICE_TYPE_CAMERA,
            DeviceType.DEVICE_TYPE_PORTABLE_MEDIA_PAYER,
        };

        private static readonly List<int> NonMobileDeviceTypes = new List<int>
        {
            DeviceType.DEVICE_TYPE_TV,
            DeviceType.DEVICE_TYPE_SMART_DISPLAY,
            DeviceType.DEVICE_TYPE_CONSOLE,
        };

        public static bool IsTablet(this string userAgent)
        {
            ParseResult<DeviceMatchResult> devResult = DetectDevice(userAgent);
            if (devResult.Success && devResult.Match.Type.HasValue)
            {
                return devResult.Match.Type.Value == DeviceType.DEVICE_TYPE_TABLET;
            }

            return false;
        }

        /// <summary>
        /// Determines if <paramref name="userAgent"/> represents a mobile browser.
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static bool IsMobile(this string userAgent)
        {
            ParseResult<OsMatchResult> os = DetectOs(userAgent);
            var osShort = os.Success ? os.Match.ShortName : "";
            if (string.IsNullOrEmpty(osShort) || UNKNOWN == osShort)
            {
                return false;
            }

            OperatingSystemParser.GetOsFamily(osShort, out string decodedFamily);
            bool isDesktop = Array.IndexOf(desktopOsArray, decodedFamily) > -1;
            if (isDesktop)
            {
                return false;
            }

            ParseResult<DeviceMatchResult> devResult = DetectDevice(userAgent);
            if (devResult.Success && devResult.Match.Type.HasValue)
            {
                if (MobileDeviceTypes.Contains(devResult.Match.Type.Value))
                {
                    return true;
                }
                if (NonMobileDeviceTypes.Contains(devResult.Match.Type.Value))
                {
                    return false;
                }
            }

            // Check for browsers available for mobile devices only
            if (IsMobileBrowser(userAgent))
            {
                return true;
            }

            //true? unknown?
            return false;
        }

        private static bool IsMobileBrowser(string userAgent)
        {
            ParseResult<ClientMatchResult> client = DetectClient(userAgent);
            if (!client.Success)
            {
                return false;
            }
            var match = client.Match;
            return match.Type == ClientType.Browser.Name &&
                   BrowserParser.IsMobileOnlyBrowser(((BrowserMatchResult)match).ShortName);
        }

        private static ParseResult<DeviceMatchResult> DetectDevice(string userAgent)
        {
            foreach (var devParser in deviceParsers)
            {
                devParser.SetUserAgent(userAgent);
                ParseResult<DeviceMatchResult> devResult = devParser.Parse(true);
                if (devResult.Success && devResult.Match.Type.HasValue)
                {
                    return devResult;
                }
            }

            return new ParseResult<DeviceMatchResult>();
        }

        private static ParseResult<ClientMatchResult> DetectClient(string userAgent)
        {
            foreach (var clientParser in clientParsers)
            {
                clientParser.SetCache(cache);
                clientParser.SetUserAgent(userAgent);
                if ((((dynamic)clientParser).Parse() is ParseResult<ClientMatchResult> result) && result.Success)
                {
                    return result;
                }
            }

            return new ParseResult<ClientMatchResult>();
        }

        private static ParseResult<OsMatchResult> DetectOs(string userAgent)
        {
            osParser.SetUserAgent(userAgent);
            return osParser.Parse(simple: true);
        }
    }
}
