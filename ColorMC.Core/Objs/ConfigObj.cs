﻿using ColorMC.Core.Http;

namespace ColorMC.Core.Objs;

public record JvmConfigObj
{
    public string Name { get; set; }
    public string Local { get; set; }
}

public record HttpObj
{
    public SourceLocal Source { get; set; }
    public int DownloadThread { get; set; }
    public bool Proxy { get; set; }
    public string ProxyIP { get; set; }
    public ushort ProxyPort { get; set; }
    public string ProxyUser { get; set; }
    public string ProxyPassword { get; set; }
}

public record WindowSettingObj
{
    /// <summary>
    /// 是否全屏
    /// </summary>
    public bool FullScreen { get; set; }

    /// <summary>
    /// 高px
    /// </summary>
    public ushort Height { get; set; }

    /// <summary>
    /// 宽px
    /// </summary>
    public ushort Width { get; set; }
}

public record JvmArgObj
{
    /// <summary>
    /// GC类型
    /// </summary>
    public enum GCType
    {
        /// <summary>
        /// 默认G1垃圾回收器 兼容JAVA9
        /// </summary>
        G1GC = 0,

        /// <summary>
        /// 串行垃圾回收器
        /// </summary>
        SerialGC = 1,

        /// <summary>
        /// 并行垃圾回收器
        /// </summary>
        ParallelGC = 2,

        /// <summary>
        /// 并发标记扫描垃圾回收器
        /// </summary>
        CMSGC = 3,

        /// <summary>
        /// 设置为空（手动设置）
        /// </summary>
        User = 4
    }
    public string? AdvencedJvmArguments { get; set; }
    public string? GCArgument { get; set; }
    public GCType? GC { get; set; }
    public string? JavaAgent { get; set; }
    public uint? MaxMemory { get; set; }
    public uint? MinMemory { get; set; }
}

public record ConfigObj
{
    public string Version { get; set; }
    public string MCPath { get; set; }

    public List<JvmConfigObj> JavaList { get; set; }

    public HttpObj Http { get; set; }
    public JvmArgObj DefaultJvmArg { get; set; }
    public WindowSettingObj Window { get; set; }
}
