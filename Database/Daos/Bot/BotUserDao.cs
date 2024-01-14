namespace WibboEmulator.Database.Daos.Bot;
using System.Data;
using WibboEmulator.Games.Rooms;
using WibboEmulator.Games.Rooms.AI;
using Dapper;

internal sealed class BotUserDao
{
    internal static void SaveBots(IDbConnection dbClient, List<RoomUser> botList)
    {
        var botUpdateList = new List<BotUserEntity>();

        foreach (var bot in botList)
        {
            var botData = bot.BotData;
            if (botData.AiType == BotAIType.RoleplayBot)
            {
                continue;
            }

            if (bot.X != botData.X || bot.Y != botData.Y || bot.Z != botData.Z || bot.RotBody != botData.Rot)
            {
                botUpdateList.Add(new BotUserEntity { X = bot.X, Y = bot.Y, Z = bot.Z, Rotation = bot.RotBody, Id = botData.Id });
            }
        }

        if (botUpdateList.Count != 0)
        {
            _ = dbClient.Execute(
                @"UPDATE bot_user 
                SET x = @X, y = @Y, z = @Z, rotation = @RotBody 
                WHERE id = @Id",
                botUpdateList);
        }
    }

    internal static void UpdateRoomId(IDbConnection dbClient, int botId) => dbClient.Execute(
        "UPDATE bot_user SET room_id = 0 WHERE id = @BotId LIMIT 1",
        new { BotId = botId });

    internal static void UpdatePosition(IDbConnection dbClient, int botId, int roomId, int x, int y) => dbClient.Execute(
        "UPDATE `bot_user` SET room_id = '" + roomId + "', x = '" + x + "', y = '" + y + "' WHERE id = '" + botId + "'");

    internal static void UpdateLookGender(IDbConnection dbClient, int botId, string gender, string look) => dbClient.Execute(
        "UPDATE bot_user SET look = @Look, gender = @Gender WHERE id = @BotId LIMIT 1",
        new { Look = look, Gender = gender, BotId = botId });

    internal static void UpdateChat(IDbConnection dbClient, int botId, bool automaticChat, int speakingInterval, bool mixChat, string chatText) => dbClient.Execute(
        @"UPDATE bot_user 
        SET chat_enabled = @AutomaticChat, 
            chat_seconds = @SpeakingInterval, 
            is_mixchat = @MixChat, 
            chat_text = @ChatText 
        WHERE id = @BotId LIMIT 1",
        new
        {
            BotId = botId,
            AutomaticChat = automaticChat,
            SpeakingInterval = speakingInterval,
            MixChat = mixChat,
            ChatText = chatText
        });

    internal static void UpdateChatGPT(IDbConnection dbClient, int botId, string chatText) => dbClient.Execute(
        "UPDATE bot_user SET chat_text = @ChatText, ai_type = 'chatgpt' WHERE id = @BotId LIMIT 1",
        new { BotId = botId, ChatText = chatText });

    internal static void UpdateWalkEnabled(IDbConnection dbClient, int botId, bool balkingEnabled) => dbClient.Execute(
        "UPDATE `bot_user` SET walk_enabled = '" + (balkingEnabled ? "1" : "0") + "' WHERE id = '" + botId + "'");

    internal static void UpdateIsDancing(IDbConnection dbClient, int botId, bool isDancing) => dbClient.Execute(
        "UPDATE `bot_user` SET is_dancing = '" + (isDancing ? "1" : "0") + "' WHERE id = '" + botId + "'");

    internal static void UpdateName(IDbConnection dbClient, int botId, string name) => dbClient.Execute(
        "UPDATE bot_user SET name = @Name WHERE id = @BotId LIMIT 1",
        new { BotId = botId, Name = name });

    internal static void UpdateRoomBot(IDbConnection dbClient, int roomId) => dbClient.Execute(
        "UPDATE `bot_user` SET room_id = '0' WHERE room_id = '" + roomId + "'");

