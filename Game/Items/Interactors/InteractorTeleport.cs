using Wibbo.Communication.Packets.Outgoing.Rooms.Session;
using Wibbo.Game.Clients;
using Wibbo.Game.Rooms;

namespace Wibbo.Game.Items.Interactors
{
    public class InteractorTeleport : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
                if (roomUserByUserId != null)
                {
                    roomUserByUserId.AllowOverride = false;
                    roomUserByUserId.CanWalk = true;
                }
                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 == 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTwo = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.AllowOverride = false;
                roomUserByUserIdTwo.CanWalk = true;
            }

            Item.InteractingUser2 = 0;
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser);
                if (roomUserByUserId != null)
                {
                    roomUserByUserId.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 == 0)
            {
                return;
            }

            RoomUser roomUserByUserIdTwo = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Item.InteractingUser2);
            if (roomUserByUserIdTwo != null)
            {
                roomUserByUserIdTwo.UnlockWalking();
            }

            Item.InteractingUser2 = 0;
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (Item == null || Item.GetRoom() == null || (Session == null || Session.GetUser() == null))
            {
                return;
            }

            RoomUser roomUserByUserId = Item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (roomUserByUserId == null)
            {
                return;
            }

            if (roomUserByUserId.Coordinate == Item.Coordinate || roomUserByUserId.Coordinate == Item.SquareInFront)
            {
                if (Item.InteractingUser != 0)
                {
                    return;
                }

                Item.InteractingUser = roomUserByUserId.GetClient().GetUser().Id;
                Item.ReqUpdate(2);
            }
            else
            {
                if (!roomUserByUserId.CanWalk)
                {
                    return;
                }

                roomUserByUserId.MoveTo(Item.SquareInFront);
            }
        }

        public override void OnTick(Item item)
        {
            bool keepDoorOpen = false;
            bool showTeleEffect = false;

            if (item.InteractingUser > 0)
            {
                RoomUser roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser);
                if (roomUserTarget != null)
                {
                    if (roomUserTarget.Coordinate == item.Coordinate)
                    {
                        roomUserTarget.AllowOverride = false;
                        if (ItemTeleporterFinder.IsTeleLinked(item.Id, item.GetRoom()))
                        {
                            showTeleEffect = true;
                            int linkedTele = ItemTeleporterFinder.GetLinkedTele(item.Id);
                            int teleRoomId = ItemTeleporterFinder.GetTeleRoomId(linkedTele, item.GetRoom());
                            if (teleRoomId == item.RoomId)
                            {
                                Item roomItem = item.GetRoom().GetRoomItemHandler().GetItem(linkedTele);
                                if (roomItem == null)
                                {
                                    roomUserTarget.UnlockWalking();
                                }
                                else
                                {
                                    roomUserTarget.SetRot(roomItem.Rotation, false);
                                    roomItem.GetRoom().GetGameMap().TeleportToItem(roomUserTarget, roomItem);
                                    roomItem.ExtraData = "2";
                                    roomItem.UpdateState(false, true);
                                    roomItem.InteractingUser2 = item.InteractingUser;
                                    roomItem.ReqUpdate(2);
                                }
                            }
                            else if (!roomUserTarget.IsBot && roomUserTarget != null && (roomUserTarget.GetClient() != null && roomUserTarget.GetClient().GetUser() != null))
                            {
                                roomUserTarget.GetClient().GetUser().IsTeleporting = true;
                                roomUserTarget.GetClient().GetUser().TeleportingRoomID = teleRoomId;
                                roomUserTarget.GetClient().GetUser().TeleporterId = linkedTele;
                                roomUserTarget.GetClient().SendPacket(new RoomForwardComposer(teleRoomId));
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
                RoomUser roomUserTarget = item.GetRoom().GetRoomUserManager().GetRoomUserByUserId(item.InteractingUser2);
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
                    item.UpdateState(false, true);
                }
            }
            else if (showTeleEffect)
            {
                if (item.ExtraData != "2")
                {
                    item.ExtraData = "2";
                    item.UpdateState(false, true);
                }
            }
            else if (item.ExtraData != "0")
            {
                item.ExtraData = "0";
                item.UpdateState(false, true);
            }
        }
    }
}
