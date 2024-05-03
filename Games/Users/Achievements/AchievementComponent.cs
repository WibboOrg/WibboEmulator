namespace WibboEmulator.Games.Users.Achievements;
using System.Data;
using WibboEmulator.Database.Daos.User;

public class AchievementComponent(User user) : IDisposable
{

    public Dictionary<string, UserAchievement> Achievements { get; } = [];

    public void Initialize(IDbConnection dbClient)
    {
        var achievementList = UserAchievementDao.GetAll(dbClient, user.Id);

        foreach (var achievement in achievementList)
        {
            var group = achievement.Group;
            var level = achievement.Level;
            var progress = achievement.Progress;

            var userAchievement = new UserAchievement(group, level, progress);

            _ = this.Achievements.TryAdd(group, userAchievement);
        }

        this.AddAchievement(new UserAchievement("ACH_CameraPhotoCount", 10, 0));
    }

    public void AddAchievement(UserAchievement userAchivement) => _ = this.Achievements.TryAdd(userAchivement.Group, userAchivement);

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
