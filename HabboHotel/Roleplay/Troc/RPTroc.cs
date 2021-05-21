namespace Butterfly.HabboHotel.Roleplay.Troc
{
    public class RPTroc
    {
        public RPTrocUser UserOne;
        public RPTrocUser UserTwo;

        public int Id;
        public int RPId;

        public RPTroc(int pId, int pRPId, int pUserOne, int pUserTwo)
        {
            this.Id = pId;
            this.RPId = pRPId;
            this.UserOne = new RPTrocUser(pUserOne);
            this.UserTwo = new RPTrocUser(pUserTwo);
        }

        public RPTrocUser GetUser(int UserId)
        {
            if (this.UserOne.UserId == UserId)
            {
                return this.UserOne;
            }
            else if (this.UserTwo.UserId == UserId)
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

        public bool AllAccepted => this.UserOne.Accepted == true && this.UserTwo.Accepted == true;

        public bool AllConfirmed => this.UserOne.Confirmed == true && this.UserTwo.Confirmed == true;
    }
}
