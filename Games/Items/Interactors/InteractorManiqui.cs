namespace WibboEmulator.Games.Items.Interactors;
using WibboEmulator.Communication.Packets.Outgoing.Avatar;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Engine;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Rooms;

public class InteractorManiqui : FurniInteractor
{
    public override void OnPlace(GameClient Session, Item item)
    {
    }

    public override void OnRemove(GameClient Session, Item item)
    {
    }

    public override void OnTrigger(GameClient Session, Item item, int request, bool userHasRights, bool reverse)
    {
        if (Session == null || Session.User == null || item == null)
        {
            return;
        }

        if (!item.ExtraData.Contains(';'))
        {
            return;
        }

        var allowedParts = new List<string> { "ha", "he", "ea", "ch", "fa", "cp", "lg", "cc", "ca", "sh", "wa" };
        var look = string.Join(".", Session.User.Look.Split('.').Where(part => !allowedParts.Contains(part.Split('-')[0])));
        var stuff = item.ExtraData.Split(';');

        var newLook = look + "." + stuff[1];

        Session.User.Look = FigureDataManager.ProcessFigure(newLook, Session.User.Gender, true);

        if (!RoomManager.TryGetRoom(Session.User.RoomId, out var room))
        {
            return;
        }

        var roomUser = room.RoomUserManager.GetRoomUserByUserId(Session.User.Id);
        if (roomUser == null)
        {
            return;
        }

        if (roomUser.IsTransf || roomUser.IsSpectator)
        {
            return;
        }

        if (!Session.User.InRoom)
        {
            return;
        }

        Session.SendPacket(new FigureUpdateComposer(Session.User.Look, Session.User.Gender));
        room.SendPacket(new UserChangeComposer(roomUser, false));
    }

    public override void OnTick(Item item)
    {
    }
}
