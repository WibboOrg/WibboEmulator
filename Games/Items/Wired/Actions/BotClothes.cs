namespace WibboEmulator.Games.Items.Wired.Actions;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using System.Data;
using WibboEmulator.Games.Items.Wired.Bases;
using WibboEmulator.Games.Items.Wired.Interfaces;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Utilities;

public class BotClothes(Item item, Room room) : WiredActionBase(item, room, (int)WiredActionType.BOT_CHANGE_FIGURE), IWired, IWiredEffect, IWiredCycleable
{
    private readonly Dictionary<string, string> _updatedBots = [];

    public override bool OnCycle(RoomUser user, Item item)
    {
        if (string.IsNullOrWhiteSpace(this.StringParam) || !this.StringParam.Contains('\t'))
        {
            return false;
        }

        var nameAndLook = this.StringParam.Split('\t');
        var botName = (nameAndLook.Length == 2) ? nameAndLook[0] : "";
        var botLook = (nameAndLook.Length == 2) ? nameAndLook[1] : "";

        if (string.IsNullOrEmpty(botName) || string.IsNullOrEmpty(botLook))
        {
            return false;
        }

        this._updatedBots.Remove(botName);
        this._updatedBots.TryAdd(botName, botLook);

        this.OnUpdate();

        return false;
    }

    public void OnUpdate()
    {
        if (this._updatedBots.Count > 0)
        {
            var packets = new ServerPacketList();
            foreach (var updateBot in this._updatedBots)
            {
                var botName = updateBot.Key;
                var botLook = updateBot.Value;

                var bot = this.Room.RoomUserManager.GetBotOrPetByName(botName);
                if (bot == null)
                {
                    continue;
                }

                bot.BotData.Look = botLook;

                packets.Add(new UserChangeComposer(bot));
            }
            this._updatedBots.Clear();

            this.Room.SendPackets(packets);
        }
    }

    public void SaveToDatabase(IDbConnection dbClient) => WiredUtillity.SaveInDatabase(dbClient, this.Id, string.Empty, this.StringParam, false, null, this.Delay);

    public void LoadFromDatabase(string wiredTriggerData, string wiredTriggerData2, string wiredTriggersItem, bool wiredAllUserTriggerable, int wiredDelay)
    {
        this.Delay = wiredDelay;

        this.StringParam = wiredTriggerData;
    }
}
