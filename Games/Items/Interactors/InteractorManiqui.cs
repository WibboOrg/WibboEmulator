namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Database.Daos.User;
using WibboEmulator.Games.GameClients;

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
        if (session == null || session.GetUser() == null || item == null)
        {
            return;
        }

        if (!item.ExtraData.Contains(';'))
        {
            return;
        }

        var lookSplit = session.GetUser().Look.Split(new char[1] { '.' });
        var lookCode = "";
        foreach (var part in lookSplit)
        {
            if (!part.StartsWith("ch") && !part.StartsWith("lg") && !part.StartsWith("cc") && !part.StartsWith("ca") && !part.StartsWith("sh") && !part.StartsWith("wa"))
            {
                lookCode = lookCode + part + ".";
            }
        }

        var look = lookCode + item.ExtraData.Split(new char[1] { ';' })[1];
        session.GetUser().Look = look;

        using (var dbClient = WibboEnvironment.GetDatabaseManager().GetQueryReactor())
        {
            UserDao.UpdateLook(dbClient, session.GetUser().Id, look);
        }


        if (!WibboEnvironment.GetGame().GetRoomManager().TryGetRoom(session.GetUser().CurrentRoomId, out var room))
        {
            return;
        }

        var roomUser = room.GetRoomUserManager().GetRoomUserByUserId(session.GetUser().Id);
        if (roomUser == null)
        {
            return;
        }

        if (roomUser.IsTransf || roomUser.IsSpectator)
        {
            return;
        }

        if (!session.GetUser().InRoom)
        {
            return;
        }

        session.SendPacket(new FigureUpdateComposer(session.GetUser().Look, session.GetUser().Gender));
        room.SendPacket(new UserChangeComposer(roomUser, false));
    }

    public override void OnTick(Item item)
    {
    }
}
