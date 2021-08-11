using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Navigator.New;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class HighScore : IWired, IWiredEffect
    {
        private readonly Item item;

        public HighScore(Item item)
        {
            this.item = item;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (user == null || user.IsBot || user.GetClient() == null)
            {
                return;
            }

            Dictionary<string, int> Scores = this.item.Scores;

            List<string> ListUsernameScore = new List<string>() { user.GetUsername() };

            if (Scores.ContainsKey(ListUsernameScore[0]))
            {
                Scores[ListUsernameScore[0]] += 1;
            }
            else
            {
                Scores.Add(ListUsernameScore[0], 1);
            }

            Room room = this.item.GetRoom();
            if (room == null)
            {
                return;
            }

            room.SendPacket(new ObjectUpdateComposer(this.item, room.RoomData.OwnerId));

        }

        public void Dispose()
        {
            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                this.SaveToDatabase(queryreactor);
            }
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggerdata = "";

            int i = 0;
            foreach (KeyValuePair<string, int> score in this.item.Scores.OrderByDescending(x => x.Value).Take(20))
            {
                if (i != 0)
                {
                    triggerdata += ";";
                }

                triggerdata += score.Key + ":" + score.Value;

                i++;
            }

            WiredUtillity.SaveTriggerItem(dbClient, this.item.Id, string.Empty, triggerdata, false, null);
        }

        public void LoadFromDatabase(IQueryAdapter dbClient, Room insideRoom)
        {
            string triggerdata = null;

            dbClient.SetQuery("SELECT trigger_data FROM wired_items WHERE trigger_id = @id");
            dbClient.AddParameter("id", this.item.Id);
            DataRow row = dbClient.GetRow();
            if (row != null)
            {
                triggerdata = row["trigger_data"].ToString();
            }

            if (string.IsNullOrEmpty(triggerdata))
            {
                return;
            }

            foreach (string score in triggerdata.Split(';'))
            {
                //:lucie:42
                string[] score2 = score.Split(':');
                int.TryParse(score2[score2.Count() - 1], out int ScoreNum);
                string Pseudo = "";
                for (int i = 0; i < score2.Count() - 1; i++)
                {
                    if (i == 0)
                    {
                        Pseudo = score2[i];
                    }
                    else
                    {
                        Pseudo += ':' + score2[i];
                    }
                }

                //List<string> ListUsernameScore = new List<string>() { score2[0] };
                if (!this.item.Scores.ContainsKey(Pseudo))
                {
                    this.item.Scores.Add(Pseudo, ScoreNum);
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {

            int.TryParse(this.item.ExtraData, out int NumMode);

            if (NumMode != 1)
            {
                NumMode = 1;
            }
            else
            {
                NumMode = 0;
            }

            this.item.ExtraData = NumMode.ToString();
            this.item.UpdateState(false, true);
        }

        public void DeleteFromDatabase(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("DELETE FROM wired_items WHERE trigger_id = '" + this.item.Id + "'");
        }
    }
}
