namespace WibboEmulator.Games.Rooms.Utils;

internal static class TransformUtility
{
    public static string GetRace(string namePet, int raceId)
    {
        switch (namePet.ToLower())
        {
            /*case "bigeuthanasie":
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
                }*/
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
            /*case "littleleanie":
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

                }*/
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
                return "51 " + raceId + " FFFFFF";

            }
            case "littlemartial":
            {
                return "50 " + raceId + " FFFFFF";

            }
            case "bigzeers":
            {
                return "49 " + raceId + " FFFFFF";

            }
            case "littlezeers":
            {
                return "48 " + raceId + " FFFFFF";

            }
            case "bigkodamas":
            {
                return "47 " + raceId + " FFFFFF";

            }
            case "littlekodamas":
            {
                return "46 " + raceId + " FFFFFF";

            }
            case "maggie":
            {
                return "45 " + raceId + " FFFFFF";

            }
            case "vache":
            {
                return "44 " + raceId + " FFFFFF";

            }
            case "bebe":
            {
                return "27 " + raceId + " FFFFFF";

            }
            //case "pikachuold":
            //{
            //return "28 " + RaceId + " FFFFFF";
            //
            //}
            case "bebeterrier":
            {
                return "26 " + raceId + " FFFFFF";

            }
            case "lapinjaune":
            {
                return "24 " + raceId + " FFFFFF";

            }
            case "singedemon":
            {
                return "23 " + raceId + " FFFFFF";

            }
            case "pigeonnoir":
            {
                return "22 " + raceId + " FFFFFF";

            }
            case "pigeonblanc":
            {
                return "21 " + raceId + " FFFFFF";

            }
            case "lapinrose":
            {
                return "20 " + raceId + " FFFFFF";

            }
            case "lapinbrun":
            {
                return "19 " + raceId + " FFFFFF";

            }
            case "lapinnoir":
            {
                return "18 " + raceId + " FFFFFF";

            }
            case "lapin":
            case "lapinmonstre":
            {
                return "17 " + raceId + " FFFFFF";

            }
            case "monster":
            {
                return "15 " + raceId + " FFFFFF";

            }
            case "monsterplante":
            {
                return "16 " + raceId + " FFFFFF";

            }
            case "cheval":
            {
                return "13 " + raceId + " FFFFFF";

            }
            case "singe":
            {
                return "14 " + raceId + " FFFFFF";

            }
            case "tortue":
            {
                return "9 " + raceId + " FFFFFF";

            }
            case "dragon":
            {
                return "12 " + raceId + " FFFFFF";

            }
            case "poussin":
            {
                return "10 " + raceId + " FFFFFF";

            }
            case "grenouille":
            {
                return "11 " + raceId + " FFFFFF";

            }
            case "spider":
            {
                return "8 " + raceId + " FFFFFF";

            }
            case "lion":
            {
                return "6 " + raceId + " FFFFFF";

            }
            case "cochon":
            {
                return "5 " + raceId + " FFFFFF";

            }
            case "terrier":
            {
                return "3 " + raceId + " FFFFFF";

            }
            case "ours":
            {
                return "4 " + raceId + " FFFFFF";

            }
            case "chat":
            {
                return "1 " + raceId + " FFFFFF";

            }
            case "chien":
            {
                return "0 " + raceId + " FFFFFF";

            }
            case "crocodile":
            {
                return "2 " + raceId + " FFFFFF";

            }
            case "rhino":
            {
                return "7 " + raceId + " FFFFFF";

            }
            case "gnome":
            {
                return "29 " + raceId + " FFFFFF";

            }
            case "oursons":
            {
                return "25 " + raceId + " FFFFFF";

            }
            case "bebeelephant":
            {
                return "30 " + raceId + " FFFFFF";

            }
            case "bebepingouin":
            {
                return "31 " + raceId + " FFFFFF";

            }
            case "pikachu":
            {
                return "32 " + raceId + " FFFFFF";

            }
            case "louveteau":
            {
                return "33 " + raceId + " FFFFFF";

            }
            case "hamster":
            {
                return "34 " + raceId + " FFFFFF";

            }
            case "oeuf":
            case "monsteregg":
            {
                return "36 " + raceId + " FFFFFF";

            }
            case "yoshi":
            {
                return "35 " + raceId + " FFFFFF";

            }
            case "kittenbaby":
            case "chaton":
            {
                return "37 " + raceId + " FFFFFF";

            }
            case "puppybaby":
            case "chiot":
            {
                return "38 " + raceId + " FFFFFF";

            }
            case "pigletbaby":
            case "porcelet":
            {
                return "39 " + raceId + " FFFFFF";

            }
            case "fools":
            case "pierre":
            {
                return "40 " + raceId + " FFFFFF";

            }
            case "haloompa":
            case "wiloompa":
            {
                return "41 " + raceId + " FFFFFF";

            }
            case "pterosaur":
            {
                return "42 " + raceId + " FFFFFF";

            }
            case "velociraptor":
            {
                return "43 " + raceId + " FFFFFF";

            }
            default:
            {
                return "";
            }
        }
    }
}
