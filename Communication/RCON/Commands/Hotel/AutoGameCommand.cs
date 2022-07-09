namespace Wibbo.Communication.RCON.Commands.Hotel
{
    internal class AutoGameCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            WibboEnvironment.GetGame().GetAnimationManager().ForceDisabled((parameters[0] == "1"));
            return true;
        }
    }
}
