using Bot.BusinessLogic.GoogleApi;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgBotRSIS.Controllers
{
    public class BotController
    {
        public static readonly string SpreadsheetsId = "1-9e5eaCj_aIrVNjfgsbutNWhfJCHTBgSZkdmmIGVkGE";
        public static readonly string sheetSettings = "Настройки";
        public static readonly string sheetData = "Данные";
        static SheetsService service;

        bool isGroupReg = false;
        bool isTimeReg = false;

        string userName;
        string userGroup;
        string userDate;
        string userTime;

        static string ReadDay(int n)
        {
            var range = $"{sheetSettings}!I13:13";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if(values != null && values.Count > 0)
            {
                foreach(var row in values)
                {
                    return $"{row[n]}"; 
                }
            }
            return null;
        }
        static string ReadTime()
        {
            var range = $"{sheetSettings}!I14:J";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    return $"{row}";
                }
            }
            return null;
        }
        static string ReadCountUserInGroup()
        {
            var range = $"{sheetSettings}!C3";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    return $"{row[0]},{row[1]}";
                }
            }
            return null;

        }
        static string GroupComposition()
        {
            var range = $"{sheetSettings}!E3:F16";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    return $"{row[0]},{row[1]}";
                }
            }
            return null;
        }
        static void CreateHeader()
        {
            var range = $"{sheetData}!A:E";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { "Id", "Name", "Group", "Date", "Time" };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetsId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource
                .AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            GoogleCredential credential;
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleSheetHelper.Credentials,
                ApplicationName = GoogleSheetHelper.ApplicationName,
            });
            //CreateHeader();
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(bot, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(bot, update.CallbackQuery);
            }
        }

        public async Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Text.ToLower() == "/start")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Напишите свое имя и фамилию✍️");
                userName = message.Text;
                return;
            }
            //Fix
            if (userName != null)
            {
                int group = 1;
                await bot.SendTextMessageAsync(message.Chat.Id, text: $"Ваша группа №{group}");
                isGroupReg = true;
            }
            //Fix
            if (isGroupReg)
            {
                isGroupReg = false;
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время⏳");
                List<InlineKeyboardButton[]> listButton = new List<InlineKeyboardButton[]>();
                foreach(var item in ReadTime())
                {
                    if(item != null)
                    {
                        listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: item.ToString(), callbackData : item.ToString()) });
                    }
                }
                //listButton.Add(new[]
                //{
                //    InlineKeyboardButton.WithCallbackData(text: "8:00 - 9:00", "time1Day1"),
                //    InlineKeyboardButton.WithCallbackData(text: "9:00 - 10:00", "time2Day1"),
                //    InlineKeyboardButton.WithCallbackData(text: "10:00 - 11:00", "time3Day1"),
                //    InlineKeyboardButton.WithCallbackData(text: "11:00 - 12:00", "time4Day1")
                //});
                InlineKeyboardMarkup keyboard = new(listButton.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: ReadDay(0), replyMarkup: keyboard);
                InlineKeyboardMarkup keyboardSecondDay = new(new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: "8:00 - 9:00", "time1Day2"),
                    InlineKeyboardButton.WithCallbackData(text: "9:00 - 10:00", "time2Day2"),
                    InlineKeyboardButton.WithCallbackData(text: "10:00 - 11:00", "time3Day2"),
                    InlineKeyboardButton.WithCallbackData(text: "11:00 - 12:00", "time4Day2")
                });
                await bot.SendTextMessageAsync(message.Chat.Id, text: ReadDay(1), replyMarkup: keyboardSecondDay);
                isTimeReg = true;
            }
        }
        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
            //Fix
            if (callbackQuery.Data == "time1Day1" || callbackQuery.Data == "time2Day1" ||
                callbackQuery.Data == "time3Day1" || callbackQuery.Data == "time4Day1" ||
                callbackQuery.Data == "time1Day2" || callbackQuery.Data == "time2Day2" ||
                callbackQuery.Data == "time1Day2" || callbackQuery.Data == "time2Day2")
            {
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на субботу 23 июля 8:00 - 9:00");
                ReplyKeyboardMarkup keyboard = new(new[]
                            {
                                new KeyboardButton[] { "Да", "Нет" },
                            })
                {
                    ResizeKeyboard = true
                };
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Хотите изменить время?", replyMarkup: keyboard);
            }
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
