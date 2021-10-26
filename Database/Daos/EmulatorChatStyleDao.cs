using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class EmulatorChatStyleDao
    {
        internal static void Query8(IQueryAdapter dbClient)
        {
            dbClient.SetQuery("SELECT id, name, required_right FROM room_chat_styles");
            Table = dbClient.GetTable();
        }
    }
}