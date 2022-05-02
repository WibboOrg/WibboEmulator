using Butterfly.Game.Groups;
using Butterfly.Game.Rooms;
using Butterfly.Game.Rooms.AI;
using Butterfly.Game.Users;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Communication.Packets.Outgoing.Rooms.Engine
{
    internal class UsersComposer : ServerPacket
    {
        public UsersComposer(ICollection<RoomUser> Users)
            : base(ServerPacketHeader.UNIT)
        {
            this.WriteInteger(Users.Count);
            foreach (RoomUser User in Users.ToList())
            {
                this.WriteUser(User);
            }
        }

        public UsersComposer(RoomUser User)
            : base(ServerPacketHeader.UNIT)
        {
            this.WriteInteger(1);//1 avatar
            this.WriteUser(User);
        }

        private void WriteUser(RoomUser User)
        {
            if (User.IsBot)
            {
                this.WriteInteger(User.BotAI.Id);
                this.WriteString(User.BotData.Name);
                this.WriteString(User.BotData.Motto);
                if (User.BotData.AiType == BotAIType.Pet || User.BotData.AiType == BotAIType.RoleplayPet)
                {
                    this.WriteString(User.BotData.Look.ToLower() + ((User.PetData.Saddle > 0) ? " 3 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 4 " + User.PetData.Saddle + " 0" : " 2 2 " + User.PetData.PetHair + " " + User.PetData.HairDye + " 3 " + User.PetData.PetHair + " " + User.PetData.HairDye + ""));
                }
                else
                {
                    this.WriteString(User.BotData.Look);
                }

                this.WriteInteger(User.VirtualId);
                this.WriteInteger(User.X);
                this.WriteInteger(User.Y);
                this.WriteString(User.Z.ToString());
                this.WriteInteger(2);
                this.WriteInteger(User.BotData.AiType == BotAIType.Pet || User.BotData.AiType == BotAIType.RoleplayPet ? 2 : 4);
                if (User.BotData.AiType == BotAIType.Pet || User.BotData.AiType == BotAIType.RoleplayPet)
                {
                    this.WriteInteger(User.PetData.Type);
                    this.WriteInteger(User.PetData.OwnerId);
                    this.WriteString(User.PetData.OwnerName);
                    this.WriteInteger(1);
                    this.WriteBoolean(User.PetData.Saddle > 0);
                    this.WriteBoolean(User.RidingHorse);
                    this.WriteInteger(0);
                    this.WriteInteger(0);
                    this.WriteString("");
                }
                else
                {
                    this.WriteString(User.BotData.Gender);
                    this.WriteInteger(User.BotData.OwnerId);
                    this.WriteString(User.BotData.OwnerName);

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
                if (User.GetClient() == null || User.GetClient().GetUser() == null)
                {
                    this.WriteInteger(0);
                    this.WriteString("");
                    this.WriteString("");
                    this.WriteString("");
                    this.WriteInteger(User.VirtualId);
                    this.WriteInteger(User.X);
                    this.WriteInteger(User.Y);
                    this.WriteString(User.Z.ToString());
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
                    User user = User.GetClient().GetUser();

                    Group Group = null;
                    if (user != null)
                    {
                        if (user.FavouriteGroupId > 0)
                        {
                            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(user.FavouriteGroupId, out Group))
                            {
                                Group = null;
                            }
                        }
                    }

                    if (User.TransfBot)
                    {
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteString("Beep beep.");
                        this.WriteString(user.Look);
                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
                        this.WriteInteger(0);
                        this.WriteInteger(4);

                        this.WriteString(user.Gender);
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteInteger(0);
                    }
                    else if (User.IsTransf)
                    {
                        this.WriteInteger(user.Id);
                        this.WriteString(user.Username);
                        this.WriteString(user.Motto);
                        this.WriteString(User.TransfRace + " 2 2 -1 0 3 4 -1 0");

                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
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
                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
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
