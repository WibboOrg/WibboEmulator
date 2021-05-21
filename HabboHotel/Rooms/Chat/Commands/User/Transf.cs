using Butterfly.Communication.Packets.Outgoing.Structure;
using Butterfly.HabboHotel.GameClients;using Butterfly.HabboHotel.Rooms.Games;
using System;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd{    internal class Transf : IChatCommand    {        public void Execute(GameClient Session, Room Room, RoomUser UserRoom, string[] Params)        {
            if (UserRoom.Team != Team.none || UserRoom.InGame)
            {
                return;
            }

            if (Session.GetHabbo().SpectatorMode || UserRoom.InGame)
            {
                return;
            }

            if (Params.Length == 3 || Params.Length == 2)            {                if (Room != null)                {                    int raceid = 0;                    if (Params.Length == 3)                    {                        string x = Params[2];                        if (int.TryParse(x, out int value))                        {                            raceid = Convert.ToInt32(Params[2]);                            if (raceid < 1 || raceid > 50)                            {                                raceid = 0;                            }                        }                    }                    else                    {                        raceid = 0;                    }                    if (!UserRoom.SetPetTransformation(Params[1], raceid))                    {                        Session.SendHugeNotif(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", Session.Langue));                        return;                    }                    UserRoom.transformation = true;                    Room.SendPacket(new UserRemoveComposer(UserRoom.VirtualId));
                    Room.SendPacket(new UsersComposer(UserRoom));                }            }            else            {                Session.SendHugeNotif(ButterflyEnvironment.GetLanguageManager().TryGetValue("cmd.transf.help", Session.Langue));            }        }    }}