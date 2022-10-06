namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Give : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        var TargetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);

        var UpdateVal = parameters[2];
        switch (UpdateVal.ToLower())
        {
            case "coins":
            case "credits":
            {
                if (!session.GetUser().HasPermission("perm_give_credits"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var Amount))
                    {
                        TargetUser.GetUser().Credits += Amount;
                        TargetUser.SendPacket(new CreditBalanceComposer(TargetUser.GetUser().Credits));

                        if (TargetUser.GetUser().Id != session.GetUser().Id)
                        {
                            TargetUser.SendNotification(session.GetUser().Username + " t'a donné  " + Amount.ToString() + " crédit(s)!");
                        }

                        session.SendWhisper("Tu as donné " + Amount + " crédit(s) à " + TargetUser.GetUser().Username + "!");
                        break;
                    }
                    else
                    {
                        session.SendWhisper("Désolé, le montant n'est pas valide");
                        break;
                    }
                }
            }
            case "wibbopoints":
            case "wbpts":
            case "wp":
            {
                if (!session.GetUser().HasPermission("perm_give_wibbopoints"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var Amount))
                    {
                        TargetUser.GetUser().WibboPoints += Amount;
                        TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.GetUser().WibboPoints, 0, 105));

                        if (TargetUser.GetUser().Id != session.GetUser().Id)
                        {
                            TargetUser.SendNotification(session.GetUser().Username + " t'a donné " + Amount.ToString() + " WibboPoint(s)!");
                        }

                        session.SendWhisper("Tu as donné " + Amount + " WibboPoint(s) à " + TargetUser.GetUser().Username + "!");
                        break;
                    }
                    else
                    {
                        session.SendWhisper("Désolé, le montant n'est pas valide");
                        break;
                    }
                }
            }

            case "limitcoins":
            case "ltc":
            {
                if (!session.GetUser().HasPermission("perm_give_limitcoins"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var Amount))
                    {
                        TargetUser.GetUser().LimitCoins += Amount;
                        TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.GetUser().LimitCoins, 0, 55));

                        if (TargetUser.GetUser().Id != session.GetUser().Id)
                        {
                            TargetUser.SendNotification(session.GetUser().Username + " t'a donné " + Amount.ToString() + " Limit'Coin(s)!");
                        }

                        session.SendWhisper("Tu as donné " + Amount + " Limit'Coin(s) à " + TargetUser.GetUser().Username + "!");
                        break;
                    }
                    else
                    {
                        session.SendWhisper("Désolé, le montant n'est pas valide");
                        break;
                    }
                }
            }

            default:
                session.SendWhisper("'" + UpdateVal + "' n'est pas une monnaie ! ");
                break;
        }
    }
}
