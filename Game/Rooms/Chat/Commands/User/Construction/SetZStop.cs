using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class SetZStop : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitZMode = false;
            Session.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());
        }
    }
}
