using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
{
    internal class Invisible : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Session.GetHabbo().SpectatorMode)
            {
                Session.GetHabbo().SpectatorMode = false;
                Session.GetHabbo().HideInRoom = false; // Pas besoin d'être suivi, si on est en invisible ?

                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("invisible.disabled", Session.Langue));
                UserRoom.SendWhisperChat("Vous ne pouvez pas être suivit en mode invisible");
                //voir pour faire reload le staff uniquement
            }
            else
            {
                Session.GetHabbo().SpectatorMode = true;
                Session.GetHabbo().HideInRoom = true; // Retour à la normale des paramètres si nous sommes plus invisible ?

                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("invisible.enabled", Session.Langue));
                UserRoom.SendWhisperChat("Vous pouvez être de nouveau suivi car vous n'êtes plus invisible");
                //voir pour faire reload le staff uniquement

            }

        }
    }
}
