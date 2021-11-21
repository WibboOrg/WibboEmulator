using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat.Commands.Cmd
{
    internal class ConfigBot : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            if (Params.Length < 3)
            {
                return;
            }

            RoomUser Bot = Room.GetRoomUserManager().GetBotByName(Params[1]);
            if (Bot == null)
            {
                return;
            }

            switch (Params[2])
            {
                case "enable":
                    {
                        if (Params.Length < 4)
                        {
                            break;
                        }

                        int.TryParse(Params[3], out int IntValue);

                        if (!ButterflyEnvironment.GetGame().GetEffectManager().HaveEffect(IntValue, false))
                        {
                            return;
                        }

                        if (Bot.CurrentEffect != IntValue)
                        {
                            Bot.ApplyEffect(IntValue);
                        }

                        if (Bot.BotData.Enable != IntValue)
                        {
                            Bot.BotData.Enable = IntValue;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                BotUserDao.UpdateEnable(dbClient, Bot.BotData.Id, IntValue);
                        }
                        break;
                    }
                case "handitem":
                    {
                        if (Params.Length < 4)
                        {
                            break;
                        }

                        int.TryParse(Params[3], out int IntValue);

                        if (Bot.CarryItemID != IntValue)
                        {
                            Bot.CarryItem(IntValue, true);
                        }

                        if (Bot.BotData.Handitem != IntValue)
                        {
                            Bot.BotData.Handitem = IntValue;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                BotUserDao.UpdateHanditem(dbClient, Bot.BotData.Id, IntValue);
                        }
                        break;
                    }
                case "rot":
                    {
                        if (Params.Length < 4)
                        {
                            break;
                        }

                        int.TryParse(Params[3], out int IntValue);
                        IntValue = (IntValue > 7 || IntValue < 0) ? 0 : IntValue;

                        if (Bot.RotBody != IntValue)
                        {
                            Bot.RotBody = IntValue;
                            Bot.RotHead = IntValue;
                            Bot.UpdateNeeded = true;
                        }

                        if (Bot.BotData.Rot != IntValue)
                        {
                            Bot.BotData.Rot = IntValue;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                BotUserDao.UpdateRotation(dbClient, Bot.BotData.Id, Bot.RotBody);
                        }
                        break;
                    }
                case "sit":
                    {
                        if (Bot.BotData.Status == 1)
                        {
                            Bot.BotData.Status = 0;

                            Bot.RemoveStatus("sit");
                            Bot.IsSit = false;
                            Bot.UpdateNeeded = true;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                BotUserDao.UpdateStatus0(dbClient, Bot.BotData.Id);
                            }
                        }
                        else
                        {
                            if (!Bot.IsSit)
                            {
                                Bot.SetStatus("sit", (Bot.IsPet) ? "" : "0.5");
                                Bot.IsSit = true;
                                Bot.UpdateNeeded = true;
                            }

                            Bot.BotData.Status = 1;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                BotUserDao.UpdateStatus1(dbClient, Bot.BotData.Id);
                            }
                        }
                        break;
                    }
                case "lay":
                    {
                        if (Bot.BotData.Status == 2)
                        {
                            Bot.BotData.Status = 0;

                            Bot.RemoveStatus("lay");
                            Bot.IsSit = false;
                            Bot.UpdateNeeded = true;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                            {
                                BotUserDao.UpdateStatus0(dbClient, Bot.BotData.Id);
                            }
                        }
                        else
                        {
                            if (!Bot.IsLay)
                            {
                                Bot.SetStatus("lay", (Bot.IsPet) ? "" : "0.7");
                                Bot.IsLay = true;
                                Bot.UpdateNeeded = true;
                            }

                            Bot.BotData.Status = 2;

                            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                                BotUserDao.UpdateStatus2(dbClient, Bot.BotData.Id);
                        }
                        break;
                    }
            }
        }
    }
}
