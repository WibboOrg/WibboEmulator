using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;

using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;
            {
                return;
            }

            string username = Params[1];
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)
                {
                    return;
                }

                RoomUser roomUserByHabbo = currentRoom2.GetRoomUserManager().GetRoomUserByHabboId(clientByUsername.GetHabbo().Id);
                {
                    return;
                }

                currentRoom2.SendPacket(new UserChangeComposer(roomUserByHabbo, false));