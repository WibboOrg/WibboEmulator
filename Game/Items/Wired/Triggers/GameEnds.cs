using Butterfly.Database.Interfaces;
using Butterfly.Game.Rooms;
using Butterfly.Game.Items.Wired.Interfaces;
using System;
using System.Data;

namespace Butterfly.Game.Items.Wired.Triggers
{
    public class GameEnds : WiredTriggerBase, IWired
    {
        private readonly RoomEventDelegate gameEndsDeletgate;

        public GameEnds(Item item, Room room) : base(item, room, (int)WiredTriggerType.GAME_ENDS)
        {
            this.gameEndsDeletgate = new RoomEventDelegate(this.gameManager_OnGameEnd);
            this.RoomInstance.GetGameManager().OnGameEnd += this.gameEndsDeletgate;
        }

        private void gameManager_OnGameEnd(object sender, EventArgs e)
        {
            this.RoomInstance.GetWiredHandler().ExecutePile(this.ItemInstance.Coordinate, null, null);
        }

        public override void Dispose()
        {
            base.Dispose();

            this.RoomInstance.GetWiredHandler().GetRoom().GetGameManager().OnGameEnd -= this.gameEndsDeletgate;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
        }

        public void LoadFromDatabase(DataRow row)
        {
        }
    }
}
