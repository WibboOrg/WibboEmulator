namespace WibboEmulator.Game.Chat.Pets.Commands
{
    public struct PetCommand
    {
        private int _commandID;
        private string _commandInput;

        public PetCommand(int CommandID, string CommandInput)
        {
            this._commandID = CommandID;
            this._commandInput = CommandInput;
        }

        public int CommandID
        {
            get => this._commandID;
            set => this._commandID = value;
        }

        public string CommandInput
        {
            get => this._commandInput;
            set => this._commandInput = value;
        }
    }
}
