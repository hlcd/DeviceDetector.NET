using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeviceDetector = DeviceDetectorNET.DeviceDetector;

namespace DeviceDetectorNET.Console
{
    class Program
    {
        private static readonly DeviceDetectorNET.DeviceDetector detector = new DeviceDetector("");
        static void Main(string[] args)
        {
            detector.DiscardBotInformation();
            detector.SkipBotDetection();

            string[] agents = new string[]
            {
                "Mozilla/5.0 (Linux; Android 8.0.0; SM-G960F Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.84 Mobile Safari/537.36",
                "Mozilla/5.0 (Linux; Android 7.0; SM-G892A Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/60.0.3112.107 Mobile Safari/537.36",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/12.0 Mobile/15E148 Safari/604.1",
                "Mozilla/5.0 (iPhone; CPU iPhone OS 12_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) CriOS/69.0.3497.105 Mobile/15E148 Safari/605.1",
                "Mozilla/5.0 (Linux; Android 7.0; Pixel C Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.98 Safari/537.36",
                "Mozilla/5.0 (Linux; Android 6.0.1; SGP771 Build/32.2.A.0.253; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/52.0.2743.98 Safari/537.36",
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/42.0.2311.135 Safari/537.36 Edge/12.246",
                "Mozilla/5.0 (X11; CrOS x86_64 8172.45.0) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.64 Safari/537.36",
                "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_11_2) AppleWebKit/601.3.9 (KHTML, like Gecko) Version/9.0.2 Safari/601.3.9",
                "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/47.0.2526.111 Safari/537.36",
                "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:15.0) Gecko/20100101 Firefox/15.0.1",
            };

            for (int i = 0; i < 1; i++)
            {
                foreach (var agent in agents)
                {
                    bool isMobOld = CheckUserAgentOld(agent);
                    bool isMob = CheckUserAgent(agent);
                    if (isMob ^ isMobOld)
                    {
                        System.Console.WriteLine(agent);
                    }
                }
            }

            DateTime start = DateTime.Now;
            int mobileCount = 0;
            for (int i = 0; i < 100; i++)
            {
                foreach (var agent in agents)
                {
                    bool isMob = CheckUserAgent(agent);
                    if (isMob)
                    {
                        mobileCount++;
                    }
                }
            }
            DateTime end = DateTime.Now;

            System.Console.WriteLine($"total mobile: {mobileCount} in {(end - start)}");

//            start = DateTime.Now;
//            mobileCount = 0;
//            for (int i = 0; i < 1; i++)
//            {
//                foreach (var agent in agents)
//                {
//                    bool isMob = CheckUserAgentOld(agent);
//                    if (isMob)
//                    {
//                        mobileCount++;
//                    }
//                }
//            }
//            end = DateTime.Now;
//
//            System.Console.WriteLine($"total mobile: {mobileCount} in {(end - start)}");
            System.Console.ReadLine();
        }

        private static bool CheckUserAgent(string agent)
        {
            return agent.IsMobile() || agent.IsTablet();
        }

        private static bool CheckUserAgentOld(string agent)
        {
            detector.SetUserAgent(agent);
            detector.Parse();
            return detector.IsMobile() || detector.IsTablet();
        }
    }
}
