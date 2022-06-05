using Butterfly.Core;
using System.Collections.Concurrent;

namespace Butterfly.Game.Rooms.Jankens
{
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

        public void Start(RoomUser User, RoomUser DuelUser)
        {
            if (User.PartyId > 0)
            {
                Janken party = this.GetParty(User.PartyId);
                if (party == null)
                {
                    User.PartyId = 0;
                    return;
                }
                if (party.Started)
                {
                    return;
                }

                this._party.TryRemove(User.PartyId, out party);
            }

            if (DuelUser.PartyId > 0)
            {
                Janken party = this.GetParty(DuelUser.PartyId);
                if (party == null)
                {
                    DuelUser.PartyId = 0;
                    return;
                }

                if (party.UserTwo == User.UserId)
                {
                    party.Started = true;
                    User.PartyId = party.UserOne;

                    User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.start", User.GetClient().Langue));
                    DuelUser.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.start", DuelUser.GetClient().Langue));
                }
                else
                {
                    User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.notwork", User.GetClient().Langue), DuelUser.GetUsername()));
                }
            }
            else
            {
                User.PartyId = User.UserId;
                this._party.TryAdd(User.PartyId, new Janken(User.UserId, DuelUser.UserId));

                User.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.wait", User.GetClient().Langue), DuelUser.GetUsername()));
                DuelUser.SendWhisperChat(User.GetUsername() + " vous défie au JanKen! Utilisez la commande :janken " + User.GetUsername() + " pour accepter le défie");
            }
        }

        public void OnCycle()
        {
            if (this._party.Count == 0)
            {
                return;
            }

            foreach (Janken party in this._party.Values)
            {
                if (!party.Started)
                {
                    continue;
                }

                party.Timer++;

                if (party.Timer >= 60 || (party.ChoiceOne != JankenType.NONE && party.ChoiceTwo != JankenType.NONE))
                {
                    if (this.EndGame(party))
                    {
                        party.Started = false;

                        RoomUser roomuserOne = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserOne);
                        RoomUser roomuserTwo = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserTwo);

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
                foreach (Janken party in this._remove)
                {
                    this._party.TryRemove(party.UserOne, out Janken outparty);
                }

                this._remove.Clear();
            }
        }

        public bool PlayerStarted(RoomUser User)
        {
            Janken party = this.GetParty(User.PartyId);
            if (party == null)
            {
                return false;
            }

            if (!party.Started)
            {
                return false;
            }

            if (party.UserOne == User.UserId && party.ChoiceOne != JankenType.NONE)
            {
                return false;
            }

            if (party.UserTwo == User.UserId && party.ChoiceTwo != JankenType.NONE)
            {
                return false;
            }

            return true;
        }

        public bool PickChoice(RoomUser User, string Message)
        {
            Janken party = this.GetParty(User.PartyId);

            JankenType choice;
            if (Message.ToLower().StartsWith("p"))
            {
                choice = JankenType.ROCK;
            }
            else if (Message.ToLower().StartsWith("f"))
            {
                choice = JankenType.PAPER;
            }
            else if (Message.ToLower().StartsWith("c"))
            {
                choice = JankenType.SCISSORS;
            }
            else
            {
                return false;
            }

            if (party.UserOne == User.UserId)
            {
                party.ChoiceOne = choice;
            }
            else
            {
                party.ChoiceTwo = choice;
            }

            if (User.GetClient() != null)
            {
                User.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.confirmechoice", User.GetClient().Langue) + this.GetStringChoix(choice, User.GetClient().Langue));
            }

            return true;
        }

