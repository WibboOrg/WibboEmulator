namespace WibboEmulator.Games.Users.Achievements;
using System.Data;
using WibboEmulator.Database.Daos.User;

public class AchievementComponent : IDisposable
{
    private readonly User _user;

    public Dictionary<string, UserAchievement> Achievements { get; }

    public AchievementComponent(User user)
    {
        this._user = user;
        this.Achievements = new Dictionary<string, UserAchievement>();
    }

    public void Initialize(IDbConnection dbClient)
    {
        var achievementList = UserAchievementDao.GetAll(dbClient, this._user.Id);

        foreach (var achievement in achievementList)
        {
            var group = achievement.Group;
            var level = achievement.Level;
            var progress = achievement.Progress;

            var userAchievement = new UserAchievement(group, level, progress);

            if (!this.Achievements.ContainsKey(group))
            {
                this.Achievements.Add(group, userAchievement);
            }
        }

        this.AddAchievement(new UserAchievement("ACH_CameraPhotoCount", 10, 0));
    }

    public void AddAchievement(UserAchievement userAchivement)
    {
        if (!this.Achievements.ContainsKey(userAchivement.Group))
        {
            this.Achievements.Add(userAchivement.Group, userAchivement);
        }
    }

    public UserAchievement GetAchievementData(string p)
    {
        _ = this.Achievements.TryGetValue(p, out var userAchievement);
        return userAchievement;
    }

    public void Dispose()
    {
        this.Achievements.Clear();
        GC.SuppressFinalize(this);
    }
}