    internal static int InsertAndGetId(IDbConnection dbClient, int ownerId, string name, string motto, string figure, string gender) => dbClient.ExecuteScalar<int>(
        "INSERT INTO bot_user (user_id, name, motto, look, gender, chat_text) VALUES (@OwnerId, @Name, @Motto, @Figure, @Gender, ''); SELECT LAST_INSERT_ID()",
        new { OwnerId = ownerId, Name = name, Motto = motto, Figure = figure, Gender = gender });

    internal static BotUserEntity GetOne(IDbConnection dbClient, int ownerId, int id) => dbClient.QuerySingleOrDefault<BotUserEntity>(
        "SELECT id, user_id, name, motto, look, gender FROM `bot_user` WHERE user_id = @OwnerId AND id = @Id LIMIT 1",
        new { OwnerId = ownerId, Id = id });

    internal static void DupliqueAllBotInRoomId(IDbConnection dbClient, int userId, int roomId, int oldRoomId) => dbClient.Execute(
        "INSERT INTO `bot_user` (user_id, name, motto, gender, look, room_id, walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat) " +
        "SELECT '" + userId + "', name, motto, gender, look, '" + roomId + "', walk_enabled, x, y, z, rotation, chat_enabled, chat_text, chat_seconds, is_dancing, is_mixchat FROM `bot_user` WHERE room_id = '" + oldRoomId + "'");

    internal static void UpdateEnable(IDbConnection dbClient, int botId, int enableId) => dbClient.Execute(
        "UPDATE `bot_user` SET enable = '" + enableId + "' WHERE id = '" + botId + "'");

    internal static void UpdateHanditem(IDbConnection dbClient, int botId, int handItem) => dbClient.Execute(
        "UPDATE `bot_user` SET handitem = '" + handItem + "' WHERE id = '" + botId + "'");

    internal static void UpdateRotation(IDbConnection dbClient, int botId, int rotBody) => dbClient.Execute(
        "UPDATE `bot_user` SET rotation = '" + rotBody + "' WHERE id = '" + botId + "'");

    internal static void UpdateStatus(IDbConnection dbClient, int botId, int status) => dbClient.Execute(
        "UPDATE `bot_user` SET status = '" + status + "' WHERE id = '" + botId + "'");

    internal static List<BotUserEntity> GetAllByRoomId(IDbConnection dbClient, int roomId) => dbClient.Query<BotUserEntity>(
        @"SELECT `id`, `user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`, `status`, `enable`, `handitem`, `ai_type` 
        FROM `bot_user` 
        WHERE room_id = @RoomId",
        new { RoomId = roomId }
    ).ToList();

    internal static void Delete(IDbConnection dbClient, int userId) => dbClient.Execute(
        "DELETE FROM `bot_user` WHERE room_id = '0' AND user_id = '" + userId + "'");

    internal static List<BotUserEntity> GetAllByUserId(IDbConnection dbClient, int userId, int limit) => dbClient.Query<BotUserEntity>(
        @"SELECT `id`, `user_id`, `name`, `motto`, `gender`, `look`, `room_id`, `walk_enabled`, `x`, `y`, `z`, `rotation`, `chat_enabled`, `chat_text`, `chat_seconds`, `is_dancing`, `is_mixchat`, `status`, `enable`, `handitem`, `ai_type` 
        FROM `bot_user` 
        WHERE user_id = @UserId AND room_id = '0' LIMIT @Limit;",
        new { UserId = userId, Limit = limit }
    ).ToList();
}

public class BotUserEntity
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Name { get; set; }
    public string Motto { get; set; }
    public string Gender { get; set; }
    public string Look { get; set; }
    public int RoomId { get; set; }
    public bool WalkEnabled { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public double Z { get; set; }
    public int Rotation { get; set; }
    public bool ChatEnabled { get; set; }
    public string ChatText { get; set; }
    public int ChatSeconds { get; set; }
    public bool IsDancing { get; set; }
    public bool IsMixChat { get; set; }
    public int Status { get; set; }
    public int Enable { get; set; }
    public int HandItem { get; set; }
    public string AiType { get; set; }
}