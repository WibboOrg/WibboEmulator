namespace WibboEmulator.Games.Chats.Commands.User.Info;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;

using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class InfoSuperWired : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        Session.SendPacket(new InClientLinkComposer("habbopages/infosuperwired"));

        return;
    }
}
