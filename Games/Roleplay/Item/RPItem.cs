namespace WibboEmulator.Games.Roleplay.Item;
public class RPItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Title { get; set; }
    public string Desc { get; set; }
    public int Price { get; set; }
    public string Type { get; set; }
    public int Value { get; set; }
    public bool AllowStack { get; set; }
    public RPItemCategory Category { get; set; }
    public int UseType { get; set; }

    public RPItem(int pId, string pName, string pDesc, int pPrice, string pType, int pValue, bool pAllowStack, RPItemCategory pCatagory)
    {
        this.Id = pId;
        this.Name = pName;
        this.Title = pDesc;
        this.Desc = GenerateDesc(pDesc, pType, pValue);
        this.Price = pPrice;
        this.Type = pType;
        this.Value = pValue;
        this.AllowStack = pAllowStack;
        this.Category = pCatagory;
        this.UseType = GetUseType(this.Type);
    }

    private static int GetUseType(string type) =>
        /*  UseType:
        0: Non utilisable
        1: Utilisable mais limité à 1
        2: Utilisable et possibilité de choisir le nombre à utiliser
        */
        type switch
        {
            "openguide" => 1,
            "hit" => 2,
            "enable" => 1,
            "showtime" => 1,
            "money" => 2,
            "munition" => 2,
            "energytired" => 2,
            "healthtired" => 2,
            "healthenergy" => 2,
            "energy" => 2,
            "health" => 2,
            "weapon_cac" => 1,
            "weapon_far" => 1,
            _ => 0,
        };

    private static string GenerateDesc(string desc, string type, int value)
    {
        var text = "[u]" + desc + "[/u][br]";

        switch (type)
        {
            case "openguide":
            {
                text += "Divers aide pour commencer le jeu";
                break;
            }
            case "hit":
            {
                text += "Vous fait perdre " + value + " vie";
                break;
            }
            case "enable":
            {
                text += "Active l'enable " + value;
                break;
            }
            case "showtime":
            {
                text += "Affiche l'heure du jeu";
                break;
            }
            case "money":
            {
                text += "Ajoute " + value + " Dollars";
                break;
            }
            case "munition":
            {
                text += "Ajoute " + value + " munitions";
                break;
            }
            case "energytired":
            {
                text += "Augmente votre énergie de " + value + " mais diminue votre vie du même montant";
                break;
            }
            case "healthtired":
            {
                text += "Augmente votre vie de " + value + " mais diminue votre énergie du même montant";
                break;
            }
            case "healthenergy":
            {
                text += "Augmente votre vie et énergie de " + value;
                break;
            }
            case "energy":
            {
                text += "Augmente votre énergie de " + value;
                break;
            }
            case "health":
            {
                text += "Soigne " + value + " points de vie";
                break;
            }
            case "weapon_cac":
            {
                text += "Arme corps à corps (:cac x)";
                break;
            }
            case "weapon_far":
            {
                text += "Arme distante (:pan)";
                break;
            }
            default:
            {
                text += "Non utilisable";
                break;
            }
        }

        return text;
    }
}
