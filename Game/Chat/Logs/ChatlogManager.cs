using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Chat.Logs
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
                this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString());
            }
        }

        public void LoadRoomChatlogs(int roomId)
        {
            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                table = LogChatDao.GetAllByRoomId(dbClient, roomId);
                if (table == null)
                {
                    return;
                }

                foreach (DataRow Row in table.Rows)
                {
                    this.AddMessage(Convert.ToInt32(Row["user_id"]), Row["user_name"].ToString(), Convert.ToInt32(Row["room_id"]), Row["type"].ToString() + Row["message"].ToString());
                }
            }
        }

        public void AddMessage(int UserId, string Username, int RoomId, string MessageText)
        {
            ChatlogEntry message = new ChatlogEntry(UserId, Username, RoomId, MessageText, DateTime.Now);

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

        public List<ChatlogEntry> GetSortedMessages(int roomid)
        {
            List<ChatlogEntry> list = new List<ChatlogEntry>();

            foreach (ChatlogEntry chatMessage in this._listOfMessages)
            {
                if (roomid == chatMessage.roomID || roomid == 0)
                {
                    list.Add(chatMessage);
                }
            }

            list.Reverse();

            return list;
        }

        public void Serialize(ref ServerPacket message)
        {
            List<ChatlogEntry> ListReverse = new List<ChatlogEntry>();
            ListReverse.AddRange(this._listOfMessages);
            ListReverse.Reverse();
            foreach (ChatlogEntry chatMessage in ListReverse)
            {
                if (chatMessage != null)
                {
                    chatMessage.Serialize(ref message);
                }
                else
                {
                    message.WriteString("0"); //this.timeSpoken.Minute
                    message.WriteInteger(0); //this.timeSpoken.Minute
                    message.WriteString("");
                    message.WriteString("");
                    message.WriteBoolean(false); // Text is bold
                }
            }
        }
    }
}
