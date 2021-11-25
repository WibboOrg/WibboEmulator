namespace Butterfly.Game.Rooms.Jankens
{
    public class Janken
    {
        public int UserOne { get; set; }
        public int UserTwo { get; set; }

        public JankenType ChoiceOne { get; set; }
        public JankenType ChoiceTwo { get; set; }

        public bool Started { get; set; }
        public int Timer { get; set; }

        public Janken(int userid, int dueluserid)
        {
            this.UserOne = userid;
            this.UserTwo = dueluserid;

            this.ChoiceOne = JankenType.None;
            this.ChoiceTwo = JankenType.None;

            this.Started = false;
            this.Timer = 0;
        }
    }
}