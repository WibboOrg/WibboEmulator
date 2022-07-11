using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Rooms;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class Give : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            Client TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetUser().HasFuse("fuse_give_credits"))
                        {
                            Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetUser().Credits += Amount;
                                TargetUser.SendPacket(new CreditBalanceComposer(TargetUser.GetUser().Credits));

                                if (TargetUser.GetUser().Id != Session.GetUser().Id)
                                    TargetUser.SendNotification(Session.GetUser().Username + " t'a donné  " + Amount.ToString() + " crédit(s)!");

                                Session.SendWhisper("Tu as donné " + Amount + " crédit(s) à " + TargetUser.GetUser().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Désolé, le montant n'est pas valide");
                                break;
                            }
                        }
                    }
                case "wibbopoints":
                case "wbpts":
                case "wp":
                    {
                        if (!Session.GetUser().HasFuse("fuse_give_wibbopoints")) //only Jason
                        {
                            Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetUser().WibboPoints += Amount;
                                TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.GetUser().WibboPoints, 0, 105));

                                if (TargetUser.GetUser().Id != Session.GetUser().Id)
                                    TargetUser.SendNotification(Session.GetUser().Username + " t'a donné " + Amount.ToString() + " WibboPoint(s)!");

                                Session.SendWhisper("Tu as donné " + Amount + " WibboPoint(s) à " + TargetUser.GetUser().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Désolé, le montant n'est pas valide");
                                break;
                            }
                        }
                    }

                case "limitcoins":
                case "ltc":
                    {
                        if (!Session.GetUser().HasFuse("fuse_give_limitcoins")) // only Jason
                        {
                            Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetUser().WibboPoints += Amount;
                                TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.GetUser().WibboPoints, 0, 55));

                                if (TargetUser.GetUser().Id != Session.GetUser().Id)
                                    TargetUser.SendNotification(Session.GetUser().Username + " t'a donné " + Amount.ToString() + " Limit'Coin(s)!");

                                Session.SendWhisper("Tu as donné " + Amount + " Limit'Coin(s) à " + TargetUser.GetUser().Username + "!");
                                break;
                            }
                            else
                            {
                                Session.SendWhisper("Désolé, le montant n'est pas valide");
                                break;
                            }
                        }
                    }

                default:
                    Session.SendWhisper("'" + UpdateVal + "' n'est pas une monnaie ! ");
                    break;
            }
        }
    }
}
