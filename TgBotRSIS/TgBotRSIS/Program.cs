using AutoMapper;
using Bot.BusinessLogic.Services.Implementations;
using Bot.BusinessLogic.Services.Interfaces;
using Bot.Common.Mapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using TgBotRSIS.Controllers;

var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
IMapper mapper = mappingConfig.CreateMapper();

IHost host = Host.CreateDefaultBuilder()
               .ConfigureServices((context, services) =>
               {
                   services.AddTransient<IGoogleSheetService, GoogleSheetService>();
                   services.AddSingleton(mapper);
               })
               .Build();
GoogleSheetService _googleSheetService = ActivatorUtilities.CreateInstance<GoogleSheetService>(host.Services);

var botController = new BotController(_googleSheetService);

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



