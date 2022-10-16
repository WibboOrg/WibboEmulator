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
        if (session == null || session.User == null || !session.User.InRoom || session.User.DailyPetRespectPoints == 0)
        {
            return;
        }


        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.User.CurrentRoomId, out var room))
        {
            return;
        }

        var thisUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (thisUser == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!session.User.CurrentRoom.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var targetUser = session.User.CurrentRoom.RoomUserManager.GetRoomUserByUserId(petId);
            if (targetUser == null)
            {
                return;
            }

            if (targetUser.Client == null || targetUser.Client.User == null)
            {
                return;
            }

            if (targetUser.Client.User.Id == session.User.Id)
            {
                return;
            }

            WibboEnvironment.GetGame().GetQuestManager().ProgressUserQuest(session, QuestType.SocialRespect);
            _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_RespectGiven", 1);
            _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(targetUser.Client, "ACH_RespectEarned", 1);

            //Take away from pet respect points, just in-case users abuse this..
            session.            //Take away from pet respect points, just in-case users abuse this..
            User.DailyPetRespectPoints -= 1;
            targetUser.Client.User.Respect += 1;

            //Apply the effect.
            thisUser.CarryItemID = 999999999;
            thisUser.CarryTimer = 5;

            //Send the magic out.
            room.SendPacket(new RespectPetNotificationComposer(targetUser.Client.User, targetUser));
            room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemID));
            return;
        }

        if (pet == null || pet.PetData == null || pet.RoomId != session.User.CurrentRoomId)
        {
            return;
        }

        session.User.DailyPetRespectPoints -= 1;
        _ = WibboEnvironment.GetGame().GetAchievementManager().ProgressAchievement(session, "ACH_PetRespectGiver", 1);

        thisUser.CarryItemID = 999999999;
        thisUser.CarryTimer = 5;
        pet.PetData.OnRespect();
        room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemID));
    }
}
