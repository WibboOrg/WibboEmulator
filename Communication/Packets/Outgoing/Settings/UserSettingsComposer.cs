namespace WibboEmulator.Communication.Packets.Outgoing.Settings;

internal class UserSettingsComposer : ServerPacket
{
    public UserSettingsComposer(ICollection<int> clientVolumes, bool oldChat, bool roomInvites, bool cameraFollow, int flags, int chatType)
        : base(ServerPacketHeader.USER_SETTINGS)
    {
        foreach (var volumeValue in clientVolumes)
        {
            this.WriteInteger(volumeValue);
        }

        this.WriteBoolean(oldChat);
        this.WriteBoolean(roomInvites);
        this.WriteBoolean(cameraFollow);
        this.WriteInteger(flags);
        this.WriteInteger(chatType);
    }
}
