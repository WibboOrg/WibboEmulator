using Butterfly.HabboHotel.Groups;
using Butterfly.HabboHotel.Rooms;
using Butterfly.HabboHotel.Rooms.AI;
using Butterfly.HabboHotel.Users;
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
                this.WriteInteger(User.BotAI.BaseId);
                this.WriteString(User.BotData.Name);
                this.WriteString(User.BotData.Motto);
                if (User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet)
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
                this.WriteInteger(User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet ? 2 : 4);
                if (User.BotData.AiType == AIType.Pet || User.BotData.AiType == AIType.RolePlayPet)
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
                if (User.GetClient() == null || User.GetClient().GetHabbo() == null)
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
                    Habbo Habbo = User.GetClient().GetHabbo();

                    Group Group = null;
                    if (Habbo != null)
                    {
                        if (Habbo.FavouriteGroupId > 0)
                        {
                            if (!ButterflyEnvironment.GetGame().GetGroupManager().TryGetGroup(Habbo.FavouriteGroupId, out Group))
                            {
                                Group = null;
                            }
                        }
                    }

                    if (User.transfbot)
                    {
                        this.WriteInteger(Habbo.Id);
                        this.WriteString(Habbo.Username);
                        this.WriteString("Beep beep.");
                        this.WriteString(Habbo.Look);
                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
                        this.WriteInteger(0);
                        this.WriteInteger(4);

                        this.WriteString(Habbo.Gender);
                        this.WriteInteger(Habbo.Id);
                        this.WriteString(Habbo.Username);
                        this.WriteInteger(0);
                    }
                    else if (User.transformation)
                    {
                        this.WriteInteger(Habbo.Id);
                        this.WriteString(Habbo.Username);
                        this.WriteString(Habbo.Motto);
                        this.WriteString(User.transformationrace + " 2 2 -1 0 3 4 -1 0");

                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
                        this.WriteInteger(4);
                        this.WriteInteger(2);
                        this.WriteInteger(0);
                        this.WriteInteger(Habbo.Id);
                        this.WriteString(Habbo.Username);
                        this.WriteInteger(1);
                        this.WriteBoolean(false);
                        this.WriteBoolean(false);
                        this.WriteInteger(0);
                        this.WriteInteger(0);
                        this.WriteString("");
                    }
                    else
                    {
                        this.WriteInteger(Habbo.Id);
                        this.WriteString(Habbo.Username);
                        this.WriteString(Habbo.Motto);
                        this.WriteString(Habbo.Look);
                        this.WriteInteger(User.VirtualId);
                        this.WriteInteger(User.X);
                        this.WriteInteger(User.Y);
                        this.WriteString(User.Z.ToString());
                        this.WriteInteger(0);
                        this.WriteInteger(1);
                        this.WriteString(Habbo.Gender.ToLower());

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
                        this.WriteInteger(Habbo.AchievementPoints);
                        this.WriteBoolean(false);
                    }
                }
            }
        }
    }
}
