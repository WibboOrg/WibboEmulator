using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System.Data;

namespace Butterfly.Game.Chat.Pets.Commands
{
    public class PetCommandManager
    {
        private readonly Dictionary<string, PetCommand> _petCommands;

        public PetCommandManager()
        {
            this._petCommands = new Dictionary<string, PetCommand>();
        }

        public void Init(IQueryAdapter dbClient)
        {
            this._petCommands.Clear();

            DataTable table = EmulatorCommandPetDao.GetAll(dbClient);

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
