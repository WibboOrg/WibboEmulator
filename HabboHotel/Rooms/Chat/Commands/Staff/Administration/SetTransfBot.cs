using Butterfly.Communication.Packets.Outgoing.Rooms.Engine;
using Butterfly.HabboHotel.GameClients;
            {
                return;
            }

            string username = Params[1];
            {
                return;
            }

            if (Session.Langue != roomUserByHabbo.GetClient().Langue)
            {
                UserRoom.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue(string.Format("cmd.authorized.langue.user", roomUserByHabbo.GetClient().Langue), Session.Langue));
                return;
            }
                    RoomClient.SendPacket(new UsersComposer(roomUserByHabbo));
                }