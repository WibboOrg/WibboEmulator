namespace WibboEmulator.Games.Users.Wardrobes;

public class Wardrobe(int slotId, string look, string gender)
{
    public int SlotId { get; private set; } = slotId;
    public string Look { get; private set; } = look;
    public string Gender { get; private set; } = gender;
}
