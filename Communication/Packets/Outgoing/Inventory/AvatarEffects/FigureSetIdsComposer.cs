﻿namespace WibboEmulator.Communication.Packets.Outgoing.Inventory.AvatarEffects;

internal sealed class FigureSetIdsComposer : ServerPacket
{
    public FigureSetIdsComposer()
        : base(ServerPacketHeader.USER_CLOTHING)
    {
        this.WriteInteger(0);

        this.WriteInteger(0);
    }
}
