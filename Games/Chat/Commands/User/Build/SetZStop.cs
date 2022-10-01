using MySqlX.XDevAPI.Common;
using WibboEmulator.Games.Clients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class SetZStop : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            UserRoom.ConstruitZMode = false;
            Session.SendPacket(Room.GetGameMap().Model.SerializeRelativeHeightmap());

            Session.SendWhisper("Setz d�sactiv�!");
        }
    }
}
