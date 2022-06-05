using Wibbo.Game.Items;

namespace Wibbo.Communication.Packets.Outgoing.Inventory.Trading
{
    internal class TradingUpdateComposer : ServerPacket
    {
        public TradingUpdateComposer(int userOneId, List<Item> userOneItems, int userTwoId, List<Item> userTwoItems)
            : base(ServerPacketHeader.TRADE_LIST_ITEM)
        {
            this.WriteInteger(userOneId);
            this.WriteInteger(userOneItems.Count);
            foreach (Item userItem in userOneItems)
            {
                this.WriteItem(userItem);
            }
            this.WriteInteger(userOneItems.Count);
            this.WriteInteger(0);

            this.WriteInteger(userTwoId);
            this.WriteInteger(userTwoItems.Count);
            foreach (Item userItem in userTwoItems)
            {
                this.WriteItem(userItem);
            }
            this.WriteInteger(userTwoItems.Count);
            this.WriteInteger(0);
        }

        private void WriteItem(Item userItem)
        {
            this.WriteInteger(userItem.Id);
            this.WriteString(userItem.GetBaseItem().Type.ToString().ToLower());
            this.WriteInteger(userItem.Id);
            this.WriteInteger(userItem.GetBaseItem().SpriteId);
            this.WriteInteger(0);
            if (userItem.Limited > 0)
            {
                this.WriteBoolean(false);
                this.WriteInteger(256);
                this.WriteString("");
                this.WriteInteger(userItem.Limited);
                this.WriteInteger(userItem.LimitedStack);
            }
            else if (userItem.GetBaseItem().InteractionType == InteractionType.BADGE_DISPLAY || userItem.GetBaseItem().InteractionType == InteractionType.BADGE_TROC)
            {
                this.WriteBoolean(false);
                this.WriteInteger(2);
                this.WriteInteger(4);

                if (userItem.ExtraData.Contains(Convert.ToChar(9).ToString()))
                {
                    string[] BadgeData = userItem.ExtraData.Split(Convert.ToChar(9));

                    this.WriteString("0");//No idea
                    this.WriteString(BadgeData[0]);//Badge name
                    this.WriteString(BadgeData[1]);//Owner
                    this.WriteString(BadgeData[2]);//Date
                }
                else
                {
                    this.WriteString("0");//No idea
                    this.WriteString(userItem.ExtraData);//Badge name
                    this.WriteString("");//Owner
                    this.WriteString("");//Date
                }
            }
            else
            {
                this.WriteBoolean(true);
                this.WriteInteger(0);
                this.WriteString("");
            }
            this.WriteInteger(0);
            this.WriteInteger(0);
            this.WriteInteger(0);
            if (userItem.GetBaseItem().Type == 's')
            {
                this.WriteInteger(0);
            }
        }
    }
}
