using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TgBotRSIS.Controllers
{
    public class BotController
    {
        public BotController()
        {

        }

        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(bot, update.Message);
                return;
            }

            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(bot, update.CallbackQuery);
            }
        }

        public async Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Напишите свое имя и фамилию✍️");
                return;
            }
        }

        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
        }

        public Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Ошибка Telegram API:\n{apiRequestException.ErrorCode}\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }
    }
}