        private bool EndGame(Janken party)
        {
            RoomUser roomuserOne = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserOne);
            RoomUser roomuserTwo = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserTwo);
            if (roomuserOne == null && roomuserTwo == null)
            {
                return true;
            }

            if (roomuserOne == null)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.GetClient().Langue));
                return true;
            }
            else if (roomuserTwo == null)
            {
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoiceOne == JankenType.NONE && party.ChoiceTwo == JankenType.NONE)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserTwo.GetClient().Langue));
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.annule", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoiceOne == JankenType.NONE)
            {
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserTwo.GetClient().Langue));
                return true;
            }
            else if (party.ChoiceTwo == JankenType.NONE)
            {
                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.forfait", roomuserOne.GetClient().Langue));
                return true;
            }

            if (party.ChoiceOne == party.ChoiceTwo) //match null
            {
                party.ChoiceOne = JankenType.NONE;
                party.ChoiceTwo = JankenType.NONE;

                party.Timer = 0;

                roomuserOne.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserOne.GetClient().Langue));
                roomuserTwo.SendWhisperChat(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.nul", roomuserTwo.GetClient().Langue));

                this.EnableEffet(roomuserOne, party.ChoiceOne);
                this.EnableEffet(roomuserTwo, party.ChoiceTwo);
                return false;
            }

            //ChoixOne qui gagne
            if ((party.ChoiceOne == JankenType.SCISSORS && party.ChoiceTwo == JankenType.PAPER) ||
                (party.ChoiceOne == JankenType.PAPER && party.ChoiceTwo == JankenType.ROCK) ||
                (party.ChoiceOne == JankenType.ROCK && party.ChoiceTwo == JankenType.SCISSORS))
            {
                roomuserOne.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserOne.GetClient().Langue), this.GetStringChoix(party.ChoiceOne, roomuserOne.GetClient().Langue), this.GetStringChoix(party.ChoiceTwo, roomuserOne.GetClient().Langue), roomuserTwo.GetUsername()));
                roomuserTwo.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserTwo.GetClient().Langue), this.GetStringChoix(party.ChoiceOne, roomuserTwo.GetClient().Langue), this.GetStringChoix(party.ChoiceTwo, roomuserTwo.GetClient().Langue), roomuserOne.GetUsername()));

                this.EnableEffet(roomuserOne, party.ChoiceOne);
                this.EnableEffet(roomuserTwo, party.ChoiceTwo);
                return true;
            }

            //ChoixTwo qui gagne
            if ((party.ChoiceOne == JankenType.SCISSORS && party.ChoiceTwo == JankenType.ROCK) ||
                (party.ChoiceOne == JankenType.PAPER && party.ChoiceTwo == JankenType.SCISSORS) ||
                (party.ChoiceOne == JankenType.ROCK && party.ChoiceTwo == JankenType.PAPER))
            {
                roomuserTwo.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.win", roomuserTwo.GetClient().Langue), this.GetStringChoix(party.ChoiceTwo, roomuserTwo.GetClient().Langue), this.GetStringChoix(party.ChoiceOne, roomuserTwo.GetClient().Langue), roomuserOne.GetUsername()));
                roomuserOne.SendWhisperChat(string.Format(ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.loose", roomuserOne.GetClient().Langue), this.GetStringChoix(party.ChoiceTwo, roomuserOne.GetClient().Langue), this.GetStringChoix(party.ChoiceOne, roomuserOne.GetClient().Langue), roomuserTwo.GetUsername()));

                this.EnableEffet(roomuserOne, party.ChoiceOne);
                this.EnableEffet(roomuserTwo, party.ChoiceTwo);
                return true;
            }

            return true;
        }

        private string GetStringChoix(JankenType Choix, Language langue)
        {
            switch (Choix)
            {
                case JankenType.SCISSORS:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.ciseaux", langue);
                case JankenType.PAPER:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.feuille", langue);
                case JankenType.ROCK:
                    return ButterflyEnvironment.GetLanguageManager().TryGetValue("janken.pierre", langue);
                default:
                    return "";
            }
        }

        private void EnableEffet(RoomUser user, JankenType Janken)
        {
            if (Janken == JankenType.SCISSORS)
            {
                user.ApplyEffect(563, true);
            }
            else if (Janken == JankenType.ROCK)
            {
                user.ApplyEffect(565, true);
            }
            else if (Janken == JankenType.PAPER)
            {
                user.ApplyEffect(564, true);
            }

            user.TimerResetEffect = 10;
        }

        public void RemovePlayer(RoomUser User)
        {
            if (User.PartyId == 0)
            {
                return;
            }

            Janken party = this.GetParty(User.PartyId);
            if (party == null)
            {
                return;
            }

            if (!party.Started)
            {
                RoomUser roomuserOne = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserOne);
                RoomUser roomuserTwo = this._roomInstance.GetRoomUserManager().GetRoomUserByUserId(party.UserTwo);

                if (roomuserOne != null)
                {
                    roomuserOne.PartyId = 0;
                }

                if (roomuserTwo != null)
                {
                    roomuserTwo.PartyId = 0;
                }

                this._party.TryRemove(party.UserOne, out party);
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
}
