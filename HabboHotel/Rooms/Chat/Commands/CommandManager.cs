using Butterfly.Communication.Packets.Outgoing.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Butterfly.HabboHotel.GameClients;
using Butterfly.HabboHotel.Permissions;
using System.Data;
using Butterfly.Database.Interfaces;
using Butterfly.HabboHotel.Rooms.Chat.Commands.Cmd;

namespace Butterfly.HabboHotel.Rooms.Chat.Commands
{
    public class CommandManager
    {
        private string _prefix = ":";
        private readonly Dictionary<string, IChatCommand> _commands;
        private readonly Dictionary<string, ChatCommand> _commandRegisterInvokeable;

        public void Init()
        {
            this.RegisterGod();
            this.RegisterAdmin();
            this.RegisterMod();
            this.RegisterPremium();
            this.RegisterUser();
        }
        public CommandManager(string Prefix)
        {
            _prefix = Prefix;
            _commands = new Dictionary<string, IChatCommand>();

            RegisterUser();
            RegisterPremium();
            RegisterMod();
            RegisterAdmin();
            RegisterGod();
        }

        public bool Parse(GameClient Session, RoomUser User, Room Room, string Message)
        {
            if (Session == null || Session.GetHabbo() == null || Session.GetHabbo().CurrentRoom == null)
                return false;

            if (!Message.StartsWith(_prefix))
                return false;

            if (Message == _prefix + "commands")
            {
                StringBuilder List = new StringBuilder();
                List.Append("La liste de vos commandes : \n");
                foreach (var CmdList in _commands.ToList())
                {
                    if (!string.IsNullOrEmpty(CmdList.Value.PermissionRequired))
                    {
                        if (!Session.GetHabbo().GetPermissions().HasCommand(CmdList.Value.PermissionRequired))
                            continue;
                    }

                    List.Append(":" + CmdList.Key + " " + CmdList.Value.Parameters + " - " + CmdList.Value.Description + "\n");
                }
                Session.SendPacket(new MOTDNotificationMessageComposer(List.ToString()));
                return true;
            }

            Message = Message.Substring(1);
            string[] Split = Message.Split(' ');

            if (Split.Length == 0)
                return false;

            IChatCommand Cmd = null;
            if (_commands.TryGetValue(Split[0].ToLower(), out Cmd))
            {
                if (!string.IsNullOrEmpty(Cmd.PermissionRequired))
                {
                    if (!Session.GetHabbo().GetPermissions().HasCommand(Cmd.PermissionRequired))
                        return false;
                }


                Session.GetHabbo().IChatCommand = Cmd;

                Cmd.Execute(Session, Session.GetHabbo().CurrentRoom, Split);
                return true;
            }
            return false;
        }

        private void RegisterUser()
        {

        }
        private void RegisterPremium()
        {

        }
        private void RegisterMod()
        {
            Register("staffalert", new StaffAlert());
        }
        private void RegisterAdmin()
        {

        }
        private void RegisterGod()
        {

        }

        public void Register(string CommandText, IChatCommand Command)
        {
            _commands.Add(CommandText, Command);
        }

        public static string MergeParams(string[] Params, int Start)
        {
            var Merged = new StringBuilder();
            for (int i = Start; i < Params.Length; i++)
            {
                if (i > Start)
                    Merged.Append(" ");
                Merged.Append(Params[i]);
            }

            return Merged.ToString();
        }

        //public void LogCommand(int UserId, string Data, string MachineId)
        //{
            //using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            //{
                //dbClient.SetQuery("INSERT INTO `logs_client_staff` (`user_id`,`data_string`,`machine_id`, `timestamp`) VALUES (@UserId,@Data,@MachineId,@Timestamp)");
                //dbClient.AddParameter("UserId", UserId);
                //dbClient.AddParameter("Data", Data);
                //dbClient.AddParameter("MachineId", MachineId);
                //dbClient.AddParameter("Timestamp", ButterflyEnvironment.GetUnixTimestamp());
                //dbClient.RunQuery();
            //}
        //}

    }
}
