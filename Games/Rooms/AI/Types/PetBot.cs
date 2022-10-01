using WibboEmulator.Core;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Pets;
using WibboEmulator.Games.Rooms.PathFinding;
using System.Drawing;

namespace WibboEmulator.Games.Rooms.AI.Types
{
    public class PetBot : BotAI
    {
        private int _speechTimer;
        private int _actionTimer;
        private int _energyTimer;

        public PetBot(int VirtualId)
        {
            this._speechTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
            this._actionTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 30 + VirtualId);
            this._energyTimer = new Random((VirtualId ^ 2) + DateTime.Now.Millisecond).Next(10, 60);
        }

        private void RemovePetStatus()
        {
            RoomUser roomUser = this.GetRoomUser();
            roomUser.RemoveStatus("sit");
            roomUser.RemoveStatus("lay");
            roomUser.RemoveStatus("snf");
            roomUser.RemoveStatus("eat");
            roomUser.RemoveStatus("ded");
            roomUser.RemoveStatus("jmp");
            roomUser.RemoveStatus("beg");
        }

        public override void OnSelfEnterRoom()
        {
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
            RoomUser roomUser = this.GetRoomUser();
            roomUser.PetData.DBState = DatabaseUpdateState.NEEDS_UPDATE;

            if (roomUser.PetData.OwnerId != User.GetClient().GetUser().Id)
            {
                return;
            }

            if (!Message.ToLower().StartsWith(roomUser.PetData.Name.ToLower() + " "))
            {
                return;
            }

            if (Message.ToLower().Equals(roomUser.PetData.Name.ToLower()))
            {
                roomUser.SetRot(Rotation.Calculate(roomUser.X, roomUser.Y, User.X, User.Y), false);
            }
            else
            {
                string input = Message.Substring(roomUser.PetData.Name.ToLower().Length + 1);
                int randomNumber = WibboEnvironment.GetRandomNumber(1, 8);
                if (roomUser.PetData.Energy > 10 && randomNumber < 6)
                {
                    this.RemovePetStatus();
                    switch (WibboEnvironment.GetGame().GetChatManager().GetPetCommands().TryInvoke(input))
                    {
                        case 0: //Libre
                            this.RemovePetStatus();
                            Point randomWalkableSquare = this.GetRoom().GetGameMap().getRandomWalkableSquare(this.GetBotData().X, this.GetBotData().Y);
                            roomUser.MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
                            roomUser.PetData.AddExpirience(10);
                            break;
                        case 1: //Assis
                            this.RemovePetStatus();
                            roomUser.PetData.AddExpirience(10);
                            roomUser.SetStatus("sit", roomUser.Z.ToString());
                            roomUser.IsSit = true;
                            roomUser.UpdateNeeded = true;
                            this._actionTimer = 25;
                            this._energyTimer = 10;
                            break;
                        case 2: //Couché
                            this.RemovePetStatus();
                            roomUser.SetStatus("lay", roomUser.Z.ToString());
                            roomUser.IsLay = true;
                            roomUser.UpdateNeeded = true;
                            roomUser.PetData.AddExpirience(10);
                            this._actionTimer = 30;
                            this._energyTimer = 5;
                            break;
                        case 3:
                            this.RemovePetStatus();
                            int pX = User.X;
                            int pY = User.Y;
                            this._actionTimer = 30;
                            if (User.RotBody == 4)
                            {
                                pY = User.Y + 1;
                            }
                            else if (User.RotBody == 0)
                            {
                                pY = User.Y - 1;
                            }
                            else if (User.RotBody == 6)
                            {
                                pX = User.X - 1;
                            }
                            else if (User.RotBody == 2)
                            {
                                pX = User.X + 1;
                            }
                            else if (User.RotBody == 3)
                            {
                                pX = User.X + 1;
                                pY = User.Y + 1;
                            }
                            else if (User.RotBody == 1)
                            {
                                pX = User.X + 1;
                                pY = User.Y - 1;
                            }
                            else if (User.RotBody == 7)
                            {
                                pX = User.X - 1;
                                pY = User.Y - 1;
                            }
                            else if (User.RotBody == 5)
                            {
                                pX = User.X - 1;
                                pY = User.Y + 1;
                            }
                            roomUser.PetData.AddExpirience(10);
                            roomUser.MoveTo(pX, pY);
                            break;
                        case 4: //demande
                            this.RemovePetStatus();
                            roomUser.PetData.AddExpirience(10);
                            roomUser.SetRot(Rotation.Calculate(roomUser.X, roomUser.Y, User.X, User.Y), false);
                            roomUser.SetStatus("beg", roomUser.Z.ToString());
                            roomUser.UpdateNeeded = true;
                            this._actionTimer = 25;
                            this._energyTimer = 10;
                            break;
                        case 5:
                            this.RemovePetStatus();
                            roomUser.SetStatus("ded", roomUser.Z.ToString());
                            roomUser.UpdateNeeded = true;
                            roomUser.PetData.AddExpirience(10);
                            this._speechTimer = 45;
                            this._actionTimer = 30;
                            break;
                        case 6:
                            break;
                        case 7:
                            break;
                        case 8:
                            break;
                        case 9:
                            this.RemovePetStatus();
                            roomUser.SetStatus("jmp", roomUser.Z.ToString());
                            roomUser.UpdateNeeded = true;
                            roomUser.PetData.AddExpirience(10);
                            this._energyTimer = 5;
                            this._speechTimer = 10;
                            this._actionTimer = 5;
                            break;
                        case 10:
                            break;
                        case 11:
                            break;
                        case 12:
                            break;
                        case 13: //Panier ?
                            this.RemovePetStatus();
                            roomUser.OnChat("ZzzZZZzzzzZzz", 0, false);
                            roomUser.SetStatus("lay", roomUser.Z.ToString());
                            roomUser.IsLay = true;
                            roomUser.UpdateNeeded = true;
                            roomUser.PetData.AddExpirience(10);
                            this._energyTimer = 5;
                            this._speechTimer = 30;
                            this._actionTimer = 45;
                            break;
                        case 14:
                            break;
                        case 15:
                            break;
                        case 16:
                            break;
                        case 17:
                            break;
                        case 18:
                            break;
                        case 19:
                            break;
                        case 20:
                            break;
                        default:
                            string[] strArray = WibboEnvironment.GetLanguageManager().TryGetValue("pet.unknowncommand", roomUser.Room.RoomData.Langue).Split(new char[1] { ',' });
                            roomUser.OnChat(strArray[WibboEnvironment.GetRandomNumber(0, strArray.Length - 1)], 0, false);
                            break;
                    }
                    roomUser.PetData.PetEnergy(false);
                    roomUser.PetData.PetEnergy(false);
                }
                else
                {
                    this.RemovePetStatus();
                    if (!roomUser.Room.RoomMutePets)
                    {
                        if (roomUser.PetData.Energy < 10)
                        {
                            string[] strArray = WibboEnvironment.GetLanguageManager().TryGetValue("pet.tired", roomUser.Room.RoomData.Langue).Split(new char[1] { ',' });
                            
                            roomUser.OnChat(strArray[WibboEnvironment.GetRandomNumber(0, strArray.Length - 1)], 0, false);
                            roomUser.SetStatus("lay", roomUser.Z.ToString());
                            roomUser.IsLay = true;
                            this._speechTimer = 50;
                            this._actionTimer = 45;
                            this._energyTimer = 5;
                        }
                        else
                        {
                            string[] strArray = WibboEnvironment.GetLanguageManager().TryGetValue("pet.lazy", roomUser.Room.RoomData.Langue).Split(new char[1] { ',' });
                            
                            roomUser.OnChat(strArray[WibboEnvironment.GetRandomNumber(0, strArray.Length - 1)], 0, false);
                            roomUser.PetData.PetEnergy(false);
                        }
                    }
                }
            }
        }

