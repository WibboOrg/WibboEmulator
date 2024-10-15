namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Bots;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Bots;
using WibboEmulator.Games.GameClients;

internal sealed class OpenBotActionEvent : IPacketEvent
{
    public double Delay => 0;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }

        var botId = packet.PopInt();
        var actionId = packet.PopInt();

        if (botId <= 0)
        {
            return;
        }

        var room = session.User.Room;
        if (room == null || !room.CheckRights(session))
        {
            return;
        }

        if (!room.RoomUserManager.TryGetBot(botId, out var botUser))
        {
            return;
        }

        var botSpeech = "";
        foreach (var speech in botUser.BotData.RandomSpeech.ToList())
        {
            botSpeech += speech + "\n";
        }

        botSpeech += ";#;";
        botSpeech += botUser.BotData.AutomaticChat;
        botSpeech += ";#;";
        botSpeech += botUser.BotData.SpeakingInterval;
        botSpeech += ";#;";
        botSpeech += botUser.BotData.MixSentences;

        if (actionId is 2 or 5 or 9)
        {
            session.SendPacket(new OpenBotActionComposer(botUser, actionId, botSpeech));
        }
    }
}
