using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Achievements
{
    public class AchievementLevelFactory
    {
        public static void GetAchievementLevels(out Dictionary<string, Achievement> achievements)
        {
            achievements = new Dictionary<string, Achievement>();
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, category, group_name, level, reward_pixels, reward_points, progress_needed FROM achievements");
                foreach (DataRow dataRow in dbClient.GetTable().Rows)
                {
                    int Id = Convert.ToInt32(dataRow["id"]);
                    string Category = (string)dataRow["category"];
                    string GroupName = (string)dataRow["group_name"];

                    if (!GroupName.StartsWith("ACH_"))
                    {
                        GroupName = "ACH_" + GroupName;
                    }

                    AchievementLevel Level = new AchievementLevel(Convert.ToInt32(dataRow["level"]), Convert.ToInt32(dataRow["reward_pixels"]), Convert.ToInt32(dataRow["reward_points"]), Convert.ToInt32(dataRow["progress_needed"]));
                    if (!achievements.ContainsKey(GroupName))
                    {
                        Achievement achievement = new Achievement(Id, GroupName, Category);
                        achievement.AddLevel(Level);
                        achievements.Add(GroupName, achievement);
                    }
                    else
                    {
                        achievements[GroupName].AddLevel(Level);
                    }
                }
            }
        }
    }
}
