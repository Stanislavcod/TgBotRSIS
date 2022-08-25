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
        bool isNameReg = false;

        string userName;
        string userGroup;
        string userDate;
        string userTime;

        static string ReadDay(int n)
        {
            var range = $"{sheetSettings}!I13:J13";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    return $"{row[n]}";
                }
            }
            return null;
        }
        //static List<object> ReadTime()
        //{
        //    var range = $"{sheetSettings}!I14:J21";
        //    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        return (List<object>)values;
        //    }
        //    return null;
        //}
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
            if (message.Text == "/start")
            {
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Напишите свое имя и фамилию✍️");
                isNameReg = true;
            }
            if (isNameReg && message.Text != "/start")
            {
                isNameReg = false;
                userName = message.Text;
            }
            //Fix
            if (userName != null && userGroup == null)
            {
                int group = 1;
                await bot.SendTextMessageAsync(message.Chat.Id, text: $"Ваша группа №{group}");
                userGroup = group.ToString();
                isGroupReg = true;
            }
            //Fix
            if (isGroupReg)
            {
                isGroupReg = false;
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время⏳");
                List<InlineKeyboardButton[]> listButton = new List<InlineKeyboardButton[]>();
                var range = $"{sheetSettings}!I14:J17";
                var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

                var response = request.Execute();
                var values = response.Values;
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[0]}", callbackData: "timeFirst_" + row[0].ToString()) });
                    }
                }
                InlineKeyboardMarkup keyboard = new(listButton.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: ReadDay(0), replyMarkup: keyboard);
                List<InlineKeyboardButton[]> keyboardSecondDay = new List<InlineKeyboardButton[]>();
                if (values != null && values.Count > 0)
                {
                    foreach (var row in values)
                    {
                        keyboardSecondDay.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[1]}", callbackData: "timeSecond_" + row[1].ToString()) });
                    }
                }
                InlineKeyboardMarkup inlineKeyboard = new(keyboardSecondDay.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: ReadDay(1), replyMarkup: inlineKeyboard);
            }
            if (message.Text == "Нет")
            {
                var range = $"{sheetData}!A:D";
                var valueRange = new ValueRange();

                var objectList = new List<object>() { userName, userGroup, userDate, userTime };
                valueRange.Values = new List<IList<object>> { objectList };

                var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetsId, range);
                appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
                var appendResponse = appendRequest.Execute();

                await bot.SendTextMessageAsync(message.Chat.Id, text: "Спасибо! Увидимся на встрече😉");
                return;
            }
            if (message.Text == "Да")
            {
                isGroupReg = true;
                return;
            }
        }
        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
            //Fix
            if (callbackQuery.Data.StartsWith("timeFirst_"))
            {
                userDate = ReadDay(0);
                userTime = callbackQuery.Data.Substring(10);
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {ReadDay(0)} {userTime}");
                ReplyKeyboardMarkup keyboard = new(new[]
                            {
                                new KeyboardButton[] { "Да", "Нет" },
                            })
                {
                    ResizeKeyboard = true
                };
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Хотите изменить время?", replyMarkup: keyboard);
                return;
            }
            if (callbackQuery.Data.StartsWith("timeSecond_"))
            {
                userDate = ReadDay(1);
                userTime = callbackQuery.Data.Substring(11);
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {ReadDay(1)} {userTime}");
                ReplyKeyboardMarkup keyboard = new(new[]
                            {
                                new KeyboardButton[] { "Да", "Нет" },
                            })
                {
                    ResizeKeyboard = true
                };
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Хотите изменить время?", replyMarkup: keyboard);
                return;
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
