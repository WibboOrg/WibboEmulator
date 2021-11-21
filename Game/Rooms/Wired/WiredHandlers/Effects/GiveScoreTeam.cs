using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Effects
{
    public class GiveScoreTeam : IWiredEffect, IWired
    {
        private int maxCountPerGame;
        private int currentGameCount;
        private int scoreToGive;
        private GameManager gameManager;
        private RoomEventDelegate delegateFunction;
        private readonly int itemID;
        private Team team;

        public GiveScoreTeam(int TeamId, int maxCountPerGame, int scoreToGive, GameManager gameManager, int itemID)
        {
            if (TeamId < 1 || TeamId > 4)
            {
                TeamId = 1;
            }

            this.maxCountPerGame = maxCountPerGame;
            this.currentGameCount = 0;
            this.scoreToGive = scoreToGive;
            this.delegateFunction = new RoomEventDelegate(this.gameManager_OnGameStart);
            this.gameManager = gameManager;
            this.itemID = itemID;
            gameManager.OnGameStart += this.delegateFunction;
            this.team = (Team)TeamId;
        }

        private void gameManager_OnGameStart(object sender, EventArgs e)
        {
            this.currentGameCount = 0;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.maxCountPerGame <= this.currentGameCount)
            {
                return;
            }

            this.currentGameCount++;
            this.gameManager.AddPointToTeam(this.team, this.scoreToGive, user);
        }

        public void Dispose()
        {
            this.gameManager.OnGameStart -= this.delegateFunction;
            this.gameManager = null;
            this.delegateFunction = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, ((int)this.team).ToString(), this.maxCountPerGame.ToString() + ":" + this.scoreToGive.ToString(), false, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            string data = row["trigger_data"].ToString();
            string data2 = row["trigger_data_2"].ToString();

            if (data.Contains(":"))
            {
                string[] dataSplit = data.Split(':');

                if (int.TryParse(dataSplit[0], out int maxCount))
                    this.maxCountPerGame = maxCount;

                if (int.TryParse(dataSplit[1], out int score))
                    this.scoreToGive = score;
            }

            if (int.TryParse(data2, out int number))
            {
                this.team = (Team)number;
            }

        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(3);
            Message.WriteInteger(this.scoreToGive);
            Message.WriteInteger(this.maxCountPerGame);
            Message.WriteInteger((int)this.team);

            Message.WriteInteger(0);
            Message.WriteInteger(14);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
