using NetCord;
using NetCord.Gateway;
using NetCord.Rest;
using NetCord.Services;
using NetCord.Services.ApplicationCommands;
using rca.bot.modules;
using rca.config;
using rca.data.db;
using rca.data.entities;
using MessageType = rca.data.entities.MessageType;

namespace rca.bot;

public static class Bot
{
    private static GatewayClient? _client;
    private static MessagesModule? _messages;

    public static void CreateInstance()
    {
        try
        {
            if (_client == null)
            {
                var config = new GatewayClientConfiguration
                {
                    Intents = GatewayIntents.Guilds
                };

                _client = new GatewayClient(
                    new BotToken(Config.DS_API_KEY),
                    config
                );
                _messages = new MessagesModule(_client);
            }

            _client.ConfigureClientAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating client instance: {e.Message}");
            throw;
        }
    }

    private static async Task ConfigureClientAsync(this GatewayClient client)
    {
        // Slash commands module configuration 
        ApplicationCommandService<ApplicationCommandContext> commandService = new();
        commandService.AddModule<SlashCommandsModule>();
        client.InteractionCreate += async interaction =>
        {
            if (interaction is not ApplicationCommandInteraction applicationCommandInteraction) return;

            var result =
                await commandService.ExecuteAsync(new ApplicationCommandContext(applicationCommandInteraction, client));

            if (result is not IFailResult failResult) return;

            await interaction.SendResponseAsync(InteractionCallback.Message(failResult.Message));
        };

        // Commands creation on Server
        try
        {
            await commandService.CreateCommandsAsync(client.Rest, client.Id);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error creating commands: {e.Message}");
        }
    }

    public static async Task PostRcaAsync(Rca rca,
        MessageType type)
    {
        try
        {
            var guilds = _client.Rest.GetCurrentUserGuildsAsync().GetAsyncEnumerator();
            var channelIds = await Database.GetServerConfigsAsync();

            while (await guilds.MoveNextAsync())
                try
                {
                    var ids = channelIds.First(ids => ids.Server == guilds.Current.Id);
                    var channelId = 0UL;

                    if (type is MessageType.Rca)
                        channelId = ids.Rca;
                    else
                        channelId = ids.Contract;
                    
                    PostRcaToGuildAsync(channelId, rca, type);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error sending message to server: {e.Message}");
                }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error sending message: {e.Message}");
        }
    }

    private static async Task PostRcaToGuildAsync(ulong channelId, Rca rca,
        MessageType type)
    {
        _messages.SendRcaDetailsAsync(channelId, rca, type);
    }

    public static async Task Log(string message)
    {
        _client.Rest.SendMessageAsync(Config.LOG_CHANNEL_ID, message);
    }

    public static async Task StartBotAsync()
    {
        _client.StartAsync();
    }
}