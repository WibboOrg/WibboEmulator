namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Navigators;

internal sealed class GetUserFlatCatsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var categories = NavigatorManager.FlatCategories;

        session.SendPacket(new UserFlatCatsComposer(categories, session.User.Rank));
    }
}
