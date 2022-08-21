using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;


string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
string ApplicationName = "Google Sheets API .NET Quickstart";

try
{
    UserCredential credential;
    // Load client secrets.
    using (var stream =
           new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
    {
        /* The file token.json stores the user's access and refresh tokens, and is created
         automatically when the authorization flow completes for the first time. */
        string credPath = "token.json";
        credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            Scopes,
            "user",
            CancellationToken.None,
            new FileDataStore(credPath, true)).Result;
        Console.WriteLine("Credential file saved to: " + credPath);
    }

    // Create Google Sheets API service.
    var service = new SheetsService(new BaseClientService.Initializer
    {
        HttpClientInitializer = credential,
        ApplicationName = ApplicationName
    });

    // Define request parameters.
    //String spreadsheetId = "1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms";
    String spreadsheetId = "16-vif6eRhHd-ezwTpxZq1_NFr-aShT4zb9VMemnzSYk";
    String range = "Лист1!D7:E7";
    //String range = "Class Data!A2:E";
    SpreadsheetsResource.ValuesResource.GetRequest request =
        service.Spreadsheets.Values.Get(spreadsheetId, range);

    // Prints the names and majors of students in a sample spreadsheet:
    // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
    ValueRange response = request.Execute();
    IList<IList<Object>> dateofChoise = response.Values;
    if (dateofChoise == null || dateofChoise.Count == 0)
    {
        Console.WriteLine("No data found.");
        return;
    }
    foreach (var row in dateofChoise)
    {
        // Print columns A and E, which correspond to indices 0 and 4.
        Console.WriteLine("{0}, {1}", row[0], row[1]);
    }
}
catch (FileNotFoundException e)
{
    Console.WriteLine(e.Message);
}

var botClient = new TelegramBotClient("5721181824:AAE_ZzRam-Ik3b6StGQkqHQc4wXS7tiSVWY");

using var cts = new CancellationTokenSource();

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
var receiverOptions = new ReceiverOptions
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types
};
botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    Console.WriteLine($"Received a '{messageText}' message in chat {chatId} and {update.Message}.");

    // Echo received message text
    Message sentMessage = await botClient.SendTextMessageAsync(
        chatId: chatId,
        text: "You said:\n" + messageText,
        cancellationToken: cancellationToken);
}

Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
{
    var ErrorMessage = exception switch
    {
        ApiRequestException apiRequestException
            => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
        _ => exception.ToString()
    };

    Console.WriteLine(ErrorMessage);
    return Task.CompletedTask;
}



