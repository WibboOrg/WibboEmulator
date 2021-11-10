using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.GameClients;
using Butterfly.Game.Items;
using Butterfly.Game.Rooms.Wired.WiredHandlers.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.Game.Rooms.Wired.WiredHandlers.Conditions
{
    public class FurniStatePosMatch : IWiredCondition, IWired
    {
        private readonly int itemID;
        private Dictionary<int, ItemsPosReset> items;
        private bool isDisposed;

        private int EtatActuel;
        private int DirectionActuel;
        private int PositionActuel;

        public FurniStatePosMatch(Item item, List<Item> items, int etatActuel, int directionActuel, int positionActuel)
        {
            this.itemID = item.Id;
            this.isDisposed = false;

            this.EtatActuel = etatActuel;
            this.DirectionActuel = directionActuel;
            this.PositionActuel = positionActuel;

            this.items = new Dictionary<int, ItemsPosReset>();

            foreach (Item roomItem in items)
            {
                if (!this.items.ContainsKey(roomItem.Id))
                {
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
                else
                {
                    this.items.Remove(roomItem.Id);
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, roomItem.GetX, roomItem.GetY, roomItem.GetZ, roomItem.Rotation, roomItem.ExtraData));
                }
            }
        }

        public bool AllowsExecution(RoomUser user, Item TriggerItem)
        {
            foreach (ItemsPosReset roomItem in this.items.Values)
            {
                if (this.EtatActuel == 1)
                {
                    if (roomItem.extradata != "Null")
                    {
                        if (!(roomItem.item.ExtraData == "" && roomItem.extradata == "0") && !(roomItem.item.ExtraData == "0" && roomItem.extradata == ""))
                        {

                            if (roomItem.item.ExtraData != roomItem.extradata)
                            {
                                return false;
                            }
                        }
                    }
                }

                if (this.DirectionActuel == 1)
                {
                    if (roomItem.rot != roomItem.item.Rotation)
                    {
                        return false;
                    }
                }

                if (this.PositionActuel == 1)
                {
                    if (roomItem.x != roomItem.item.GetX || roomItem.y != roomItem.item.GetY)
                    {
                        return false;
                    }
                }
            }
            return true;
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
            string triggerItem = row["triggers_item"].ToString();

            string triggerDataTwo = row["trigger_data_2"].ToString();

            if (triggerDataTwo.Length == 5)
            {
                string[] dataSplit = triggerDataTwo.Split(';');

                if (int.TryParse(dataSplit[0], out int state))
                    this.EtatActuel = state;
                if (int.TryParse(dataSplit[1], out int direct))
                    this.DirectionActuel = direct;
                if (int.TryParse(dataSplit[2], out int position))
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
                if (roomItem != null && !this.items.ContainsKey(roomItem.Id) && roomItem.Id != this.itemID)
                {
                    this.items.Add(roomItem.Id, new ItemsPosReset(roomItem, Convert.ToInt32(Item2[1]), Convert.ToInt32(Item2[2]), Convert.ToDouble(Item2[3]), Convert.ToInt32(Item2[4]), Item2[5]));
                }
            }
        }

        public void OnTrigger(GameClient Session, int SpriteId)
        {
            ServerPacket Message = new ServerPacket(ServerPacketHeader.WIRED_CONDITION);
            Message.WriteBoolean(false);
            Message.WriteInteger(10);
            Message.WriteInteger(this.items.Count);
            foreach (int roomItemid in this.items.Keys)
            {
                Message.WriteInteger(roomItemid);
            }

            Message.WriteInteger(SpriteId);
            Message.WriteInteger(this.itemID);
            Message.WriteString("");
            Message.WriteInteger(3);

            Message.WriteInteger(this.EtatActuel); //Etat actuel du mobi
            Message.WriteInteger(this.DirectionActuel); //Direction  actuelle
            Message.WriteInteger(this.PositionActuel); //position actuelle dans l'appart

            Message.WriteInteger(0);
            Message.WriteInteger(0);
            Session.SendPacket(Message);
        }

        public void Dispose()
        {
            this.isDisposed = true;
            if (this.items != null)
            {
                this.items.Clear();
            }

            this.items = null;
        }

        public bool Disposed()
        {
            return this.isDisposed;
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

            if (int.TryParse(Extradata, out int result) || (Extradata.Length < 10 && !Extradata.Contains(";") && !Extradata.Contains(":")))
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
