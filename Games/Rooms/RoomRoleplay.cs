namespace WibboEmulator.Games.Rooms;

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
        var now = DateTime.Now;

        var hourNow = (int)Math.Floor((double)(((now.Minute * 60) + now.Second) / 150)); //150sec = 2m30s = 1heure dans le rp

        var minuteNow = (int)Math.Floor(((now.Minute * 60) + now.Second - (hourNow * 150)) / 2.5);

        if (hourNow >= 16)
        {
            hourNow = hourNow + 8 - 24;
        }
        else
        {
            hourNow += 8;
        }

        if (this.TimeSpeed)
        {
            hourNow = (int)Math.Floor((double)(now.Second / 2.5));
        }

        if (this.Minute != minuteNow)
        {
            this.Minute = minuteNow;
        }

        if (this.Hour == hourNow)
        {
            return false;
        }

        this.Hour = hourNow;

        if (!this.CycleHourEffect)
        {
            return false;
        }

        if (this.Hour is >= 8 and < 20) //Journée
        {
            this.Intensity = 255;
        }
        else if (this.Hour is >= 20 and < 21)  //Crépuscule
        {
            this.Intensity = 200;
        }
        else if (this.Hour is >= 21 and < 22)  //Crépuscule
        {
            this.Intensity = 150;
        }
        else if (this.Hour is >= 22 and < 23)  //Crépuscule
        {
            this.Intensity = 100;
        }
        else if (this.Hour is >= 23 and < 24)  //Crépuscule
        {
            this.Intensity = 75;
        }
        else if (this.Hour is >= 0 and < 4)  //Nuit
        {
            this.Intensity = 50;
        }
        else if (this.Hour is >= 4 and < 5)  //Aube
        {
            this.Intensity = 75;
        }
        else if (this.Hour is >= 5 and < 6)  //Aube
        {
            this.Intensity = 100;
        }
        else if (this.Hour is >= 6 and < 7)  //Aube
        {
            this.Intensity = 150;
        }
        else if (this.Hour is >= 7 and < 8)  //Aube
        {
            this.Intensity = 200;
        }

        return true;
    }
}
