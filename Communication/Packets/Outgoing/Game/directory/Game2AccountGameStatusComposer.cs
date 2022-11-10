namespace WibboEmulator.Communication.Packets.Outgoing.GameCenter;

internal class Game2AccountGameStatusComposer : ServerPacket
{
    public Game2AccountGameStatusComposer(int gameTypeId, int freeGamesLeft, int gamesPlayedTotal)
        : base(ServerPacketHeader.GAME_CENTER_STATUS)
    {
        this.WriteInteger(gameTypeId);
        this.WriteInteger(freeGamesLeft);
        this.WriteInteger(gamesPlayedTotal);
    }
}
