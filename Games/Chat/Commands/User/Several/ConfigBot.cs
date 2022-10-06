namespace WibboEmulator.Games.Chat.Commands.User.Several;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal class ConfigBot : IChatCommand
{
    public void Execute(GameClient session, Room Room, RoomUser UserRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var Bot = Room.GetRoomUserManager().GetBotByName(parameters[1]);
        if (Bot == null)
        {
            return;
        }

        switch (parameters[2])
        {
            case "enable":
            {
                if (parameters.Length < 4)
                {
                    break;
                }

                int.TryParse(parameters[3], out var IntValue);

                if (!WibboEnvironment.GetGame().GetEffectManager().HaveEffect(IntValue, false))
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

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateEnable(dbClient, Bot.BotData.Id, IntValue);
                }
                break;
            }
            case "handitem":
            {
                if (parameters.Length < 4)
                {
                    break;
                }

                int.TryParse(parameters[3], out var IntValue);

                if (Bot.CarryItemID != IntValue)
                {
                    Bot.CarryItem(IntValue, true);
                }

                if (Bot.BotData.Handitem != IntValue)
                {
                    Bot.BotData.Handitem = IntValue;

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateHanditem(dbClient, Bot.BotData.Id, IntValue);
                }
                break;
            }
            case "rot":
            {
                if (parameters.Length < 4)
                {
                    break;
                }

                int.TryParse(parameters[3], out var IntValue);
                IntValue = IntValue is > 7 or < 0 ? 0 : IntValue;

                if (Bot.RotBody != IntValue)
                {
                    Bot.RotBody = IntValue;
                    Bot.RotHead = IntValue;
                    Bot.UpdateNeeded = true;
                }

                if (Bot.BotData.Rot != IntValue)
                {
                    Bot.BotData.Rot = IntValue;

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
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

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateStatus0(dbClient, Bot.BotData.Id);
                }
                else
                {
                    if (!Bot.IsSit)
                    {
                        Bot.SetStatus("sit", Bot.IsPet ? "" : "0.5");
                        Bot.IsSit = true;
                        Bot.UpdateNeeded = true;
                    }

                    Bot.BotData.Status = 1;

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateStatus1(dbClient, Bot.BotData.Id);
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

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateStatus0(dbClient, Bot.BotData.Id);
                }
                else
                {
                    if (!Bot.IsLay)
                    {
                        Bot.SetStatus("lay", Bot.IsPet ? "" : "0.7");
                        Bot.IsLay = true;
                        Bot.UpdateNeeded = true;
                    }

                    Bot.BotData.Status = 2;

                    using var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor();
                    BotUserDao.UpdateStatus2(dbClient, Bot.BotData.Id);
                }
                break;
            }
        }
    }
}
