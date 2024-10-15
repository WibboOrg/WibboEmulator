namespace WibboEmulator.Games.Chats.Commands.Staff.Administration;
using WibboEmulator.Communication.Packets.Outgoing.Catalog;
using WibboEmulator.Communication.Packets.Outgoing.Economy;
using WibboEmulator.Communication.Packets.Outgoing.Notifications.NotifCustom;
using WibboEmulator.Communication.Packets.Outgoing.Rooms.Chat;
using WibboEmulator.Communication.WebSocket;
using WibboEmulator.Core.FigureData;
using WibboEmulator.Core.Language;
using WibboEmulator.Core.Settings;
using WibboEmulator.Database;
using WibboEmulator.Games.Animations;
using WibboEmulator.Games.Badges;
using WibboEmulator.Games.Banners;
using WibboEmulator.Games.Catalogs;
using WibboEmulator.Games.Chats.Filter;
using WibboEmulator.Games.Chats.Styles;
using WibboEmulator.Games.Effects;
using WibboEmulator.Games.GameClients;
using WibboEmulator.Games.Items;
using WibboEmulator.Games.LandingView;
using WibboEmulator.Games.Loots;
using WibboEmulator.Games.Moderations;
using WibboEmulator.Games.Navigators;
using WibboEmulator.Games.Permissions;
using WibboEmulator.Games.Roleplays;
using WibboEmulator.Games.Roleplays.Enemy;
using WibboEmulator.Games.Roleplays.Item;
using WibboEmulator.Games.Roleplays.Weapon;
using WibboEmulator.Games.Rooms;

internal sealed class Update : IChatCommand
{
    public void Execute(GameClient Session, Room room, RoomUser userRoom, string[] parameters)
    {
        if (parameters.Length != 2)
        {
            return;
        }

        var cmd = parameters[1];

        if (string.IsNullOrEmpty(cmd))
        {
            return;
        }

        using var dbClient = DatabaseManager.Connection;
        switch (cmd.ToLower())
        {
            case "emuban":
            {
                WebSocketManager.ResetBan();
                Session.SendWhisper("Réinitialisation des bannissements de l'émulateur");
                break;
            }
            case "landingview":
            case "view":
            case "lv":
            case "vue":
            {
                LandingViewManager.Initialize(dbClient);
                Session.SendWhisper("Vue et promotion mises à jour");
                break;
            }
            case "hof":
            {
                HallOfFameManager.Initialize(dbClient);
                Session.SendWhisper("Hof mises à jour");
                break;
            }
            case "text":
            case "texte":
            case "locale":
            {
                LanguageManager.Initialize(dbClient);
                Session.SendWhisper("Local mis à jour");
                break;
            }
            case "autogame":
            {
                AnimationManager.Initialize(dbClient);
                Session.SendWhisper("Jeux automatique mis à jour");
                break;
            }
            case "lootbox":
            {
                LootManager.Initialize(dbClient);
                Session.SendWhisper("Lootbox mis à jour");
                break;
            }
            case "badge":
            {
                BadgeManager.Initialize(dbClient);
                Session.SendWhisper("RP Items mis à jour");
                break;
            }
            case "rpitems":
            {
                RPItemManager.Initialize(dbClient);
                Session.SendWhisper("Badges mis à jour");
                break;
            }
            case "rpweapon":
            {
                RPWeaponManager.Initialize(dbClient);
                Session.SendWhisper("RP Weapon mis à jour");
                break;
            }
            case "rpenemy":
            {
                RPEnemyManager.Initialize(dbClient);
                Session.SendWhisper("RP Enemy mis à jour");
                break;
            }
            case "cmd":
            case "commands":
            {
                CommandManager.Initialize(dbClient);
                Session.SendWhisper("Commands mis à jour");
                break;
            }
            case "chat":
            {
                ChatStyleManager.Initialize(dbClient);
                Session.SendWhisper("Chat mis à jour");
                break;
            }
            case "permission":
            {
                PermissionManager.Initialize(dbClient);
                Session.SendWhisper("Permissions mises à jour !");
                break;
            }
            case "effet":
            case "enable":
            {
                EffectManager.Initialize(dbClient);
                Session.SendWhisper("Effet mis à jour");
                break;
            }
            case "rp":
            case "roleplay":
            {
                RoleplayManager.Initialize(dbClient);
                Session.SendWhisper("Role play mis à jour");
                break;
            }
            case "moderation":
            {
                ModerationManager.Initialize(dbClient);
                Session.SendWhisper("Moderation mis à jour");
                GameClientManager.SendMessageStaff(new WhisperComposer(userRoom.VirtualId, "Les outils de modération viennent d'être mis à jour, reconnectez-vous!", 23));
                break;
            }
            case "catalogue":
            case "cata":
            {
                ItemManager.Initialize(dbClient);
                CatalogManager.Initialize(dbClient);
                GameClientManager.SendMessage(new CatalogUpdatedComposer());
                Session.SendWhisper("Catalogue mis à jour");
                break;
            }
            case "navigateur":
            case "navi":
            case "navigator":
            {
                NavigatorManager.Initialize(dbClient);
                Session.SendWhisper("Navigateur mis à jour");
                break;
            }
            case "filter":
            case "filtre":
            {
                WordFilterManager.Initialize(dbClient);
                Session.SendWhisper("Filtre mis à jour");
                break;
            }
            case "items":
            case "furni":
            {
                ItemManager.Initialize(dbClient);
                Session.SendWhisper("Items mis à jour");
                break;
            }
            case "model":
            {
                RoomManager.Initialize(dbClient);
                Session.SendWhisper("Model mis à jour");
                break;
            }
            case "mutant":
            case "figure":
            {
                FigureDataManager.Initialize();
                Session.SendWhisper("Mutant/Figure mises à jour");
                break;
            }
            case "setting":
            case "settings":
            {
                SettingsManager.Initialize(dbClient);
                Session.SendWhisper("Paramètre mises à jour");
                break;
            }
            case "banner":
            {
                BannerManager.Initialize(dbClient);
                Session.SendWhisper("Bannière mises à jour");
                break;
            }
            case "economy":
                EconomyCenterManager.Initialize(dbClient);
                GameClientManager.SendMessage(new EconomyCenterComposer(EconomyCenterManager.EconomyCategory, EconomyCenterManager.EconomyItem));
                Session.SendWhisper("Centre économique mis à jour");
                break;
            default:
            {
                Session.SendWhisper(LanguageManager.TryGetValue("cmd.notfound", Session.Language));
                return;
            }
        }
    }
}
