using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Communication.Packets.Outgoing.Avatar;
using Butterfly.Communication.Packets.Outgoing.BuildersClub;
using Butterfly.Communication.Packets.Outgoing.Camera;
using Butterfly.Communication.Packets.Outgoing.Catalog;
using Butterfly.Communication.Packets.Outgoing.GameCenter;
using Butterfly.Communication.Packets.Outgoing.Groups;
using Butterfly.Communication.Packets.Outgoing.Handshake;
using Butterfly.Communication.Packets.Outgoing.Help;
using Butterfly.Communication.Packets.Outgoing.Inventory;
using Butterfly.Communication.Packets.Outgoing.Inventory.Achievements;
using Butterfly.Communication.Packets.Outgoing.Inventory.AvatarEffects;
using Butterfly.Communication.Packets.Outgoing.Inventory.Badges;
using Butterfly.Communication.Packets.Outgoing.Inventory.Bots;
using Butterfly.Communication.Packets.Outgoing.Inventory.Furni;
using Butterfly.Communication.Packets.Outgoing.Inventory.Pets;
using Butterfly.Communication.Packets.Outgoing.Inventory.Purse;
using Butterfly.Communication.Packets.Outgoing.Inventory.Trading;
using Butterfly.Communication.Packets.Outgoing.LandingView;
using Butterfly.Communication.Packets.Outgoing.MarketPlace;
using Butterfly.Communication.Packets.Outgoing.Messenger;
using Butterfly.Communication.Packets.Outgoing.Misc;
using Butterfly.Communication.Packets.Outgoing.Moderation;
using Butterfly.Communication.Packets.Outgoing.Navigator;
using Butterfly.Communication.Packets.Outgoing.Notifications;
using Butterfly.Communication.Packets.Outgoing.Pets;
using Butterfly.Communication.Packets.Outgoing.Quests;
using Butterfly.Communication.Packets.Outgoing.Rooms;
using Butterfly.Communication.Packets.Outgoing.Rooms.Action;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Bots;
using Butterfly.Communication.Packets.Outgoing.Rooms.AI.Pets;
using Butterfly.Communication.Packets.Outgoing.Rooms.Avatar;
using Butterfly.Communication.Packets.Outgoing.Rooms.Chat;
using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Communication.Packets.Outgoing.Rooms.FloorPlan;
using Butterfly.Communication.Packets.Outgoing.Rooms.Freeze;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Furni;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Moodlight;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Stickys;
using Butterfly.Communication.Packets.Outgoing.Rooms.Furni.Wired;
using Butterfly.Communication.Packets.Outgoing.Rooms.Notifications;
using Butterfly.Communication.Packets.Outgoing.Rooms.Permissions;
using Butterfly.Communication.Packets.Outgoing.Rooms.Session;
using Butterfly.Communication.Packets.Outgoing.Rooms.Settings;
using Butterfly.Communication.Packets.Outgoing.Rooms.Wireds;
using Butterfly.Communication.Packets.Outgoing.Sound;
using Butterfly.Communication.Packets.Outgoing.Users;
using Butterfly.Communication.Packets.Outgoing.WebSocket;

using Butterfly.HabboHotel.GameClients;
using System.Drawing;

namespace Butterfly.HabboHotel.Rooms.RoomBots
{
    public class SuperBot : BotAI
    {
        private readonly int _virtualId;
        private readonly int _speechTimer;
        private int _actionTimer;

        public SuperBot(int VirtualId)
        {
            this._virtualId = VirtualId;
            this._speechTimer = ButterflyEnvironment.GetRandomNumber(30, 120);
            this._actionTimer = ButterflyEnvironment.GetRandomNumber(0, 60);
        }

        public override void OnSelfEnterRoom()
        {
            this.GetRoomUser().MoveTo(this.GetRoomUser().X + ButterflyEnvironment.GetRandomNumber(-10, 10), this.GetRoomUser().Y + ButterflyEnvironment.GetRandomNumber(-10, 10), true);
        }

