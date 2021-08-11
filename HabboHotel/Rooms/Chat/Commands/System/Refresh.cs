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

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Refresh : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string Cmd = Params[1];

            if (string.IsNullOrEmpty(Cmd))
            {
                return;
            }

            switch (Cmd)
            {
                case "random":
                    {
                        ButterflyEnvironment.RegenRandom();
                        break;
                    }
                case "view":
                case "vue":
                    {
                        ButterflyEnvironment.GetGame().GetHotelView().InitHotelViewPromo();
                        break;
                    }
                case "text":
                case "texte":
                case "locale":
                    {
                        ButterflyEnvironment.GetLanguageManager().InitLocalValues();
                        break;
                    }

                case "wibbogame":
                    {
                        ButterflyEnvironment.GetGame().GetAnimationManager().Init();
                        break;
                    }
                case "autogame":
                    {

                        if (!ButterflyEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
                        {
                            UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", Session.Langue));
                        }
                        else
                        {
                            UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", Session.Langue));
                        }

                        break;
                    }
                case "rpitems":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().Init();
                        break;
                    }
                case "rpweapon":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init();
                        break;
                    }
                case "rpenemy":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init();
                        break;
                    }
                case "cmd":
                case "commands":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Init();
                        break;
                    }
                case "role":
                    {
                        ButterflyEnvironment.GetGame().GetRoleManager().Init();
                        break;
                    }
                case "effet":
                    {
                        ButterflyEnvironment.GetGame().GetEffectManager().Init();
                        break;
                    }
                case "rp":
                case "roleplay":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().Init();
                        break;
                    }
                case "modo":
                    {
                        ButterflyEnvironment.GetGame().GetModerationManager().Init();
                        break;
                    }
                case "catalogue":
                case "cata":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        ButterflyEnvironment.GetGame().GetCatalog().Init(ButterflyEnvironment.GetGame().GetItemManager());
                        ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        break;
                    }
                case "navigateur":
                case "navi":
                    {
                        ButterflyEnvironment.GetGame().GetNavigator().Init();
                        break;
                    }
                case "filter":
                case "filtre":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Init();
                        break;
                    }
                case "items":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        break;
                    }
                case "model":
                    ButterflyEnvironment.GetGame().GetRoomManager().LoadModels();
                    break;
                case "mutant":
                case "figure":
                    {
                        ButterflyEnvironment.GetFigureManager().Init();
                        break;
                    }
                default:
                    {
                        UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.notfound", Session.Langue));
                        return;
                    }
            }
            UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.refresh", Session.Langue));
        }
    }
}
