namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Avatar;
using WibboEmulator.Games.Achievements;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Quests;
using WibboEmulator.Games.Rooms;

internal sealed class RespectPetEvent : IPacketEvent
{
    public double Delay => 100;

    public void Parse(GameClient Session, ClientPacket packet)
    {
        if (Session == null || Session.User == null || !Session.User.InRoom || Session.User.DailyPetRespectPoints == 0)
        {
            return;
        }


        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var thisUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (thisUser == null)
        {
            return;
        }

        var petId = packet.PopInt();

        if (!Session.User.Room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            var TargetUser = Session.User.Room.RoomUserManager.GetRoomUserByUserId(petId);
            if (TargetUser == null)
            {
                return;
            }

            if (TargetUser.Client == null || TargetUser.Client.User == null)
            {
                return;
            }

            if (TargetUser.Client.User.Id == Session.User.Id)
            {
                return;
            }

            QuestManager.ProgressUserQuest(Session, QuestType.SocialRespect);
            _ = AchievementManager.ProgressAchievement(Session, "ACH_RespectGiven", 1);
            _ = AchievementManager.ProgressAchievement(TargetUser.Client, "ACH_RespectEarned", 1);

            //Take away from pet respect points, just in-case users abuse this..
            Session.            //Take away from pet respect points, just in-case users abuse this..
            User.DailyPetRespectPoints -= 1;
            TargetUser.Client.User.Respect += 1;

            //Apply the effect.
            thisUser.CarryItemId = 999999999;
            thisUser.CarryTimer = 5;

            //Send the magic out.
            room.SendPacket(new RespectPetNotificationComposer(TargetUser.Client.User, TargetUser));
            room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
            return;
        }

        if (pet == null || pet.PetData == null || pet.RoomId != Session.User.RoomId)
        {
            return;
        }

        Session.User.DailyPetRespectPoints -= 1;
        _ = AchievementManager.ProgressAchievement(Session, "ACH_PetRespectGiver", 1);

        thisUser.CarryItemId = 999999999;
        thisUser.CarryTimer = 5;
        pet.PetData.OnRespect();
        room.SendPacket(new CarryObjectComposer(thisUser.VirtualId, thisUser.CarryItemId));
    }
}
