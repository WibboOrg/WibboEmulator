using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.RoomBots;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class BotTalkToAvatar : IWired, IWiredEffect
    {
        private readonly WiredHandler handler;
        private readonly int itemID;
        private string NomBot;
        private string message;
        private bool IsMurmur;

        public BotTalkToAvatar(string nombot, string message, bool iscrier, WiredHandler handler, int itemID)
        {
            this.itemID = itemID;
            this.handler = handler;
            this.message = message;
            this.NomBot = nombot;
            this.IsMurmur = iscrier;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.NomBot == "" || this.message == "" || user == null || user.GetClient() == null)
            {
                return;
            }

            Room room = this.handler.GetRoom();
            RoomUser Bot = room.GetRoomUserManager().GetBotOrPetByName(this.NomBot);
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

            if (this.IsMurmur && TextMessage.Contains(" : ") && (room.IsRoleplay || room.RoomData.OwnerName == "LieuPublic"))
            {
                this.SendBotChoose(TextMessage, user, Bot.BotData);
            }

            if (this.IsMurmur)
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
            WiredUtillity.SaveTriggerItem(dbClient, this.itemID, string.Empty, this.NomBot + '\t' + this.message, this.IsMurmur, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            dbClient.SetQuery("SELECT trigger_data, all_user_triggerable FROM wired_items WHERE trigger_id = @id ");
            dbClient.AddParameter("id", this.itemID);
            DataRow row = dbClient.GetRow();
            if (row == null)
            {
                return;
            }

            this.IsMurmur = (bool)(row["all_user_triggerable"]);

            string Data = row["trigger_data"].ToString();

            if (string.IsNullOrWhiteSpace(Data) || !Data.Contains("\t"))
            {
                return;
            }

            string[] SplitData = Data.Split('\t');

            this.NomBot = SplitData[0].ToString();
            this.message = SplitData[1].ToString();
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message15 = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message15.WriteBoolean(false);
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(SpriteId);
            Message15.WriteInteger(this.itemID);
            Message15.WriteString(this.NomBot + '\t' + this.message);
            Message15.WriteInteger(1);
            Message15.WriteInteger(this.IsMurmur ? 1 : 0);
            Message15.WriteInteger(0);
            Message15.WriteInteger(27); //7
            Message15.WriteInteger(0);
            Message15.WriteInteger(0);
            Session.SendPacket(Message15);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.itemID + "'");
        }
    }
}
