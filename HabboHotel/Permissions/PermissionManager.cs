using System;
using System.Linq;
using System.Collections.Generic;
using log4net;
using Butterfly.Database.Interfaces;
using System.Data;
using Butterfly.HabboHotel.Users;

namespace Butterfly.HabboHotel.Permissions
{
    public sealed class PermissionManager
    {
        private static readonly ILog log = LogManager.GetLogger("Butterfly.HabboHotel.Permissions.PermissionManager");

        private readonly Dictionary<int, Permission> Permissions = new Dictionary<int, Permission>();

        private readonly Dictionary<string, PermissionCommand> _commands = new Dictionary<string, PermissionCommand>();

        private readonly Dictionary<int, PermissionGroup> PermissionGroups = new Dictionary<int, PermissionGroup>();

        private readonly Dictionary<int, List<string>> PermissionGroupRights = new Dictionary<int, List<string>>();

        private readonly Dictionary<int, List<string>> PermissionSubscriptionRights = new Dictionary<int, List<string>>();
        public PermissionManager()
        {
            //Permissions = new Dictionary<int, Permission>();
            //_commands = new Dictionary<string, PermissionCommand>();
        }

        public void Init()
        {
            Permissions.Clear();
            _commands.Clear();
            PermissionGroups.Clear();
            PermissionGroupRights.Clear();

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions`");
                DataTable GetPermissions = dbClient.GetTable();

                if (GetPermissions != null)
                {
                    foreach (DataRow Row in GetPermissions.Rows)
                    {
                        Permissions.Add(Convert.ToInt32(Row["id"]), new Permission(Convert.ToInt32(Row["id"]), Convert.ToInt32(Row["rank"]), Convert.ToString(Row["fuse"]), Convert.ToString(Row["description"])));
                    }
                }
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT * FROM `permissions_commands`");
                DataTable GetCommands = dbClient.GetTable();

                if (GetCommands != null)
                {
                    foreach (DataRow Row in GetCommands.Rows)
                    {
                        _commands.Add(Convert.ToString(Row["command"]), new PermissionCommand(Convert.ToString(Row["command"]), Convert.ToInt32(Row["minrank"])));
                    }
                }
            }

            log.Info("Loaded " + Permissions.Count + " permissions.");
            log.Info("Loaded " + PermissionGroups.Count + " permissions groups.");
            log.Info("Loaded " + PermissionGroupRights.Count + " permissions group rights.");
            log.Info("Loaded " + PermissionSubscriptionRights.Count + " permissions subscription rights.");
        }

        public bool TryGetGroup(int Id, out PermissionGroup Group)
        {
            return PermissionGroups.TryGetValue(Id, out Group);
        }

        public List<string> GetPermissionsForPlayer(Habbo Player)
        {
            List<string> PermissionSet = new List<string>();

            List<string> PermRights = null;
            if (PermissionGroupRights.TryGetValue(Player.Rank, out PermRights))
            {
                PermissionSet.AddRange(PermRights);
            }

            List<string> SubscriptionRights = null;
            if (PermissionSubscriptionRights.TryGetValue(Player.Rank, out SubscriptionRights))
            {
                PermissionSet.AddRange(SubscriptionRights);
            }

            return PermissionSet;
        }
        //public List<string> GetCommandsForPlayer(Habbo Player)
        //{
        //return _commands.Where(x => Player.Rank >= x.Value.GroupId && Player.VIPRank >= x.Value.SubscriptionId).Select(x => x.Key).ToList();
        //}
    }
}