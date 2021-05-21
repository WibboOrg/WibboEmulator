using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Quests;
using Butterfly.HabboHotel.Rooms;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class RespectPetEvent : IPacketEvent
    {
        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (Session == null || Session.GetHabbo() == null || !Session.GetHabbo().InRoom || Session.GetHabbo().DailyPetRespectPoints == 0)
            {
                return;
            }


            if (!ButterflyEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetHabbo().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser ThisUser = Room.GetRoomUserManager().GetRoomUserByHabboId(Session.GetHabbo().Id);
            if (ThisUser == null)
            {
                return;
            }

            int PetId = Packet.PopInt();

            if (!Session.GetHabbo().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
                RoomUser TargetUser = Session.GetHabbo().CurrentRoom.GetRoomUserManager().GetRoomUserByHabboId(PetId);
                if (TargetUser == null)
                {
                    return;
                }

                //Check some values first, please!
                if (TargetUser.GetClient() == null || TargetUser.GetClient().GetHabbo() == null)
                {
                    return;
                }

                if (TargetUser.GetClient().GetHabbo().Id == Session.GetHabbo().Id)
                {
                    return;
                }

                //And boom! Let us send some respect points.
                ButterflyEnvironment.GetGame().GetQuestManager().ProgressUserQuest(Session, QuestType.SOCIAL_RESPECT);
                ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_RespectGiven", 1);
                ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(TargetUser.GetClient(), "ACH_RespectEarned", 1);

                //Take away from pet respect points, just in-case users abuse this..
                Session.GetHabbo().DailyPetRespectPoints -= 1;
                TargetUser.GetClient().GetHabbo().Respect += 1;

                //Apply the effect.
                ThisUser.CarryItemID = 999999999;
                ThisUser.CarryTimer = 5;

                //Send the magic out.
                Room.SendPacket(new RespectPetNotificationComposer(TargetUser.GetClient().GetHabbo(), TargetUser));
                Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
                return;
            }

            if (Pet == null || Pet.PetData == null || Pet.RoomId != Session.GetHabbo().CurrentRoomId)
            {
                return;
            }

            Session.GetHabbo().DailyPetRespectPoints -= 1;
            ButterflyEnvironment.GetGame().GetAchievementManager().ProgressAchievement(Session, "ACH_PetRespectGiver", 1);

            ThisUser.CarryItemID = 999999999;
            ThisUser.CarryTimer = 5;
            Pet.PetData.OnRespect();
            Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
        }
    }
}
