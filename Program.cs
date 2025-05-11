﻿using System.Net;
using rca.bot;
using rca.data.db;
using rca.data.remote.api;
using rca.services;

namespace rca;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Bot.CreateInstance();
        Database.CreateInstance();

        var cookieContainer = new CookieContainer();
        var handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true
        };
        var client = new HttpClient(handler);

        var redditApi = new RedditApi(client, cookieContainer);
        var redditGqlApi = new RedditGqlApi(client);
        var polyscanApi = new PolyscanApi(client);

        var rcaService = new RcaService(redditApi, redditGqlApi, cookieContainer);
        var contractService = new ContractService(polyscanApi, redditGqlApi);

        Bot.StartBotAsync();
        rcaService.StartAsync();
        contractService.StartAsync();

        Task.WaitAll(Task.Delay(-1));
    }
}