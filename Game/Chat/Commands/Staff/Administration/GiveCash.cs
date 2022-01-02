using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class GiveCash : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetHabbo().HasFuse("fuse_give_credits"))
                        {
                            UserRoom.SendWhisperChat("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetHabbo().Credits += Amount;
                                TargetUser.SendPacket(new CreditBalanceComposer(TargetUser.GetHabbo().Credits));

                                if (TargetUser.GetHabbo().Id != Session.GetHabbo().Id)
                                    TargetUser.SendNotification(Session.GetHabbo().Username + " t'a donné  " + Amount.ToString() + " crédit(s)!");

                                UserRoom.SendWhisperChat("Tu as donné " + Amount + " crédit(s) à " + TargetUser.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                UserRoom.SendWhisperChat("Désolé, le montant n'est pas valide");
                                break;
                            }
                        }
                    }
                case "wibbopoints":
                case "wbpts":
                case "wp":
                    {
                        if (!Session.GetHabbo().HasFuse("fuse_give_wibbopoints"))
                        {
                            UserRoom.SendWhisperChat("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetHabbo().WibboPoints += Amount;
                                TargetUser.SendPacket(new HabboActivityPointNotificationComposer(TargetUser.GetHabbo().WibboPoints, 0, 105));

                                if (TargetUser.GetHabbo().Id != Session.GetHabbo().Id)
                                    TargetUser.SendNotification(Session.GetHabbo().Username + " t'a donné " + Amount.ToString() + " WibboPoint(s)!");

                                UserRoom.SendWhisperChat("Tu as donné " + Amount + " WibboPoint(s) à " + TargetUser.GetHabbo().Username + "!");
                                break;
                            }
                            else
                            {
                                UserRoom.SendWhisperChat("Désolé, le montant n'est pas valide");
                                break;
                            }
                        }
                    }
                default:
                    UserRoom.SendWhisperChat("'" + UpdateVal + "' n'est pas une monnaie ! ");
                    break;
            }
        }
    }
}
