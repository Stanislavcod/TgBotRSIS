﻿using Bot.BusinessLogic.GoogleApi;
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
            var appendResponse = appendRequest.Execute();
        }
        public string ReadDayForCalling(int n)
        {
            var range = $"{sheetSettings}!R23:S23";
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
            return "";
        }
        public string ReadDayToCheck(int n)
        {
            var range = $"{sheetSettings}!N18:O18";
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
            return "";
        }
        public List<List<object>> ReadTimeForCalling()
        {
            var range = $"{sheetSettings}!A11:I12";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                return values.Select(x => x.ToList()).ToList();
            }
            values.Clear();
            return null;
        }
        public List<List<object>> ReadTimeToCheck()
        {
            var range = $"{sheetSettings}!A2:I3";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            if (values != null && values.Count > 0)
            {
                return values.Select(x => x.ToList()).ToList();
            }
            return null;
        }
        //public void TestRead()
        //{
        //    var range = $"{sheetSettings}!A2:I3";
        //    var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

        //    var response = request.Execute();
        //    var values = response.Values;
        //    if (values != null && values.Count > 0)
        //    {
        //        foreach (var row in values)
        //        {
        //            for (int i = 0; i < row.Count(); i++)
        //            {
        //                Console.WriteLine(row[i]);
        //            }
        //        }
        //    }
        //}

        public void UpdateTimeToCheck(string time)
        {
            var range = $"{sheetSettings}!A2:I3";
            //var rangeTwoRow = $"{sheetSettings}!A3:I3";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            List<object> newValues = new List<object>();
            foreach(var row in values)
            {
                for (int i = 0; i < row.Count(); i++)
                {
                    newValues.Add(row[i]);
                }
            }
            if (values != null && values.Count > 0)
            {
                foreach(var row in values)
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
                            ClearValuesResponse responseClear = requestClear.Execute();

                            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetsId, range);
                            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                            var updateResponse = updateRequest.Execute();
                        }
                    }
                }
            }
        }
        public void UpdateTimeForCalling(string time)
        {
            var range = $"{sheetSettings}!A11:I11";
            var request = service.Spreadsheets.Values.Get(SpreadsheetsId, range);

            var response = request.Execute();
            var values = response.Values;
            List<object> newValues = new List<object>();
            foreach (var row in values)
            {
                for (int i = 0; i < row.Count(); i++)
                {
                    newValues.Add(row[i]);
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
                            ClearValuesResponse responseClear = requestClear.Execute();

                            var updateRequest = service.Spreadsheets.Values.Update(valueRange, SpreadsheetsId, range);
                            updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
                            var updateResponse = updateRequest.Execute();
                        }
                    }
                }
            }
        }
        public string ReadCountUserInGroup()
        {
            var range = $"{sheetSettings}!B2";
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
            return "";

        }
        public string GroupComposition()
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
            return "";
        }
        public void CreateHeader()
        {
            var range = $"{sheetData}!A:E";
            var valueRange = new ValueRange();

            var objectList = new List<object>() { "TgName", "Name", "Group", "Date", "Time" };
            valueRange.Values = new List<IList<object>> { objectList };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, SpreadsheetsId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource
                .AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendResponse = appendRequest.Execute();
        }
    }
}
