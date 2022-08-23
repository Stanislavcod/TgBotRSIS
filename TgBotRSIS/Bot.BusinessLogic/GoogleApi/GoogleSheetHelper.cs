using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace Bot.BusinessLogic.GoogleApi
{
    public static class GoogleSheetHelper
    {
        public static UserCredential Credentials { get; set; }
        public static string ApplicationName = "Rocket time";
        static GoogleSheetHelper()
        {
            string[] Scopes = {
                DriveService.Scope.Drive
            };
            using (var stream =
                       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                Credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
        }
    }
}
//string[] Scopes = {
//    SheetsService.Scope.SpreadsheetsReadonly,
//    DriveService.Scope.Drive };
//string ApplicationName = "Google Sheets API .NET Quickstart";

//UserCredential credential;
//using (var stream =
//       new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
//{
//    string credPath = "token.json";
//    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//        GoogleClientSecrets.FromStream(stream).Secrets,
//        Scopes,
//        "user",
//        CancellationToken.None,
//        new FileDataStore(credPath, true)).Result;
//    Console.WriteLine("Credential file saved to: " + credPath);
//}
//var sheetService = new SheetsService(new BaseClientService.Initializer
//{
//    HttpClientInitializer = credential,
//    ApplicationName = ApplicationName
//});

////var driveService = new DriveService(new BaseClientService.Initializer
////{
////    HttpClientInitializer = credential,
////    ApplicationName = ApplicationName
////});

//String spreadsheetId = "1-9e5eaCj_aIrVNjfgsbutNWhfJCHTBgSZkdmmIGVkGE";
//String range = "Время Для Созвона!E3:F10";
//SpreadsheetsResource.ValuesResource.GetRequest request =
//    sheetService.Spreadsheets.Values.Get(spreadsheetId, range);
////var requestDrive = driveService.Files.List();

//ValueRange response = request.Execute();

//IList<IList<Object>> values = response.Values;
//if (values == null || values.Count == 0)
//{
//    Console.WriteLine("No data found.");
//    return;
//}
//foreach (var row in values)
//{
//    Console.WriteLine("{0}", row[0]);
//}