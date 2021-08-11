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
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Roleplay;
using Butterfly.HabboHotel.Roleplay.Player;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.WebClients;
using System;

namespace Butterfly.Communication.Packets.Incoming.WebSocket
{
    internal class RpUseItemsEvent : IPacketWebEvent
    {
        public void Parse(WebClient Session, ClientPacket Packet)
        {
            int ItemId = Packet.PopInt();
            int UseCount = Packet.PopInt();

            GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUserID(Session.UserId);
            if (Client == null || Client.GetHabbo() == null)
            {
                return;
            }

            Room Room = Client.GetHabbo().CurrentRoom;
            if (Room == null || !Room.IsRoleplay)
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByHabboId(Client.GetHabbo().Id);
            if (User == null)
            {
                return;
            }

            if (User.Freeze)
            {
                return;
            }

            RolePlayer Rp = User.Roleplayer;
            if (Rp == null || Rp.Dead || Rp.SendPrison || Rp.TradeId > 0)
            {
                return;
            }

            if (Rp.AggroTimer > 0)
            {
                User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.useitem.notallowed", Client.Langue), Math.Round((double)Rp.AggroTimer / 2)));
                return;
            }

            RPItem RpItem = ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().GetItem(ItemId);
            if (RpItem == null)
            {
                return;
            }

            RolePlayInventoryItem RpItemInventory = Rp.GetInventoryItem(ItemId);
            if (RpItemInventory == null || RpItemInventory.Count <= 0 || RpItem.Type == "none")
            {
                return;
            }

            if (UseCount <= 0 || RpItem.UseType != 2)
            {
                UseCount = 1;
            }

            if (UseCount > RpItemInventory.Count)
            {
                UseCount = RpItemInventory.Count;
            }

            if (User.FreezeEndCounter <= 1)
            {
                User.Freeze = true;
                User.FreezeEndCounter = 1;
            }

            if (RpItem.Id == 75)
            {
                Rp.AddInventoryItem(45, UseCount);
            }

            switch (RpItem.Type)
            {
                case "openpage":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/roleplay/" + RpItem.Value));
                        break;
                    }
                case "openguide":
                    {
                        User.GetClient().SendPacket(new NuxAlertComposer("habbopages/westworld/westworld"));
                        break;
                    }
                case "hit":
                    {
                        Rp.Hit(User, RpItem.Value * UseCount, Room, false, true, false);
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        break;
                    }
                case "enable":
                    {
                        User.ApplyEffect(RpItem.Value);
                        break;
                    }
                case "showtime":
                    {
                        User.SendWhisperChat("Il est " + Room.RpHour + " heures et " + Room.RpMinute + " minutes");
                        break;
                    }
                case "money":
                    {
                        Rp.Money += RpItem.Value * UseCount;
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        Rp.SendUpdate();
                        break;
                    }
                case "munition":
                    {
                        Rp.AddMunition(RpItem.Value * UseCount);
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);
                        Rp.SendUpdate();
                        break;
                    }
                case "energytired":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.Hit(User, RpItem.Value * UseCount, Room, false, true, false);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consomme " + char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "healthtired":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.RemoveEnergy(RpItem.Value * UseCount);
                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consomme " + char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "healthenergy":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consomme " + char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "energy":
                    {
                        User.ApplyEffect(4, true);
                        User.TimerResetEffect = 2;

                        Rp.AddEnergy(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consomme " + char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "health":
                    {
                        User.ApplyEffect(737, true);
                        User.TimerResetEffect = 4;

                        Rp.AddHealth(RpItem.Value * UseCount);
                        Rp.SendUpdate();
                        Rp.RemoveInventoryItem(RpItem.Id, UseCount);

                        User.OnChat("*Consomme " + char.ToLowerInvariant(RpItem.Title[0]) + RpItem.Title.Substring(1) + "*");
                        break;
                    }
                case "weapon_cac":
                    {
                        if (Rp.WeaponCac.Id == RpItem.Value)
                        {
                            break;
                        }

                        Rp.WeaponCac = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponCac(RpItem.Value);
                        User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.changearmecac", Client.Langue));
                        break;
                    }
                case "weapon_far":
                    {
                        if (Rp.WeaponGun.Id == RpItem.Value)
                        {
                            break;
                        }

                        Rp.WeaponGun = ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().GetWeaponGun(RpItem.Value);
                        User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("rp.changearmefar", Client.Langue));
                        break;
                    }
            }
        }
    }
}
