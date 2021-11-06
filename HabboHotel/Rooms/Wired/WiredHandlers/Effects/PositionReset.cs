using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Items;
using Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Wired.WiredHandlers.Effects
{
    public class PositionReset : IWired, IWiredCycleable, IWiredEffect
    {
        private RoomItemHandling roomItemHandler;
        private WiredHandler handler;
        private readonly int itemID;
        private ConcurrentDictionary<int, ItemsPosReset> items;
        public int Delay { get; set; }
        private bool disposed;

        private int EtatActuel;
        private int DirectionActuel;
        private int PositionActuel;

        public PositionReset(List<Item> items, int delay, RoomItemHandling roomItemHandler, WiredHandler handler, int itemID, int etatActuel, int directionActuel, int positionActuel)
        {
            this.Delay = delay;
            this.roomItemHandler = roomItemHandler;
            this.itemID = itemID;
            this.handler = handler;
            this.disposed = false;

            this.EtatActuel = etatActuel;
            this.DirectionActuel = directionActuel;
            this.PositionActuel = positionActuel;

            this.items = new ConcurrentDictionary<int, ItemsPosReset>();

            foreach (Item roomItem in items)
            {
                if (!this.items.ContainsKey(roomItem.Id))
                {
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
                else
                {
                    this.items.TryRemove(roomItem.Id, out ItemsPosReset RemoveItem);
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
            }
        }

        public bool OnCycle(RoomUser user, Item item)
        {
            this.HandleItems();
            return false;
        }

        public void Handle(RoomUser user, Item TriggerItem)
        {
            if (this.Delay > 0)
            {
                this.handler.RequestCycle(new WiredCycle(this, user, TriggerItem, this.Delay));
            }
            else
            {
                this.HandleItems();
            }
        }

        private void HandleItems()
        {
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                this.HandleReset(roomItem);
            }
        }

        private void HandleReset(ItemsPosReset roomItem)
        {
            if (roomItem == null)
            {
                return;
            }

            if (this.EtatActuel == 1)
            {
                if (roomItem.extradata != "Null")
                {
                    if (roomItem.item.ExtraData != roomItem.extradata)
                    {
                        roomItem.item.ExtraData = roomItem.extradata;
                        roomItem.item.UpdateState();
                        roomItem.item.GetRoom().GetGameMap().updateMapForItem(roomItem.item);
                    }
                }
            }

            if (this.DirectionActuel == 1)
            {
                if (roomItem.rot != roomItem.item.Rotation)
                {
                    this.roomItemHandler.RotReset(roomItem.item, roomItem.rot);
                }
            }

            if (this.PositionActuel == 1)
            {
                if (roomItem.x != roomItem.item.GetX || roomItem.y != roomItem.item.GetY || roomItem.z != roomItem.item.GetZ)
                {
                    this.roomItemHandler.PositionReset(roomItem.item, roomItem.x, roomItem.y, roomItem.z);
                }
            }
        }

        public void Dispose()
        {
            this.disposed = true;
            this.roomItemHandler = null;
            this.handler = null;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public void SaveToDatabase(IQueryAdapter dbClient)
        {
            string triggersitem = "";

            int i = 0;
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                if (i != 0)
                {
                    triggersitem += ";";
                }

                triggersitem += roomItem.item.Id + ":" + roomItem.x + ":" + roomItem.y + ":" + roomItem.z + ":" + roomItem.rot + ":" + roomItem.extradata;

                i++;
            }

            string triggerData2 = this.EtatActuel + ";" + this.DirectionActuel + ";" + this.PositionActuel;

            ItemWiredDao.Delete(dbClient, this.itemID);
            ItemWiredDao.Insert(dbClient, this.itemID, "", triggerData2, false, triggersitem);
        }

        public void LoadFromDatabase(DataRow row, Room insideRoom)
        {
            if(int.TryParse(row["trigger_data"].ToString(), out int delay))
                this.Delay = delay;

            string triggerItem = row["triggers_item"].ToString();

            string data2 = row["trigger_data_2"].ToString();

            if (data2.Length == 5)
            {
                string[] dataSplit = data2.Split(';');

                if(int.TryParse(dataSplit[0], out int state))
                    this.EtatActuel = state;
                if(int.TryParse(dataSplit[1], out int direction))
                    this.DirectionActuel = direction;
                if(int.TryParse(dataSplit[2], out int position))
                    this.PositionActuel = position;
            }

            if (triggerItem == "")
            {
                return;
            }

            foreach (string item in triggerItem.Split(';'))
            {
                string[] Item2 = item.Split(':');
                if (Item2.Length != 6)
                {
                    continue;
                }

                Item roomItem = insideRoom.GetRoomItemHandler().GetItem(Convert.ToInt32(Item2[0]));
                if (roomItem != null && !this.items.ContainsKey(roomItem.Id))
                {
                    this.items.TryAdd(roomItem.Id, new ItemsPosReset(roomItem, Convert.ToInt32(Item2[1]), Convert.ToInt32(Item2[2]), Convert.ToDouble(Item2[3]), Convert.ToInt32(Item2[4]), Item2[5]));
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_ACTION);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (int roomItemId in this.items.Keys)
            {
                Message.WriteInteger(roomItemId);
            }

            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(3);

            Message.WriteInteger(this.EtatActuel); //Etat actuel du mobi
            Message.WriteInteger(this.DirectionActuel); //Direction  actuelle
            Message.WriteInteger(this.PositionActuel); //position actuelle dans l'appart

            Message.WriteInteger(1);
            Message.WriteInteger(3);
            Message.WriteInteger(this.Delay);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public bool Disposed()
        {
            return this.disposed;
        }
    }

    public class ItemsPosReset
    {
        public Item item;

        public int x;
        public int y;
        public double z;
        public int rot;
        public string extradata;

        public ItemsPosReset(Item Item, int X, int Y, double Z, int Rot, string Extradata)
        {
            this.item = Item;

            this.x = X;
            this.y = Y;
            this.z = Z;
            this.rot = Rot;

            if (int.TryParse(Extradata, out int result) || (!Extradata.Contains(";") && !Extradata.Contains(":")))
            {
                this.extradata = Extradata;
            }
            else
            {
                this.extradata = "Null";
            }
        }

    }
}
