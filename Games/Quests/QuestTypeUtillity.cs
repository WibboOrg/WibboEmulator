namespace WibboEmulator.Games.Quests;

public class QuestTypeUtillity
{
    public static int GetIntValue(string questCategory) => questCategory switch
    {
        "room_builder" => 2,
        "social" => 3,
        "identity" => 4,
        "explore" => 5,
        "battleball" => 7,
        "freeze" => 8,
        _ => 0,
    };

    public static string GetString(QuestType type) => type switch
    {
        QuestType.FurniMove => "MOVE_ITEM",
        QuestType.FurniRotate => "ROTATE_ITEM",
        QuestType.FurniPlace => "PLACE_ITEM",
        QuestType.FurniPick => "PICKUP_ITEM",
        QuestType.FurniSwitch => "SWITCH_ITEM_STATE",
        QuestType.FurniStack => "STACK_ITEM",
        QuestType.FurniDecorationFloor => "PLACE_FLOOR",
        QuestType.FurniDecorationWall => "PLACE_WALLPAPER",
        QuestType.SocialVisit => "ENTER_OTHERS_ROOM",
        QuestType.SocialChat => "CHAT_WITH_SOMEONE",
        QuestType.SocialFriend => "REQUEST_FRIEND",
        QuestType.SocialRespect => "GIVE_RESPECT",
        QuestType.SocialDance => "DANCE",
        QuestType.SocialWave => "WAVE",
        QuestType.ProfileChangeLook => "CHANGE_FIGURE",
        QuestType.ProfileChangeMotto => "CHANGE_MOTTO",
        QuestType.ProfileBadge => "WEAR_BADGE",
        _ => "FIND_STUFF",
    };
}
