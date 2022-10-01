using MySqlX.XDevAPI.Common;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class SetZStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitZMode = false;
            Session.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());

            Session.SendWhisper("Setz désactivé!");
        }
    }
}
