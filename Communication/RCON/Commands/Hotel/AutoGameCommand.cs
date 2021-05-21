namespace Butterfly.Communication.RCON.Commands.Hotel
{
    internal class AutoGameCommand : IRCONCommand
    {
        public bool TryExecute(string[] parameters)
        {
            ButterflyEnvironment.GetGame().GetAnimationManager().ForceDisabled((parameters[0] == "1"));
            return true;
        }
    }
}
