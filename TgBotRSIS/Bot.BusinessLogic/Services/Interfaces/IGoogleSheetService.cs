
namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface IGoogleSheetService
    {
        public void WriteData(string tgName, string userName, string userGroup, string userDate, string userTime);
        //public void CreateHeader();
        public void UpdateTimeToCheck(string time, string adress);
        public string ReadDay(string adress);
        public List<List<object>> ReadTime(string adress);
    }
}
