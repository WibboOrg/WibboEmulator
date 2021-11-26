﻿using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.Game.Users.Messenger
{
    public struct SearchResult
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string Look { get; set; }

        public SearchResult(int userID, string username, string look)
        {
            this.UserId = userID;
            this.Username = username;
            this.Look = look;
        }

        public void Searialize(ServerPacket reply)
        {
            reply.WriteInteger(this.UserId);
            reply.WriteString(this.Username);
            reply.WriteString(""); //motto
            reply.WriteBoolean(ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(this.UserId) != null);
            reply.WriteBoolean(false);
            reply.WriteString(string.Empty);
            reply.WriteInteger(0);
            reply.WriteString(this.Look);
            reply.WriteString(""); //realName
        }
    }
}