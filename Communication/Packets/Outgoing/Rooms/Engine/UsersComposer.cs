using WibboEmulator.Games.Groups;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Games.GameClients;

namespace WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomUser> users)
            : base(ServerPacketHeader.UNIT)
        {
            this.WriteInteger(users.Count);
            foreach (RoomUser User in users.ToList())
            {
                this.WriteUser(User);
            }
        }

        public UsersComposer(RoomUser user)
            : base(ServerPacketHeader.UNIT)
        {
            this.WriteInteger(1);//1 avatar
            this.WriteUser(user);
        }

        private void WriteUser(RoomUser roomUser)
        {
            if (roomUser.IsBot)
            {
                this.WriteInteger(roomUser.BotAI.Id);
                this.WriteString(roomUser.BotData.Name);
                this.WriteString(roomUser.BotData.Motto);
                if (roomUser.BotData.AiType == BotAIType.Pet || roomUser.BotData.AiType == BotAIType.RoleplayPet)
                {
                    this.WriteString(roomUser.BotData.Look.ToLower() + ((roomUser.PetData.Saddle > 0) ? " 3 2 " + roomUser.PetData.PetHair + " " + roomUser.PetData.HairDye + " 3 " + roomUser.PetData.PetHair + " " + roomUser.PetData.HairDye + " 4 " + roomUser.PetData.Saddle + " 0" : " 2 2 " + roomUser.PetData.PetHair + " " + roomUser.PetData.HairDye + " 3 " + roomUser.PetData.PetHair + " " + roomUser.PetData.HairDye + ""));
                }
                else
                {
                    this.WriteString(roomUser.BotData.Look);
                }

                this.WriteInteger(roomUser.VirtualId);
                this.WriteInteger(roomUser.X);
                this.WriteInteger(roomUser.Y);
                this.WriteString(roomUser.Z.ToString());
                this.WriteInteger(2);
                this.WriteInteger(roomUser.BotData.AiType == BotAIType.Pet || roomUser.BotData.AiType == BotAIType.RoleplayPet ? 2 : 4);
                if (roomUser.BotData.AiType == BotAIType.Pet || roomUser.BotData.AiType == BotAIType.RoleplayPet)
                {
                    this.WriteInteger(roomUser.PetData.Type);
                    this.WriteInteger(roomUser.PetData.OwnerId);
                    this.WriteString(roomUser.PetData.OwnerName);
                    this.WriteInteger(1);
                    this.WriteBoolean(roomUser.PetData.Saddle > 0);
                    this.WriteBoolean(roomUser.RidingHorse);
                    this.WriteInteger(0);
                    this.WriteInteger(0);
                    this.WriteString("");
                }
                else
                {
                    this.WriteString(roomUser.BotData.Gender);
                    this.WriteInteger(roomUser.BotData.OwnerId);
                    this.WriteString(roomUser.BotData.OwnerName);

                    //List<int> ActionIds = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 24, 25 };
                    List<int> ActionIds = new List<int>() { 1, 2, 3, 4, 5, 6 };
                    this.WriteInteger(ActionIds.Count);
                    foreach (int id in ActionIds)
                    {
                        this.WriteShort(id);
                    }
                }
            }
            else
            {
                if (roomUser.GetClient() == null || roomUser.GetClient().GetUser() == null)
                {
                    this.WriteInteger(0);
                    this.WriteString("");
                    this.WriteString("");
                    this.WriteString("");
                    this.WriteInteger(roomUser.VirtualId);
                    this.WriteInteger(roomUser.X);
                    this.WriteInteger(roomUser.Y);
                    this.WriteString(roomUser.Z.ToString());
                    this.WriteInteger(0);
                    this.WriteInteger(1);
                    this.WriteString("M");
                    this.WriteInteger(0);
                    this.WriteInteger(0);
                    this.WriteString("");

                    this.WriteString("");//Whats this?
                    this.WriteInteger(0);
                    this.WriteBoolean(false);
                }
                else
                {
                    User user = roomUser.GetClient().GetUser();

                    Group Group = null;
                    if (user != null)
                    {
                        if (user.FavouriteGroupId > 0)
                        {
                            if (!WibboEnvironment.GetGame().GetGroupManager().TryGetGroup(user.FavouriteGroupId, out Group))
                            {
                                Group = null;
                            }
                        }
                    }

                    if (roomUser.TransfBot)
                    {
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteString("Beep beep.");
                        this.WriteString(user.Look);
                        this.WriteInteger(roomUser.VirtualId);
                        this.WriteInteger(roomUser.X);
                        this.WriteInteger(roomUser.Y);
                        this.WriteString(roomUser.Z.ToString());
                        this.WriteInteger(0);
                        this.WriteInteger(4);

                        this.WriteString(user.Gender);
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteInteger(0);
                    }
                    else if (roomUser.IsTransf)
                    {
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteString(user.Motto);
                        this.WriteString(roomUser.TransfRace + " 2 2 -1 0 3 4 -1 0");

                        this.WriteInteger(roomUser.VirtualId);
                        this.WriteInteger(roomUser.X);
                        this.WriteInteger(roomUser.Y);
                        this.WriteString(roomUser.Z.ToString());
                        this.WriteInteger(4);
                        this.WriteInteger(2);
                        this.WriteInteger(0);
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteInteger(1);
                        this.WriteBoolean(false);
                        this.WriteBoolean(false);
                        this.WriteInteger(0);
                        this.WriteInteger(0);
                        this.WriteString("");
                    }
                    else
                    {
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteString(user.Motto);
                        this.WriteString(user.Look);
                        this.WriteInteger(roomUser.VirtualId);
                        this.WriteInteger(roomUser.X);
                        this.WriteInteger(roomUser.Y);
                        this.WriteString(roomUser.Z.ToString());
                        this.WriteInteger(0);
                        this.WriteInteger(1);
                        this.WriteString(user.Gender.ToLower());

                        if (Group != null)
                        {
                            this.WriteInteger(Group.Id);
                            this.WriteInteger(0);
                            this.WriteString(Group.Name);
                        }
                        else
                        {
                            this.WriteInteger(0);
                            this.WriteInteger(0);
                            this.WriteString("");
                        }

                        this.WriteString("");//Whats this?
                        this.WriteInteger(user.AchievementPoints);
                        this.WriteBoolean(false);
                    }
                }
            }
        }
    }
}
