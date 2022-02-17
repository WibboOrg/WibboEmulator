using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using Butterfly.Game.Clients;
using Butterfly.Game.Rooms;
using System.Collections.Generic;
using System.Text;

namespace Butterfly.Communication.Packets.Incoming.Structure
{
    internal class SaveRoomSettingsEvent : IPacketEvent
    {
        public double Delay => 500;

        public void Parse(Client Session, ClientPacket Packet)
        {
            int RoomId = Packet.PopInt();

            Room room = ButterflyEnvironment.GetGame().GetRoomManager().GetRoom(RoomId);
            if (room == null)
            {
                return;
            }

            if (!room.CheckRights(Session, true) && !Session.GetHabbo().HasFuse("fuse_settings_room"))
            {
                return;
            }

            string Name = Packet.PopString();
            string Description = Packet.PopString();
            int State = Packet.PopInt();
            string Password = Packet.PopString();
            int MaxUsers = Packet.PopInt();
            int CategoryId = Packet.PopInt();
            int TagCount = Packet.PopInt();
            List<string> tags = new List<string>();
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < TagCount; ++index)
            {
                if (index > 0)
                {
                    stringBuilder.Append(",");
                }

                string tag = Packet.PopString().ToLower();
                tags.Add(tag);
                stringBuilder.Append(tag);
            }
            int TrocStatus = Packet.PopInt();
            bool AllowPets = Packet.PopBoolean();
            bool AllowPetsEat = Packet.PopBoolean();
            bool AllowWalkthrough = Packet.PopBoolean();
            bool Hidewall = Packet.PopBoolean();
            int WallThickness = Packet.PopInt();
            int FloorThickness = Packet.PopInt();
            int mutefuse = Packet.PopInt();
            int kickfuse = Packet.PopInt();
            int banfuse = Packet.PopInt();
            int ChatType = Packet.PopInt();
            int ChatBalloon = Packet.PopInt();
            int ChatSpeed = Packet.PopInt();
            int ChatMaxDistance = Packet.PopInt();
            int ChatFloodProtection = Packet.PopInt();

            if (WallThickness < -2 || WallThickness > 1)
            {
                WallThickness = 0;
            }

            if (FloorThickness < -2 || FloorThickness > 1)
            {
                FloorThickness = 0;
            }

            if (Name.Length < 1 || Name.Length > 100)
            {
                return;
            }

            if (State < 0 || State > 3)
            {
                return;
            }

            if (MaxUsers < 10 || MaxUsers > 75)
            {
                MaxUsers = 25;
            }

            if (TrocStatus < 0 || TrocStatus > 2)
            {
                TrocStatus = 0;
            }

            if (TagCount > 2 || mutefuse != 0 && mutefuse != 1 || kickfuse != 0 && kickfuse != 1 && kickfuse != 2 || banfuse != 0 && banfuse != 1)
            {
                return;
            }

            if (ChatMaxDistance > 99)
            {
                ChatMaxDistance = 99;
            }

            room.RoomData.AllowPets = AllowPets;
            room.RoomData.AllowPetsEating = AllowPetsEat;
            room.RoomData.AllowWalkthrough = AllowWalkthrough;
            room.RoomData.Hidewall = Hidewall;
            room.RoomData.Name = Name;
            room.RoomData.State = State;
            room.RoomData.Description = Description;
            room.RoomData.Category = CategoryId;
            if (!string.IsNullOrEmpty(Password))
            {
                room.RoomData.Password = Password;
            }

            room.ClearTags();
            room.AddTagRange(tags);
            room.RoomData.Tags.Clear();
            room.RoomData.Tags.AddRange(tags);
            room.RoomData.UsersMax = MaxUsers;
            room.RoomData.WallThickness = WallThickness;
            room.RoomData.FloorThickness = FloorThickness;
            room.RoomData.MuteFuse = mutefuse;
            room.RoomData.WhoCanKick = kickfuse;
            room.RoomData.BanFuse = banfuse;

            room.RoomData.ChatType = ChatType;
            room.RoomData.ChatBalloon = ChatBalloon;
            room.RoomData.ChatSpeed = ChatSpeed;
            room.RoomData.ChatMaxDistance = ChatMaxDistance;
            room.RoomData.ChatFloodProtection = ChatFloodProtection;

            room.RoomData.TrocStatus = TrocStatus;
            string str5 = "open";
            if (room.RoomData.State == 1)
            {
                str5 = "locked";
            }
            else if (room.RoomData.State == 2)
            {
                str5 = "password";
            }
            else if (room.RoomData.State == 3)
            {
                str5 = "hide";
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                RoomDao.UpdateAll(dbClient, room.Id, room.RoomData.Name, room.RoomData.Description, room.RoomData.Password, (stringBuilder).ToString(), CategoryId, str5, MaxUsers, AllowPets, AllowPetsEat, AllowWalkthrough, room.RoomData.Hidewall, room.RoomData.FloorThickness, room.RoomData.WallThickness, mutefuse, kickfuse, banfuse, ChatType, ChatBalloon, ChatSpeed, ChatMaxDistance, ChatFloodProtection, TrocStatus);
            }

            Session.SendPacket(new RoomSettingsSavedComposer(room.Id));

            room.SendPacket(new RoomVisualizationSettingsComposer(room.RoomData.WallThickness, room.RoomData.FloorThickness, room.RoomData.Hidewall));
            room.SendPacket(new RoomChatOptionsComposer(room.RoomData.ChatType, room.RoomData.ChatBalloon, room.RoomData.ChatSpeed, room.RoomData.ChatMaxDistance, room.RoomData.ChatFloodProtection));

            Session.SendPacket(new GetGuestRoomResultComposer(Session, room.RoomData, true, false));
        }
    }
}
