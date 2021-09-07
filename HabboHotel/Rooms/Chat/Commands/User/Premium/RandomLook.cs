using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.GameClients;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd
            {
                return;
            }

            //GameClient Client = ButterflyEnvironment.GetGame().GetClientManager().GetRandomClient();
            //if (Client == null || Client.GetHabbo() == null)
            //return;

            if (Session == null || Session.GetHabbo() == null)
            {
                return;
            }

            using (IQueryAdapter queryreactor = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                return;
            }

            if (!Session.GetHabbo().InRoom)
            {
                return;
            }

            Room currentRoom = Session.GetHabbo().CurrentRoom;
            {
                return;
            }

            RoomUser roomUserByHabbo = UserRoom;
            {
                return;
            }

            Session.SendPacket(new UserChangeComposer(roomUserByHabbo, true));