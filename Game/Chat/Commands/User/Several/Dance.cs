using Wibbo.Game.Rooms;
using Wibbo.Game.Clients;
using Wibbo.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Wibbo.Game.Chat.Commands.Cmd
{
    internal class Dance : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                Session.SendWhisper("Entre un numéro à ta danse");
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    Session.SendWhisper("Entre un numéro entre 0 et 4");
                    return;
                }
                Session.GetUser().CurrentRoom.SendPacket(new DanceComposer(UserRoom.VirtualId, DanceId));
            }
            else
                Session.SendWhisper("Entre un numéro de danse valide");
        }
    }
}