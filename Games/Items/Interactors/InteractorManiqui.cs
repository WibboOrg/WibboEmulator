namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class InteractorManiqui : FurniInteractor
{
    public override void OnPlace(GameClient session, Item item)
    {
    }

    public override void OnRemove(GameClient session, Item item)
    {
    }

    public override void OnTrigger(GameClient session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (session == null || session.User == null || item == null)
        {
            return;
        }

        if (!item.ExtraData.Contains(';'))
        {
            return;
        }

        var lookSplit = session.User.Look.Split('.');
        var lookCode = "";
        foreach (var part in lookSplit)
        {
            if (!part.StartsWith("ch") && !part.StartsWith("lg") && !part.StartsWith("cc") && !part.StartsWith("ca") && !part.StartsWith("sh") && !part.StartsWith("wa"))
            {
                lookCode = lookCode + part + ".";
            }
        }

        var look = lookCode + item.ExtraData.Split(';')[1];
        session.User.Look = look;

        using (var dbClient = DatabaseManager.Connection)
        {
            UserDao.UpdateLook(dbClient, session.User.Id, look);
        }


        if (!RoomManager.TryGetRoom(session.User.RoomId, out var room))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(session.User.Id);
        if (roomUser == null)
        {
            return;
        }

        if (roomUser.IsTransf || roomUser.IsSpectator)
        {
            return;
        }

        if (!session.User.InRoom)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.User.Look, session.User.Gender));
        room.SendPacket(new UserChangeComposer(roomUser, false));
    }

    public override void OnTick(Item item)
    {
    }
}
