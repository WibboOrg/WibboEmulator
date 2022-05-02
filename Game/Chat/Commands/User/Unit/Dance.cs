using Butterfly.Game.Rooms;
using Butterfly.Game.Clients;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Dance : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length == 1)
            {
                UserRoom.SendWhisperChat("Entre un numéro à ta danse");
            }

            int DanceId;
            if (int.TryParse(Params[1], out DanceId))
            {
                if (DanceId > 4 || DanceId < 0)
                {
                    UserRoom.SendWhisperChat("Entre un numéro entre 0 et 4");
                    return;
                }
                Session.GetUser().CurrentRoom.SendPacket(new DanceComposer(UserRoom.VirtualId, DanceId));
            }
            else
                UserRoom.SendWhisperChat("Entre un numéro de danse valide");
        }
    }
}