using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
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

            using IQueryAdapter dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
            switch (cmd.ToLower())
            {
                case "staticevents":
                    {
                        WibboEnvironment.StaticEvents = !WibboEnvironment.StaticEvents;
                        break;
                    }
                case "random":
                    {
                        WibboEnvironment.RegenRandom();
                        Session.SendWhisper("Random mis à jour");
                        break;
                    }
                case "landingview":
                case "view":
                case "lv":
                case "vue":
                    {
                        WibboEnvironment.GetGame().GetHotelView().Init(dbClient);
                        Session.SendWhisper("Vue et promotion mises à jour");
                        break;
                    }
                case "text":
                case "texte":
                case "locale":
                    {
                        WibboEnvironment.GetLanguageManager().Init(dbClient);
                        Session.SendWhisper("Local mis à jour");
                        break;
                    }

                case "autogame":
                    {
                        WibboEnvironment.GetGame().GetAnimationManager().Init(dbClient);
                        Session.SendWhisper("Jeux automatique mis à jour");
                        break;
                    }
                case "rpitems":
                    {
                        WibboEnvironment.GetGame().GetRoleplayManager().GetItemManager().Init(dbClient);
                        Session.SendWhisper("RP Items mis à jour");
                        break;
                    }
                case "rpweapon":
                    {
                        WibboEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init(dbClient);
                        Session.SendWhisper("RP Weapon mis à jour");
                        break;
                    }
                case "rpenemy":
                    {
                        WibboEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init(dbClient);
                        Session.SendWhisper("RP Enemy mis à jour");
                        break;
                    }
                case "cmd":
                case "commands":
                    {
                        WibboEnvironment.GetGame().GetChatManager().GetCommands().Init(dbClient);
                        Session.SendWhisper("Commands mis à jour");
                        break;
                    }
                case "permission":
                    {
                        WibboEnvironment.GetGame().GetPermissionManager().Init(dbClient);
                        Session.SendWhisper("Permissions mises à jour !");
                        break;
                    }
                case "effet":
                    {
                        WibboEnvironment.GetGame().GetEffectManager().Init(dbClient);
                        Session.SendWhisper("Effet mis à jour");
                        break;
                    }
                case "rp":
                case "roleplay":
                    {
                        WibboEnvironment.GetGame().GetRoleplayManager().Init(dbClient);
                        Session.SendWhisper("Role play mis à jour");
                        break;
                    }
                case "moderation":
                    {
                        WibboEnvironment.GetGame().GetModerationManager().Init(dbClient);
                        Session.SendWhisper("Moderation mis à jour");
                        WibboEnvironment.GetGame().GetClientManager().SendMessageStaff(new WhisperComposer(UserRoom.VirtualId, "Les outils de modération viennent d'être mis à jour, reconnectez-vous!", 23));
                        break;
                    }
                case "catalogue":
                case "cata":
                    {
                        WibboEnvironment.GetGame().GetItemManager().Init(dbClient);
                        WibboEnvironment.GetGame().GetCatalog().Init(dbClient, WibboEnvironment.GetGame().GetItemManager());
                        WibboEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        Session.SendWhisper("Catalogue mis à jour");
                        break;
                    }
                case "navigateur":
                case "navi":
                    {
                        WibboEnvironment.GetGame().GetNavigator().Init(dbClient);
                        Session.SendWhisper("Navigateur mis à jour");
                        break;
                    }
                case "filter":
                case "filtre":
                    {
                        WibboEnvironment.GetGame().GetChatManager().GetFilter().Init(dbClient);
                        Session.SendWhisper("Filtre mis à jour");
                        break;
                    }
                case "items":
                case "furni":
                    {
                        WibboEnvironment.GetGame().GetItemManager().Init(dbClient);
                        Session.SendWhisper("Items mis à jour");
                        break;
                    }
                case "model":
                    {
                        WibboEnvironment.GetGame().GetRoomManager().Init(dbClient);
                        Session.SendWhisper("Model mis à jour");
                        break;
                    }
                case "mutant":
                case "figure":
                    {
                        WibboEnvironment.GetFigureManager().Init();
                        Session.SendWhisper("Mutant/Figure mises à jour");
                        break;
                    }
                default:
                    {
                        Session.SendWhisper(WibboEnvironment.GetLanguageManager().TryGetValue("cmd.notfound", Session.Langue));
                        return;
                    }
            }
        }
    }
}
