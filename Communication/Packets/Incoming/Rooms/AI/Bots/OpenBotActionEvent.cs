using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Bots;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class OpenBotActionEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }

            int BotId = Packet.PopInt();
            int ActionId = Packet.PopInt();

            if (BotId <= 0)
            {
                return;
            }

            Room Room = Session.GetUser().CurrentRoom;
            if (Room == null || !Room.CheckRights(Session))
            {
                return;
            }

            if (!Room.GetRoomUserManager().TryGetBot(BotId, out RoomUser BotUser))
            {
                return;
            }

            string BotSpeech = "";
            foreach (string Speech in BotUser.BotData.RandomSpeech.ToList())
            {
                BotSpeech += (Speech + "\n");
            }

            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.AutomaticChat;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.SpeakingInterval;
            BotSpeech += ";#;";
            BotSpeech += BotUser.BotData.MixSentences;

            if (ActionId == 2 || ActionId == 5)
            {
                Session.SendPacket(new OpenBotActionComposer(BotUser, ActionId, BotSpeech));
            }
        }
    }
}