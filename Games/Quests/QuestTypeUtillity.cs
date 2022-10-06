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
        QuestType.FURNI_MOVE => "MOVE_ITEM",
        QuestType.FURNI_ROTATE => "ROTATE_ITEM",
        QuestType.FURNI_PLACE => "PLACE_ITEM",
        QuestType.FURNI_PICK => "PICKUP_ITEM",
        QuestType.FURNI_SWITCH => "SWITCH_ITEM_STATE",
        QuestType.FURNI_STACK => "STACK_ITEM",
        QuestType.FURNI_DECORATION_FLOOR => "PLACE_FLOOR",
        QuestType.FURNI_DECORATION_WALL => "PLACE_WALLPAPER",
        QuestType.SOCIAL_VISIT => "ENTER_OTHERS_ROOM",
        QuestType.SOCIAL_CHAT => "CHAT_WITH_SOMEONE",
        QuestType.SOCIAL_FRIEND => "REQUEST_FRIEND",
        QuestType.SOCIAL_RESPECT => "GIVE_RESPECT",
        QuestType.SOCIAL_DANCE => "DANCE",
        QuestType.SOCIAL_WAVE => "WAVE",
        QuestType.PROFILE_CHANGE_LOOK => "CHANGE_FIGURE",
        QuestType.PROFILE_CHANGE_MOTTO => "CHANGE_MOTTO",
        QuestType.PROFILE_BADGE => "WEAR_BADGE",
        _ => "FIND_STUFF",
    };
}
