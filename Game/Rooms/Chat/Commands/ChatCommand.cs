using Butterfly.Core;
using Butterfly.Game.Clients;

namespace Butterfly.Game.Rooms.Chat
{
    public struct ChatCommand
    {
        public readonly int commandID;
        public readonly string input;
        public readonly int minrank;
        public readonly string descriptionFr;
        public readonly string descriptionEn;
        public readonly string descriptionBr;

        public ChatCommand(int CommandID, string Input, int Rank, string DescriptionFr, string DescriptionEn, string DescriptionBr)
        {
            this.commandID = CommandID;
            this.input = Input;
            this.minrank = Rank;
            this.descriptionFr = DescriptionFr;
            this.descriptionEn = DescriptionEn;
            this.descriptionBr = DescriptionBr;
        }

        public bool UserGotAuthorization(Client session)
        {
            if (this.minrank == 0)
            {
                return true;
            }

            if (this.minrank > 0)
            {
                if (this.minrank <= (long)session.GetHabbo().Rank)
                {
                    return true;
                }
            }
            else if (this.minrank < 0)
            {
                if (this.minrank == -1)
                {
                    if (session.GetHabbo().CurrentRoom.CheckRights(session))
                    {
                        return true;
                    }
                }
                else if (this.minrank == -2 && session.GetHabbo().CurrentRoom.CheckRights(session, true))
                {
                    return true;
                }
            }

            return false;
        }

        public bool UserGotAuthorizationStaffLog()
        {
            if (this.minrank > 4)
            {
                return true;
            }

            return false;
        }

        public int UserGotAuthorization2(Client session, Language RoomLangue)
        {
            if (this.minrank == 0)
            {
                return 0;
            }

            if (this.minrank > 2 && session.GetHabbo().Rank < 13 && RoomLangue != session.Langue)
            {
                return 5;
            }

            if (this.minrank > 0)
            {
                if (this.minrank <= session.GetHabbo().Rank)
                {
                    return 0;
                }
                else if (this.minrank == 2)
                {
                    return 2;
                }
            }
            else if (this.minrank < 0)
            {
                if (this.minrank == -1)
                {
                    if (session.GetHabbo().CurrentRoom.CheckRights(session))
                    {
                        return 0;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else if (this.minrank == -2 && session.GetHabbo().CurrentRoom.CheckRights(session, true))
                {
                    return 0;
                }
                else
                {
                    return 4;
                }
            }

            return 1;
        }
    }
}
