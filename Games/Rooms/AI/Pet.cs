namespace WibboEmulator.Games.Rooms.AI;
using WibboEmulator.Communication.Packets.Outgoing.Pets;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.AI.Pets;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Users;

public class Pet
{
    private readonly int[] _experienceLevels =
    [
        100,
        200,
        400,
        600,
        1000,
        1300,
        1800,
        2400,
        3200,
        4300,
        7200,
        8500,
        10100,
        13300,
        17500,
        23000,
        51900,
        120000,
        240000
    ];
    public int PetId { get; set; }
    public int OwnerId { get; set; }
    public int VirtualId { get; set; }
    public int Type { get; set; }
    public string Name { get; set; }
    public string Race { get; set; }
    public string Color { get; set; }
    public int Expirience { get; set; }
    public int Energy { get; set; }
    public int Nutrition { get; set; }
    public int RoomId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int Respect { get; set; }
    public int CreationStamp { get; set; }
    public bool PlacedInRoom { get; set; }
    public int Saddle { get; set; }
    public int HairDye { get; set; }
    public int PetHair { get; set; }
    public bool AnyoneCanRide { get; set; }
    public Dictionary<short, bool> PetCommands { get; set; }

    public Room Room
    {
        get
        {
            if (!this.IsInRoom)
            {
                return null;
            }
            else
            {
                _ = RoomManager.TryGetRoom(this.RoomId, out var room);

                return room;
            }
        }
    }

    public bool IsInRoom => this.RoomId > 0;

    public int Level
    {
        get
        {
            for (var index = 0; index < this._experienceLevels.Length; ++index)
            {
                if (this.Expirience < this._experienceLevels[index])
                {
                    return index + 1;
                }
            }
            return this._experienceLevels.Length + 1;
        }
    }

    public static int MaxLevel => 20;

    public int ExpirienceGoal
    {
        get
        {
            if (this.Level < 19)
            {
                return this._experienceLevels[this.Level - 1];
            }
            else
            {
                return this._experienceLevels[18];
            }
        }
    }

    public static int MaxEnergy => 100;

    public static int MaxNutrition => 150;

    public int Age => (int)Math.Floor((WibboEnvironment.GetUnixTimestamp() - this.CreationStamp) / 86400.0);

    public string Look => this.Type + " " + this.Race + " " + this.Color;

    public string OwnerName => UserManager.GetUsernameById(this.OwnerId);

    public Pet(int petId, int ownerId, int roomId, string name, int type, string race, string color, int expirience, int energy, int nutrition, int respect, int creationStamp, int x, int y, double z, int havesaddle, int hairdye, int petHair, bool canMountAllPeople)
    {
        this.PetId = petId;
        this.OwnerId = ownerId;
        this.RoomId = roomId;
        this.Name = name;
        this.Type = type;
        this.Race = race;
        this.Color = color;
        this.Expirience = expirience;
        this.Energy = energy;
        this.Nutrition = nutrition;
        this.Respect = respect;
        this.CreationStamp = creationStamp;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.PlacedInRoom = false;
        this.Saddle = havesaddle == 1 ? 9 : havesaddle == 2 ? 10 : 0;
        this.HairDye = hairdye;
        this.PetHair = petHair;
        this.AnyoneCanRide = canMountAllPeople;
        this.PetCommands = this.GetPetCommands();
    }

    public bool HasCommand(short command) => this.PetCommands.TryGetValue(command, out var value) && value;

    public Dictionary<short, bool> GetPetCommands()
    {
        var output = new Dictionary<short, bool>();
        var qLevel = (short)this.Level;

        switch (this.Type)
        {
            default:
            {
                output.Add(0, true); // Libre
                output.Add(1, true); // Assis
                output.Add(13, true); // Panier
                output.Add(2, qLevel >= 2); // Couché
                output.Add(4, qLevel >= 3); // Demande
                output.Add(3, qLevel >= 4); // Viens ici
                output.Add(5, qLevel >= 4); // Fais le mort
                output.Add(43, qLevel >= 5); // mange
                output.Add(14, qLevel >= 5); // Bois
                output.Add(6, qLevel >= 6); // Reste
                output.Add(17, qLevel >= 6); // Joue au Foot
                output.Add(8, qLevel >= 8); // Debout
                output.Add(7, qLevel >= 9); // Suis moi
                output.Add(9, qLevel >= 11); // Saute
                output.Add(11, qLevel >= 11); // Joue
                output.Add(12, qLevel >= 12); // Silence
                output.Add(10, qLevel >= 12); // Parle
                output.Add(15, qLevel >= 16); // Suis à gauche
                output.Add(16, qLevel >= 16); // Suis à droite
                output.Add(24, qLevel >= 17); // Avance

                if (this.Type is 3 or 4)
                {
                    output.Add(46, true); //Reproduire
                }
            }
            break;

            case 8:
                output.Add(1, true); // Assis
                output.Add(2, true); // Couché
                output.Add(3, qLevel >= 2); // Viens ici
                output.Add(17, qLevel >= 3); // Joue au Foot
                output.Add(6, qLevel >= 4); // Reste
                output.Add(5, qLevel >= 4); // Fais le mort
                output.Add(7, qLevel >= 5); // Suis moi
                output.Add(23, qLevel >= 6); // Allume la télé
                output.Add(9, qLevel >= 7); // Saute
                output.Add(10, qLevel >= 8); // Parle
                output.Add(11, qLevel >= 8); // Joue
                output.Add(24, qLevel >= 9); // Avance
                output.Add(15, qLevel >= 10); // Suis à gauche
                output.Add(16, qLevel >= 10); // Suis à droite
                output.Add(13, qLevel >= 12); // Panier
                output.Add(14, qLevel >= 13); // Bois
                output.Add(19, qLevel >= 14); // Rebondis
                output.Add(20, qLevel >= 14); // Aplatis toi
                output.Add(22, qLevel >= 15); // Tourne
                output.Add(21, qLevel >= 16); // Danse
                break;

            case 16:
                break;
        }

        return output;
    }

    public void OnRespect()
    {
        this.Respect++;

        if (this.Expirience > 51900)
        {
            return;
        }

        this.AddExpirience(10);
    }

    public void AddExpirience(int amount)
    {
        this.Expirience += amount;
        if (this.Expirience >= 51900)
        {
            return;
        }

        if (this.Room == null)
        {
            return;
        }

        this.Room.SendPacket(new AddExperiencePointsComposer(this.PetId, this.VirtualId, amount));

        if (this.Expirience <= this.ExpirienceGoal)
        {
            return;
        }

        this.Room.SendPacket(new NotifyNewPetLevelComposer(this.PetId, this.Name, this.Level));

        this.PetCommands.Clear();
        this.PetCommands = this.GetPetCommands();
    }

    public void PetEnergy(bool addEnergy)
    {
        if (this.Energy >= 100)
        {
            return;
        }

        var randomUsage = WibboEnvironment.GetRandomNumber(4, 15);

        if (!addEnergy)
        {
            this.Energy -= randomUsage;

            if (this.Energy < 0)
            {
                this.Energy = 1;
            }
        }
        else
        {
            this.Energy = (this.Energy + randomUsage) % 100;
        }
    }
}
