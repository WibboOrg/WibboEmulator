using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.AI;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Actions
{
    public class BotTalkToAvatar : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string name;
        private string message;
        private bool IsWhisper;

        public BotTalkToAvatar(string stringParam, bool isWhisper, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            string[] messageAndName = stringParam.Split('\t');
            this.message = (messageAndName.Length == 2) ? messageAndName[0] : "";
            this.name = (messageAndName.Length == 2) ? messageAndName[1] : "";
            this.IsWhisper = isWhisper;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.name == "" || this.message == "" || user == null || user.GetClient() == null)
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.name);
            if (Bot == null || Bot.BotData == null)
            {
                return;
            }

            string TextMessage = this.message;
            TextMessage = TextMessage.Replace("#username#", user.GetUsername());
            TextMessage = TextMessage.Replace("#point#", user.WiredPoints.ToString());
            TextMessage = TextMessage.Replace("#roomname#", this.handler.GetRoom().RoomData.Name.ToString());
            TextMessage = TextMessage.Replace("#vote_yes#", this.handler.GetRoom().VotedYesCount.ToString());
            TextMessage = TextMessage.Replace("#vote_no#", this.handler.GetRoom().VotedNoCount.ToString());

            if (user.Roleplayer != null)
            {
                TextMessage = TextMessage.Replace("#money#", user.Roleplayer.Money.ToString());
            }

            if (this.IsWhisper && TextMessage.Contains(" : ") && (room.IsRoleplay || room.RoomData.OwnerName == "LieuPublic"))
            {
                this.SendBotChoose(TextMessage, user, Bot.BotData);
            }

            if (this.IsWhisper)
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_CHAT_WHISPER);
                Message.WriteInteger(Bot.VirtualId);
                Message.WriteString(TextMessage);
                Message.WriteInteger(0);
                Message.WriteInteger(2);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                user.GetClient().SendPacket(Message);
            }
            else
            {
                ServerPacket Message = new ServerPacket(ServerPacketHeader.UNIT_CHAT);
                Message.WriteInteger(Bot.VirtualId);
                Message.WriteString(TextMessage);
                Message.WriteInteger(ButterflyEnvironment.GetGame().GetChatManager().GetEmotions().GetEmotionsForText(this.message));
                Message.WriteInteger(2);
                Message.WriteInteger(0);
                Message.WriteInteger(-1);
                user.GetClient().SendPacket(Message);
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

        public void Dispose()
        {
            this.message = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.name + '\t' + this.message, this.IsWhisper, null);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            this.IsWhisper = (row["all_user_triggerable"].ToString() == "1");

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            string[] SplitData = Data.Split('\t');

            this.name = SplitData[0].ToString();
            this.message = SplitData[1].ToString();
        }

        public void OnTrigger(Client Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString(this.name + '\t' + this.message);
            Message.WriteInteger(1);
            Message.WriteInteger(this.IsWhisper ? 1 : 0);
            Message.WriteInteger(0);
            Message.WriteInteger(27); //7
            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }
    }
}
