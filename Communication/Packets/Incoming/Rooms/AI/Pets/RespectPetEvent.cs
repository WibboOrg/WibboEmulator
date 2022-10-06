namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal class RespectPetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket Packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom || session.GetUser().DailyPetRespectPoints == 0)
        {
            return;
        }


        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var Room))
        {
            return;
        }

        var ThisUser = Room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (ThisUser == null)
        {
            return;
        }

        var PetId = Packet.PopInt();

        if (!session.GetUser().CurrentRoom.GetRoomUserManager().TryGetPet(PetId, out var Pet))
        {
            //Okay so, we've established we have no pets in this room by this virtual Id, let us check out users, maybe they're creeping as a pet?!
            var TargetUser = session.GetUser().CurrentRoom.GetRoomUserManager().GetRoomUserByUserId(PetId);
            if (TargetUser == null)
            {
                return;
            }

            //Check some values first, please!
            if (TargetUser.GetClient() == null || TargetUser.GetClient().GetUser() == null)
            {
                return;
            }

            if (TargetUser.GetClient().GetUser().Id == session.GetUser().Id)
            {
                return;
            }

            //And boom! Let us send some respect points.
            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_RESPECT);
            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
            WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(TargetUser.GetClient(), "ACH_RespectEarned", 1);

            //Take away from pet respect points, just in-case users abuse this..
            session.GetUser().DailyPetRespectPoints -= 1;
            TargetUser.GetClient().GetUser().Respect += 1;

            //Apply the effect.
            ThisUser.CarryItemID = 999999999;
            ThisUser.CarryTimer = 5;

            //Send the magic out.
            Room.SendPacket(new RespectPetNotificationComposer(TargetUser.GetClient().GetUser(), TargetUser));
            Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
            return;
        }

        if (Pet == null || Pet.PetData == null || Pet.RoomId != session.GetUser().CurrentRoomId)
        {
            return;
        }

        session.GetUser().DailyPetRespectPoints -= 1;
        WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetRespectGiver", 1);

        ThisUser.CarryItemID = 999999999;
        ThisUser.CarryTimer = 5;
        Pet.PetData.OnRespect();
        Room.SendPacket(new CarryObjectComposer(ThisUser.VirtualId, ThisUser.CarryItemID));
    }
}
