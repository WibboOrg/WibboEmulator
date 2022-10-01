using WibboEmulator.Games.Clients;

namespace WibboEmulator.Games.Items.Interactors
{
    public class InteractorCrackable : FurniInteractor
    {
        private readonly int Modes;

        public InteractorCrackable(int Modes)
        {
            this.Modes = Modes - 1;
            if (this.Modes >= 0)
            {
                return;
            }

            this.Modes = 0;
        }

        public override void OnPlace(Client Session, Item Item)
        {
        }

        public override void OnRemove(Client Session, Item Item)
        {
        }

        public override void OnTrigger(Client Session, Item Item, int Request, bool UserHasRights, bool Reverse)
        {
            if (!UserHasRights || this.Modes == 0)
            {
                return;
            }

            int.TryParse(Item.ExtraData, out int NumMode);

            NumMode++;

            if (NumMode > this.Modes)
            {
                NumMode = 0;
            }

            Item.ExtraData = NumMode.ToString();
            Item.UpdateState();
        }

        public override void OnTick(Item item)
        {
        }
    }
}
