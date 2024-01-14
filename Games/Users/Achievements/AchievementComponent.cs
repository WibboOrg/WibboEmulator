namespace WibboEmulator.Games.Users.Achievements;
using System.Data;
using WibboEmulator.Database.Daos.User;

public class AchievementComponent : IDisposable
{
    private readonly User _userInstance;
    private readonly Dictionary<string, UserAchievement> _achievements;

    public AchievementComponent(User user)
    {
        this._userInstance = user;
        this._achievements = new Dictionary<string, UserAchievement>();
    }

    public void Init(IDbConnection dbClient)
    {
        var achievementList = UserAchievementDao.GetAll(dbClient, this._userInstance.Id);

        foreach (var achievement in achievementList)
        {
            var group = achievement.Group;
            var level = achievement.Level;
            var progress = achievement.Progress;

            var userAchievement = new UserAchievement(group, level, progress);

            if (!this._achievements.ContainsKey(group))
            {
                this._achievements.Add(group, userAchievement);
            }
        }

        this.AddAchievement(new UserAchievement("ACH_CameraPhotoCount", 10, 0));
    }

    public void AddAchievement(UserAchievement userAchivement)
    {
        if (!this._achievements.ContainsKey(userAchivement.Group))
        {
            this._achievements.Add(userAchivement.Group, userAchivement);
        }
    }

    public Dictionary<string, UserAchievement> GetAchievements() => this._achievements;

    public UserAchievement GetAchievementData(string p)
    {
        _ = this._achievements.TryGetValue(p, out var userAchievement);
        return userAchievement;
    }

    public void Dispose()
    {
        this._achievements.Clear();
        GC.SuppressFinalize(this);
    }
}
