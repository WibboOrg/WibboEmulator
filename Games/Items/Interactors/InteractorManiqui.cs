namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.FigureData;
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

        var allowedParts = new List<string> { "ha", "he", "ea", "ch", "fa", "cp", "lg", "cc", "ca", "sh", "wa" };
        var look = string.Join(".", session.User.Look.Split('.').Where(part => !allowedParts.Contains(part.Split('-')[0])));
        var stuff = item.ExtraData.Split(';');

        var newLook = look + "." + stuff[1];

        session.User.Look = FigureDataManager.ProcessFigure(newLook, session.User.Gender, true);

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
