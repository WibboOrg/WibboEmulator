namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;

using WibboEmulator.Communication.Packets.Outgoing.Sound;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class PlaySoundRoom : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var songName = parameters[1];

        room.SendPacket(new PlaySoundComposer(songName, 1)); //Type = Trax
    }
}
