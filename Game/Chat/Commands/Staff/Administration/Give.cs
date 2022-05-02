using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Chat.Commands.Cmd
{
    internal class Give : IChatCommand
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
                    /// -> UPDATE `emulator_command` SET `input`='give' WHERE  `id`=204;
                    /// UPDATE `emulator_command` SET `description_fr`='Donne des crédits, des wibbopoints ou encore des limitcoins à des joueurs en récompense !' WHERE  `id`=204;
                    /// INSERT INTO `emulator_fuseright` (`id`, `rank`, `fuse`) VALUES ('29', '13', 'fuse_give_limitcoins');
                    {
                        if (!Session.GetUser().HasFuse("fuse_give_limitcoins")) //only Jason
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
                        if (!Session.GetUser().HasFuse("fuse_give_limitecoins")) // only Jason
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
