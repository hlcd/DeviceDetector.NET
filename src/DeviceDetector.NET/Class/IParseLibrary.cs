using System.Text.RegularExpressions;

namespace DeviceDetectorNET.Class
{
    public interface IParseLibrary
    {
        string Name { get; set; }
        string Regex { get; set; }
        Regex CompiledRegex { get; }
    }
}
