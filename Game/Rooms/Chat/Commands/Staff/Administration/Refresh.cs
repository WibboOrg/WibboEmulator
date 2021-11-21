using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.GameClients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
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

                        UserRoom.SendWhisperChat("Random mis à jour");
                    }
                case "view":
                case "vue":
                    {
                        ButterflyEnvironment.GetGame().GetHotelView().InitHotelViewPromo();
                        break;
                        UserRoom.SendWhisperChat("Vue et promotion mises à jour");
                    }
                case "text":
                case "texte":
                case "locale":
                    {
                        ButterflyEnvironment.GetLanguageManager().InitLocalValues();
                        break;
                        UserRoom.SendWhisperChat("Local mis à jour");
                    }

                case "wibbogame":
                    {
                        ButterflyEnvironment.GetGame().GetAnimationManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Wibbo Game mis à jour");
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
                        UserRoom.SendWhisperChat("RP Items mis à jour");
                    }
                case "rpweapon":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init();
                        break;
                        UserRoom.SendWhisperChat("RP Weapon mis à jour");
                    }
                case "rpenemy":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init();
                        break;
                        UserRoom.SendWhisperChat("RP Enemy mis à jour");
                    }
                case "cmd":
                case "commands":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Init();
                        break;
                        UserRoom.SendWhisperChat("Commands mis à jour");
                    }
                case "role":
                    {
                        ButterflyEnvironment.GetGame().GetRoleManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Rôle mis à jour");
                    }
                case "effet":
                    {
                        ButterflyEnvironment.GetGame().GetEffectManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Effet mis à jour");
                    }
                case "rp":
                case "roleplay":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Role play mis à jour");
                    }
                case "modo":
                    {
                        ButterflyEnvironment.GetGame().GetModerationManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Moderation mis à jour");
                    }
                case "catalogue":
                case "cata":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        ButterflyEnvironment.GetGame().GetCatalog().Init(ButterflyEnvironment.GetGame().GetItemManager());
                        ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        break;
                        UserRoom.SendWhisperChat("Catalogue mis à jour");
                    }
                case "navigateur":
                case "navi":
                    {
                        ButterflyEnvironment.GetGame().GetNavigator().Init();
                        break;
                        UserRoom.SendWhisperChat("Navigateur mis à jour");
                    }
                case "filter":
                case "filtre":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Init();
                        break;
                        UserRoom.SendWhisperChat("Filtre mis à jour");
                    }
                case "items":
                case "furni":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Items mis à jour");
                    }
                case "model":
                    {
                    ButterflyEnvironment.GetGame().GetRoomManager().LoadModels();
                    break;
                    UserRoom.SendWhisperChat("Model mis à jour");
                    }
                case "mutant":
                case "figure":
                    {
                        ButterflyEnvironment.GetFigureManager().Init();
                        break;
                        UserRoom.SendWhisperChat("Mutant/Figure mises à jour");
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
