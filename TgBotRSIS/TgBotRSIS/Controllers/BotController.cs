using Bot.BusinessLogic.Services.Interfaces;
using Google.Apis.Util;
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
        bool editTime = false;


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
            if (message.Text == "Да")
            {
                editTime = true;
                return;
            }
            if(message.Text == "Изменить время")
            {
                editTime = true;
                return;
            }
            if (isChoise || editTime)
            {
                editTime = false;
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
                string day = "";
                List<InlineKeyboardButton[]> listButton = new List<InlineKeyboardButton[]>();
                foreach (var row in _googleSheet.ReadTime("A9:I10").ToList())
                {
                    for (int i = 0; i < row.Count(); i++)
                    {
                        if (row != null)
                        {
                            if (row[i].ToString()[0] > 57)
                            {
                                day = row[i].ToString();
                                listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[i]}",
                            callbackData: "dayCall"  )});
                            }
                            else
                            {
                                listButton.AsReadOnly();
                                listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[i]}",
                            callbackData: $"Cal*{day}*" + row[i].ToString()) });
                            }
                        }
                    }
                }
                InlineKeyboardMarkup keyboardMarkup = new(listButton.ToArray());
                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время:\n(Минск, МСК)⏳", replyMarkup: keyboardMarkup);
            }
            if (message.Text == "На проверку")
            {
                string day = "";
                List<InlineKeyboardButton[]> listButton = new List<InlineKeyboardButton[]>();
                foreach (var row in _googleSheet.ReadTime("A2:I3").ToList())
                {
                    for (int i = 0; i < row.Count(); i++)
                    {
                        if (row != null)
                        {
                            if (row[i].ToString()[0] > 57)
                            {
                                day = row[i].ToString();
                                listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[i]}",
                            callbackData: "dayCheck"  )});
                            }
                            else
                            {
                                listButton.Add(new[] { InlineKeyboardButton.WithCallbackData(text: $"{row[i]}",
                            callbackData: $"Che*{day}*" + row[i].ToString()) });
                            }
                        }
                    }
                }
                InlineKeyboardMarkup keyboard = new(listButton.ToArray());

                await bot.SendTextMessageAsync(message.Chat.Id, text: "Выберите дату и время: \n(Минск, МСК)⏳", replyMarkup: keyboard);
            }
            if (message.Text == "Нет")
            {
                if (userDate == _googleSheet.ReadDay("A2"))
                {
                    _googleSheet.UpdateTimeToCheck(userTime, "A2:I2");
                }
                else if (userDate == _googleSheet.ReadDay("A3"))
                {
                    _googleSheet.UpdateTimeToCheck(userTime, "A3:I3");
                }
                else if (userDate == _googleSheet.ReadDay("A9"))
                {
                    _googleSheet.UpdateTimeToCheck(userTime, "A9:I9");
                }
                else if (userDate == _googleSheet.ReadDay("A10"))
                {
                    _googleSheet.UpdateTimeToCheck(userTime, "A10:I10");
                }
                _googleSheet.WriteData(tgName, userName, userGroup, userDate, userTime);
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
        }
        async Task HandleCallbackQuery(ITelegramBotClient bot, CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data.StartsWith("Cal"))
            {
                string[] day = callbackQuery.Data.Split('*');
                userDate = day[1];
                userTime = day[2];
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {userDate} {userTime} (Минск, МСК)");
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
            if (callbackQuery.Data.StartsWith("Che"))
            {
                string[] day = callbackQuery.Data.Split('*');
                userDate = day[1];
                userTime = day[2];
                await bot.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: $"Вы записанны на {userDate} {userTime} (Минск, МСК)");
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
