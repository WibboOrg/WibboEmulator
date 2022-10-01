using WibboEmulator.Database.Interfaces;
using WibboEmulator.Games.Chat.Commands;
using WibboEmulator.Games.Chat.Emotions;
using WibboEmulator.Games.Chat.Filter;
using WibboEmulator.Games.Chat.Mentions;
using WibboEmulator.Games.Chat.Pets.Commands;
using WibboEmulator.Games.Chat.Styles;

namespace WibboEmulator.Games.Chat
{
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

        public void Init(IQueryAdapter dbClient)
        {
            this._petCommands.Init(dbClient);
            this._commands.Init(dbClient);
            this._chatStyles.Init(dbClient);
            this._filter.Init(dbClient);
        }

        public ChatEmotionsManager GetEmotions()
        {
            return this._emotions;
        }

        public WordFilterManager GetFilter()
        {
            return this._filter;
        }

        public CommandManager GetCommands()
        {
            return this._commands;
        }

        public PetCommandManager GetPetCommands()
        {
            return this._petCommands;
        }

        public ChatStyleManager GetChatStyles()
        {
            return this._chatStyles;
        }

        public MentionManager GetMention()
        {
            return this._mention;
        }
    }
}
