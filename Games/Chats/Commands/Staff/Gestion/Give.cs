namespace WibboEmulator.Games.Chats.Commands.Staff.Gestion;
using WibboEmulator.Communication.Packets.Outgoing.Inventory.Purse;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class Give : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        var TargetUser = GameClientManager.GetClientByUsername(parameters[1]);

        var updateVal = parameters[2];
        switch (updateVal.ToLower())
        {
            case "coins":
            case "credits":
            {
                if (!Session.User.HasPermission("give_credits"))
                {
                    Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        TargetUser.User.Credits += amount;
                        TargetUser.SendPacket(new CreditBalanceComposer(TargetUser.User.Credits));

                        if (TargetUser.User.Id != Session.User.Id)
                        {
                            TargetUser.SendNotification(Session.User.Username + " t'a donné  " + amount.ToString() + " crédit(s)!");
                        }

                        Session.SendWhisper("Tu as donné " + amount + " crédit(s) à " + TargetUser.User.Username + "!");
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
                if (!Session.User.HasPermission("give_wibbopoints"))
                {
                    Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        TargetUser.User.WibboPoints += amount;
                        TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.User.WibboPoints, 0, 105));

                        if (TargetUser.User.Id != Session.User.Id)
                        {
                            TargetUser.SendNotification(Session.User.Username + " t'a donné " + amount.ToString() + " WibboPoint(s)!");
                        }

                        Session.SendWhisper("Tu as donné " + amount + " WibboPoint(s) à " + TargetUser.User.Username + "!");
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
                if (!Session.User.HasPermission("give_limitcoins"))
                {
                    Session.SendWhisper("Désolé, vous n'avez pas la permission...");
                    break;
                }
                else
                {
                    if (int.TryParse(parameters[3], out var amount))
                    {
                        TargetUser.User.LimitCoins += amount;
                        TargetUser.SendPacket(new ActivityPointNotificationComposer(TargetUser.User.LimitCoins, 0, 55));

                        if (TargetUser.User.Id != Session.User.Id)
                        {
                            TargetUser.SendNotification(Session.User.Username + " t'a donné " + amount.ToString() + " Limit'Coin(s)!");
                        }

                        Session.SendWhisper("Tu as donné " + amount + " Limit'Coin(s) à " + TargetUser.User.Username + "!");
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
                Session.SendWhisper("'" + updateVal + "' n'est pas une monnaie ! ");
                break;
        }
    }
}
