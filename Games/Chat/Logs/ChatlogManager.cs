using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Games.Chat.Logs
{
    public class ChatlogManager
    {
        private readonly List<ChatlogEntry> _listOfMessages;

        public int MessageCount => this._listOfMessages.Count;

        public ChatlogManager()
        {
            this._listOfMessages = new List<ChatlogEntry>();
        }

        public void LoadUserChatlogs(IQueryAdapter dbClient, int userId)
        {
            DataTable table = LogChatDao.GetAllByUserId(dbClient, userId);
            if (table == null)
            {
                return;
            }

            foreach (DataRow Row in table.Rows)
            {
                this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString(), (double)Row["timestamp"]);
            }
        }

        public void LoadRoomChatlogs(int roomId)
        {
            DataTable table;
            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            table = LogChatDao.GetAllByRoomId(dbClient, roomId);
            if (table == null)
            {
                return;
            }

            foreach (DataRow Row in table.Rows)
            {
                this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString(), (double)Row["timestamp"]);
            }
        }

        public void AddMessage(int UserId, string Username, int RoomId, string MessageText, double timestamp)
        {
            ChatlogEntry message = new ChatlogEntry(UserId, Username, RoomId, MessageText, timestamp);

            lock (this._listOfMessages)
            {
                this._listOfMessages.Add(message);
            }

            int CountMessage = this._listOfMessages.Count;
            if (CountMessage >= 100)
            {
                this._listOfMessages.RemoveRange(0, 1);
            }
        }

        public List<ChatlogEntry> GetSortedMessages(int roomId)
        {
            List<ChatlogEntry> list = new List<ChatlogEntry>();

            foreach (ChatlogEntry chatMessage in this._listOfMessages)
            {
                if (roomId == chatMessage.roomID || roomId == 0)
                {
                    list.Add(chatMessage);
                }
            }

            list.Reverse();

            return list;
        }

        public List<ChatlogEntry> ListOfMessages => this._listOfMessages;
    }
}
