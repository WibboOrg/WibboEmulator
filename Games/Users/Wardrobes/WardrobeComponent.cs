namespace WibboEmulator.Games.Users.Wardrobes;
using System.Data;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Database;
using WibboEmulator.Database.Daos.User;

public class WardrobeComponent : IDisposable
{
    private static readonly int MAX_SLOT = 24;
    private readonly User _user;

    public WardrobeComponent(User user)
    {
        this._user = user;
        this.Wardrobes = new Dictionary<int, Wardrobe>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        this.Wardrobes.Clear();

        var userWardrobeList = UserWardrobeDao.GetAll(dbClient, this._user.Id);

        foreach (var userWardrobe in userWardrobeList)
        {
            var slotId = userWardrobe.SlotId;

            if (this.Wardrobes.ContainsKey(slotId))
            {
                continue;
            }

            if (slotId < 1 || slotId > MAX_SLOT)
            {
                continue;
            }

            var wardrobe = new Wardrobe(slotId, userWardrobe.Look, userWardrobe.Gender!.ToUpper());
            this.Wardrobes.Add(slotId, wardrobe);
        }
    }

    internal void AddWardrobe(string look, string gender, int slotId)
    {
        if (slotId < 1 || slotId > MAX_SLOT)
        {
            return;
        }

        gender = gender.ToUpper();

        if (gender is not "M" and not "F")
        {
            return;
        }

        if (this.Wardrobes.ContainsKey(slotId))
        {
            _ = this.Wardrobes.Remove(slotId);
        }

        look = FigureDataManager.ProcessFigure(look, gender, true);

        var wardrobe = new Wardrobe(slotId, look, gender);
        this.Wardrobes.Add(slotId, wardrobe);

        using var dbClient = DatabaseManager.Connection;
        UserWardrobeDao.Insert(dbClient, this._user.Id, slotId, look, gender);
    }

    public Dictionary<int, Wardrobe> Wardrobes { get; }

    public void Dispose()
    {
        this.Wardrobes.Clear();
        GC.SuppressFinalize(this);
    }
}
