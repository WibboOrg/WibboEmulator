namespace WibboEmulator.Communication.Packets.Outgoing.Settings;

internal class UserSettingsComposer : ServerPacket
{
    public UserSettingsComposer(ICollection<int> ClientVolumes, bool oldChat, bool roomInvites, bool cameraFollow, int flags, int chatType)
        : base(ServerPacketHeader.USER_SETTINGS)
    {
        foreach (var VolumeValue in ClientVolumes)
        {
            this.WriteInteger(VolumeValue);
        }

        this.WriteBoolean(oldChat);
        this.WriteBoolean(roomInvites);
        this.WriteBoolean(cameraFollow);
        this.WriteInteger(flags);
        this.WriteInteger(chatType);
    }
}
