using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Update : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length != 2)
            {
                return;
            }

            string cmd = Params[1];

            if (string.IsNullOrEmpty(cmd))
            {
                return;
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                switch (cmd.ToLower())
                {
                    case "staticevents":
                        {
                            ButterflyEnvironment.StaticEvents = !ButterflyEnvironment.StaticEvents;
                            break;
                        }
                    case "random":
                        {
                            ButterflyEnvironment.RegenRandom();
                            Session.SendWhisper("Random mis à jour");
                            break;
                        }
                    case "landingview":
                    case "view":
                    case "lv":
                    case "vue":
                        {
                            ButterflyEnvironment.GetGame().GetHotelView().Init(dbClient);
                            Session.SendWhisper("Vue et promotion mises à jour");
                            break;
                        }
                    case "text":
                    case "texte":
                    case "locale":
                        {
                            ButterflyEnvironment.GetLanguageManager().Init(dbClient);
                            Session.SendWhisper("Local mis à jour");
                            break;
                        }

                    case "wibbogame":
                        {
                            ButterflyEnvironment.GetGame().GetAnimationManager().Init(dbClient);
                            Session.SendWhisper("Wibbo Game mis à jour");
                            break;
                        }
                    case "autogame":
                        {
                            if (!ButterflyEnvironment.GetGame().GetAnimationManager().ToggleForceDisabled())
                            {
                                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.false", Session.Langue));
                            }
                            else
                            {
                                Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.autogame.true", Session.Langue));
                            }

                            break;
                        }
                    case "rpitems":
                        {
                            ButterflyEnvironment.GetGame().GetRoleplayManager().GetItemManager().Init(dbClient);
                            Session.SendWhisper("RP Items mis à jour");
                            break;
                        }
                    case "rpweapon":
                        {
                            ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init(dbClient);
                            Session.SendWhisper("RP Weapon mis à jour");
                            break;
                        }
                    case "rpenemy":
                        {
                            ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init(dbClient);
                            Session.SendWhisper("RP Enemy mis à jour");
                            break;
                        }
                    case "cmd":
                    case "commands":
                        {
                            ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Init(dbClient);
                            Session.SendWhisper("Commands mis à jour");
                            break;
                        }
                    case "role":
                    case "fuse":
                        {
                            ButterflyEnvironment.GetGame().GetPermissionManager().Init(dbClient);
                            Session.SendWhisper("Rôle mis à jour");
                            break;
                        }
                    case "effet":
                        {
                            ButterflyEnvironment.GetGame().GetEffectManager().Init(dbClient);
                            Session.SendWhisper("Effet mis à jour");
                            break;
                        }
                    case "rp":
                    case "roleplay":
                        {
                            ButterflyEnvironment.GetGame().GetRoleplayManager().Init(dbClient);
                            Session.SendWhisper("Role play mis à jour");
                            break;
                        }
                    case "modo":
                        {
                            ButterflyEnvironment.GetGame().GetModerationManager().Init(dbClient);
                            Session.SendWhisper("Moderation mis à jour");
                            ButterflyEnvironment.GetGame().GetClientManager().SendMessageStaff(new WhisperComposer(UserRoom.VirtualId, "Les outils de modération viennent d'être mis à jour, reconnectez-vous !" + "L'équipe développement", 23));
                            break;
                        }
                    case "catalogue":
                    case "cata":
                        {
                            ButterflyEnvironment.GetGame().GetItemManager().Init(dbClient);
                            ButterflyEnvironment.GetGame().GetCatalog().Init(dbClient, ButterflyEnvironment.GetGame().GetItemManager());
                            ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                            Session.SendWhisper("Catalogue mis à jour");
                            break;
                        }
                    case "navigateur":
                    case "navi":
                        {
                            ButterflyEnvironment.GetGame().GetNavigator().Init(dbClient);
                            Session.SendWhisper("Navigateur mis à jour");
                            break;
                        }
                    case "filter":
                    case "filtre":
                        {
                            ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Init(dbClient);
                            Session.SendWhisper("Filtre mis à jour");
                            break;
                        }
                    case "items":
                    case "furni":
                        {
                            ButterflyEnvironment.GetGame().GetItemManager().Init(dbClient);
                            Session.SendWhisper("Items mis à jour");
                            break;
                        }
                    case "model":
                        {
                            ButterflyEnvironment.GetGame().GetRoomManager().Init(dbClient);
                            Session.SendWhisper("Model mis à jour");
                            break;
                        }
                    case "mutant":
                    case "figure":
                        {
                            ButterflyEnvironment.GetFigureManager().Init();
                            Session.SendWhisper("Mutant/Figure mises à jour");
                            break;
                        }
                    default:
                        {
                            Session.SendWhisper(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.notfound", Session.Langue));
                            return;
                        }
                }
            }
        }
    }
}
