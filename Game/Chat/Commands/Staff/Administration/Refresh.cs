using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Refresh : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
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
                        UserRoom.SendWhisperChat("Random mis à jour");
                        break;
                    }
                case "view":
                case "vue":
                    {
                        ButterflyEnvironment.GetGame().GetHotelView().Init();
                        UserRoom.SendWhisperChat("Vue et promotion mises à jour");
                        break;
                    }
                case "text":
                case "texte":
                case "locale":
                    {
                        ButterflyEnvironment.GetLanguageManager().InitLocalValues();
                        UserRoom.SendWhisperChat("Local mis à jour");
                        break;
                    }

                case "wibbogame":
                    {
                        ButterflyEnvironment.GetGame().GetAnimationManager().Init();
                        UserRoom.SendWhisperChat("Wibbo Game mis à jour");
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
                        UserRoom.SendWhisperChat("RP Items mis à jour");
                        break;
                    }
                case "rpweapon":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetWeaponManager().Init();
                        UserRoom.SendWhisperChat("RP Weapon mis à jour");
                        break;
                    }
                case "rpenemy":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().GetEnemyManager().Init();
                        UserRoom.SendWhisperChat("RP Enemy mis à jour");
                        break;
                    }
                case "cmd":
                case "commands":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetCommands().Init();
                        UserRoom.SendWhisperChat("Commands mis à jour");
                        break;
                    }
                case "role":
                    {
                        ButterflyEnvironment.GetGame().GetRoleManager().Init();
                        UserRoom.SendWhisperChat("Rôle mis à jour");
                        break;
                    }
                case "effet":
                    {
                        ButterflyEnvironment.GetGame().GetEffectManager().Init();
                        UserRoom.SendWhisperChat("Effet mis à jour");
                        break;
                    }
                case "rp":
                case "roleplay":
                    {
                        ButterflyEnvironment.GetGame().GetRoleplayManager().Init();
                        UserRoom.SendWhisperChat("Role play mis à jour");
                        break;
                    }
                case "modo":
                    {
                        ButterflyEnvironment.GetGame().GetModerationManager().Init();
                        UserRoom.SendWhisperChat("Moderation mis à jour");
                        break;
                    }
                case "catalogue":
                case "cata":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        ButterflyEnvironment.GetGame().GetCatalog().Init(ButterflyEnvironment.GetGame().GetItemManager());
                        ButterflyEnvironment.GetGame().GetClientManager().SendMessage(new CatalogUpdatedComposer());
                        UserRoom.SendWhisperChat("Catalogue mis à jour");
                        break;
                    }
                case "navigateur":
                case "navi":
                    {
                        ButterflyEnvironment.GetGame().GetNavigator().Init();
                        UserRoom.SendWhisperChat("Navigateur mis à jour");
                        break;
                    }
                case "filter":
                case "filtre":
                    {
                        ButterflyEnvironment.GetGame().GetChatManager().GetFilter().Init();
                        UserRoom.SendWhisperChat("Filtre mis à jour");
                        break;
                    }
                case "items":
                case "furni":
                    {
                        ButterflyEnvironment.GetGame().GetItemManager().Init();
                        UserRoom.SendWhisperChat("Items mis à jour");
                        break;
                    }
                case "model":
                    {
                        ButterflyEnvironment.GetGame().GetRoomManager().Init();
                        UserRoom.SendWhisperChat("Model mis à jour");
                        break;
                    }
                case "mutant":
                case "figure":
                    {
                        ButterflyEnvironment.GetFigureManager().Init();
                        UserRoom.SendWhisperChat("Mutant/Figure mises à jour");
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
