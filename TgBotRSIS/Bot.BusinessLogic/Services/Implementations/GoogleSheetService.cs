
using Bot.BusinessLogic.GoogleApi;
using Bot.BusinessLogic.Services.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace Bot.BusinessLogic.Services.Implementations
{
    public class GoogleSheetService : IGoogleSheetService
    {
        private static readonly string SpreadsheetsId = "1-9e5eaCj_aIrVNjfgsbutNWhfJCHTBgSZkdmmIGVkGE";
        private const string sheetSettings = "Настройки";
        private const string sheetData = "Данные";
        static SheetsService service;
        public GoogleSheetService()
        {
            service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GoogleSheetHelper.Credentials,
                ApplicationName = GoogleSheetHelper.ApplicationName,
            });
        }
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
        //static string ReadDayTwo(int n)
        //{
        //    var range = $"{sheetSettings}!H2:I2";
        //    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            return $"{row[n]}";
        //        }
        //    }
        //    return null;
        //}
        ////static List<object> ReadTime()
        ////{
        ////    var range = $"{sheetSettings}!I14:J21";
        ////    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        ////    var response = request.Execute();
        ////    var values = response.Values;
        ////    if (values != null && values.Count > 0)
        ////    {
        ////        return (List<object>)values;
        ////    }
        ////    return null;
        ////}
        //static string ReadCountUserInGroup()
        //{
        //    var range = $"{sheetSettings}!C3";
        //    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            return $"{row[0]},{row[1]}";
        //        }
        //    }
        //    return null;

        //}
        //static string GroupComposition()
        //{
        //    var range = $"{sheetSettings}!E3:F16";
        //    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            return $"{row[0]},{row[1]}";
        //        }
        //    }
        //    return null;
        //}
        //static void CreateHeader()
        //{
        //    var range = $"{sheetData}!A:E";
        //    var valueRange = new ValueRange();

        //    var objectList = new List<object>() { "TgName", "Name", "Group", "Date", "Time" };
        //    valueRange.Values = new List<IList<object>> { objectList };

        //    var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetsId, range);
        //    appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource
        //        .AppendRequest.ValueInputOptionEnum.USERENTERED;
        //    var appendResponse = appendRequest.Execute();
        //}
    }
}
