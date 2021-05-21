using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;

namespace Butterfly.HabboHotel.Rooms.Chat.Pets.Commands
{
    public class PetCommandManager
    {
        private readonly Dictionary<string, PetCommand> _petCommands;

        public PetCommandManager()
        {
            this._petCommands = new Dictionary<string, PetCommand>();

            this.Init();
        }

        public void Init()
        {
            this._petCommands.Clear();

            DataTable table;
            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                dbClient.SetQuery("SELECT id, command FROM system_commands_pets");
                table = dbClient.GetTable();
            }
            if (table == null)
            {
                return;
            }

            foreach (DataRow dataRow in table.Rows)
            {
                int key = Convert.ToInt32(dataRow["id"]);
                string str1 = (string)dataRow["command"];

                this._petCommands.Add(str1, new PetCommand(key, str1));
            }
        }

        public int TryInvoke(string input)
        {
            if (this._petCommands.TryGetValue(input, out PetCommand petCommand))
            {
                return petCommand.CommandID;
            }
            else
            {
                return 99;
            }
        }
    }
}
