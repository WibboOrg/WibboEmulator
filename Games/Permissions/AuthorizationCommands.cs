namespace WibboEmulator.Games.Permissions;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public struct AuthorizationCommands
{
    public int CommandID { get; private set; }
    public string Input { get; private set; }
    public int MinRank { get; private set; }
    public string DescriptionFr { get; private set; }
    public string DescriptionEn { get; private set; }
    public string DescriptionBr { get; private set; }

    public AuthorizationCommands(int commandID, string input, int rank, string descriptionFr, string descriptionEn, string descriptionBr)
    {
        this.CommandID = commandID;
        this.Input = input;
        this.MinRank = rank;
        this.DescriptionFr = descriptionFr;
        this.DescriptionEn = descriptionEn;
        this.DescriptionBr = descriptionBr;
    }

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

        if (this.MinRank > 2 && session.User.Rank < 13 && room.RoomData.Langue != session.Langue)
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
