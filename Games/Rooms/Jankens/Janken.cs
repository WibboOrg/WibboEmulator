namespace WibboEmulator.Games.Rooms.Jankens;

public class Janken(int userid, int dueluserid)
{
    public int UserOne { get; set; } = userid;
    public int UserTwo { get; set; } = dueluserid;
    public JankenType ChoiceOne { get; set; } = JankenType.None;
    public JankenType ChoiceTwo { get; set; } = JankenType.None;
    public bool Started { get; set; } = false;
    public int Timer { get; set; } = 0;
}
