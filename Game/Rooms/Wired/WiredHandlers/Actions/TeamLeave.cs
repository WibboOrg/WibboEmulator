using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Games;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class TeamLeave : WiredActionBase, IWired, IWiredEffect
    {
        public TeamLeave(Item item, Room room) : base(item, room, (int)WiredActionType.LEAVE_TEAM)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user != null && !user.IsBot && user.GetClient() != null && user.Team != Team.none && user.Room != null)
            {
                TeamManager managerForBanzai = user.Room.GetTeamManager();
                if (managerForBanzai == null)
                {
                    return;
                }

                managerForBanzai.OnUserLeave(user);
                user.Room.GetGameManager().UpdateGatesTeamCounts();
                user.ApplyEffect(0);
                user.Team = Team.none;

                user.GetClient().SendPacket(new IsPlayingComposer(false));
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            
        }

        public void LoadFromDatabase(DataRow row)
        {
        }
    }
}
