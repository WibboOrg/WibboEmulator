using Butterfly.Game.GameClients;
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;
            {
                return;
            }

            try








                /*if (roomUserByHabbo.sentadoBol)
                if (roomUserByHabbo.Statusses.ContainsKey("lay") || roomUserByHabbo.Statusses.ContainsKey("sit"))
                {
                    return;
                }

                if (roomUserByHabbo.RotBody % 2 == 0 || roomUserByHabbo.transformation)
                        {
                            roomUserByHabbo.RotBody = 0;
                        }
                        else
                        {
                            return;
                        }
                    }
                        {
                            return;
                        }
                    }

                    //roomUserByHabbo.AddStatus("lay", Convert.ToString((double) room.GetGameMap().Model.SqFloorHeight[roomUserByHabbo.X, roomUserByHabbo.Y] + 0.85).Replace(",", "."));
                    if (UserRoom.transformation)
                    {
                        roomUserByHabbo.SetStatus("lay", "");
                    }
                    else
                    {
                        roomUserByHabbo.SetStatus("lay", "0.7");
                    }

                    roomUserByHabbo.IsLay = true;