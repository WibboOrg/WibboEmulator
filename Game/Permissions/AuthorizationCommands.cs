using Wibbo.Core;
using Wibbo.Game.Clients;

namespace Wibbo.Game.Permissions
{
    public struct AuthorizationCommands
    {
        public readonly int CommandID;
        public readonly string Input;
        public readonly int MinRank;
        public readonly string DescriptionFr;
        public readonly string DescriptionEn;
        public readonly string DescriptionBr;

        public AuthorizationCommands(int commandID, string input, int rank, string descriptionFr, string descriptionEn, string descriptionBr)
        {
            this.CommandID = commandID;
            this.Input = input;
            this.MinRank = rank;
            this.DescriptionFr = descriptionFr;
            this.DescriptionEn = descriptionEn;
            this.DescriptionBr = descriptionBr;
        }

        public bool UserGotAuthorization(Client session)
        {
            if (this.MinRank == 0)
            {
                return true;
            }

            if (this.MinRank > 0)
            {
                if (this.MinRank <= (long)session.GetUser().Rank)
                {
                    return true;
                }
            }
            else if (this.MinRank < 0)
            {
                if (this.MinRank == -1)
                {
                    if (session.GetUser().CurrentRoom.CheckRights(session))
                    {
                        return true;
                    }
                }
                else if (this.MinRank == -2 && session.GetUser().CurrentRoom.CheckRights(session, true))
                {
                    return true;
                }
            }

            return false;
        }

        public bool UserGotAuthorizationStaffLog()
        {
            if (this.MinRank >= 4)
            {
                return true;
            }

            return false;
        }

        public int UserGotAuthorization2(Client session, Language RoomLangue)
        {
            if (this.MinRank == 0)
            {
                return 0;
            }

            if (this.MinRank > 2 && session.GetUser().Rank < 13 && RoomLangue != session.Langue)
            {
                return 5;
            }

            if (this.MinRank > 0)
            {
                if (this.MinRank <= session.GetUser().Rank)
                {
                    return 0;
                }
                else if (this.MinRank == 2)
                {
                    return 2;
                }
            }
            else if (this.MinRank < 0)
            {
                if (this.MinRank == -1)
                {
                    if (session.GetUser().CurrentRoom.CheckRights(session))
                    {
                        return 0;
                    }
                    else
                    {
                        return 3;
                    }
                }
                else if (this.MinRank == -2 && session.GetUser().CurrentRoom.CheckRights(session, true))
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
