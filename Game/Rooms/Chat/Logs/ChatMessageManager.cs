using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Chat.Logs
{
    public class ChatMessageManager
    {
        private readonly List<ChatMessage> listOfMessages;

        public int messageCount => this.listOfMessages.Count;

        public ChatMessageManager()
        {
            this.listOfMessages = new List<ChatMessage>();
        }

        public void LoadUserChatlogs(int UserId)
        {
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable table = LogChatDao.GetAllByUserId(dbClient, UserId);
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

        public void LoadRoomChatlogs(int RoomId)
        {
            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                table = LogChatDao.GetAllByRoomId(dbClient, RoomId);
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
            ChatMessage message = new ChatMessage(UserId, Username, RoomId, MessageText, DateTime.Now);

            lock (this.listOfMessages)
            {
                this.listOfMessages.Add(message);
            }

            int CountMessage = this.listOfMessages.Count;
            if (CountMessage >= 100)
            {
                this.listOfMessages.RemoveRange(0, 1);
            }
        }

        public List<ChatMessage> GetSortedMessages(int roomid)
        {
            List<ChatMessage> list = new List<ChatMessage>();

            foreach (ChatMessage chatMessage in this.listOfMessages)
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
            List<ChatMessage> ListReverse = new List<ChatMessage>();
            ListReverse.AddRange(this.listOfMessages);
            ListReverse.Reverse();
            foreach (ChatMessage chatMessage in ListReverse)
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
