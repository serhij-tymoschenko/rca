using NetCord.Gateway;
using NetCord.Rest;
using rca.data.entities;
using rca.utils;

namespace rca.bot.modules;

public class MessagesModule(GatewayClient client)
{
    private string BuildTitleSection(MessageType type)
    {
        if (type is MessageType.Rca)
            return "**New rca!**\n";
        
        return "**New contract!**\n";
    }

    private string BuildNftMessageSection(Rca rca)
    {
        return "**NFT info:**" 
               + "\n"
               + $"Price: {rca.Price.ValueToPrice()} • "
               + $"Amount: {rca.Count} • "
               + $"Author: [{rca.AuthorName}]({rca.AuthorUrl})\n";
    }

    private string BuildAvatarMessageSection(Rca rca)
    {
        var avatarMessage = $"**[{rca.Name}]({rca.ShopUrl})**\n";
        rca.Description
            .Split('\n')
            .ToList()
            .ForEach(line => avatarMessage += $"> {line}\n");
        return avatarMessage;
    }

    private string BuildTraitsMessageSection(Rca rca)
    {
        var traitsMessage = "**Avatar traits:**\n";
        traitsMessage += $"[face]({rca.Traits.FaceUrl}) • "
                         + $"[eyes]({rca.Traits.EyesUrl}) • "
                         + $"[tops]({rca.Traits.TopsUrl}) • "
                         + $"[bottoms]({rca.Traits.BottomsUrl}) • "
                         + $"[background]({rca.Traits.BackgroundUrl})"
                         + "\n";

        var nextLineTraits = new List<string>();
        if (rca.Traits.HairUrl != null) nextLineTraits.Add($"[hair]({rca.Traits.HairUrl})");

        if (rca.Traits.HairBackUrl != null) nextLineTraits.Add($"[hair back]({rca.Traits.HairBackUrl})");

        if (rca.Traits.HatsUrl != null) nextLineTraits.Add($"[hats]({rca.Traits.HatsUrl})");

        if (rca.Traits.LeftUrl != null) nextLineTraits.Add($"[left]({rca.Traits.LeftUrl})");

        if (rca.Traits.RightUrl != null) nextLineTraits.Add($"[right]({rca.Traits.RightUrl})");

        if (nextLineTraits.Count > 0)
            traitsMessage += string.Join(" • ", nextLineTraits.ToArray()) + "\n";

        return traitsMessage;
    }

    private string BuildMessage(Rca rca, MessageType type)
    {
        var message = BuildTitleSection(type)
                      + "\n"
                      + BuildNftMessageSection(rca)
                      + "\n"
                      + BuildAvatarMessageSection(rca)
                      + "\n"
                      + BuildTraitsMessageSection(rca)
                      + "\n"
                      + "**Overall look:**";
        return message;
    }

    public async Task SendRcaDetailsAsync(ulong channelId, Rca rca, MessageType type)
    {
        try
        {
            var content = BuildMessage(rca, type);
            var embed = new EmbedProperties
            {
                Image = new EmbedImageProperties(rca.ImageUrl)
            };
            var message = new MessageProperties
            {
                Content = content,
                Embeds = new[] { embed }
            };

            await client.Rest.SendMessageAsync(channelId, message);
        }
        catch (Exception e)
        {
            Bot.Log($"Error sending RCA details: {e.Message}");
        }
    }
}