        public override void OnSelfLeaveRoom(bool Kicked)
        {
        }

        public override void OnUserEnterRoom(RoomUser User)
        {
        }

        public override void OnUserLeaveRoom(GameClient Client)
        {
        }

        public override void OnUserSay(RoomUser User, string Message)
        {
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
            if (this.GetBotData() == null)
            {
                return;
            }

            RoomUser OwnerUser = this.GetRoom().GetRoomUserManager().GetRoomUserByHabboId((this.GetBotData().OwnerId == 0) ? this.GetRoom().RoomData.OwnerId : this.GetBotData().OwnerId);
            if (OwnerUser == null)
            {
                this.GetRoom().GetRoomUserManager().RemoveBot(this._virtualId, false);

                return;
            }

            if (this._actionTimer <= 0)
            {
                if (this.GetBotData().FollowUser == 0)
                {
                    Point randomWalkableSquare = this.GetRoom().GetGameMap().getRandomWalkableSquare(this.GetRoomUser().GoalX, this.GetRoomUser().GoalY);
                    this.GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
                }

                this._actionTimer = ButterflyEnvironment.GetRandomNumber(10, 60);
            }
            else
            {
                this._actionTimer--;
            }

            if (OwnerUser.DanceId != this.GetRoomUser().DanceId)
            {
                this.GetRoomUser().DanceId = OwnerUser.DanceId;
                this.GetRoom().SendPacket(new DanceComposer(this.GetRoomUser(), this.GetRoomUser().DanceId));
            }
            else if (OwnerUser.IsAsleep != this.GetRoomUser().IsAsleep)
            {
                this.GetRoomUser().IsAsleep = OwnerUser.IsAsleep;
                this.GetRoom().SendPacket(new SleepComposer(this.GetRoomUser(), this.GetRoomUser().IsAsleep));
            }
            else if (OwnerUser.CarryItemID != this.GetRoomUser().CarryItemID)
            {
                this.GetRoomUser().CarryItemID = OwnerUser.CarryItemID;
                this.GetRoom().SendPacket(new CarryObjectComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().CarryItemID));
            }
            else if (OwnerUser.CurrentEffect != this.GetRoomUser().CurrentEffect)
            {
                this.GetRoomUser().CurrentEffect = OwnerUser.CurrentEffect;
                this.GetRoom().SendPacket(new AvatarEffectComposer(this.GetRoomUser().VirtualId, this.GetRoomUser().CurrentEffect));
            }
            /*else if (OwnerUser.GetClient().GetHabbo().Look != this.GetRoomUser().BotData.Look)
            {
                this.GetRoomUser().BotData.Look = OwnerUser.GetClient().GetHabbo().Look;
                this.GetRoomUser().BotData.Gender = OwnerUser.GetClient().GetHabbo().Gender;

                this.GetRoom().SendPacket(new UserChangeComposer(this.GetRoomUser()));
            }*/

            if (this.GetBotData().FollowUser > 0)
            {
                RoomUser user = this.GetRoom().GetRoomUserManager().GetRoomUserByVirtualId(this.GetBotData().FollowUser);
                if (user == null)
                {
                    this.GetBotData().FollowUser = 0;
                }
                else
                {
                    if (!Gamemap.TilesTouching(this.GetRoomUser().X, this.GetRoomUser().Y, user.X, user.Y))
                    {
                        int NewX = user.X;
                        int NewY = user.Y;

                        switch (ButterflyEnvironment.GetRandomNumber(1, 3))
                        {
                            case 1:
                                NewY--;
                                break;
                            case 2:
                                NewY++;
                                break;
                            case 3:
                                break;
                        }

                        switch (ButterflyEnvironment.GetRandomNumber(1, 3))
                        {
                            case 1:
                                NewX--;
                                break;
                            case 2:
                                NewX++;
                                break;
                            case 3:
                                break;
                        }

                        this.GetRoomUser().MoveTo(NewX, NewY, true);
                    }
                }
            }
        }
    }
}
