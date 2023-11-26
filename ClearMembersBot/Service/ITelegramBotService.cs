using Telegram.Bot.Types;
using Telegram.Bot;

namespace ClearMembersBot.Service
{
    public interface ITelegramBotService
    {
        Task<bool> DeleteUnnecessaryUsers(Update upd);

        Task<TelegramBotClient> GetClient();
    }
}
