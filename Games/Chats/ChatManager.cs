namespace WibboEmulator.Games.Chats;
using System.Data;
using WibboEmulator.Games.Chats.Commands;
using WibboEmulator.Games.Chats.Emotions;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Mentions;
using WibboEmulator.Games.Chats.Pets.Commands;
using WibboEmulator.Games.Chats.Styles;

public sealed class ChatManager
{
    /// <summary>
    /// Chat Emoticons.
    /// </summary>
    private readonly ChatEmotionsManager _emotions;

    /// <summary>
    /// Filter Manager.
    /// </summary>
    private readonly WordFilterManager _filter;

    /// <summary>
    /// Commands.
    /// </summary>
    private readonly CommandManager _commands;

    /// <summary>
    /// Pet Commands.
    /// </summary>
    private readonly PetCommandManager _petCommands;

    /// <summary>
    /// Chat styles.
    /// </summary>
    private readonly ChatStyleManager _chatStyles;

    /// <summary>
    /// Chat styles.
    /// </summary>
    private readonly MentionManager _mention;

    /// <summary>
    /// Initializes a new instance of the ChatManager class.
    /// </summary>
    public ChatManager()
    {
        this._emotions = new ChatEmotionsManager();
        this._mention = new MentionManager();
        this._commands = new CommandManager();
        this._petCommands = new PetCommandManager();
        this._chatStyles = new ChatStyleManager();
        this._filter = new WordFilterManager();
    }

    public void Initialize(IDbConnection dbClient)
    {
        this._petCommands.Initialize(dbClient);
        this._commands.Initialize(dbClient);
        this._chatStyles.Initialize(dbClient);
        this._filter.Initialize(dbClient);
    }

    public ChatEmotionsManager GetEmotions() => this._emotions;

    public WordFilterManager GetFilter() => this._filter;

    public CommandManager GetCommands() => this._commands;

    public PetCommandManager GetPetCommands() => this._petCommands;

    public ChatStyleManager GetChatStyles() => this._chatStyles;

    public MentionManager GetMention() => this._mention;
}
