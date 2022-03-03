using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Game.Clients;
using Butterfly.Game.Quests;
using Butterfly.Game.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RespectPetEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(Client Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetUser() == null || !Session.GetUser().InRoom || Session.GetUser().DailyPetRespectPoints == 0)
            {
                return;
            }


            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (ThisUser == null)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser TargetUser = Session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
                if (TargetUser == null)
                {
                    return;
                }

                //Check some values first, please!
                if (TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
                {
                    return;
                }

                if (TargetUser.GetClient().GetUser().Id == Session.GetUser().Id)
                {
                    return;
                }

                //And boom! Let us send some respect points.
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT);
                ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
                ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(TargetUser.GetClient(), "ACH_RespectEarned", 1);

                //Take away from pet respect points, just in-case users abuse this..
                Session.GetUser().DailyPetRespectPoints -= 1;
                TargetUser.GetClient().GetUser().Respect += 1;

                //Apply the effect.
                ThisUser.CarryItemID = 999999999;
                ThisUser.CarryTimer = 5;

                //Send the magic out.
                Room.SendPacket(new RespectPetNotificationComposer(TargetUser.GetClient().GetUser(), TargetUser));
                Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
                return;
            }

            if (Pet == null || Pet.PetData == null || Pet.RoomId != Session.GetUser().CurrentRoomId)
            {
                return;
            }

            Session.GetUser().DailyPetRespectPoints -= 1;
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetRespectGiver", 1);

            ThisUser.CarryItemID = 999999999;
            ThisUser.CarryTimer = 5;
            Pet.PetData.OnRespect();
            Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
        }
    }
}
