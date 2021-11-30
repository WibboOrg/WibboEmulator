using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;

namespace Butterfly.Game.Items.Interactors
{
    public class InteractorTeleport : FurniInteractor
    {
        public override void OnPlace(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.AllowOverride = false;
                    roomUserByHabbo.CanWalk = true;
                }
                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 == 0)
            {
                return;
            }

            RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
            if (roomUserByHabbo1 != null)
            {
                roomUserByHabbo1.AllowOverride = false;
                roomUserByHabbo1.CanWalk = true;
            }

            Item.InteractingUser2 = 0;
        }

        public override void OnRemove(Client Session, Item Item)
        {
            Item.ExtraData = "0";

            if (Item.InteractingUser != 0)
            {
                RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser);
                if (roomUserByHabbo != null)
                {
                    roomUserByHabbo.UnlockWalking();
                }

                Item.InteractingUser = 0;
            }

            if (Item.InteractingUser2 == 0)
            {
                return;
            }

            RoomUser roomUserByHabbo1 = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Item.InteractingUser2);
            if (roomUserByHabbo1 != null)
            {
                roomUserByHabbo1.UnlockWalking();
            }

            Item.InteractingUser2 = 0;
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights)
        {
            if (Item == null || Item.GetRoom() == null || (Session == null || Session.GetHabbo() == null))
            {
                return;
            }

            RoomUser roomUserByHabbo = Item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (roomUserByHabbo == null)
            {
                return;
            }

            if (roomUserByHabbo.Coordinate == Item.Coordinate || roomUserByHabbo.Coordinate == Item.SquareInFront)
            {
                if (Item.InteractingUser != 0)
                {
                    return;
                }

                Item.InteractingUser = roomUserByHabbo.GetClient().GetHabbo().Id;
                Item.ReqUpdate(2);
            }
            else
            {
                if (!roomUserByHabbo.CanWalk)
                {
                    return;
                }

                roomUserByHabbo.MoveTo(Item.SquareInFront);
            }
        }

        public override void OnTick(Item item)
        {
            bool keepDoorOpen = false;
            bool showTeleEffect = false;

            if (item.InteractingUser > 0)
            {
                RoomUser roomUserByHabbo2 = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser);
                if (roomUserByHabbo2 != null)
                {
                    if (roomUserByHabbo2.Coordinate == item.Coordinate)
                    {
                        roomUserByHabbo2.AllowOverride = false;
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
                                    roomUserByHabbo2.UnlockWalking();
                                }
                                else
                                {
                                    roomUserByHabbo2.SetRot(roomItem.Rotation, false);
                                    roomItem.GetRoom().GetGameMap().TeleportToItem(roomUserByHabbo2, roomItem);
                                    roomItem.ExtraData = "2";
                                    roomItem.UpdateState(false, true);
                                    roomItem.InteractingUser2 = item.InteractingUser;
                                    roomItem.ReqUpdate(2);
                                }
                            }
                            else if (!roomUserByHabbo2.IsBot && roomUserByHabbo2 != null && (roomUserByHabbo2.GetClient() != null && roomUserByHabbo2.GetClient().GetHabbo() != null))
                            {
                                roomUserByHabbo2.GetClient().GetHabbo().IsTeleporting = true;
                                roomUserByHabbo2.GetClient().GetHabbo().TeleportingRoomID = teleRoomId;
                                roomUserByHabbo2.GetClient().GetHabbo().TeleporterId = linkedTele;
                                roomUserByHabbo2.GetClient().GetHabbo().PrepareRoom(teleRoomId, "");
                            }
                            item.InteractingUser = 0;
                        }
                        else
                        {
                            roomUserByHabbo2.UnlockWalking();
                            item.InteractingUser = 0;
                        }
                    }
                    else if (roomUserByHabbo2.Coordinate == item.SquareInFront)
                    {
                        roomUserByHabbo2.AllowOverride = true;
                        keepDoorOpen = true;

                        roomUserByHabbo2.CanWalk = false;
                        roomUserByHabbo2.AllowOverride = true;
                        roomUserByHabbo2.MoveTo(item.Coordinate.X, item.Coordinate.Y, true);
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
                RoomUser roomUserByHabbo2 = item.GetRoom().GetRoomUserManager().GetRoomUserByHabboId(item.InteractingUser2);
                if (roomUserByHabbo2 != null)
                {
                    keepDoorOpen = true;
                    roomUserByHabbo2.UnlockWalking();
                    roomUserByHabbo2.MoveTo(item.SquareInFront);
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
