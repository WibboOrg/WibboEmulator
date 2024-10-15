namespace WibboEmulator.Games.Permissions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public struct AuthorizationCommands(int commandID, string input, int rank, string descriptionFr, string descriptionEn, string descriptionBr)
{
    public int CommandID { get; private set; } = commandID;
    public string Input { get; private set; } = input;
    public int MinRank { get; private set; } = rank;
    public string DescriptionFr { get; private set; } = descriptionFr;
    public string DescriptionEn { get; private set; } = descriptionEn;
    public string DescriptionBr { get; private set; } = descriptionBr;

    public readonly bool UserGotAuthorization(GameClient session, Room room)
    {
        if (this.MinRank == 0)
        {
            return true;
        }

        if (this.MinRank > 0)
        {
            if (this.MinRank <= (long)session.User.Rank)
            {
                return true;
            }
        }
        else if (this.MinRank < 0)
        {
            if (this.MinRank == -1)
            {
                if (room.CheckRights(session))
                {
                    return true;
                }
            }
            else if (this.MinRank == -2 && room.CheckRights(session, true))
            {
                return true;
            }
        }

        return false;
    }

    public readonly bool UserGotAuthorizationStaffLog()
    {
        if (this.MinRank >= 3)
        {
            return true;
        }

        return false;
    }

    public readonly int UserGotAuthorizationType(GameClient session, Room room)
    {
        if (this.MinRank == 0)
        {
            return 0;
        }

        if (this.MinRank > 2 && session.User.Rank < 13 && room.RoomData.Language != session.Language)
        {
            return 5;
        }

        if (this.MinRank > 0)
        {
            if (this.MinRank <= session.User.Rank)
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
                if (room.CheckRights(session))
                {
                    return 0;
                }
                else
                {
                    return 3;
                }
            }
            else if (this.MinRank == -2 && room.CheckRights(session, true))
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
