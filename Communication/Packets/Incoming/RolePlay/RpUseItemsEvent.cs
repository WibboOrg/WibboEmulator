namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Weapon;

internal sealed class RpUseItemsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var useCount = packet.PopInt();

        if (Session == null || Session.User == null)
        {
            return;
        }

        var room = Session.User.Room;
        if (room == null || !room.IsRoleplay)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (user == null)
        {
            return;
        }

        if (user.Freeze)
        {
            return;
        }

        var rp = user.Roleplayer;
        if (rp == null || rp.Dead || rp.SendPrison || rp.TradeId > 0)
        {
            return;
        }

        if (rp.AggroTimer > 0)
        {
            user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.useitem.notallowed", Session.Language), Math.Round((double)rp.AggroTimer / 2)));
            return;
        }

        var rpItem = RPItemManager.GetItem(itemId);
        if (rpItem == null)
        {
            return;
        }

        var rpItemInventory = rp.GetInventoryItem(itemId);
        if (rpItemInventory == null || rpItemInventory.Count <= 0 || rpItem.Type == "none")
        {
            return;
        }

        if (useCount <= 0 || rpItem.UseType != 2)
        {
            useCount = 1;
        }

        if (useCount > rpItemInventory.Count)
        {
            useCount = rpItemInventory.Count;
        }

        if (user.FreezeEndCounter <= 1)
        {
            user.Freeze = true;
            user.FreezeEndCounter = 1;
        }

        if (rpItem.Id == 75)
        {
            rp.AddInventoryItem(45, useCount);
        }

        switch (rpItem.Type)
        {
            case "openpage":
            {
                user.Client?.SendPacket(new InClientLinkComposer("habbopages/" + rpItem.Value));
                break;
            }
            case "openguide":
            {
                user.Client?.SendPacket(new InClientLinkComposer("habbopages/westworld/westworld"));
                break;
            }
            case "hit":
            {
                rp.Hit(user, rpItem.Value * useCount, room, false, true, false);
                rp.RemoveInventoryItem(rpItem.Id, useCount);
                break;
            }
            case "enable":
            {
                user.ApplyEffect(rpItem.Value);
                break;
            }
            case "showtime":
            {
                user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("rp.useitem.showtime", Session.Language), room.RoomRoleplay.Hour, room.RoomRoleplay.Minute));
                break;
            }
            case "money":
            {
                rp.Money += rpItem.Value * useCount;
                rp.RemoveInventoryItem(rpItem.Id, useCount);
                rp.SendUpdate();
                break;
            }
            case "munition":
            {
                rp.AddMunition(rpItem.Value * useCount);
                rp.RemoveInventoryItem(rpItem.Id, useCount);
                rp.SendUpdate();
                break;
            }
            case "energytired":
            {
                user.ApplyEffect(4, true);
                user.TimerResetEffect = 2;

                rp.AddEnergy(rpItem.Value * useCount);
                rp.Hit(user, rpItem.Value * useCount, room, false, true, false);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                var titleItem = char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..];
                user.OnChat(string.Format(LanguageManager.TryGetValue("rp.chat.consumes", Session.Language), titleItem));
                break;
            }
            case "healthtired":
            {
                user.ApplyEffect(4, true);
                user.TimerResetEffect = 2;

                rp.RemoveEnergy(rpItem.Value * useCount);
                rp.AddHealth(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                var titleItem = char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..];
                user.OnChat(string.Format(LanguageManager.TryGetValue("rp.chat.consumes", Session.Language), titleItem));
                break;
            }
            case "healthenergy":
            {
                user.ApplyEffect(4, true);
                user.TimerResetEffect = 2;

                rp.AddEnergy(rpItem.Value * useCount);
                rp.AddHealth(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                var titleItem = char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..];
                user.OnChat(string.Format(LanguageManager.TryGetValue("rp.chat.consumes", Session.Language), titleItem));
                break;
            }
            case "energy":
            {
                user.ApplyEffect(4, true);
                user.TimerResetEffect = 2;

                rp.AddEnergy(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                var titleItem = char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..];
                user.OnChat(string.Format(LanguageManager.TryGetValue("rp.chat.consumes", Session.Language), titleItem));
                break;
            }
            case "health":
            {
                user.ApplyEffect(737, true);
                user.TimerResetEffect = 4;

                rp.AddHealth(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                var titleItem = char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..];
                user.OnChat(string.Format(LanguageManager.TryGetValue("rp.chat.consumes", Session.Language), titleItem));
                break;
            }
            case "weapon_cac":
            {
                if (rp.WeaponCac.Id == rpItem.Value)
                {
                    break;
                }

                rp.WeaponCac = RPWeaponManager.GetWeaponCac(rpItem.Value);
                user.SendWhisperChat(LanguageManager.TryGetValue("rp.changearmecac", Session.Language));
                break;
            }
            case "weapon_far":
            {
                if (rp.WeaponGun.Id == rpItem.Value)
                {
                    break;
                }

                rp.WeaponGun = RPWeaponManager.GetWeaponGun(rpItem.Value);
                user.SendWhisperChat(LanguageManager.TryGetValue("rp.changearmefar", Session.Language));
                break;
            }
        }
    }
}
