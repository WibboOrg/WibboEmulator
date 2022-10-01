namespace WibboEmulator.Games.Catalog.Pets
{
    public class PetRace
    {
        private int _raceId;
        private int _primaryColour;
        private int _secondaryColour;
        private bool _hasPrimaryColour;
        private bool _hasSecondaryColour;

        public PetRace(int RaceId, int PrimaryColour, int SecondaryColour, bool HasPrimaryColour, bool HasSecondaryColour)
        {
            this._raceId = RaceId;
            this._primaryColour = PrimaryColour;
            this._secondaryColour = SecondaryColour;
            this._hasPrimaryColour = HasPrimaryColour;
            this._hasSecondaryColour = HasSecondaryColour;
        }

        public int RaceId
        {
            get => this._raceId;
            set => this._raceId = value;
        }

        public int PrimaryColour
        {
            get => this._primaryColour;
            set => this._primaryColour = value;
        }

        public int SecondaryColour
        {
            get => this._secondaryColour;
            set => this._secondaryColour = value;
        }

        public bool HasPrimaryColour
        {
            get => this._hasPrimaryColour;
            set => this._hasPrimaryColour = value;
        }

        public bool HasSecondaryColour
        {
            get => this._hasSecondaryColour;
            set => this._hasSecondaryColour = value;
        }
    }
}
