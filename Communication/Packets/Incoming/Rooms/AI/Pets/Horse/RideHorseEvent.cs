using System.Drawing;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

namespace WibboEmulator.Communication.Packets.Incoming.Structure
{
    internal class RideHorseEvent : IPacketEvent
    {
        public double Delay => 250;

        public void Parse(GameClient Session, ClientPacket Packet)
        {
            if (!Session.GetUser().InRoom)
            {
                return;
            }


            if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(Session.GetUser().CurrentRoomId, out Room Room))
            {
                return;
            }

            RoomUser User = Room.GetRoomUserManager().GetRoomUserByUserId(Session.GetUser().Id);
            if (User == null)
            {
                return;
            }

            int PetId = Packet.PopInt();
            bool Type = Packet.PopBoolean();

            if (!Room.GetRoomUserManager().TryGetPet(PetId, out RoomUser Pet))
            {
                return;
            }

            if (Pet.PetData == null || Pet.PetData.Type != 13)
            {
                return;
            }

            if (!Pet.PetData.AnyoneCanRide && Pet.PetData.OwnerId != User.UserId)
            {
                return;
            }

            if (Math.Abs(User.X - Pet.X) >= 2 || Math.Abs(User.Y - Pet.Y) >= 2)
            {
                User.MoveTo(Pet.X, Pet.Y);
                return;
            }

            if (Type && !User.RidingHorse)
            {
                if (Pet.RidingHorse)
                {
                    string Speechtxt = WibboEnvironment.GetLanguageManager().TryGetValue("pet.alreadymounted", Session.Langue);
                    Pet.OnChat(Speechtxt, 0, false);
                }
                else if (User.RidingHorse)
                {
                    return;
                }
                else
                {
                    User.RemoveStatus("sit");
                    User.RemoveStatus("lay");
                    User.RemoveStatus("snf");
                    User.RemoveStatus("eat");
                    User.RemoveStatus("ded");
                    User.RemoveStatus("jmp");

                    int NewX2 = Pet.X;
                    int NewY2 = Pet.Y;
                    Room.SendPacket(Room.GetRoomItemHandler().TeleportUser(User, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2) + 1));
                    Room.SendPacket(Room.GetRoomItemHandler().TeleportUser(Pet, new Point(NewX2, NewY2), 0, Room.GetGameMap().SqAbsoluteHeight(NewX2, NewY2)));

                    User.MoveTo(NewX2, NewY2);

                    User.RidingHorse = true;
                    Pet.RidingHorse = true;
                    Pet.HorseID = User.VirtualId;
                    User.HorseID = Pet.VirtualId;

                    if (Pet.PetData.Saddle == 9)
                    {
                        User.ApplyEffect(77);
                    }
                    else
                    {
                        User.ApplyEffect(103);
                    }

                    User.RotBody = Pet.RotBody;
                    User.RotHead = Pet.RotHead;

                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
            }
            else
            {
                if (User.VirtualId == Pet.HorseID)
                {
                    Pet.RemoveStatus("sit");
                    Pet.RemoveStatus("lay");
                    Pet.RemoveStatus("snf");
                    Pet.RemoveStatus("eat");
                    Pet.RemoveStatus("ded");
                    Pet.RemoveStatus("jmp");
                    User.RidingHorse = false;
                    User.HorseID = 0;
                    Pet.RidingHorse = false;
                    Pet.HorseID = 0;
                    User.MoveTo(new Point(User.X + 1, User.Y + 1));
                    User.ApplyEffect(-1);
                    User.UpdateNeeded = true;
                    Pet.UpdateNeeded = true;
                }
            }

            Room.SendPacket(new PetHorseFigureInformationComposer(Pet));
        }
    }
}
