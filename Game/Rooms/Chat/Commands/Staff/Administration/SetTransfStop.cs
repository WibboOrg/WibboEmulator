using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Game.GameClients;
            {
                return;
            }

            string username = Params[1];
            {
                return;
            }

            if (roomUserByHabbo.transformation && !roomUserByHabbo.IsSpectator)
                    RoomClient.SendPacket(new UsersComposer(roomUserByHabbo));