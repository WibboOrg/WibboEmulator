namespace Butterfly.Game.Rooms.Utils
{
    internal static class TransformUtility
    {
        public static string GetRace(string NamePet, int RaceId)
        {
            switch (NamePet.ToLower())
            {
                case "bigeuthanasie":
                    {
                        return "85 0 FFFFFF";
                    }
                case "littleeuthanasie":
                    {
                        return "84 0 FFFFFF";
                    }
                case "littleninou":
                    {
                        return "83 0 FFFFFF";
                    }
                case "bigninou":
                    {
                        return "82 0 FFFFFF";
                    }
                case "littlexorcist":
                    {
                        return "81 0 FFFFFF";
                    }
                case "bigxorcist":
                    {
                        return "80 0 FFFFFF";
                    }
                case "littlejason":
                    {
                        return "79 0 FFFFFF";
                    }
                case "bigjason":
                    {
                        return "78 0 FFFFFF";
                    }
                case "littleseonsaengnim":
                    {
                        return "77 0 FFFFFF";
                    }
                case "bigseonsaengnim":
                    {
                        return "76 0 FFFFFF";

                    }
                case "littleleanie":
                    {
                        return "75 0 FFFFFF";

                    }
                case "bigleanie":
                    {
                        return "74 0 FFFFFF";

                    }
                case "littlebelchonok":
                    {
                        return "73 0 FFFFFF";

                    }
                case "bigbelchonok":
                    {
                        return "72 0 FFFFFF";

                    }
                case "littleboy1":
                    {
                        return "62 0 FFFFFF";

                    }
                case "littleboy2":
                    {
                        return "63 0 FFFFFF";

                    }
                case "littleboy3":
                    {
                        return "64 0 FFFFFF";

                    }
                case "littleboy4":
                    {
                        return "65 0 FFFFFF";

                    }
                case "littleboy5":
                    {
                        return "66 0 FFFFFF";

                    }
                case "littlegirl1":
                    {
                        return "67 0 FFFFFF";

                    }
                case "littlegirl2":
                    {
                        return "68 0 FFFFFF";

                    }
                case "littlegirl3":
                    {
                        return "69 0 FFFFFF";

                    }
                case "littlegirl4":
                    {
                        return "70 0 FFFFFF";

                    }
                case "littlegirl5":
                    {
                        return "71 0 FFFFFF";

                    }
                case "bigboy1":
                    {
                        return "52 0 FFFFFF";

                    }
                case "bigboy2":
                    {
                        return "53 0 FFFFFF";

                    }
                case "bigboy3":
                    {
                        return "54 0 FFFFFF";

                    }
                case "bigboy4":
                    {
                        return "55 0 FFFFFF";

                    }
                case "bigboy5":
                    {
                        return "56 0 FFFFFF";

                    }
                case "biggirl1":
                    {
                        return "57 0 FFFFFF";

                    }
                case "biggirl2":
                    {
                        return "58 0 FFFFFF";

                    }
                case "biggirl3":
                    {
                        return "59 0 FFFFFF";

                    }
                case "biggirl4":
                    {
                        return "60 0 FFFFFF";

                    }
                case "biggirl5":
                    {
                        return "61 0 FFFFFF";

                    }
                case "bigmartial":
                    {
                        return "51 " + RaceId + " FFFFFF";

                    }
                case "littlemartial":
                    {
                        return "50 " + RaceId + " FFFFFF";

                    }
                case "bigzeers":
                    {
                        return "49 " + RaceId + " FFFFFF";

                    }
                case "littlezeers":
                    {
                        return "48 " + RaceId + " FFFFFF";

                    }
                case "bigkodamas":
                    {
                        return "47 " + RaceId + " FFFFFF";

                    }
                case "littlekodamas":
                    {
                        return "46 " + RaceId + " FFFFFF";

                    }
                case "maggie":
                    {
                        return "45 " + RaceId + " FFFFFF";

                    }
                case "vache":
                    {
                        return "44 " + RaceId + " FFFFFF";

                    }
                case "bebe":
                    {
                        return "27 " + RaceId + " FFFFFF";

                    }
                //case "pikachuold":
                //{
                //return "28 " + RaceId + " FFFFFF";
                //
                //}
                case "bebeterrier":
                    {
                        return "26 " + RaceId + " FFFFFF";

                    }
                case "lapinjaune":
                    {
                        return "24 " + RaceId + " FFFFFF";

                    }
                case "singedemon":
                    {
                        return "23 " + RaceId + " FFFFFF";

                    }
                case "pigeonnoir":
                    {
                        return "22 " + RaceId + " FFFFFF";

                    }
                case "pigeonblanc":
                    {
                        return "21 " + RaceId + " FFFFFF";

                    }
                case "lapinrose":
                    {
                        return "20 " + RaceId + " FFFFFF";

                    }
                case "lapinbrun":
                    {
                        return "19 " + RaceId + " FFFFFF";

                    }
                case "lapinnoir":
                    {
                        return "18 " + RaceId + " FFFFFF";

                    }
                case "lapin":
                case "lapinmonstre":
                    {
                        return "17 " + RaceId + " FFFFFF";

                    }
                case "monster":
                    {
                        return "15 " + RaceId + " FFFFFF";

                    }
                case "monsterplante":
                    {
                        return "16 " + RaceId + " FFFFFF";

                    }
                case "cheval":
                    {
                        return "13 " + RaceId + " FFFFFF";

                    }
                case "singe":
                    {
                        return "14 " + RaceId + " FFFFFF";

                    }
                case "tortue":
                    {
                        return "9 " + RaceId + " FFFFFF";

                    }
                case "dragon":
                    {
                        return "12 " + RaceId + " FFFFFF";

                    }
                case "poussin":
                    {
                        return "10 " + RaceId + " FFFFFF";

                    }
                case "grenouille":
                    {
                        return "11 " + RaceId + " FFFFFF";

                    }
                case "arraigne":
                    {
                        return "8 " + RaceId + " FFFFFF";

                    }
                case "lion":
                    {
                        return "6 " + RaceId + " FFFFFF";

                    }
                case "cochon":
                    {
                        return "5 " + RaceId + " FFFFFF";

                    }
                case "terrier":
                    {
                        return "3 " + RaceId + " FFFFFF";

                    }
                case "ours":
                    {
                        return "4 " + RaceId + " FFFFFF";

                    }
                case "chat":
                    {
                        return "1 " + RaceId + " FFFFFF";

                    }
                case "chien":
                    {
                        return "0 " + RaceId + " FFFFFF";

                    }
                case "crocodile":
                    {
                        return "2 " + RaceId + " FFFFFF";

                    }
                case "rhino":
                    {
                        return "7 " + RaceId + " FFFFFF";

                    }
                case "gnome":
                    {
                        return "29 " + RaceId + " FFFFFF";

                    }
                case "oursons":
                    {
                        return "25 " + RaceId + " FFFFFF";

                    }
                case "bebeelephant":
                    {
                        return "30 " + RaceId + " FFFFFF";

                    }
                case "bebepingouin":
                    {
                        return "31 " + RaceId + " FFFFFF";

                    }
                case "pikachu":
                    {
                        return "32 " + RaceId + " FFFFFF";

                    }
                case "louveteau":
                    {
                        return "33 " + RaceId + " FFFFFF";

                    }
                case "hamster":
                    {
                        return "34 " + RaceId + " FFFFFF";

                    }
                case "oeuf":
                case "monsteregg":
                    {
                        return "36 " + RaceId + " FFFFFF";

                    }
                case "yoshi":
                    {
                        return "35 " + RaceId + " FFFFFF";

                    }
                case "kittenbaby":
                case "chaton":
                    {
                        return "37 " + RaceId + " FFFFFF";

                    }
                case "puppybaby":
                case "chiot":
                    {
                        return "38 " + RaceId + " FFFFFF";

                    }
                case "pigletbaby":
                case "porcelet":
                    {
                        return "39 " + RaceId + " FFFFFF";

                    }
                case "fools":
                case "pierre":
                    {
                        return "40 " + RaceId + " FFFFFF";

                    }
                case "haloompa":
                case "wiloompa":
                    {
                        return "41 " + RaceId + " FFFFFF";

                    }
                case "pterosaur":
                    {
                        return "42 " + RaceId + " FFFFFF";

                    }
                case "velociraptor":
                    {
                        return "43 " + RaceId + " FFFFFF";

                    }
                default:
                    {
                        return "";
                    }
            }
        }
    }
}