        public override void OnUserShout(RoomUser User, string Message)
        {
        }

        public override void OnTimerTick()
        {
            if (this._actionTimer <= 0)
            {
                try
                {
                    this.RemovePetStatus();
                    this._actionTimer = WibboEnvironment.GetRandomNumber(10, 60);
                    if (!this.GetRoomUser().RidingHorse && this.GetRoomUser().PetData.Type != 16)
                    {
                        this.RemovePetStatus();
                        Point randomWalkableSquare = this.GetRoom().GetGameMap().getRandomWalkableSquare(this.GetBotData().X, this.GetBotData().Y);
                        this.GetRoomUser().MoveTo(randomWalkableSquare.X, randomWalkableSquare.Y);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.HandleException(ex, "PetBot.OnTimerTick");
                }
            }
            else
            {
                --this._actionTimer;
            }

            if (this._energyTimer <= 0)
            {
                this.RemovePetStatus();
                this.GetRoomUser().PetData.PetEnergy(true);
                this._energyTimer = WibboEnvironment.GetRandomNumber(30, 120);
            }
            else
            {
                --this._energyTimer;
            }

            if (this.GetBotData().FollowUser != 0)
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
                        this.GetRoomUser().MoveTo(user.X, user.Y, true);
                    }
                }
            }
        }
    }
}
