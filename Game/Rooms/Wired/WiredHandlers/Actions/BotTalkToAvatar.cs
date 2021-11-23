using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.AI;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotTalkToAvatar : WiredActionBase, IWired, IWiredEffect
    {
        public BotTalkToAvatar(Item item, Room room) : base(item, room, (int)WiredActionType.BOT_TALK_DIRECT_TO_AVTR)
        {
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.StringParam.Contains("\t"))
                return;

            string[] splitData = this.StringParam.Split('\t');

            string name = splitData[0].ToString();
            string message = splitData[1].ToString();

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(message) || user == null || user.GetClient() == null)
            {
                return;
            }

            RoomUser Bot = this.RoomInstance.GetRoomUserManager().GetBotOrPetByName(name);
            if (Bot == null || Bot.BotData == null)
            {
                return;
            }

            bool isWhisper = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            string TextMessage = message;
            TextMessage = TextMessage.Replace("#username#", user.GetUsername());
            TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
            TextMessage = TextMessage.Replace("#roomname#", this.RoomInstance.RoomData.Name.ToString());
            TextMessage = TextMessage.Replace("#vote_yes#", this.RoomInstance.VotedYesCount.ToString());
            TextMessage = TextMessage.Replace("#vote_no#", this.RoomInstance.VotedNoCount.ToString());

            if (user.Roleplayer != null)
            {
                TextMessage = TextMessage.Replace("#money#", user.Roleplayer.Money.ToString());
            }

            if (isWhisper && TextMessage.Contains(" : ") && (this.RoomInstance.IsRoleplay || this.RoomInstance.RoomData.OwnerName == "LieuPublic"))
            {
                this.SendBotChoose(TextMessage, user, Bot.BotData);
            }

            if (isWhisper)
            {
                ServerPacket MessagePacket = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                MessagePacket.WriteInteger(Bot.VirtualId);
                MessagePacket.WriteString(TextMessage);
                MessagePacket.WriteInteger(0);
                MessagePacket.WriteInteger(2);
                MessagePacket.WriteInteger(0);
                MessagePacket.WriteInteger(-1);
                user.GetClient().SendPacket(MessagePacket);
            }
            else
            {
                ServerPacket MessagePacket = new ServerPacket(ServerPacketHeader.UNIT_CHAT);
                MessagePacket.WriteInteger(Bot.VirtualId);
                MessagePacket.WriteString(TextMessage);
                MessagePacket.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(message));
                MessagePacket.WriteInteger(2);
                MessagePacket.WriteInteger(0);
                MessagePacket.WriteInteger(-1);
                user.GetClient().SendPacket(MessagePacket);
            }
        }

        private void SendBotChoose(string TextMessage, RoomUser user, RoomBot BotData)
        {
            string[] SplitText = TextMessage.Split(new[] { " : " }, StringSplitOptions.None);
            if (SplitText.Length != 2)
            {
                return;
            }

            List<string[]> ChooseList = new List<string[]>
            {
                new List<string>
                {
                    BotData.Name,
                    SplitText[0],
                    SplitText[1],
                    BotData.Look
                }.ToArray()
            };

            user.GetClient().GetHabbo().SendWebPacket(new BotChooseComposer(ChooseList));
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            bool isWhisper = (((this.IntParams.Count > 0) ? this.IntParams[0] : 0)) == 1;

            WiredUtillity.SaveTriggerItem(dbClient, this.Id, string.Empty, this.StringParam, isWhisper, null);
        }

        public void LoadFromDatabase(DataRow row)
        {
            if (int.TryParse(row["all_user_triggerable"].ToString(), out int isWhisper))
                this.IntParams.Add(isWhisper);

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            this.StringParam = Data;
        }
    }
}
