namespace WibboEmulator.Communication.Packets.Outgoing.Navigator.New;
using WibboEmulator.Games.Navigators;

internal class NavigatorMetaDataParserComposer : ServerPacket
{
    public NavigatorMetaDataParserComposer(ICollection<TopLevelItem> topLevelItems)
        : base(ServerPacketHeader.NAVIGATOR_METADATA)
    {
        this.WriteInteger(topLevelItems.Count);//Count
        foreach (var topLevelItem in topLevelItems.ToList())
        {
            //TopLevelContext
            this.WriteString(topLevelItem.SearchCode);//Search code
            this.WriteInteger(0);//Count of saved searches?
            /*{
                //SavedSearch
                base.WriteInteger(TopLevelItem.Id);//Id
               base.WriteString(TopLevelItem.SearchCode);//Search code
               base.WriteString(TopLevelItem.Filter);//Filter
               base.WriteString(TopLevelItem.Localization);//localization
            }*/
        }
    }
}
