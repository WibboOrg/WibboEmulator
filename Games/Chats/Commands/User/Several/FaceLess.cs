namespace WibboEmulator.Games.Chats.Commands.User.Several;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class FaceLess : IChatCommand
{
    internal static readonly string[] Separator = ["hd-"];

    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (userRoom.IsTransf || userRoom.IsSpectator)
        {
            return;
        }

        var look = session.User.Look;

        if (look.Contains("hd-"))
        {
            var hdlook = look.Split(Separator, StringSplitOptions.None)[1];
            var hdcode = "hd-" + hdlook.Split(['.'])[0]; //ex : hd-180-22
            var hdcodecolor = "";
            if (hdcode.Split('-').Length == 3)
            {
                hdcodecolor = hdcode.Split('-')[2];
            }

            var hdcodenoface = "hd-99999-" + hdcodecolor; //hd-9999-22

            look = look.Replace(hdcode, hdcodenoface);

            session.User.Look = look;

            room.SendPacket(new UserChangeComposer(userRoom, false));
        }
    }
}
