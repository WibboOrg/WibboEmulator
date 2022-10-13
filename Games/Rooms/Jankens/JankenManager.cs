namespace WibboEmulator.Games.Rooms.Jankens;
using System.Collections.Concurrent;
using WibboEmulator.Core.Language;

public class JankenManager
{
    private readonly ConcurrentDictionary<int, Janken> _party;
    private readonly List<Janken> _remove;
    private readonly Room _roomInstance;

    public JankenManager(Room room)
    {
        this._party = new ConcurrentDictionary<int, Janken>();
        this._remove = new List<Janken>();
        this._roomInstance = room;
    }

    public void Start(RoomUser user, RoomUser duelUser)
    {
        if (user.PartyId > 0)
        {
            var party = this.GetParty(user.PartyId);
            if (party == null)
            {
                user.PartyId = 0;
                return;
            }
            if (party.Started)
            {
                return;
            }

            _ = this._party.TryRemove(user.PartyId, out _);
        }

        if (duelUser.PartyId > 0)
        {
            var party = this.GetParty(duelUser.PartyId);
            if (party == null)
            {
                duelUser.PartyId = 0;
                return;
            }

            if (party.UserTwo == user.UserId)
            {
                party.Started = true;
                user.PartyId = party.UserOne;

                user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.start", user.Client.Langue));
                duelUser.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.start", duelUser.Client.Langue));
            }
            else
            {
                user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.notwork", user.Client.Langue), duelUser.GetUsername()));
            }
        }
        else
        {
            user.PartyId = user.UserId;
            _ = this._party.TryAdd(user.PartyId, new Janken(user.UserId, duelUser.UserId));

            user.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.wait", user.Client.Langue), duelUser.GetUsername()));
            duelUser.SendWhisperChat(user.GetUsername() + " vous défie au JanKen! Utilisez la commande :janken " + user.GetUsername() + " pour accepter le défie");
        }
    }

