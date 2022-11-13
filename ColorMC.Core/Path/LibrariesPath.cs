﻿using ColorMC.Core.Http.Downloader;
using ColorMC.Core.Http;
using ColorMC.Core.Objs;
using ColorMC.Core.Objs.Game;
using ColorMC.Core.Utils;
using System.Collections.Generic;
using System;

namespace ColorMC.Core.Path;

public static class LibrariesPath
{
    private const string Name = "libraries";

    public static string BaseDir { get; private set; }

    public static void Init(string dir)
    {
        BaseDir = dir + "/" + Name;

        Directory.CreateDirectory(BaseDir);
    }

    public static List<GameArgObj.Libraries> Check(GameArgObj obj)
    {
        var list = new List<GameArgObj.Libraries>();
        foreach (var item in obj.libraries)
        {
            if (!CheckRule.CheckAllow(item.rules))
                continue;
            string file = $"{BaseDir}/{item.downloads.artifact.path}";
            if (!File.Exists(file))
            {
                list.Add(item);
                continue;
            }
            using var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite,
                FileShare.ReadWrite);
            var sha1 = Sha1.GenSha1(stream);
            if (item.downloads.artifact.sha1 != sha1)
            {
                list.Add(item);
            }
        }

        return list;
    }

    public static List<ForgeInstallObj.Libraries>? CheckForge(GameSettingObj obj)
    {
        var forge = VersionPath.GetForgeObj(obj.Version, obj.LoaderInfo.Version);
        if (forge == null)
            return null;

        var list = new List<ForgeInstallObj.Libraries>();

        foreach (var item in forge.libraries)
        {
            if (item.name.StartsWith("net.minecraftforge:forge:"))
            {
                string file = $"{BaseDir}/net/minecraftforge/forge/" +
                    PathC.MakeForgeName(obj.Version, obj.LoaderInfo.Version);
                if (!File.Exists(file))
                {
                    list.Add(item);
                    continue;
                }
                using var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                var sha1 = Sha1.GenSha1(stream);
                if (item.downloads.artifact.sha1 != sha1)
                {
                    list.Add(item);
                }
            }
            else
            {
                string file = $"{BaseDir}/{item.downloads.artifact.path}";
                if (!File.Exists(file))
                {
                    list.Add(item);
                    continue;
                }
                using var stream = new FileStream(file, FileMode.Open, FileAccess.ReadWrite,
                    FileShare.ReadWrite);
                var sha1 = Sha1.GenSha1(stream);
                if (item.downloads.artifact.sha1 != sha1)
                {
                    list.Add(item);
                }
            }
        }

        return list;
    }

    public static List<FabricLoaderObj.Libraries>? CheckFabric(GameSettingObj obj)
    {
        var fabric = VersionPath.GetFabricObj(obj.Version, obj.LoaderInfo.Version);
        if (fabric == null)
            return null;

        var list = new List<FabricLoaderObj.Libraries>();

        foreach (var item in fabric.libraries)
        {
            var name = PathC.ToName(item.name);
            string file = $"{BaseDir}/{name.Item1}";
            if (!File.Exists(file))
            {
                list.Add(item);
                continue;
            }
        }

        return list;
    }

    public static string ForgeWrapper => BaseDir + "/io/github/zekerzhayard/ForgeWrapper/mmc3/ForgeWrapper-mmc3.jar";
    public static string ForgeWrapper1 => BaseDir + "net/minecraftforge/installertools/1.3.0/installertools-1.3.0.jar";
    public static async Task ReadyForgeWrapper()
    {
        var file = new FileInfo(ForgeWrapper);
        if (!file.Exists)
        {
            if (!Directory.Exists(file.DirectoryName))
            {
                Directory.CreateDirectory(file.DirectoryName!);
            }
            await File.WriteAllBytesAsync(file.FullName, Resource1.ForgeWrapper_mmc3);
        }

        file = new FileInfo(ForgeWrapper1);
        if (!file.Exists)
        {
            if (!Directory.Exists(file.DirectoryName))
            {
                Directory.CreateDirectory(file.DirectoryName!);
            }
            await File.WriteAllBytesAsync(file.FullName, Resource1.installertools_1_3_0);
        }
    }
}
