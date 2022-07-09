namespace Wibbo.Game.Items
{
    public enum InteractionType
    {
        NONE,
        GATE,
        POSTIT,
        PHOTO,
        ROOMEFFECT,
        MOODLIGHT,
        TROPHY,
        BED,
        SCOREBOARD,
        VENDINGMACHINE,
        ALERT,
        ONEWAYGATE,
        LOVESHUFFLER,
        HABBOWHEEL,
        DICE,
        BOTTLE,
        TELEPORT,
        ARROW,
        rentals,
        PET,
        POOL,
        ROLLER,
        FBGATE,
        ICESKATES,
        NORMSLASKATES,
        LOWPOOL,
        HALOWEENPOOL,
        WALLPAPER,
        FLOOR,
        LANDSCAPE,
        FOOTBALL,
        FOOTBALLGOALGREEN,
        FOOTBALLGOALYELLOW,
        footballgoalblue,
        FOOTBALLGOALRED,
        FOOTBALLCOUNTERGREEN,
        FOOTBALLCOUNTERYELLOW,
        FOOTBALLCOUNTERBLUE,
        FOOTBALLCOUNTERRED,
        BANZAIGATEBLUE,
        BANZAIGATERED,
        BANZAIGATEYELLOW,
        BANZAIGATEGREEN,
        BANZAIFLOOR,
        BANZAISCOREBLUE,
        BANZAISCORERED,
        BANZAISCOREYELLOW,
        BANZAISCOREGREEN,
        BANZAITELE,
        BANZAIPUCK,
        BANZAIPYRAMID,
        BANZAIBLO,
        BANZAIBLOB,
        CHRONOTIMER,
        FREEZEEXIT,
        FREEZEREDCOUNTER,
        FREEZEBLUECOUNTER,
        FREEZEYELLOWCOUNTER,
        FREEZEGREENCOUNTER,
        FREEZEYELLOWGATE,
        FREEZEREDGATE,
        FREEZEGREENGATE,
        FREEZEBLUEGATE,
        FREEZETILEBLOCK,
        FREEZETILE,
        JUKEBOX,
        PUZZLEBOX,
        TRIGGER_ONCE,
        TRIGGER_AVATAR_ENTERS_ROOM,
        TRIGGER_GAME_ENDS,
        TRIGGER_GAME_STARTS,
        TRIGGER_PERIODICALLY,
        TRIGGER_PERIODICALLY_LONG,
        TRIGGER_AVATAR_SAYS_SOMETHING,
        TRIGGER_COMMAND,
        TRIGGER_SELF,
        TRIGGER_COLLISION_USER,
        TRIGGER_SCORE_ACHIEVED,
        TRIGGER_STATE_CHANGED,
        TRIGGER_WALK_ON_FURNI,
        TRIGGER_WALK_OFF_FURNI,
        TRIGGER_COLLISION,
        ACTION_GIVE_SCORE,
        ACTION_POS_RESET,
        ACTION_MOVE_ROTATE,
        ACTION_RESET_TIMER,
        ACTIONSHOWMESSAGE,
        ACTION_TELEPORT_TO,
        ACTION_ENDGAME_TEAM,
        ACTION_CALL_STACKS,
        ACTION_TOGGLE_STATE,
        ACTION_KICK_USER,
        ACTION_FLEE,
        ACTION_CHASE,
        ACTION_COLLISION_CASE,
        ACTION_COLLISION_TEAM,
        ACTION_GIVE_REWARD,
        ACTION_MOVE_TO_DIR,
        CONDITION_FURNIS_HAVE_USERS,
        CONDITION_STUFF_IS,
        CONDITION_NOT_STUFF_IS,
        CONDITION_STATE_POS,
        CONDITION_STATE_POS_NEGATIVE,
        CONDITION_TIME_LESS_THAN,
        CONDITION_TIME_MORE_THAN,
        CONDITION_TRIGGER_ON_FURNI,
        CONDITION_TRIGGER_ON_FURNI_NEGATIVE,
        CONDITION_HAS_FURNI_ON_FURNI,
        CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE,
        CONDITION_FURNIS_HAVE_NO_USERS,
        CONDITION_ACTOR_IN_GROUP,
        CONDITION_NOT_IN_GROUP,
        RINGPLATE,
        COLORTILE,
        COLORWHEEL,
        FLOORSWITCH1,
        FIREGATE,
        GLASSFOOR,
        SPECIALRANDOM,
        SPECIALUNSEEN,
        WIRE,
        WIRECENTER,
        WIRECORNER,
        WIRESPLITTER,
        WIRESTANDARD,
        GIFT,
        MANNEQUIN,
        TONER,
        BOT,
        ADS_BACKGROUND,
        BADGE_DISPLAY,
        BADGE_TROC,
        TVYOUTUBE,
        PILEMAGIC,
        ACTION_SUPER_WIRED,
        CONDITION_SUPER_WIRED,
        JMPHORSE,
        GUILD_ITEM,
        GUILD_GATE,
        PRESSUREPAD,
        GNOMEBOX,
        MONSTERGRAINE,
        HIGHSCORE,
        HIGHSCOREPOINTS,
        VENDINGENABLEMACHINE,
        TRIGGER_BOT_REACHED_STF,
        TRIGGER_BOT_REACHED_AVTR,
        ACTION_BOT_CLOTHES,
        ACTION_BOT_TELEPORT,
        ACTION_BOT_FOLLOW_AVATAR,
        ACTION_BOT_GIVE_HANDITEM,
        ACTION_BOT_MOVE,
        ACTION_BOT_TALK_TO_AVATAR,
        ACTION_BOT_TALK,
        CONDITION_HAS_HANDITEM,
        ACTION_JOIN_TEAM,
        ACTION_LEAVE_TEAM,
        ACTION_GIVE_SCORE_TM,
        CONDITION_ACTOR_IN_TEAM,
        CONDITION_NOT_IN_TEAM,
        CONDITION_NOT_USER_COUNT,
        CONDITION_USER_COUNT_IN,
        ACTION_USER_MOVE,
        CRACKABLE,
        LOVELOCK,
        EXTRABOX,
        DELUXEBOX,
        LEGENDBOX,
        BADGEBOX,
        LOOTBOX2022,
        EXCHANGE,
        HORSE_SADDLE_1,
        HORSE_SADDLE_2,
        HORSE_HAIRSTYLE,
        HORSE_BODY_DYE,
        HORSE_HAIR_DYE,
        BADGE,
        TRAMPOLINE,
        TREADMILL,
        CROSSTRAINER,
        CONDITION_DATE_RNG_ACTIVE
    }

    public class InteractionTypes
    {
        public static InteractionType GetTypeFromString(string pType)
        {
            switch (pType)
            {
                case "default":
                    return InteractionType.NONE;
                case "gate":
                    return InteractionType.GATE;
                case "photo":
                    return InteractionType.PHOTO;
                case "postit":
                    return InteractionType.POSTIT;
                case "roomeffect":
                    return InteractionType.ROOMEFFECT;
                case "dimmer":
                    return InteractionType.MOODLIGHT;
                case "trophy":
                    return InteractionType.TROPHY;
                case "bed":
                    return InteractionType.BED;
                case "scoreboard":
                    return InteractionType.SCOREBOARD;
                case "vendingmachine":
                    return InteractionType.VENDINGMACHINE;
                case "vendingenablemachine":
                    return InteractionType.VENDINGENABLEMACHINE;
                case "alert":
                    return InteractionType.ALERT;
                case "onewaygate":
                    return InteractionType.ONEWAYGATE;
                case "loveshuffler":
                    return InteractionType.LOVESHUFFLER;
                case "habbowheel":
                    return InteractionType.HABBOWHEEL;
                case "dice":
                    return InteractionType.DICE;
                case "bottle":
                    return InteractionType.BOTTLE;
                case "teleport":
                    return InteractionType.TELEPORT;
                case "teleportfloor":
                    return InteractionType.ARROW;
                case "rentals":
                    return InteractionType.rentals;
                case "pet":
                    return InteractionType.PET;
                case "pool":
                case "water":
                    return InteractionType.POOL;
                case "roller":
                    return InteractionType.ROLLER;
                case "fbgate":
                    return InteractionType.FBGATE;
                case "iceskates":
                    return InteractionType.ICESKATES;
                case "normalskates":
                    return InteractionType.NORMSLASKATES;
                case "lowpool":
                    return InteractionType.LOWPOOL;
                case "haloweenpool":
                    return InteractionType.HALOWEENPOOL;
                case "football":
                case "ball":
                    return InteractionType.FOOTBALL;
                case "footballgoalgreen":
                case "green_goal":
                    return InteractionType.FOOTBALLGOALGREEN;
                case "footballgoalyellow":
                case "yellow_goal":
                    return InteractionType.FOOTBALLGOALYELLOW;
                case "footballgoalred":
                case "red_goal":
                    return InteractionType.FOOTBALLGOALRED;
                case "footballgoalblue":
                case "blue_goal":
                    return InteractionType.footballgoalblue;
                case "footballcountergreen":
                    return InteractionType.FOOTBALLCOUNTERGREEN;
                case "footballcounteryellow":
                    return InteractionType.FOOTBALLCOUNTERYELLOW;
                case "footballcounterblue":
                    return InteractionType.FOOTBALLCOUNTERBLUE;
                case "footballcountered":
                    return InteractionType.FOOTBALLCOUNTERRED;
                case "banzaigateblue":
                case "bb_blue_gate":
                    return InteractionType.BANZAIGATEBLUE;
                case "banzaigatered":
                case "bb_red_gate":
                    return InteractionType.BANZAIGATERED;
                case "banzaigateyellow":
                case "bb_yellow_gate":
                    return InteractionType.BANZAIGATEYELLOW;
                case "banzaigategreen":
                case "bb_green_gate":
                    return InteractionType.BANZAIGATEGREEN;
                case "banzaifloor":
                case "bb_patch":
                    return InteractionType.BANZAIFLOOR;
                case "banzaiscoreblue":
                    return InteractionType.BANZAISCOREBLUE;
                case "banzaiscorered":
                    return InteractionType.BANZAISCORERED;
                case "banzaiscoreyellow":
                    return InteractionType.BANZAISCOREYELLOW;
                case "banzaiscoregreen":
                    return InteractionType.BANZAISCOREGREEN;
                case "banzaitele":
                case "bb_teleport":
                    return InteractionType.BANZAITELE;
                case "banzaipuck":
                    return InteractionType.BANZAIPUCK;
                case "banzaipyramid":
                    return InteractionType.BANZAIPYRAMID;
                case "wf_blob2":
                    return InteractionType.BANZAIBLO;
                case "wf_blob":
                    return InteractionType.BANZAIBLOB;
                case "freezetimer":
                case "banzaicounter":
                case "counter":
                    return InteractionType.CHRONOTIMER;
                case "freezeexit":
                    return InteractionType.FREEZEEXIT;
                case "freezeredcounter":
                    return InteractionType.FREEZEREDCOUNTER;
                case "freezebluecounter":
                    return InteractionType.FREEZEBLUECOUNTER;
                case "freezeyellowcounter":
                    return InteractionType.FREEZEYELLOWCOUNTER;
                case "freezegreencounter":
                    return InteractionType.FREEZEGREENCOUNTER;
                case "freezeyellowgate":
                    return InteractionType.FREEZEYELLOWGATE;
                case "freezeredgate":
                    return InteractionType.FREEZEREDGATE;
                case "freezegreengate":
                    return InteractionType.FREEZEGREENGATE;
                case "freezebluegate":
                    return InteractionType.FREEZEBLUEGATE;
                case "freezetileblock":
                    return InteractionType.FREEZETILEBLOCK;
                case "freezetile":
                    return InteractionType.FREEZETILE;
                case "jukebox":
                    return InteractionType.JUKEBOX;
                case "triggertimer":
                case "wf_trg_attime":
                    return InteractionType.TRIGGER_ONCE;
                case "wf_trg_collision":
                    return InteractionType.TRIGGER_COLLISION;
                case "triggerroomenter":
                case "wf_trg_enterroom":
                    return InteractionType.TRIGGER_AVATAR_ENTERS_ROOM;
                case "triggergameend":
                case "wf_trg_gameend":
                    return InteractionType.TRIGGER_GAME_ENDS;
                case "triggergamestart":
                case "wf_trg_gamestart":
                    return InteractionType.TRIGGER_GAME_STARTS;
                case "triggerrepeater":
                case "wf_trg_timer":
                    return InteractionType.TRIGGER_PERIODICALLY;
                case "wf_trg_period_long":
                    return InteractionType.TRIGGER_PERIODICALLY_LONG;
                case "triggeronusersay":
                case "wf_trg_onsay":
                    return InteractionType.TRIGGER_AVATAR_SAYS_SOMETHING;
                case "wf_trg_cmd":
                    return InteractionType.TRIGGER_COMMAND;
                case "wf_trg_cls_user":
                    return InteractionType.TRIGGER_COLLISION_USER;
                case "triggerscoreachieved":
                case "wf_trg_atscore":
                    return InteractionType.TRIGGER_SCORE_ACHIEVED;
                case "triggerstatechanged":
                case "wf_trg_furnistate":
                    return InteractionType.TRIGGER_STATE_CHANGED;
                case "triggerwalkonfurni":
                case "wf_trg_onfurni":
                    return InteractionType.TRIGGER_WALK_ON_FURNI;
                case "triggerwalkofffurni":
                case "wf_trg_offfurni":
                    return InteractionType.TRIGGER_WALK_OFF_FURNI;
                case "actiongivescore":
                case "wf_act_givepoints":
                    return InteractionType.ACTION_GIVE_SCORE;
                case "actionposreset":
                case "wf_act_matchfurni":
                    return InteractionType.ACTION_POS_RESET;
                case "actionmoverotate":
                case "wf_act_moverotate":
                    return InteractionType.ACTION_MOVE_ROTATE;
                case "actionresettimer":
                    return InteractionType.ACTION_RESET_TIMER;
                case "actionshowmessage":
                case "wf_act_saymsg":
                    return InteractionType.ACTIONSHOWMESSAGE;
                case "wf_act_give_reward":
                    return InteractionType.ACTION_GIVE_REWARD;
                case "superwired":
                    return InteractionType.ACTION_SUPER_WIRED;
                case "superwiredcondition":
                    return InteractionType.CONDITION_SUPER_WIRED;
                case "actionteleportto":
                case "wf_act_moveuser":
                    return InteractionType.ACTION_TELEPORT_TO;
                case "wf_act_endgame_team":
                    return InteractionType.ACTION_ENDGAME_TEAM;
                case "wf_act_call_stacks":
                    return InteractionType.ACTION_CALL_STACKS;
                case "actiontogglestate":
                case "wf_act_togglefurni":
                    return InteractionType.ACTION_TOGGLE_STATE;
                case "wf_act_kick_user":
                case "wf_act_kickuser":
                case "wf_act_kick":
                    return InteractionType.ACTION_KICK_USER;
                case "wf_act_flee":
                    return InteractionType.ACTION_FLEE;
                case "wf_act_chase":
                    return InteractionType.ACTION_CHASE;
                case "wf_act_collisioncase":
                    return InteractionType.ACTION_COLLISION_CASE;
                case "wf_act_collisionteam":
                    return InteractionType.ACTION_COLLISION_TEAM;
                case "wf_act_move_to_dir":
                    return InteractionType.ACTION_MOVE_TO_DIR;
                case "conditionfurnishaveusers":
                case "wf_cnd_furnis_hv_avtrs":
                    return InteractionType.CONDITION_FURNIS_HAVE_USERS;
                case "wf_cnd_furnis_hv_prsn":
                case "wf_cnd_not_hv_avtrs":
                    return InteractionType.CONDITION_FURNIS_HAVE_NO_USERS;
                case "conditionstatepos":
                    return InteractionType.CONDITION_STATE_POS;
                case "wf_cnd_stuff_is":
                    return InteractionType.CONDITION_STUFF_IS;
                case "wf_cnd_not_stuff_is":
                    return InteractionType.CONDITION_NOT_STUFF_IS;
                case "wf_cnd_not_match_snap":
                    return InteractionType.CONDITION_STATE_POS_NEGATIVE;
                case "conditiontimelessthan":
                    return InteractionType.CONDITION_TIME_LESS_THAN;
                case "conditiontimemorethan":
                    return InteractionType.CONDITION_TIME_MORE_THAN;
                case "conditiontriggeronfurni":
                case "wf_cnd_trggrer_on_frn":
                    return InteractionType.CONDITION_TRIGGER_ON_FURNI;
                case "wf_cnd_not_trggrer_on":
                    return InteractionType.CONDITION_TRIGGER_ON_FURNI_NEGATIVE;
                case "wf_cnd_has_furni_on":
                    return InteractionType.CONDITION_HAS_FURNI_ON_FURNI;
                case "wf_cnd_not_furni_on":
                    return InteractionType.CONDITION_HAS_FURNI_ON_FURNI_NEGATIVE;
                case "wf_cnd_date_rng_active":
                    return InteractionType.CONDITION_DATE_RNG_ACTIVE;
                case "ringplate":
                    return InteractionType.RINGPLATE;
                case "colortile":
                    return InteractionType.COLORTILE;
                case "colorwheel":
                    return InteractionType.COLORWHEEL;
                case "floorswitch1":
                case "floorswitch2":
                    return InteractionType.FLOORSWITCH1;
                case "firegate":
                    return InteractionType.FIREGATE;
                case "glassfoor":
                    return InteractionType.GLASSFOOR;
                case "specialrandom":
                case "wf_xtra_random":
                    return InteractionType.SPECIALRANDOM;
                case "specialunseen":
                case "wf_xtra_unseen":
                    return InteractionType.SPECIALUNSEEN;
                case "wire":
                    return InteractionType.WIRE;
                case "wireCenter":
                    return InteractionType.WIRECENTER;
                case "wireCorner":
                    return InteractionType.WIRECORNER;
                case "wireSplitter":
                    return InteractionType.WIRESPLITTER;
                case "wireStandard":
                    return InteractionType.WIRESTANDARD;
                case "puzzlebox":
                    return InteractionType.PUZZLEBOX;
                case "gift":
                    return InteractionType.GIFT;
                case "extrabox":
                    return InteractionType.EXTRABOX;
                case "deluxebox":
                    return InteractionType.DELUXEBOX;
                case "legendbox":
                    return InteractionType.LEGENDBOX;
                case "badgebox":
                    return InteractionType.BADGEBOX;
                case "lootbox2022":
                    return InteractionType.LOOTBOX2022;
                case "maniqui":
                    return InteractionType.MANNEQUIN;
                case "bgupdater":
                    return InteractionType.TONER;
                case "bot":
                    return InteractionType.BOT;
                case "adsbackground":
                    return InteractionType.ADS_BACKGROUND;
                case "badge_display":
                    return InteractionType.BADGE_DISPLAY;
                case "badge_troc":
                    return InteractionType.BADGE_TROC;
                case "tvyoutube":
                    return InteractionType.TVYOUTUBE;
                case "pilemagic":
                    return InteractionType.PILEMAGIC;
                case "jmphorse":
                    return InteractionType.JMPHORSE;
                case "groupfurni":
                    return InteractionType.GUILD_ITEM;
                case "groupgate":
                    return InteractionType.GUILD_GATE;
                case "floor":
                    return InteractionType.FLOOR;
                case "wallpaper":
                    return InteractionType.WALLPAPER;
                case "landscape":
                    return InteractionType.LANDSCAPE;
                case "wf_cnd_actor_in_group":
                    return InteractionType.CONDITION_ACTOR_IN_GROUP;
                case "wf_cnd_not_in_group":
                    return InteractionType.CONDITION_NOT_IN_GROUP;
                case "pressurepad":
                case "arrowplate":
                case "pressure_pad":
                    return InteractionType.PRESSUREPAD;
                case "gnomebox":
                    return InteractionType.GNOMEBOX;
                case "monstergraine":
                    return InteractionType.MONSTERGRAINE;
                case "highscore":
                    return InteractionType.HIGHSCORE;
                case "hightscorepoints":
                    return InteractionType.HIGHSCOREPOINTS;
                case "wf_trg_bot_reached_stf":
                    return InteractionType.TRIGGER_BOT_REACHED_STF;
                case "wf_trg_bot_reached_avtr":
                    return InteractionType.TRIGGER_BOT_REACHED_AVTR;
                case "wf_act_bot_clothes":
                    return InteractionType.ACTION_BOT_CLOTHES;
                case "wf_trg_trigger_self":
                    return InteractionType.TRIGGER_SELF;
                case "wf_act_bot_teleport":
                    return InteractionType.ACTION_BOT_TELEPORT;
                case "wf_act_bot_follow_avatar":
                    return InteractionType.ACTION_BOT_FOLLOW_AVATAR;
                case "wf_act_bot_give_handitem":
                    return InteractionType.ACTION_BOT_GIVE_HANDITEM;
                case "wf_act_bot_move":
                    return InteractionType.ACTION_BOT_MOVE;
                case "wf_act_user_move":
                    return InteractionType.ACTION_USER_MOVE;
                case "wf_act_bot_talk_to_avatar":
                    return InteractionType.ACTION_BOT_TALK_TO_AVATAR;
                case "wf_act_bot_talk":
                    return InteractionType.ACTION_BOT_TALK;
                case "wf_cnd_has_handitem":
                    return InteractionType.CONDITION_HAS_HANDITEM;
                case "wf_act_join_team":
                    return InteractionType.ACTION_JOIN_TEAM;
                case "wf_act_leave_team":
                    return InteractionType.ACTION_LEAVE_TEAM;
                case "wf_act_give_score_tm":
                    return InteractionType.ACTION_GIVE_SCORE_TM;
                case "wf_cnd_actor_in_team":
                    return InteractionType.CONDITION_ACTOR_IN_TEAM;
                case "wf_cnd_not_in_team":
                    return InteractionType.CONDITION_NOT_IN_TEAM;
                case "wf_cnd_not_user_count":
                    return InteractionType.CONDITION_NOT_USER_COUNT;
                case "wf_cnd_user_count_in":
                    return InteractionType.CONDITION_USER_COUNT_IN;
                case "crackable":
                    return InteractionType.CRACKABLE;
                case "lovelock":
                    return InteractionType.LOVELOCK;
                case "exchange":
                    return InteractionType.EXCHANGE;
                case "horse_saddle_1":
                    return InteractionType.HORSE_SADDLE_1;
                case "horse_saddle_2":
                    return InteractionType.HORSE_SADDLE_2;
                case "horse_hairstyle":
                    return InteractionType.HORSE_HAIRSTYLE;
                case "horse_body_dye":
                    return InteractionType.HORSE_BODY_DYE;
                case "horse_hair_dye":
                    return InteractionType.HORSE_HAIR_DYE;
                case "badge":
                    return InteractionType.BADGE;
                case "trampoline":
                    return InteractionType.TRAMPOLINE;
                case "treadmill":
                    return InteractionType.TREADMILL;
                case "crosstrainer":
                    return InteractionType.CROSSTRAINER;
                default:
                    return InteractionType.NONE;
            }
        }
    }
}
