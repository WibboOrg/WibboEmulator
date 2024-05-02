namespace WibboEmulator.Games.Chats.Commands.User.Several;

using WibboEmulator.Database;
using WibboEmulator.Database.Daos.Bot;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class ConfigBot : IChatCommand
{
    public void Execute(GameClient session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length < 3)
        {
            return;
        }

        var bot = room.RoomUserManager.GetBotByName(parameters[1]);
        if (bot == null)
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

                _ = int.TryParse(parameters[3], out var intValue);

                if (!EffectManager.HasEffect(intValue, false))
                {
                    return;
                }

                if (bot.CurrentEffect != intValue)
                {
                    bot.ApplyEffect(intValue);
                }

                if (bot.BotData.Enable != intValue)
                {
                    bot.BotData.Enable = intValue;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateEnable(dbClient, bot.BotData.Id, intValue);
                }
                break;
            }
            case "handitem":
            {
                if (parameters.Length < 4)
                {
                    break;
                }

                _ = int.TryParse(parameters[3], out var intValue);

                if (bot.CarryItemID != intValue)
                {
                    bot.CarryItem(intValue, true);
                }

                if (bot.BotData.Handitem != intValue)
                {
                    bot.BotData.Handitem = intValue;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateHanditem(dbClient, bot.BotData.Id, intValue);
                }
                break;
            }
            case "rot":
            {
                if (parameters.Length < 4)
                {
                    break;
                }

                _ = int.TryParse(parameters[3], out var intValue);
                intValue = intValue is > 7 or < 0 ? 0 : intValue;

                if (bot.RotBody != intValue)
                {
                    bot.RotBody = intValue;
                    bot.RotHead = intValue;
                    bot.UpdateNeeded = true;
                }

                if (bot.BotData.Rot != intValue)
                {
                    bot.BotData.Rot = intValue;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateRotation(dbClient, bot.BotData.Id, bot.RotBody);
                }
                break;
            }
            case "sit":
            {
                if (bot.BotData.Status == 1)
                {
                    bot.BotData.Status = 0;

                    bot.RemoveStatus("sit");
                    bot.IsSit = false;
                    bot.UpdateNeeded = true;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateStatus(dbClient, bot.BotData.Id, 0);
                }
                else
                {
                    if (!bot.IsSit)
                    {
                        bot.SetStatus("sit", bot.IsPet ? "" : "0.5");
                        bot.IsSit = true;
                        bot.UpdateNeeded = true;
                    }

                    bot.BotData.Status = 1;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateStatus(dbClient, bot.BotData.Id, 1);
                }
                break;
            }
            case "lay":
            {
                if (bot.BotData.Status == 2)
                {
                    bot.BotData.Status = 0;

                    bot.RemoveStatus("lay");
                    bot.IsSit = false;
                    bot.UpdateNeeded = true;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateStatus(dbClient, bot.BotData.Id, 0);
                }
                else
                {
                    if (!bot.IsLay)
                    {
                        bot.SetStatus("lay", bot.IsPet ? "" : "0.7");
                        bot.IsLay = true;
                        bot.UpdateNeeded = true;
                    }

                    bot.BotData.Status = 2;

                    using var dbClient = DatabaseManager.Connection;
                    BotUserDao.UpdateStatus(dbClient, bot.BotData.Id, 2);
                }
                break;
            }
        }
    }
}
