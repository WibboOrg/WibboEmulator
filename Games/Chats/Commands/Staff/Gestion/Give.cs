namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Give : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        var targetUser = GameClientManager.GetClientByUsername(parameters[1]);

        var updateVal = parameters[2];
        switch (updateVal.ToLower())
        {
            case "coins":
            case "credits":
            {
                if (!session.User.HasPermission("give_credits"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.User.Credits += amount;
                        targetUser.SendPacket(new CreditBalanceComposer(targetUser.User.Credits));

                        if (targetUser.User.Id != session.User.Id)
                        {
                            targetUser.SendNotification(session.User.Username + " t'a donné  " + amount.ToString() + " crédit(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " crédit(s) à " + targetUser.User.Username + "!");
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
                if (!session.User.HasPermission("give_wibbopoints"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.User.WibboPoints += amount;
                        targetUser.SendPacket(new ActivityPointNotificationComposer(targetUser.User.WibboPoints, 0, 105));

                        if (targetUser.User.Id != session.User.Id)
                        {
                            targetUser.SendNotification(session.User.Username + " t'a donné " + amount.ToString() + " WibboPoint(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " WibboPoint(s) à " + targetUser.User.Username + "!");
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
                if (!session.User.HasPermission("give_limitcoins"))
                {
                    session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        targetUser.User.LimitCoins += amount;
                        targetUser.SendPacket(new ActivityPointNotificationComposer(targetUser.User.LimitCoins, 0, 55));

                        if (targetUser.User.Id != session.User.Id)
                        {
                            targetUser.SendNotification(session.User.Username + " t'a donné " + amount.ToString() + " Limit'Coin(s)!");
                        }

                        session.SendWhisper("Tu as donné " + amount + " Limit'Coin(s) à " + targetUser.User.Username + "!");
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
