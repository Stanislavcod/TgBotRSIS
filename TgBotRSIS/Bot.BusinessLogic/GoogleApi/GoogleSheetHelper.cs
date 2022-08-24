using Google.Apis.Auth.OAuth2;
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
                SheetsService.Scope.Drive,
                SheetsService.Scope.SpreadsheetsReadonly
            };
            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                Credentials = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "Clients",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }
        }
    }
}