namespace WibboEmulator.Games.Users.Wardrobes;
using System.Data;
using WibboEmulator.Database.Daos.User;

public class WardrobeComponent : IDisposable
{
    private static readonly int MAX_SLOT = 24;
    private readonly User _userInstance;
    private readonly Dictionary<int, Wardrobe> _wardrobes;

    public WardrobeComponent(User user)
    {
        this._userInstance = user;
        this._wardrobes = new Dictionary<int, Wardrobe>();
    }

    public void Init(IDbConnection dbClient)
    {
        this._wardrobes.Clear();

        var userWardrobeList = UserWardrobeDao.GetAll(dbClient, this._userInstance.Id);

        foreach (var userWardrobe in userWardrobeList)
        {
            var slotId = userWardrobe.SlotId;

            if (this._wardrobes.ContainsKey(slotId))
            {
                continue;
            }

            if (slotId < 1 || slotId > MAX_SLOT)
            {
                continue;
            }

            var wardrobe = new Wardrobe(slotId, userWardrobe.Look, userWardrobe.Gender!.ToUpper());
            this._wardrobes.Add(slotId, wardrobe);
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

        if (this._wardrobes.ContainsKey(slotId))
        {
            _ = this._wardrobes.Remove(slotId);
        }

        look = WibboEnvironment.GetFigureManager().ProcessFigure(look, gender, true);

        var wardrobe = new Wardrobe(slotId, look, gender);
        this._wardrobes.Add(slotId, wardrobe);

        using var dbClient = WibboEnvironment.GetDatabaseManager().Connection();
        UserWardrobeDao.Insert(dbClient, this._userInstance.Id, slotId, look, gender.ToUpper());
    }

    public Dictionary<int, Wardrobe> GetWardrobes() => this._wardrobes;

    public void Dispose()
    {
        this._wardrobes.Clear();
        GC.SuppressFinalize(this);
    }
}
