using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Games.Chat.Commands.Cmd
{
    internal class Give : IChatCommand
    {
        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            GameClient TargetUser = WibboEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);

            string UpdateVal = Params[2];
            switch (UpdateVal.ToLower())
            {
                case "coins":
                case "credits":
                    {
                        if (!Session.GetUser().HasPermission("perm_give_credits"))
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
                        if (!Session.GetUser().HasPermission("perm_give_wibbopoints"))
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
                        if (!Session.GetUser().HasPermission("perm_give_limitcoins"))
                        {
                            Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                            break;
                        }
                        else
                        {
                            int Amount;
                            if (int.TryParse(Params[3], out Amount))
                            {
                                TargetUser.GetUser().LimitCoins += Amount;
                                TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.GetUser().LimitCoins, 0, 55));

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
