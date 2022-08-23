using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotRSIS.Controllers;

var botController = new BotController();

var botClient = new TelegramBotClient("5721181824:AAE_ZzRam-Ik3b6StGQkqHQc4wXS7tiSVWY");

using var cts = new CancellationTokenSource();

var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>()
};
botClient.StartReceiving(
    updateHandler: botController.HandleUpdateAsync,
    pollingErrorHandler: botController.HandleErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Bot start @{me.Username}");
Console.ReadLine();

cts.Cancel();



