namespace WibboEmulator.Communication.Packets.Incoming.Navigator;
using WibboEmulator.Communication.Packets.Outgoing.Navigator;
using WibboEmulator.Games.GameClients;

internal class GetUserFlatCatsEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var Categories = WibboEnvironment.GetGame().GetNavigator().GetFlatCategories();

        session.SendPacket(new UserFlatCatsComposer(Categories, session.GetUser().Rank));
    }
}