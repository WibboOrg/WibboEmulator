namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Session;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms.Map;

public class InteractorTeleport : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
        item.ExtraData = "0";

        if (item.InteractingUser != 0)
        {
            var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            if (roomUserByUserId != null)
            {
                roomUserByUserId.AllowOverride = false;
                roomUserByUserId.CanWalk = true;
            }
            item.InteractingUser = 0;
        }

        if (item.InteractingUser2 == 0)
        {
            return;
        }

        var roomUserByUserIdTwo = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser2);
        if (roomUserByUserIdTwo != null)
        {
            roomUserByUserIdTwo.AllowOverride = false;
            roomUserByUserIdTwo.CanWalk = true;
        }

        item.InteractingUser2 = 0;
    }

    public override void OnRemove(GameClient session, Item item)
    {
        item.ExtraData = "0";

        if (item.InteractingUser != 0)
        {
            var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            roomUserByUserId?.UnlockWalking();

            item.InteractingUser = 0;
        }

        if (item.InteractingUser2 == 0)
        {
            return;
        }

        var roomUserByUserIdTwo = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser2);
        roomUserByUserIdTwo?.UnlockWalking();

        item.InteractingUser2 = 0;
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (item == null || item.GetRoom() == null || session == null || session.User == null)
        {
            return;
        }

        var roomUserByUserId = item.GetRoom().RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUserByUserId == null)
        {
            return;
        }

        if (roomUserByUserId.Coordinate == item.Coordinate || roomUserByUserId.Coordinate == item.SquareInFront)
        {
            if (item.InteractingUser != 0)
            {
                return;
            }

            item.InteractingUser = roomUserByUserId.Client.User.Id;
            item.ReqUpdate(2);
        }
        else
        {
            if (!roomUserByUserId.CanWalk)
            {
                return;
            }

            roomUserByUserId.MoveTo(item.SquareInFront);
        }
    }

    public override void OnTick(Item item)
    {
        var keepDoorOpen = false;
        var showTeleEffect = false;

        if (item.InteractingUser > 0)
        {
            var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser);
            if (roomUserTarget != null)
            {
                if (roomUserTarget.Coordinate == item.Coordinate)
                {
                    roomUserTarget.AllowOverride = false;
                    if (ItemTeleporterFinder.IsTeleLinked(item.Id, item.GetRoom()))
                    {
                        showTeleEffect = true;
                        var linkedTele = ItemTeleporterFinder.GetLinkedTele(item.Id);
                        var teleRoomId = ItemTeleporterFinder.GetTeleRoomId(linkedTele, item.GetRoom());
                        if (teleRoomId == item.RoomId)
                        {
                            var roomItem = item.GetRoom().RoomItemHandling.GetItem(linkedTele);
                            if (roomItem == null)
                            {
                                roomUserTarget.UnlockWalking();
                            }
                            else
                            {
                                roomUserTarget.SetRot(roomItem.Rotation, false);
                                GameMap.TeleportToItem(roomUserTarget, roomItem);
                                roomItem.ExtraData = "2";
                                roomItem.UpdateState(false);
                                roomItem.InteractingUser2 = item.InteractingUser;
                                roomItem.ReqUpdate(2);
                            }
                        }
                        else if (!roomUserTarget.IsBot && roomUserTarget != null && roomUserTarget.Client != null && roomUserTarget.Client.User != null)
                        {
                            roomUserTarget.Room.RoomUserManager.RemoveUserFromRoom(roomUserTarget.Client, true, false);

                            roomUserTarget.Client.User.IsTeleporting = true;
                            roomUserTarget.Client.User.TeleportingRoomID = teleRoomId;
                            roomUserTarget.Client.User.TeleporterId = linkedTele;
                            roomUserTarget.Client.SendPacket(new RoomForwardComposer(teleRoomId));
                        }
                        item.InteractingUser = 0;
                    }
                    else
                    {
                        roomUserTarget.UnlockWalking();
                        item.InteractingUser = 0;
                    }
                }
                else if (roomUserTarget.Coordinate == item.SquareInFront)
                {
                    roomUserTarget.AllowOverride = true;
                    keepDoorOpen = true;

                    roomUserTarget.CanWalk = false;
                    roomUserTarget.AllowOverride = true;
                    roomUserTarget.MoveTo(item.Coordinate.X, item.Coordinate.Y, true);
                }
                else
                {
                    item.InteractingUser = 0;
                }
            }
            else
            {
                item.InteractingUser = 0;
            }

            item.UpdateCounter = 1;
        }

        if (item.InteractingUser2 > 0)
        {
            var roomUserTarget = item.GetRoom().RoomUserManager.GetRoomUserByUserId(item.InteractingUser2);
            if (roomUserTarget != null)
            {
                keepDoorOpen = true;
                roomUserTarget.UnlockWalking();
                roomUserTarget.MoveTo(item.SquareInFront);
            }
            item.UpdateCounter = 1;
            item.InteractingUser2 = 0;
        }

        if (keepDoorOpen)
        {
            if (item.ExtraData != "1")
            {
                item.ExtraData = "1";
                item.UpdateState(false);
            }
        }
        else if (showTeleEffect)
        {
            if (item.ExtraData != "2")
            {
                item.ExtraData = "2";
                item.UpdateState(false);
            }
        }
        else if (item.ExtraData != "0")
        {
            item.ExtraData = "0";
            item.UpdateState(false);
        }
    }
}
