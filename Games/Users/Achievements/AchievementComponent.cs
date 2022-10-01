using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using System.Data;

namespace WibboEmulator.Game.Users.Achievements
{
    public class AchievementComponent : IDisposable
    {
        private readonly User _userInstance;
        private readonly Dictionary<string, UserAchievement> _achievements;

        public AchievementComponent(User user)
        {
            this._userInstance = user;
            this._achievements = new Dictionary<string, UserAchievement>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            DataTable dAchievement = UserAchievementDao.GetAll(dbClient, this._userInstance.Id);

            foreach (DataRow dataRow in dAchievement.Rows)
            {
                string group = (string)dataRow["group"];
                int level = Convert.ToInt32(dataRow["level"]);
                int progress = Convert.ToInt32(dataRow["progress"]);

                UserAchievement userAchievement = new UserAchievement(group, level, progress);

                if (!this._achievements.ContainsKey(group))
                    this._achievements.Add(group, userAchievement);
            }

            this.AddAchievement(new UserAchievement("ACH_CameraPhotoCount", 10, 0));
        }

        public void AddAchievement(UserAchievement userAchivement)
        {
            if(!this._achievements.ContainsKey(userAchivement.Group))
                this._achievements.Add(userAchivement.Group, userAchivement);
        }

        public Dictionary<string, UserAchievement> GetAchievements()
        {
            return this._achievements;
        }

        public UserAchievement GetAchievementData(string p)
        {
            this._achievements.TryGetValue(p, out UserAchievement userAchievement);
            return userAchievement;
        }

        public void Dispose()
        {
            this._achievements.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
