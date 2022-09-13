using Bot.BusinessLogic.Services.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TgBotRSIS.Controllers
{
    public class BotController
    {
        private readonly IGoogleSheetService _googleSheet;

        bool isNameReg = false;
        bool isChoise = false;

        string userName;
        string userGroup;
        string userDate;
        string userTime;
        string tgName;
        public BotController(IGoogleSheetService googlesheet)
        {
            _googleSheet = googlesheet;
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update?.Message?.Text != null)
            {
                await HandleMessage(bot, update.Message);
                return;
            }

            if (update.Type == UpdateType.CallbackQuery)
            {
                await HandleCallbackQuery(bot, update.CallbackQuery);
                return;
            }
        }

        public async Task HandleMessage(ITelegramBotClient bot, Message message)
        {
            if (message.Text == "/start")
            {
                tgName = message.From.Username;
                if (tgName == null)
                {
                    tgName = message.From.FirstName + " " + message.From.LastName;
                }
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Напишите свое имя и фамилию✍️");
                isNameReg = true;
            }
            if (isNameReg && message.Text != "/start")
            {
                isNameReg = false;
                userName = message.Text;
                isChoise = true;
            }
            if (isChoise)
            {
                isChoise = false;
                ReplyKeyboardMarkup keyboard = new(new[]
                            {
                                new KeyboardButton[] { "На созвон", "На проверку" },
                            })
                {
                    ResizeKeyboard = true
                };
                await bot.SendTextMessageAsync(message.Chat.Id, "Куда вас записать?✍️", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "На созвон")
            {
                List<InlineKeyboardButton[]> keyboardFirstDay = new List<InlineKeyboardButton[]>();
                foreach (var row in _googleSheet.ReadTime("A9:I10").ToList())
                {
                    for (int i = 0; i < row.Count(); i++)
                    {
                        if (row != null)
                        {
                            keyboardFirstDay.Add(new[] {InlineKeyboardButton.WithCallbackData(text: $"{row[i].ToString()}",
                                callbackData: "timeFirst_" + row[i].ToString())});
                        }
                    }
                }
                InlineKeyboardMarkup keyboard = new(keyboardFirstDay.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время:\n(Минск, МСК)⏳", replyMarkup: keyboard);
            }
            if (message.Text == "На проверку")
            {
                List<InlineKeyboardButton[]> listButton = new List<InlineKeyboardButton[]>();
                foreach (var row in _googleSheet.ReadTime("A2:I3").ToList())
                {
                    for (int i = 0; i < row.Count(); i++)
                    {
                        if (row != null)
                        {
                            listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[i]}",
                            callbackData: "timeSecond_" + row[i].ToString()) });
                        }
                    }
                }
                InlineKeyboardMarkup keyboard = new(listButton.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время: \n(Минск, МСК)⏳", replyMarkup: keyboard);
            }
            if (message.Text == "Нет")
            {
                _googleSheet.UpdateTimeToCheck(userTime, "B2:I2");
                _googleSheet.WriteData(tgName, userName,userGroup, userDate,userTime);
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Спасибо! Увидимся на встрече😉");
                ReplyKeyboardMarkup keyboard = new(new[]
                            {
                                new KeyboardButton[] { "Изменить время", "/start" },
                            })
                {
                    ResizeKeyboard = true
                };
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Нажмите \"/start\" для новой записи или нажмите " +
                    "\"Изменить время\" чтобы изменить время записи", replyMarkup: keyboard);
                return;
            }
            if (message.Text == "Да")
            {
                return;
            }
        }
        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("timeFirst_"))
            {
                userDate = _googleSheet.ReadDay("A11");
                userTime = callbackQuery.Data.Substring(10);
                if (_googleSheet.ReadDay("A11") == userDate)
                {
                    await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {_googleSheet.ReadDay("A11")} {userTime} (Минск, МСК)");
                }
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
                userDate = _googleSheet.ReadDay("A11");
                userTime = callbackQuery.Data.Substring(11);
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {_googleSheet.ReadDay("A11")} {userTime} (Минск, МСК)");
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
