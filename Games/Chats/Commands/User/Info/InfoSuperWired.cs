namespace WibboEmulator.Games.Chats.Commands.User.Info;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class InfoSuperWired : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        session.SendPacket(new InClientLinkComposer("habbopages/infosuperwired"));

        return;
    }
}