    public void OnCycle()
    {
        if (this._party.IsEmpty)
        {
            return;
        }

        foreach (var party in this._party.Values)
        {
            if (!party.Started)
            {
                continue;
            }

            party.Timer++;

            if (party.Timer >= 60 || (party.ChoiceOne != JankenType.None && party.ChoiceTwo != JankenType.None))
            {
                if (this.EndGame(party))
                {
                    party.Started = false;

                    var roomuserOne = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserOne);
                    var roomuserTwo = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserTwo);

                    if (roomuserOne != null)
                    {
                        roomuserOne.PartyId = 0;
                    }

                    if (roomuserTwo != null)
                    {
                        roomuserTwo.PartyId = 0;
                    }

                    this._remove.Add(party);
                }
            }
        }

        if (this._remove.Count > 0)
        {
            foreach (var party in this._remove)
            {
                _ = this._party.TryRemove(party.UserOne, out var outparty);
            }

            this._remove.Clear();
        }
    }

    public bool PlayerStarted(RoomUser user)
    {
        var party = this.GetParty(user.PartyId);
        if (party == null)
        {
            return false;
        }

        if (!party.Started)
        {
            return false;
        }

        if (party.UserOne == user.UserId && party.ChoiceOne != JankenType.None)
        {
            return false;
        }

        if (party.UserTwo == user.UserId && party.ChoiceTwo != JankenType.None)
        {
            return false;
        }

        return true;
    }

    public bool PickChoice(RoomUser user, string message)
    {
        var party = this.GetParty(user.PartyId);

        JankenType choice;
        if (message.ToLower().StartsWith("p"))
        {
            choice = JankenType.Rock;
        }
        else if (message.ToLower().StartsWith("f"))
        {
            choice = JankenType.Paper;
        }
        else if (message.ToLower().StartsWith("c"))
        {
            choice = JankenType.Scissors;
        }
        else
        {
            return false;
        }

        if (party.UserOne == user.UserId)
        {
            party.ChoiceOne = choice;
        }
        else
        {
            party.ChoiceTwo = choice;
        }

        if (user.Client != null)
        {
            user.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.confirmechoice", user.Client.Langue) + GetStringChoix(choice, user.Client.Langue));
        }

        return true;
    }

    private bool EndGame(Janken party)
    {
        var roomuserOne = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserOne);
        var roomuserTwo = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserTwo);
        if (roomuserOne == null && roomuserTwo == null)
        {
            return true;
        }

        if (roomuserOne == null)
        {
            roomuserTwo.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.Client.Langue));
            return true;
        }
        else if (roomuserTwo == null)
        {
            roomuserOne.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.Client.Langue));
            return true;
        }

        if (party.ChoiceOne == JankenType.None && party.ChoiceTwo == JankenType.None)
        {
            roomuserTwo.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserTwo.Client.Langue));
            roomuserOne.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserOne.Client.Langue));
            return true;
        }

        if (party.ChoiceOne == JankenType.None)
        {
            roomuserTwo.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.Client.Langue));
            return true;
        }
        else if (party.ChoiceTwo == JankenType.None)
        {
            roomuserOne.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.Client.Langue));
            return true;
        }

        if (party.ChoiceOne == party.ChoiceTwo) //match null
        {
            party.ChoiceOne = JankenType.None;
            party.ChoiceTwo = JankenType.None;

            party.Timer = 0;

            roomuserOne.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserOne.Client.Langue));
            roomuserTwo.SendWhisperChat(WibboEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserTwo.Client.Langue));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return false;
        }

        //ChoixOne qui gagne
        if ((party.ChoiceOne == JankenType.Scissors && party.ChoiceTwo == JankenType.Paper) ||
            (party.ChoiceOne == JankenType.Paper && party.ChoiceTwo == JankenType.Rock) ||
            (party.ChoiceOne == JankenType.Rock && party.ChoiceTwo == JankenType.Scissors))
        {
            roomuserOne.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserOne.Client.Langue), GetStringChoix(party.ChoiceOne, roomuserOne.Client.Langue), GetStringChoix(party.ChoiceTwo, roomuserOne.Client.Langue), roomuserTwo.GetUsername()));
            roomuserTwo.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserTwo.Client.Langue), GetStringChoix(party.ChoiceOne, roomuserTwo.Client.Langue), GetStringChoix(party.ChoiceTwo, roomuserTwo.Client.Langue), roomuserOne.GetUsername()));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return true;
        }

        //ChoixTwo qui gagne
        if ((party.ChoiceOne == JankenType.Scissors && party.ChoiceTwo == JankenType.Rock) ||
            (party.ChoiceOne == JankenType.Paper && party.ChoiceTwo == JankenType.Scissors) ||
            (party.ChoiceOne == JankenType.Rock && party.ChoiceTwo == JankenType.Paper))
        {
            roomuserTwo.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserTwo.Client.Langue), GetStringChoix(party.ChoiceTwo, roomuserTwo.Client.Langue), GetStringChoix(party.ChoiceOne, roomuserTwo.Client.Langue), roomuserOne.GetUsername()));
            roomuserOne.SendWhisperChat(string.Format(WibboEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserOne.Client.Langue), GetStringChoix(party.ChoiceTwo, roomuserOne.Client.Langue), GetStringChoix(party.ChoiceOne, roomuserOne.Client.Langue), roomuserTwo.GetUsername()));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return true;
        }

        return true;
    }

    private static string GetStringChoix(JankenType choix, Language langue) => choix switch
    {
        JankenType.Scissors => WibboEnvironment.GetLanguageManager().TryGetValue("janken.ciseaux", langue),
        JankenType.Paper => WibboEnvironment.GetLanguageManager().TryGetValue("janken.feuille", langue),
        JankenType.Rock => WibboEnvironment.GetLanguageManager().TryGetValue("janken.pierre", langue),
        _ => "",
    };

    private static void EnableEffet(RoomUser user, JankenType janken)
    {
        if (janken == JankenType.Scissors)
        {
            user.ApplyEffect(563, true);
        }
        else if (janken == JankenType.Rock)
        {
            user.ApplyEffect(565, true);
        }
        else if (janken == JankenType.Paper)
        {
            user.ApplyEffect(564, true);
        }

        user.TimerResetEffect = 10;
    }

    public void RemovePlayer(RoomUser user)
    {
        if (user.PartyId == 0)
        {
            return;
        }

        var party = this.GetParty(user.PartyId);
        if (party == null)
        {
            return;
        }

        if (!party.Started)
        {
            var roomuserOne = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserOne);
            var roomuserTwo = this._roomInstance.RoomUserManager.GetRoomUserByUserId(party.UserTwo);

            if (roomuserOne != null)
            {
                roomuserOne.PartyId = 0;
            }

            if (roomuserTwo != null)
            {
                roomuserTwo.PartyId = 0;
            }

            _ = this._party.TryRemove(party.UserOne, out _);
        }
    }

    public Janken GetParty(int id)
    {
        if (this._party.ContainsKey(id))
        {
            return this._party[id];
        }
        else
        {
            return null;
        }
    }
}
