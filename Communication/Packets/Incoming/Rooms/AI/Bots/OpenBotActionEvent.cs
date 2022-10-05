namespace WibboEmulator.Communication.Packets.Incoming.Structure;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class OpenBotActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (!session.GetUser().InRoom)
        {
            return;
        }

        var BotId = Packet.PopInt();
        var ActionId = Packet.PopInt();

        if (BotId <= 0)
        {
            return;
        }

        var Room = session.GetUser().CurrentRoom;
        if (Room == null || !Room.CheckRights(session))
        {
            return;
        }

        if (!Room.GetRoomUserManager().TryGetBot(BotId, out var BotUser))
        {
            return;
        }

        var BotSpeech = "";
        foreach (var Speech in BotUser.BotData.RandomSpeech.ToList())
        {
            BotSpeech += Speech + "\n";
        }

        BotSpeech += ";#;";
        BotSpeech += BotUser.BotData.AutomaticChat;
        BotSpeech += ";#;";
        BotSpeech += BotUser.BotData.SpeakingInterval;
        BotSpeech += ";#;";
        BotSpeech += BotUser.BotData.MixSentences;

        if (ActionId is 2 or 5)
        {
            session.SendPacket(new OpenBotActionComposer(BotUser, ActionId, BotSpeech));
        }
    }
}