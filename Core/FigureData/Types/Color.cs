namespace Wibbo.Core.FigureData.Types
{
    public class Color
    {
        public int Id;
        public int Index;
        public int ClubLevel;
        public bool Selectable;

        public Color(int id, int index, int clubLevel, bool selectable)
        {
            this.Id = id;
            this.Index = index;
            this.ClubLevel = clubLevel;
            this.Selectable = selectable;
        }
    }
}
