namespace WibboEmulator.Games.Roleplays.Troc;

public class RPTroc(int id, int rpId, int userOne, int userTwo)
{
    public RPTrocUser UserOne { get; set; } = new RPTrocUser(userOne);
    public RPTrocUser UserTwo { get; set; } = new RPTrocUser(userTwo);

    public int Id { get; set; } = id;
    public int RPId { get; set; } = rpId;

    public RPTrocUser GetUser(int userId)
    {
        if (this.UserOne.UserId == userId)
        {
            return this.UserOne;
        }
        else if (this.UserTwo.UserId == userId)
        {
            return this.UserTwo;
        }

        return null;
    }

    public void ResetConfirmed()
    {
        this.UserOne.Confirmed = false;
        this.UserTwo.Confirmed = false;
    }

    public bool AllAccepted => this.UserOne.Accepted && this.UserTwo.Accepted;

    public bool AllConfirmed => this.UserOne.Confirmed && this.UserTwo.Confirmed;
}
