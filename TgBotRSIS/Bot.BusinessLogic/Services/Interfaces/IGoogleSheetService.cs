
namespace Bot.BusinessLogic.Services.Interfaces
{
    public interface IGoogleSheetService
    {
        public void WriteData(string tgName, string userName, string userGroup, string userDate, string userTime);
        public string ReadDayForCalling(int n);
        public string ReadDayToCheck(int n);
        public List<List<object>> ReadTimeForCalling();
        public List<List<object>> ReadTimeToCheck();
        public string ReadCountUserInGroup();
        public string GroupComposition();
        public void CreateHeader();
        public void UpdateTimeToCheck(string time);
        public void UpdateTimeForCalling(string time);
    }
}
