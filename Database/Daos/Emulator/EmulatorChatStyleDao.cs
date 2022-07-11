using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Database.Daos
{
    class EmulatorChatStyleDao
    {
        internal static DataTable GetAll(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, name, required_right FROM `emulator_chat_style`");
            return dbClient.GetTable();
        }
    }
}