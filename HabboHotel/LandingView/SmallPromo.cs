using Butterfly.Communication.Packets.Outgoing;

namespace Butterfly.HabboHotel.LandingView
{
    public class SmallPromo
    {
        public int Index;
        public string Header;
        public string Body;
        public string Button;
        public int inGamePromo;
        public string SpecialAction;
        public string Image;

        public SmallPromo(int index, string header, string body, string button, int inGame, string specialAction, string image)
        {
            this.Index = index;
            this.Header = header;
            this.Body = body;
            this.Button = button;
            this.inGamePromo = inGame;
            this.SpecialAction = specialAction;
            this.Image = image;
        }

        public ServerPacket Serialize(ServerPacket Composer)
        {
            Composer.WriteInteger(this.Index);
            Composer.WriteString(this.Header);
            Composer.WriteString(this.Body);
            Composer.WriteString(this.Button);
            Composer.WriteInteger(this.inGamePromo);
            Composer.WriteString(this.SpecialAction);
            Composer.WriteString(this.Image);

            return Composer;
        }

    }
}
