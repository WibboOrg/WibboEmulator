using Butterfly.HabboHotel.Pets;
using System.Collections.Generic;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets
{
    internal class PetTrainingPanelComposer : ServerPacket
    {
        public PetTrainingPanelComposer(Pet petData)
            : base(ServerPacketHeader.PetTrainingPanelMessageComposer)
        {
            this.WriteInteger(petData.PetId);

            List<short> AvailableCommands = new List<short>();

            this.WriteInteger(petData.PetCommands.Count);
            foreach (short Sh in petData.PetCommands.Keys)
            {
                this.WriteInteger(Sh);
                if (petData.PetCommands[Sh] == true)
                {
                    AvailableCommands.Add(Sh);
                }
            }

            this.WriteInteger(AvailableCommands.Count);
            foreach (short Sh in AvailableCommands)
            {
                this.WriteInteger(Sh);
            }
        }
    }
}
