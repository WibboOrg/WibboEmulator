using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Clients;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Dance : IChatCommand
    {
        public void Execute(Client session, Room room, RoomUser user, string[] parameters)
        {
            if (parameters.Length < 2)
            {
                session.SendWhisper("Entre un numéro à ta danse");
                return;
            }

            int danceId;
            if (int.TryParse(parameters[1], out danceId))
            {
                if (danceId > 4 || danceId < 0)
                {
                    session.SendWhisper("Entre un numéro entre 0 et 4");
                    return;
                }

                room.SendPacket(new DanceComposer(user.VirtualId, danceId));
            }
            else
                session.SendWhisper("Entre un numéro de danse valide");
        }
    }
}