using System;

namespace Butterfly.Game.Rooms
{
    public class RoomRoleplay
    {
        public bool Pvp { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public int Intensity { get; set; }
        public bool CycleHourEffect { get; set; }
        public bool TimeSpeed { get; set; }

        public RoomRoleplay()
        {
            this.Pvp = true;
            this.CycleHourEffect = true;
            this.TimeSpeed = false;
            this.Hour = -1;
            this.Minute = -1;
            this.Intensity = -1;
        }

        internal bool Cycle()
        {
            DateTime Now = DateTime.Now;

            int HourNow = (int)Math.Floor((double)(((Now.Minute * 60) + Now.Second) / 150)); //150sec = 2m30s = 1heure dans le rp

            int MinuteNow = (int)Math.Floor((((Now.Minute * 60) + Now.Second) - (HourNow * 150)) / 2.5);

            if (HourNow >= 16)
            {
                HourNow = (HourNow + 8) - 24;
            }
            else
            {
                HourNow += 8;
            }

            if (this.TimeSpeed)
            {
                HourNow = (int)Math.Floor((double)(Now.Second / 2.5));
            }

            if (this.Minute != MinuteNow)
            {
                this.Minute = MinuteNow;
            }

            if (this.Hour == HourNow)
            {
                return false;
            }

            this.Hour = HourNow;

            if (!this.CycleHourEffect)
            {
                return false;
            }

            if (this.Hour >= 8 && this.Hour < 20) //Journée
            {
                this.Intensity = 255;
            }
            else if (this.Hour >= 20 && this.Hour < 21)  //Crépuscule
            {
                this.Intensity = 200;
            }
            else if (this.Hour >= 21 && this.Hour < 22)  //Crépuscule
            {
                this.Intensity = 150;
            }
            else if (this.Hour >= 22 && this.Hour < 23)  //Crépuscule
            {
                this.Intensity = 100;
            }
            else if (this.Hour >= 23 && this.Hour < 24)  //Crépuscule
            {
                this.Intensity = 75;
            }
            else if (this.Hour >= 0 && this.Hour < 4)  //Nuit
            {
                this.Intensity = 50;
            }
            else if (this.Hour >= 4 && this.Hour < 5)  //Aube
            {
                this.Intensity = 75;
            }
            else if (this.Hour >= 5 && this.Hour < 6)  //Aube
            {
                this.Intensity = 100;
            }
            else if (this.Hour >= 6 && this.Hour < 7)  //Aube
            {
                this.Intensity = 150;
            }
            else if (this.Hour >= 7 && this.Hour < 8)  //Aube
            {
                this.Intensity = 200;
            }

            return true;
        }
    }
}