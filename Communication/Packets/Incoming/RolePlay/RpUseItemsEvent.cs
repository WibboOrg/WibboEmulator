namespace WibboEmulator.Communication.Packets.Incoming.RolePlay;
using WibboEmulator.Communication.Packets.Outgoing.Notifications;
using WibboEmulator.Games.GameClients;

internal class RpUseItemsEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        var itemId = packet.PopInt();
        var useCount = packet.PopInt();

        if (session == null || session.User == null)
        {
            return;
        }

        var room = session.User.CurrentRoom;
        if (room == null || !room.IsRoleplay)
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
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
            user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("rp.useitem.notallowed", session.Langue), Math.Round((double)rp.AggroTimer / 2)));
            return;
        }

        var rpItem = WibboEnvironment.GetGame().GetRoleplayManager().ItemManager.GetItem(itemId);
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
                user.Client?.SendPacket(new InClientLinkComposer("habbopages/roleplay/" + rpItem.Value));
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
                user.SendWhisperChat("Il est " + room.RoomRoleplay.Hour + " heures et " + room.RoomRoleplay.Minute + " minutes");
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

                user.OnChat("*Consomme " + char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..] + "*");
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

                user.OnChat("*Consomme " + char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..] + "*");
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

                user.OnChat("*Consomme " + char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..] + "*");
                break;
            }
            case "energy":
            {
                user.ApplyEffect(4, true);
                user.TimerResetEffect = 2;

                rp.AddEnergy(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                user.OnChat("*Consomme " + char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..] + "*");
                break;
            }
            case "health":
            {
                user.ApplyEffect(737, true);
                user.TimerResetEffect = 4;

                rp.AddHealth(rpItem.Value * useCount);
                rp.SendUpdate();
                rp.RemoveInventoryItem(rpItem.Id, useCount);

                user.OnChat("*Consomme " + char.ToLowerInvariant(rpItem.Title[0]) + rpItem.Title[1..] + "*");
                break;
            }
            case "weapon_cac":
            {
                if (rp.WeaponCac.Id == rpItem.Value)
                {
                    break;
                }

                rp.WeaponCac = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponCac(rpItem.Value);
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.changearmecac", session.Langue));
                break;
            }
            case "weapon_far":
            {
                if (rp.WeaponGun.Id == rpItem.Value)
                {
                    break;
                }

                rp.WeaponGun = WibboEnvironment.GetGame().GetRoleplayManager().WeaponManager.GetWeaponGun(rpItem.Value);
                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("rp.changearmefar", session.Langue));
                break;
            }
        }
    }
}
