using System.Threading.Channels;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TL;
using WTelegram;

namespace ClearMembersBot.Service
{
    public class TelegramBotService : ITelegramBotService
    {
        private TelegramBotClient _client;
        private static string _tel_code = string.Empty;

        public TelegramBotService()
        {

        }

        public async Task<bool> DeleteUnnecessaryUsers(Telegram.Bot.Types.Update upd)
        {
            if (upd?.Message?.Chat != null && upd.Message.Chat.Type == Telegram.Bot.Types.Enums.ChatType.Supergroup)
            {
                return true;
            }
            else if (upd?.Type == Telegram.Bot.Types.Enums.UpdateType.ChannelPost && upd.ChannelPost != null)
            {
                if (upd.ChannelPost.Text.Contains("clear"))
                {
                    var res = await GetAllUsers(upd.ChannelPost.Chat.Title);
                    return res;
                }

                return false;
            }
            else
            {
                if (upd?.Message?.Chat == null && upd?.CallbackQuery == null) return false;

                if (upd.Message != null && upd.Message.Text != null && upd.Message.Text.Contains("start"))
                    await ExecuteStart(upd);

                if (upd.Message != null && upd.Message.Text != null && upd.Message.Text.Contains("Добавь меня в свою группу"))
                    await ExecuteStart(upd);
            }
                
            return true;
        }
        private async Task<bool> GetAllUsers(string title)
        {
            try
            {
                using (var client = new WTelegram.Client(int.Parse("Your APP - ID"), "Your APP - HASH"))
                {
                    await DoLogin("YOUR TEL.NUMBER", client);
                    var chats = await client.Messages_GetAllChats();
                    var mainchannel = (chats.chats.Where(item => item.Value.Title.Equals(title)).Select(i => i.Value).FirstOrDefault());

                    if (mainchannel is TL.ChatForbidden)
                        return false;
                    if(mainchannel is TL.Chat)
                        return false;
                    TL.Channel channel = (TL.Channel)mainchannel;
  

                    var deleted = new List<TL.User>();
                    var bannedRights = new ChatBannedRights();
                    bannedRights.flags = 0;

                    CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
                    CancellationToken token = cancelTokenSource.Token;
                    //now loop through our tweets
                    for (int offset = 0; ;)
                    {
                        int count = 0;
                        var str = "БCДЕЄЖФГHИІJКЛМНОПQРСТУВWХЦЧШЩЫЮЯЗ";
                        //CЕHИJЛМНОРСТУВWЫ
                        var participants = await client.Channels_GetAllParticipants(channel);

                        for (var i = 0; i < participants.users.Count; i++)
                        {
                            if (participants.participants.Count() <= i)
                                continue;

                            if (participants.participants[i].IsAdmin)
                                continue;
                            if (participants.participants[i] is ChannelParticipantCreator)
                                continue;

                            var participant = (ChannelParticipant)participants.participants[i];
                            var member = (TL.User)participants.users[participant.user_id];

                            if (!member.IsActive)
                            {
                                await client.DeleteChatUser(channel, member);
                                deleted.Add(member);
                            }

                            if(member.LastSeenAgo == TimeSpan.FromDays(150.0))
                            {
                                await client.DeleteChatUser(channel, member);
                                deleted.Add(member);
                            }

                        }

                        offset += participants.participants.Length;

                        if (offset >= participants.count) break;

                        count++;
                    }
                    cancelTokenSource.Dispose();

                }
                
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private async Task DoLogin(string loginInfo, WTelegram.Client client)
        {
            while (client.User == null)
                switch (await client.Login(loginInfo)) // returns which config is needed to continue login
                {
                    case "verification_code": Console.Write("Code: "); loginInfo = Console.ReadLine(); break;
                    default: loginInfo = null; break;
                }
            
        }
        public async Task<TelegramBotClient> GetClient()
        {
            if (_client != null) return _client;

            _client = new TelegramBotClient("6449026519:AAFRc3JvvdNQBkXlI4lcP3JlCqp_mkQdAEk");
            return _client;
        }

        private async Task<bool> ExecuteStart(Telegram.Bot.Types.Update upd)
        {
            await _client.SendTextMessageAsync(upd.Message.Chat.Id, "Добро пожаловать! Я бот Clear Unnecesary Users - Который готов почистить всех не нужных пользователей.\n\n " +
                                                                        "Для того, чтобы я могу почистить добавьте меня в нужный канал и дайте полного админа.\n\n" +
                                                                        "После этого напишите /clearusers.\n\n"+
                                                                        "От телеграмма должен прийти код, который вы мне должны отправить!", replyMarkup: GetStartButton());
            return true;
        }

        private IReplyMarkup GetStartButton() 
        {
            return new Telegram.Bot.Types.ReplyMarkups.ReplyKeyboardMarkup(new List<List<Telegram.Bot.Types.ReplyMarkups.KeyboardButton>>
            {
                new List<Telegram.Bot.Types.ReplyMarkups.KeyboardButton>{  new Telegram.Bot.Types.ReplyMarkups.KeyboardButton("✅ Добавь меня в свою группу ✅") }
            })
            { ResizeKeyboard = true };
        }
    }
}
