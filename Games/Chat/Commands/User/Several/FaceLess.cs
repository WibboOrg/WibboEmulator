using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Clients;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class FaceLess : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (UserRoom.IsTransf || UserRoom.IsSpectator)
            {
                return;
            }

            string look = Session.GetUser().Look;

            if (look.Contains("hd-"))
            {
                string hdlook = look.Split(new string[] { "hd-" }, StringSplitOptions.None)[1];
                string hdcode = "hd-" + hdlook.Split(new char[] { '.' })[0]; //ex : hd-180-22
                string hdcodecolor = "";
                if (hdcode.Split('-').Length == 3)
                {
                    hdcodecolor = hdcode.Split('-')[2];
                }

                string hdcodenoface = "hd-99999-" + hdcodecolor; //hd-9999-22

                look = look.Replace(hdcode, hdcodenoface);

                Session.GetUser().Look = look;

                Room.SendPacket(new UserChangeComposer(UserRoom, false));
            }
        }
    }
}
