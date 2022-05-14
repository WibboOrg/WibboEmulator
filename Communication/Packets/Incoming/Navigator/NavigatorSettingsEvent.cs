﻿using Butterfly.Communication.Packets.Outgoing.Navigator;

using Butterfly.Game.Clients;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class NavigatorSettingsEvent : IPacketEvent
    {
        public double Delay => 0;

        public void Parse(Client Session, ClientPacket Packet)
        {
            Session.SendPacket(new NavigatorSettingsComposer(0, 0, 0, 0, false, 0));
        }
    }
}