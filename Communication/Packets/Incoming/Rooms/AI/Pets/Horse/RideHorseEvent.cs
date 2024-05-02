namespace WibboEmulator.Communication.Packets.Incoming.Rooms.AI.Pets.Horse;
using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Core.Language;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

internal sealed class RideHorseEvent : IPacketEvent
{
    public double Delay => 250;

    public void Parse(GameClient session, ClientPacket packet)
    {
        if (!session.User.InRoom)
        {
            return;
        }


        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var user = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (user == null)
        {
            return;
        }

        var petId = packet.PopInt();
        var type = packet.PopBoolean();

        if (!room.RoomUserManager.TryGetPet(petId, out var pet))
        {
            return;
        }

        if (pet.PetData == null || pet.PetData.Type != 13)
        {
            return;
        }

        if (!pet.PetData.AnyoneCanRide && pet.PetData.OwnerId != user.UserId)
        {
            return;
        }

        if (Math.Abs(user.X - pet.X) >= 2 || Math.Abs(user.Y - pet.Y) >= 2)
        {
            user.MoveTo(pet.X, pet.Y);
            return;
        }

        if (type && !user.RidingHorse)
        {
            if (pet.RidingHorse)
            {
                var speechtxt = LanguageManager.TryGetValue("pet.alreadymounted", session.Language);
                pet.OnChat(speechtxt, 0, false);
            }
            else if (user.RidingHorse)
            {
                return;
            }
            else
            {
                user.RemoveStatus("sit");
                user.RemoveStatus("lay");
                user.RemoveStatus("snf");
                user.RemoveStatus("eat");
                user.RemoveStatus("ded");
                user.RemoveStatus("jmp");

                var newX2 = pet.X;
                var newY2 = pet.Y;
                room.SendPacket(RoomItemHandling.TeleportUser(user, new Point(newX2, newY2), 0, room.GameMap.SqAbsoluteHeight(newX2, newY2) + 1));
                room.SendPacket(RoomItemHandling.TeleportUser(pet, new Point(newX2, newY2), 0, room.GameMap.SqAbsoluteHeight(newX2, newY2)));

                user.MoveTo(newX2, newY2);

                user.RidingHorse = true;
                pet.RidingHorse = true;
                pet.HorseID = user.VirtualId;
                user.HorseID = pet.VirtualId;

                if (pet.PetData.Saddle == 9)
                {
                    user.ApplyEffect(77);
                }
                else
                {
                    user.ApplyEffect(103);
                }

                user.RotBody = pet.RotBody;
                user.RotHead = pet.RotHead;

                user.UpdateNeeded = true;
                pet.UpdateNeeded = true;
            }
        }
        else
        {
            if (user.VirtualId == pet.HorseID)
            {
                pet.RemoveStatus("sit");
                pet.RemoveStatus("lay");
                pet.RemoveStatus("snf");
                pet.RemoveStatus("eat");
                pet.RemoveStatus("ded");
                pet.RemoveStatus("jmp");
                user.RidingHorse = false;
                user.HorseID = 0;
                pet.RidingHorse = false;
                pet.HorseID = 0;
                user.MoveTo(new Point(user.X + 1, user.Y + 1));
                user.ApplyEffect(-1);
                user.UpdateNeeded = true;
                pet.UpdateNeeded = true;
            }
        }

        room.SendPacket(new PetHorseFigureInformationComposer(pet));
    }
}
