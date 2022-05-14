using Butterfly.Game.Navigator;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Navigator
{
    internal class NavigatorHomeRoomComposer : ServerPacket
    {
        public NavigatorHomeRoomComposer(int homeRoomId, int roomIdToEnter)
            : base(ServerPacketHeader.USER_HOME_ROOM)
        {
            WriteInteger(homeRoomId);
            WriteInteger(roomIdToEnter);
        }
    }
}