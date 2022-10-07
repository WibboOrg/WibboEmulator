namespace WibboEmulator.Games.Chat.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class Give : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var targetUser = WibboEnvironment.GetGame().GetGameClientManager().GetClientByUsername(parameters[1]);

        var updateVal = parameters[2];
        switch (updateVal.ToLower())
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
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.GetUser().Credits += amount;
                        targetUser.SendPacket(new CreditBalanceComposer(targetUser.GetUser().Credits));

                        if (targetUser.GetUser().Id != session.GetUser().Id)
                        {
                            targetUser.SendNotification(session.GetUser().Username + " t'a donné  " + amount.ToString() + " crédit(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " crédit(s) à " + targetUser.GetUser().Username + "!");
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
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.GetUser().WibboPoints += amount;
                        targetUser.SendPacket(new ActivityPointNotificationComposer(targetUser.GetUser().WibboPoints, 0, 105));

                        if (targetUser.GetUser().Id != session.GetUser().Id)
                        {
                            targetUser.SendNotification(session.GetUser().Username + " t'a donné " + amount.ToString() + " WibboPoint(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " WibboPoint(s) à " + targetUser.GetUser().Username + "!");
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
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.GetUser().LimitCoins += amount;
                        targetUser.SendPacket(new ActivityPointNotificationComposer(targetUser.GetUser().LimitCoins, 0, 55));

                        if (targetUser.GetUser().Id != session.GetUser().Id)
                        {
                            targetUser.SendNotification(session.GetUser().Username + " t'a donné " + amount.ToString() + " Limit'Coin(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " Limit'Coin(s) à " + targetUser.GetUser().Username + "!");
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
                session.SendWhisper("'" + updateVal + "' n'est pas une monnaie ! ");
                break;
        }
    }
}
