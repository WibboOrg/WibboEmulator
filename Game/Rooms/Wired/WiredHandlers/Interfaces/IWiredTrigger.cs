using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWired
    {
        void Dispose();

        void SaveToDatabase(IQueryAdapter dbClient);

        void LoadFromDatabase(DataRow row, Room insideRoom);

        void OnTrigger(GameClient Session, int SpriteId);
    }
}
