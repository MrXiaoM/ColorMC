using ColorMC.Core.Objs;
using System.Runtime.InteropServices;

namespace ColorMC.Core.Utils;

public static class SystemInfo
{
    public static OsType Os { get; private set; }
    public static ArchEnum SystemArch { get; private set; }
    public static string SystemName { get; private set; }
    public static string System { get; private set; }
    public static int ProcessorCount { get; private set; }
    public static bool IsArm { get; private set; }

    /// <summary>
    /// 初始化
    /// </summary>
    public static void Init()
    {
        Logs.Info(LanguageHelper.GetName("Core.System.Info1"));
        if (Environment.Is64BitOperatingSystem)
        {
            SystemArch = ArchEnum.x64;
        }
        else
        {
            SystemArch = ArchEnum.x32;
        }

        IsArm = RuntimeInformation.OSArchitecture == Architecture.X64 ||
            RuntimeInformation.OSArchitecture == Architecture.X86;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Os = OsType.Windows;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Os = OsType.Linux;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Os = OsType.MacOS;
        }

        if (OperatingSystem.IsAndroid())
        {
            Os = OsType.Android;
        }

        SystemName = RuntimeInformation.OSDescription;
        ProcessorCount = Environment.ProcessorCount;
        System = $"Os:{Os} Arch:{SystemArch}";

        Logs.Info(System);
        Logs.Info(SystemName);
    }
}
