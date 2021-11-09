using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Butterfly.HabboHotel.Users.Permissions
{
    public sealed class PermissionComponent
    {
        private readonly List<string> _permissions;

        private readonly List<string> _commands;
        public PermissionComponent()
        {
            _permissions = new List<string>();
            _commands = new List<string>();
        }

        public bool Init(Habbo habbo)
        {
            if (_permissions.Count > 0)
                _permissions.Clear();

            if (_commands.Count > 0)
                _commands.Clear();

            _permissions.AddRange(ButterflyEnvironment.GetGame().GetPermissionManager().GetPermissionsForPlayer(habbo));
            //_commands.AddRange(ButterflyEnvironment.GetGame().GetPermissionManager().GetCommandsForPlayer(habbo));
            return true;
        }

        public bool HasRight(string Right)
        {
            return _permissions.Contains(Right);
        }
        public bool HasCommand(string Command)
        {
            return _commands.Contains(Command);
        }
        public void Dispose()
        {
            _permissions.Clear();
        }
    }
}
