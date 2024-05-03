namespace WibboEmulator.Games.Rooms.Jankens;
using System.Collections.Concurrent;
using WibboEmulator.Core.Language;

public class JankenManager(Room room)
{
    private readonly ConcurrentDictionary<int, Janken> _party = new();
    private readonly List<Janken> _remove = [];

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

                user.SendWhisperChat(LanguageManager.TryGetValue("janken.start", user.Client.Language));
                duelUser.SendWhisperChat(LanguageManager.TryGetValue("janken.start", duelUser.Client.Language));
            }
            else
            {
                user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.notwork", user.Client.Language), duelUser.Username));
            }
        }
        else
        {
            user.PartyId = user.UserId;
            _ = this._party.TryAdd(user.PartyId, new Janken(user.UserId, duelUser.UserId));

            user.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.wait", user.Client.Language), duelUser.Username));
            duelUser.SendWhisperChat(user.Username + " vous défie au JanKen! Utilisez la commande :janken " + user.Username + " pour accepter le défie");
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

                    var roomuserOne = room.RoomUserManager.GetRoomUserByUserId(party.UserOne);
                    var roomuserTwo = room.RoomUserManager.GetRoomUserByUserId(party.UserTwo);

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
        if (message.StartsWith("p", StringComparison.CurrentCultureIgnoreCase))
        {
            choice = JankenType.Rock;
        }
        else if (message.StartsWith("f", StringComparison.CurrentCultureIgnoreCase))
        {
            choice = JankenType.Paper;
        }
        else if (message.StartsWith("c", StringComparison.CurrentCultureIgnoreCase))
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
            user.SendWhisperChat(LanguageManager.TryGetValue("janken.confirmechoice", user.Client.Language) + GetStringChoix(choice, user.Client.Language));
        }

        return true;
    }

    private bool EndGame(Janken party)
    {
        var roomuserOne = room.RoomUserManager.GetRoomUserByUserId(party.UserOne);
        var roomuserTwo = room.RoomUserManager.GetRoomUserByUserId(party.UserTwo);
        if (roomuserOne == null && roomuserTwo == null)
        {
            return true;
        }

        if (roomuserOne == null)
        {
            roomuserTwo.SendWhisperChat(LanguageManager.TryGetValue("janken.forfait", roomuserTwo.Client.Language));
            return true;
        }
        else if (roomuserTwo == null)
        {
            roomuserOne.SendWhisperChat(LanguageManager.TryGetValue("janken.forfait", roomuserOne.Client.Language));
            return true;
        }

        if (party.ChoiceOne == JankenType.None && party.ChoiceTwo == JankenType.None)
        {
            roomuserTwo.SendWhisperChat(LanguageManager.TryGetValue("janken.annule", roomuserTwo.Client.Language));
            roomuserOne.SendWhisperChat(LanguageManager.TryGetValue("janken.annule", roomuserOne.Client.Language));
            return true;
        }

        if (party.ChoiceOne == JankenType.None)
        {
            roomuserTwo.SendWhisperChat(LanguageManager.TryGetValue("janken.forfait", roomuserTwo.Client.Language));
            return true;
        }
        else if (party.ChoiceTwo == JankenType.None)
        {
            roomuserOne.SendWhisperChat(LanguageManager.TryGetValue("janken.forfait", roomuserOne.Client.Language));
            return true;
        }

        if (party.ChoiceOne == party.ChoiceTwo) //match null
        {
            party.ChoiceOne = JankenType.None;
            party.ChoiceTwo = JankenType.None;

            party.Timer = 0;

            roomuserOne.SendWhisperChat(LanguageManager.TryGetValue("janken.nul", roomuserOne.Client.Language));
            roomuserTwo.SendWhisperChat(LanguageManager.TryGetValue("janken.nul", roomuserTwo.Client.Language));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return false;
        }

        //ChoixOne qui gagne
        if ((party.ChoiceOne == JankenType.Scissors && party.ChoiceTwo == JankenType.Paper) ||
            (party.ChoiceOne == JankenType.Paper && party.ChoiceTwo == JankenType.Rock) ||
            (party.ChoiceOne == JankenType.Rock && party.ChoiceTwo == JankenType.Scissors))
        {
            roomuserOne.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.win", roomuserOne.Client.Language), GetStringChoix(party.ChoiceOne, roomuserOne.Client.Language), GetStringChoix(party.ChoiceTwo, roomuserOne.Client.Language), roomuserTwo.Username));
            roomuserTwo.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.loose", roomuserTwo.Client.Language), GetStringChoix(party.ChoiceOne, roomuserTwo.Client.Language), GetStringChoix(party.ChoiceTwo, roomuserTwo.Client.Language), roomuserOne.Username));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return true;
        }

        //ChoixTwo qui gagne
        if ((party.ChoiceOne == JankenType.Scissors && party.ChoiceTwo == JankenType.Rock) ||
            (party.ChoiceOne == JankenType.Paper && party.ChoiceTwo == JankenType.Scissors) ||
            (party.ChoiceOne == JankenType.Rock && party.ChoiceTwo == JankenType.Paper))
        {
            roomuserTwo.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.win", roomuserTwo.Client.Language), GetStringChoix(party.ChoiceTwo, roomuserTwo.Client.Language), GetStringChoix(party.ChoiceOne, roomuserTwo.Client.Language), roomuserOne.Username));
            roomuserOne.SendWhisperChat(string.Format(LanguageManager.TryGetValue("janken.loose", roomuserOne.Client.Language), GetStringChoix(party.ChoiceTwo, roomuserOne.Client.Language), GetStringChoix(party.ChoiceOne, roomuserOne.Client.Language), roomuserTwo.Username));

            EnableEffet(roomuserOne, party.ChoiceOne);
            EnableEffet(roomuserTwo, party.ChoiceTwo);
            return true;
        }

        return true;
    }

    private static string GetStringChoix(JankenType choix, Language langue) => choix switch
    {
        JankenType.Scissors => LanguageManager.TryGetValue("janken.ciseaux", langue),
        JankenType.Paper => LanguageManager.TryGetValue("janken.feuille", langue),
        JankenType.Rock => LanguageManager.TryGetValue("janken.pierre", langue),
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
            var roomuserOne = room.RoomUserManager.GetRoomUserByUserId(party.UserOne);
            var roomuserTwo = room.RoomUserManager.GetRoomUserByUserId(party.UserTwo);

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
        if (this._party.TryGetValue(id, out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }
}
