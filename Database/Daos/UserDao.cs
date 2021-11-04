﻿using System;
using System.Data;
using Butterfly.Database;
using Butterfly.Database.Interfaces;

namespace Butterfly.Database.Daos
{
    class UserDao
    {
        internal static int GetIdByName(IQueryAdapter dbClient, string name)
        {
            dbClient.SetQuery("SELECT id FROM users WHERE username = @username LIMIT 1");
            dbClient.AddParameter("username", name);

            return dbClient.GetInteger();
        }

        internal static string GetNameById(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT username FROM users WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", userId);

            return dbClient.GetString();
        }

        internal static void UpdateRemovePoints(IQueryAdapter dbClient, int userId, int points)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points - '" + points + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateAddPoints(IQueryAdapter dbClient, int userId, int points)
        {
            dbClient.RunQuery("UPDATE users SET vip_points = vip_points + '" + points + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateHomeRoom(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("UPDATE users SET home_room = '" + roomId + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateMotto(IQueryAdapter dbClient, int userId, string motto)
        {
            dbClient.SetQuery("UPDATE users SET motto = @motto WHERE id = '" + userId + "'");
            dbClient.AddParameter("motto", motto);
            dbClient.RunQuery();
        }

        internal static void Update(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE users SET home_room = '0' WHERE id = '" + userId + "'");
        }

        internal static void Update(IQueryAdapter dbClient, int userId, int volume1, int volume2, int volume3)
        {
            dbClient.RunQuery("UPDATE users SET volume = '" + volume1 + "," + volume2 + "," + volume3 + "' WHERE id = '" + userId + "' LIMIT 1");
        }

        internal static void UpdateName(IQueryAdapter dbClient, int userId, string username)
        {
            dbClient.SetQuery("UPDATE users SET username = @newname WHERE id = @userid");
            dbClient.AddParameter("newname", username);
            dbClient.AddParameter("userid", userId);
            dbClient.RunQuery();
        }

        internal static void UpdateLookAndGender(IQueryAdapter dbClient, int userId, string look, string gender)
        {
            dbClient.SetQuery("UPDATE users SET look = @look, gender = @gender WHERE id = '" + userId + "'");
            dbClient.AddParameter("look", look);
            dbClient.AddParameter("gender", gender);
            dbClient.RunQuery();
        }

        internal static DataRow GetCredits(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT credits FROM users WHERE id = @userid");
            dbClient.AddParameter("userid", userId);
            return dbClient.GetRow();
        }

        internal static void UpdateAllOnline(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET online = '0' WHERE online = '1'");
        }

        internal static void UpdateAllTicket(IQueryAdapter dbClient)
        {
            dbClient.RunQuery("UPDATE users SET auth_ticket = '' WHERE auth_ticket != ''");
        }

        internal static void UpdateMachineId(IQueryAdapter dbClient, int userId, string machineId)
        {
            dbClient.SetQuery("UPDATE users SET machine_id = @machineid WHERE id = '" + userId + "'");
            dbClient.AddParameter("machineid", machineId);
            dbClient.RunQuery();
        }

        internal static void UpdateNuxEnable(IQueryAdapter dbClient, int userId, int roomId)
        {
            dbClient.RunQuery("UPDATE users SET nux_enable = '0', home_room = '" + roomId + "' WHERE id = '" + userId + "'");
        }

        internal static string GetName(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT username FROM users WHERE id = '" + userId + "'");
            return dbClient.GetString();
        }

        internal static void Update(IQueryAdapter dbClient, int userId, string look)
        {
            dbClient.SetQuery("UPDATE users SET look = @look WHERE id = '" + userId + "'");
            dbClient.AddParameter("look", look);
            dbClient.RunQuery();
        }

        internal static void UpdateAddGamePoints(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE users SET game_points = game_points + 1, game_points_month = game_points_month + 1 WHERE id = '" + userId + "';");
        }

        internal static void UpdateMazoScore(IQueryAdapter dbClient, int userId, int score)
        {
            dbClient.RunQuery("UPDATE users SET mazoscore = '" + score + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateMazo(IQueryAdapter dbClient, int userId, int score)
        {
            dbClient.RunQuery("UPDATE users SET mazo = '" + score + "' WHERE id = '" + userId + "'");
        }

        internal static DataRow GetIdAndLangue(IQueryAdapter dbClient, string owner)
        {
            dbClient.SetQuery("SELECT id, langue FROM users WHERE username = @owner");
            dbClient.AddParameter("owner", owner);
            return dbClient.GetRow();
        }

        internal static void UpdateAddRunPoints(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE users SET run_points = run_points + 1, run_points_month = run_points_month + 1 WHERE id = '" + userId + "'");
        }

        internal static int GetOneIdByUsername(IQueryAdapter dbClient, string username)
        {
            dbClient.SetQuery("SELECT id FROM users WHERE username = @username");
            dbClient.AddParameter("username", username);
            return dbClient.GetInteger();
        }

        internal static DataRow GetOneIdAndName(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT id, username FROM users WHERE id = '" + userId + "'");
            return dbClient.GetRow();
        }

        internal static string GetOneName(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT username FROM users WHERE id = '" + userId + "'");
            return dbClient.GetString();
        }


        internal static void UpdateOffline(IQueryAdapter dbClient, int userId, int duckets, int credits)
        {
            dbClient.RunQuery("UPDATE users SET online = '0', last_online = '" + ButterflyEnvironment.GetUnixTimestamp() + "', activity_points = '" + duckets + "', credits = '" + credits + "' WHERE id = '" + userId + "'");
        }

        internal static DataRow GetOneIdAndBlockNewFriend(IQueryAdapter dbClient, string username)
        {
            dbClient.SetQuery("SELECT id, block_newfriends FROM users WHERE username = @username");
            dbClient.AddParameter("username", username);
            return dbClient.GetRow();
        }

        internal static DataTable GetAllSearchUsers(IQueryAdapter dbClient, string search)
        {
            dbClient.SetQuery("SELECT id, username, look FROM users WHERE username LIKE @search LIMIT 50");
            dbClient.AddParameter("search", (search.Replace("%", "\\%").Replace("_", "\\_") + "%"));
            return dbClient.GetTable();
        }

        internal static DataRow GetOneByTicket(IQueryAdapter dbClient, string sessionTicket)
        {
            dbClient.SetQuery("SELECT * FROM users WHERE auth_ticket = @sso LIMIT 1");
            dbClient.AddParameter("sso", sessionTicket);
            return dbClient.GetRow();
        }

        internal static void UpdateLastDailyCredits(IQueryAdapter dbClient, int userId, string lastDailyCredits)
        {
            dbClient.RunQuery("UPDATE users SET lastdailycredits = '" + lastDailyCredits + "' WHERE id = '" + userId + "'");
        }

        internal static void UpdateOnline(IQueryAdapter dbClient, int userId)
        {
            dbClient.RunQuery("UPDATE users SET online = '1', auth_ticket = ''  WHERE id = '" + userId + "'");
        }

        internal static DataTable GetAllFriendShips(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT users.id,users.username,messenger_friendships.relation FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = '" + userId + "'");
            return dbClient.GetTable();
        }

        internal static DataTable GetAllFriendRequests(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT messenger_requests.from_id,messenger_requests.to_id,users.username FROM users JOIN messenger_requests ON users.id = messenger_requests.from_id WHERE messenger_requests.to_id = '" + userId + "'");
            return dbClient.GetTable();
        }

        internal static DataRow GetOne(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT * FROM users WHERE id = @id LIMIT 1");
            dbClient.AddParameter("id", userId);
            return dbClient.GetRow();
        }

        internal static DataTable GetAllFriendRelation(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT users.id, messenger_friendships.relation FROM users JOIN messenger_friendships ON users.id = messenger_friendships.user_two_id WHERE messenger_friendships.user_one_id = '" + userId + "' AND messenger_friendships.relation != '0'");
            return dbClient.GetTable();
        }

        internal static DataRow GetOneVolume(IQueryAdapter dbClient, int userId)
        {
            dbClient.SetQuery("SELECT volume FROM users WHERE id = '" + userId + "'");
            return dbClient.GetRow();
        }
    }
}