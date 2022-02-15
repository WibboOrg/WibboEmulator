using Butterfly.Communication.Packets.Outgoing;
using System;

namespace Butterfly.Utilities
{
    public class ServerPacketList
    {
        private byte[] totalBytes;
        private int current;
        private int count;

        public ServerPacketList()
        {
            this.totalBytes = new byte[0];
            this.current = 0;
            this.count = 0;
        }

        public void Add(IServerPacket packet)
        {
            byte[] toAdd = packet.GetBytes();

            int newLen = this.totalBytes.Length + toAdd.Length;

            Array.Resize(ref this.totalBytes, newLen);

            for (int i = 0; i < toAdd.Length; i++)
            {
                this.totalBytes[this.current] = toAdd[i];
                this.current++;
            }

            this.count++;
        }

        public void Clear()
        {
            this.totalBytes = new byte[0];
            this.current = 0;
            this.count = 0;
        }

        public int Count => this.count;

        public byte[] GetBytes => this.totalBytes;
    }
}
