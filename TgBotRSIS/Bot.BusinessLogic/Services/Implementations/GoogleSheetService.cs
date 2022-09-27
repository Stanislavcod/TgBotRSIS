using Bot.BusinessLogic.GoogleApi;
using Bot.BusinessLogic.Services.Interfaces;
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
        public void WriteData(string tgName, string userName, string userGroup, string userDate, string userTime)
        {
            var range = $"{sheetData}!A:E";
            var valueRange = new ValueRange();
            var objectList = new List<object>() { tgName, userName, userGroup, userDate, userTime };
            valueRange.Values = new List<IList<object>> { objectList };
            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetsId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            appendRequest.Execute();
        }
        public string ReadDay(string adress)
        {
            var range = $"{sheetSettings}!{adress}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);
            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    return $"{row[0].ToString()}";
                }
            }
            return "Данные не найдены";
        }
        public List<List<object>> ReadTime(string adress)
        {
            var range = $"{sheetSettings}!{adress}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);
            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                return values.Select(x => x.ToList()).ToList();
            }
            return null;
        }
        public void UpdateTimeToCheck(string time, string adress)
        {
            var range = $"{sheetSettings}!{adress}";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);
            var response = request.Execute();
            var values = response.Values;
            List<object> newValues = new List<object>();
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    for (int i = 0; i < row.Count(); i++)
                    {
                        newValues.Add(row[i]);
                    }
                }
            }
            if (values != null && values.Count > 0)
            {
                foreach (var row in values)
                {
                    foreach (var item in row)
                    {
                        if (item.ToString() == time)
                        {
                            newValues.Remove(item);
                            var valueRange = new ValueRange();
                            valueRange.Values = new List<IList<object>>() { newValues };
                            ClearValuesRequest requestBody = new ClearValuesRequest();
                            SpreadsheetsResource.ValuesResource.ClearRequest requestClear = service.Spreadsheets.Values.Clear(requestBody, SpreadsheetsId, range);
                            requestClear.Execute();
                            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetsId, range);
                            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                            updateRequest.Execute();
                        }
                    }
                }
            }
        }
        //public void CreateHeader()
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
