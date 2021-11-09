using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Games;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class GiveScore : IWiredEffect, IWired
    {
        private int maxCountPerGame;
        private int currentGameCount;
        private int scoreToGive;
        private GameManager gameManager;
        private RoomEventDelegate delegateFunction;
        private readonly int itemID;

        public GiveScore(int maxCountPerGame, int scoreToGive, GameManager gameManager, int itemID)
        {
            this.maxCountPerGame = maxCountPerGame;
            this.currentGameCount = 0;
            this.scoreToGive = scoreToGive;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.gameManager = gameManager;
            this.itemID = itemID;
            gameManager.OnGameStart += this.delegateFunction;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.Team == Team.none || this.maxCountPerGame <= this.currentGameCount)
            {
                return;
            }

            this.currentGameCount++;
            this.gameManager.AddPointToTeam(user.Team, this.scoreToGive, user);
        }

        public void Dispose()
        {
            this.gameManager.OnGameStart -= this.delegateFunction;
            this.gameManager = null;
            this.delegateFunction = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, this.scoreToGive.ToString(), this.maxCountPerGame.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if(int.TryParse(row["trigger_data"].ToString(), out int maxCount))
                this.maxCountPerGame = maxCount;

            if(int.TryParse(row["trigger_data_2"].ToString(), out int score))
                this.scoreToGive = score;
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(5);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(2);
            Message.WriteInteger(this.scoreToGive);
            Message.WriteInteger(this.maxCountPerGame);
            Message.WriteInteger(0);
            Message.WriteInteger(6);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
