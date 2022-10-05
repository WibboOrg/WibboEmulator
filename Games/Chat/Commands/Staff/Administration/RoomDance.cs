namespace WibboEmulator.Games.Chat.Commands.Cmd;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class RoomDance : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] Params)
    {
        if (Params.Length == 1)
        {
            session.SendWhisper("Please enter a dance ID. (1-4)");
            return;
        }

        var DanceId = Convert.ToInt32(Params[1]);
        if (DanceId is < 0 or > 4)
        {
            session.SendWhisper("Please enter a dance ID. (1-4)");
            return;
        }

        var Users = Room.GetRoomUserManager().GetRoomUsers();
        if (Users.Count > 0)
        {
            foreach (var user in Users.ToList())
            {
                if (user == null)
                {
                    continue;
                }

                if (user.CarryItemID > 0)
                {
                    user.CarryItemID = 0;
                }

                user.DanceId = DanceId;
                Room.SendPacket(new DanceComposer(user.VirtualId, DanceId));
            }
        }
    }
}