using Butterfly.Database.Daos;
using Butterfly.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Butterfly.Game.Chat.Filter
{
    public sealed class WordFilterManager
    {
        private readonly List<string> _filteredWords;
        private readonly List<string> _pubWords;

        public WordFilterManager()
        {
            this._filteredWords = new List<string>();
            this._pubWords = new List<string>();
        }

        public void Init()
        {
            if (this._filteredWords.Count > 0)
            {
                this._filteredWords.Clear();
            }

            if (this._pubWords.Count > 0)
            {
                this._pubWords.Clear();
            }

            using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
            {
                DataTable Data = RoomSwearwordFilterDao.GetAll(dbClient);

                if (Data != null)
                {
                    foreach (DataRow Row in Data.Rows)
                    {
                        this._filteredWords.Add(Convert.ToString(Row["word"]));
                    }
                }

                DataTable Data2 = WordFilterRetroDao.GetAll(dbClient);

                if (Data2 != null)
                {
                    foreach (DataRow Row in Data2.Rows)
                    {
                        this._pubWords.Add(Convert.ToString(Row["word"]));
                    }
                }
            }
        }

        public void AddFilterPub(string Word)
        {
            if (!this._pubWords.Contains(Word))
            {
                this._pubWords.Add(Word);

                using (IQueryAdapter dbClient = ButterflyEnvironment.GetDatabaseManager().GetQueryReactor())
                    WordFilterRetroDao.Insert(dbClient, Word);
            }
        }

        public string CheckMessage(string Message)
        {
            foreach (string Filter in this._filteredWords.ToList())
            {
                if (Message.ToLower().Contains(Filter))
                {
                    Message = Regex.Replace(Message, Filter, "*****", RegexOptions.IgnoreCase);
                }
            }

            return Message;
        }

        private string StringTranslate(ref string input, string frm, char to)
        {
            for (int i = 0; i < frm.Length; i++)
            {
                input = input.Replace(frm[i], to);
            }
            return input;
        }

        private void ClearMessage(ref string message, bool OnlyLetter = true)
        {
            message = message.Replace("()", "o").Replace("Æ", "ae");

            this.StringTranslate(ref message, "ĀāĂăĄąΑΔΆÀÁÂÃÄÅàáâãäå4@å", 'a');
            this.StringTranslate(ref message, "ßΒþ", 'b');
            this.StringTranslate(ref message, "¢©ĆćĈĉĊċČč€Çç", 'c');
            this.StringTranslate(ref message, "ĎďĐđ", 'd');
            this.StringTranslate(ref message, "ĒēĔĕĖėĘęĚěΈÈÉÊËèéêë3", 'e');
            this.StringTranslate(ref message, "ĜĝĞğĠġĢģ", 'g');
            this.StringTranslate(ref message, "ĤĥĦħΉ", 'h');
            this.StringTranslate(ref message, "¡ĨĩĪīĬĭĮįİıΊΐìíîïÌÍÎÏ1!", 'i');
            this.StringTranslate(ref message, "Ĵĵ", 'j');
            this.StringTranslate(ref message, "Ķķĸ", 'k');
            this.StringTranslate(ref message, "£¦ĹĺĻļĽľĿŀŁłℓ", 'l');
            this.StringTranslate(ref message, "M", 'm');
            this.StringTranslate(ref message, "ŃńŅņŇňŉŊŋπñÑ", 'n');
            this.StringTranslate(ref message, "¤ŌōŎŏŐőΌoOòóôõöÒÓÔÕÖøΩð0", 'o');
            this.StringTranslate(ref message, "Pp₱", 'p');
            this.StringTranslate(ref message, "ŔŕŖŗŘřя®", 'r');
            this.StringTranslate(ref message, "§ŚśŜŝŞşSŠš", 's');
            this.StringTranslate(ref message, "ŢţŤťŦŧ", 't');
            this.StringTranslate(ref message, "ųŨũŪūŬŭŮůŰűŲųùúûüÙÚÛÜ", 'u');
            this.StringTranslate(ref message, "√", 'v');
            this.StringTranslate(ref message, "Ŵŵω", 'w');
            this.StringTranslate(ref message, "×", 'x');
            this.StringTranslate(ref message, "ŶŷΎýÿÝÝ", 'y');
            this.StringTranslate(ref message, "ŹźŻż", 'z');

            if (OnlyLetter)
            {
                message = new Regex(@"[^a-z]", RegexOptions.IgnoreCase).Replace(message, string.Empty);
            }

            message = message.ToLower();
        }

        public bool Ispub(string message)
        {
            if (message.Length <= 3)
            {
                return false;
            }

            this.ClearMessage(ref message);

            foreach (string pattern in this._pubWords)
            {
                if (message.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }

        public bool CheckMessageWord(string message)
        {
            int OldLength = message.Replace(" ", "").Length;

            this.ClearMessage(ref message, false);

            int LetterDelCount = OldLength - message.Length;

            List<string> WordPub = new List<string>() { "go",
                                                        ".fr",
                                                        ".com",
                                                        ".me",
                                                        ".org",
                                                        ".be",
                                                        ".eu",
                                                        ".net",
                                                        "mobi",
                                                        "nouveau",
                                                        "nouvo",
                                                        "connect",
                                                        "invite",
                                                        "recru",
                                                        "staff",
                                                        "ouvr",
                                                        "rejoign",
                                                        "retro",
                                                        "meilleur",
                                                        "direction",
                                                        "rejoin",
                                                        "gratuit",
                                                        "open",
                                                        "http",
                                                        "recrutement",
                                                        "animation",
                                                        "habb",
                                                        "bbo",
                                                        "sansle",
                                                        "city",
                                                        "alpha",
                                                        "gosur",
                                                        "=bb",
                                                        "catalogue",
                                                        "recru",
                                                        };

            int DetectCount = 0;
            foreach (string Pattern in WordPub)
            {
                if (message.Contains(Pattern))
                {
                    DetectCount++;
                    continue;
                }
            }

            if (DetectCount >= 4 || (LetterDelCount > 5 && DetectCount >= 4))
            {
                return true;
            }

            foreach (string Filter in this._filteredWords.ToList())
            {
                if (message.Contains(Filter))
                {
                    return true;
                }
            }

            return false;
        }
    }
}