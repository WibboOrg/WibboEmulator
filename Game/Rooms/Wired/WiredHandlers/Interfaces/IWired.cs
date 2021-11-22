using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces
{
    public interface IWired
    {
        void Dispose();

        void SaveToDatabase(IQueryAdapter dbClient);

        void LoadFromDatabase(DataRow row, Room insideRoom);

        void OnTrigger(Client Session, int SpriteId);
    }
}
