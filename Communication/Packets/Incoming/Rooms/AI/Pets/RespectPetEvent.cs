namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;

internal class RespectPetEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (session == null || session.GetUser() == null || !session.GetUser().InRoom || session.GetUser().DailyPetRespectPoints == 0)
        {
            return;
        }


        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var thisUser = room.RoomUserManager.GetRoomUserByUserId(session.GetUser().Id);
        if (thisUser == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!session.GetUser().CurrentRoom.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var targetUser = session.GetUser().CurrentRoom.RoomUserManager.GetRoomUserByUserId(petId);
            if (targetUser == null)
            {
                return;
            }

            if (targetUser.Client == null || targetUser.Client.GetUser() == null)
            {
                return;
            }

            if (targetUser.Client.GetUser().Id == session.GetUser().Id)
            {
                return;
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SOCIAL_RESPECT);
            _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
            _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(targetUser.Client, "ACH_RespectEarned", 1);

            //Take away from pet respect points, just in-case users abuse this..
            session.GetUser().DailyPetRespectPoints -= 1;
            targetUser.Client.GetUser().Respect += 1;

            //Apply the effect.
            thisUser.CarryItemID = 999999999;
            thisUser.CarryTimer = 5;

            //Send the magic out.
            room.SendPacket(new RespectPetNotificationComposer(targetUser.Client.GetUser(), targetUser));
            room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemID));
            return;
        }

        if (pet == null || pet.PetData == null || pet.RoomId != session.GetUser().CurrentRoomId)
        {
            return;
        }

        session.GetUser().DailyPetRespectPoints -= 1;
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetRespectGiver", 1);

        thisUser.CarryItemID = 999999999;
        thisUser.CarryTimer = 5;
        pet.PetData.OnRespect();
        room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemID));
    }
}
