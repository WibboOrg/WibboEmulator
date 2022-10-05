namespace WibboEmulator.Games.Roleplay.Troc;

public class RPTroc
{
    public RPTrocUser UserOne { get; set; }
    public RPTrocUser UserTwo { get; set; }

    public int Id { get; set; }
    public int RPId { get; set; }

    public RPTroc(int id, int rpId, int userOne, int userTwo)
    {
        this.Id = id;
        this.RPId = rpId;
        this.UserOne = new RPTrocUser(userOne);
        this.UserTwo = new RPTrocUser(userTwo);
    }

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
