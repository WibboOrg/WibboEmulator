using Butterfly.Communication.Packets.Outgoing.Structure;
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
