using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Sound
{
    internal class SoundSettingsComposer : ServerPacket
    {
        public SoundSettingsComposer(ICollection<int> ClientVolumes, bool ChatPreference, bool InvitesStatus, bool FocusPreference, int FriendBarState)
            : base(ServerPacketHeader.USER_SETTINGS)
        {
            foreach (int VolumeValue in ClientVolumes)
            {
                this.WriteInteger(VolumeValue);
            }

            this.WriteBoolean(ChatPreference);
            this.WriteBoolean(InvitesStatus);
            this.WriteBoolean(FocusPreference);
            this.WriteInteger(FriendBarState);
            this.WriteInteger(0);
            this.WriteInteger(0);
        }
    }
}
