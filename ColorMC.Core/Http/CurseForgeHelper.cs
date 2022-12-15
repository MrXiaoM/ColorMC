﻿using ColorMC.Core.Objs.Pack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ColorMC.Core.Http;

public static class CurseForgeHelper
{
    private const string CurseForgeKEY = "$2a$10$6L8AkVsaGMcZR36i8XvCr.O4INa2zvDwMhooYdLZU0bb/E78AsT0m";
    private const string CurseForgeUrl = "https://api.curseforge.com/";

    public static async Task<CurseForgeObj?> GetPackList(string version = "", int index = 0, SortField sort = SortField.Popularity, string filter = "")
    {
        try
        {
            string temp = CurseForgeUrl + "v1/mods/search?gameId=432&classId=4471&"
                + $"gameVersion={version}&index={index}&sortOrder={(int)sort}&searchFilter={filter}";
            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(temp)
            };
            httpRequest.Headers.Add("x-api-key", CurseForgeKEY);
            var data = await BaseClient.Client.SendAsync(httpRequest);
            var data1 = await data.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(data1))
                return null;
            return JsonConvert.DeserializeObject<CurseForgeObj>(data1);
        }
        catch (Exception e)
        {
            Logs.Error("获取CurseForge_Pack信息发生错误", e);
            return null;
        }
    }

    public static async Task<CurseForgeModObj?> GetMod(CurseForgePackObj.Files obj)
    {
        try
        {
            string temp = CurseForgeUrl + $"v1/mods/{obj.projectID}/files/{obj.fileID}";
            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(temp)
            };
            httpRequest.Headers.Add("x-api-key", CurseForgeKEY);
            var data = await BaseClient.Client.SendAsync(httpRequest);
            var data1 = await data.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(data1))
                return null;
            return JsonConvert.DeserializeObject<CurseForgeModObj>(data1);
        }
        catch (Exception e)
        {
            Logs.Error("获取CurseForge_Mod信息发生错误", e);
            return null;
        }
    }

    private record Arg1
    {
        public List<long> fileIds { get; set; } = new();
    }

    private record Arg2
    {
        public List<CurseForgeModObj.Data> data { get; set; } = new();
    }

    public static async Task<List<CurseForgeModObj.Data>?> GetMods(List<CurseForgePackObj.Files> obj)
    {
        try
        {
            Arg1 arg1 = new();
            obj.ForEach(a => arg1.fileIds.Add(a.fileID));
            string temp = CurseForgeUrl + $"v1/mods/files";
            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(temp),
                Content = new StringContent(JsonConvert.SerializeObject(arg1), MediaTypeHeaderValue.Parse("application/json"))
            };
            httpRequest.Headers.Add("x-api-key", CurseForgeKEY);
            var data = await BaseClient.Client.SendAsync(httpRequest);
            var data1 = await data.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(data1))
                return null;
            return JsonConvert.DeserializeObject<Arg2>(data1).data;
        }
        catch (Exception e)
        {
            Logs.Error("获取CurseForge_Mod信息发生错误", e);
            return null;
        }
    }

    public static async Task<JObject?> GetCurseForgeModPage(CurseForgePackObj.Files obj)
    {
        try
        {
            string temp = CurseForgeUrl + $"v1/mods/{obj.projectID}";
            HttpRequestMessage httpRequest = new()
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(temp)
            };
            httpRequest.Headers.Add("x-api-key", CurseForgeKEY);
            var data = await BaseClient.Client.SendAsync(httpRequest);
            var data1 = await data.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(data1))
                return null;
            return JsonConvert.DeserializeObject<JObject>(data1);
        }
        catch (Exception e)
        {
            Logs.Error("获取CurseForge_Mod信息发生错误", e);
            return null;
        }
    }
}
