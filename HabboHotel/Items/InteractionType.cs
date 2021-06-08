namespace Butterfly.HabboHotel.Items
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
        bottle,
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
        TRIGGERTIMER,
        TRIGGERROOMENTER,
        TRIGGERGAMEEND,
        TRIGGERGAMESTART,
        TRIGGERREPEATER,
        TRIGGERREPEATERLONG,
        TRIGGERONUSERSAY,
        TRIGGERCOMMAND,
        WIRED_TRIGGER_SELF,
        TRIGGERCOLLISIONUSER,
        TRIGGERSCOREACHIEVED,
        TRIGGERSTATECHANGED,
        TRIGGERWALKONFURNI,
        TRIGGERWALKOFFFURNI,
        TRIGGERCOLLISION,
        ACTIONGIVESCORE,
        ACTIONPOSRESET,
        ACTIONMOVEROTATE,
        ACTIONRESETTIMER,
        ACTIONSHOWMESSAGE,
        ACTIONTELEPORTTO,
        WF_ACT_ENDGAME_TEAM,
        WF_ACT_CALL_STACKS,
        ACTIONTOGGLESTATE,
        ACTIONKICKUSER,
        ACTIONFLEE,
        ACTIONCHASE,
        COLLISIONCASE,
        COLLISIONTEAM,
        ACTIONGIVEREWARD,
        ACTIONMOVETODIR,
        CONDITIONFURNISHAVEUSERS,
        WF_CND_STUFF_IS,
        WF_CND_NOT_STUFF_IS,
        CONDITIONSTATEPOS,
        CONDITIONSTATEPOSNEGATIVE,
        CONDITIONTIMELESSTHAN,
        CONDITIONTIMEMORETHAN,
        CONDITIONTRIGGERONFURNI,
        CONDITIONTRIGGERONFURNINEGATIVE,
        CONDITIONHASFURNIONFURNI,
        CONDITIONHASFURNIONFURNINEGATIVE,
        CONDITIONFURNISHAVENOUSERS,
        CONDITIONACTORINGROUP,
        CONDITIONNOTINGROUP,
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
        ADSBACKGROUND,
        BADGE_DISPLAY,
        BADGE_TROC,
        TVYOUTUBE,
        PILEMAGIC,
        SUPERWIRED,
        SUPERWIREDCONDITION,
        JMPHORSE,
        GUILD_ITEM,
        GUILD_GATE,
        PRESSUREPAD,
        GNOMEBOX,
        MONSTERGRAINE,
        HIGHSCORE,
        HIGHSCOREPOINTS,
        VENDINGENABLEMACHINE,
        WF_TRG_BOT_REACHED_STF,
        WF_TRG_BOT_REACHED_AVTR,
        WF_ACT_BOT_CLOTHES,
        WF_ACT_BOT_TELEPORT,
        WF_ACT_BOT_FOLLOW_AVATAR,
        WF_ACT_BOT_GIVE_HANDITEM,
        WF_ACT_BOT_MOVE,
        WF_ACT_BOT_TALK_TO_AVATAR,
        WF_ACT_BOT_TALK,
        WF_CND_HAS_HANDITEM,
        WF_ACT_JOIN_TEAM,
        WF_ACT_LEAVE_TEAM,
        WF_ACT_GIVE_SCORE_TM,
        WF_CND_ACTOR_IN_TEAM,
        WF_CND_NOT_IN_TEAM,
        WF_CND_NOT_USER_COUNT,
        WF_CND_USER_COUNT_IN,
        WF_ACT_USER_MOVE,
        CRACKABLE,
        LOVELOCK,
        EXTRABOX,
        DELUXEBOX,
        LEGENDBOX,
        BADGEBOX,
        EXCHANGE,
        HORSE_SADDLE_1,
        HORSE_SADDLE_2,
        HORSE_HAIRSTYLE,
        HORSE_BODY_DYE,
        HORSE_HAIR_DYE,
        BADGE,
        TRAMPOLINE,
        TREADMILL,
        CROSSTRAINER
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
                    return InteractionType.bottle;
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
                    return InteractionType.TRIGGERTIMER;
                case "wf_trg_collision":
                    return InteractionType.TRIGGERCOLLISION;
                case "triggerroomenter":
                case "wf_trg_enterroom":
                    return InteractionType.TRIGGERROOMENTER;
                case "triggergameend":
                case "wf_trg_gameend":
                    return InteractionType.TRIGGERGAMEEND;
                case "triggergamestart":
                case "wf_trg_gamestart":
                    return InteractionType.TRIGGERGAMESTART;
                case "triggerrepeater":
                case "wf_trg_timer":
                    return InteractionType.TRIGGERREPEATER;
                case "wf_trg_period_long":
                    return InteractionType.TRIGGERREPEATERLONG;
                case "triggeronusersay":
                case "wf_trg_onsay":
                    return InteractionType.TRIGGERONUSERSAY;
                case "wf_trg_cmd":
                    return InteractionType.TRIGGERCOMMAND;
                case "wf_trg_cls_user":
                    return InteractionType.TRIGGERCOLLISIONUSER;
                case "triggerscoreachieved":
                case "wf_trg_atscore":
                    return InteractionType.TRIGGERSCOREACHIEVED;
                case "triggerstatechanged":
                case "wf_trg_furnistate":
                    return InteractionType.TRIGGERSTATECHANGED;
                case "triggerwalkonfurni":
                case "wf_trg_onfurni":
                    return InteractionType.TRIGGERWALKONFURNI;
                case "triggerwalkofffurni":
                case "wf_trg_offfurni":
                    return InteractionType.TRIGGERWALKOFFFURNI;
                case "actiongivescore":
                case "wf_act_givepoints":
                    return InteractionType.ACTIONGIVESCORE;
                case "actionposreset":
                case "wf_act_matchfurni":
                    return InteractionType.ACTIONPOSRESET;
                case "actionmoverotate":
                case "wf_act_moverotate":
                    return InteractionType.ACTIONMOVEROTATE;
                case "actionresettimer":
                    return InteractionType.ACTIONRESETTIMER;
                case "actionshowmessage":
                case "wf_act_saymsg":
                    return InteractionType.ACTIONSHOWMESSAGE;
                case "wf_act_give_reward":
                    return InteractionType.ACTIONGIVEREWARD;
                case "superwired":
                    return InteractionType.SUPERWIRED;
                case "superwiredcondition":
                    return InteractionType.SUPERWIREDCONDITION;
                case "actionteleportto":
                case "wf_act_moveuser":
                    return InteractionType.ACTIONTELEPORTTO;
                case "wf_act_endgame_team":
                    return InteractionType.WF_ACT_ENDGAME_TEAM;
                case "wf_act_call_stacks":
                    return InteractionType.WF_ACT_CALL_STACKS;
                case "actiontogglestate":
                case "wf_act_togglefurni":
                    return InteractionType.ACTIONTOGGLESTATE;
                case "wf_act_kick_user":
                case "wf_act_kickuser":
                case "wf_act_kick":
                    return InteractionType.ACTIONKICKUSER;
                case "wf_act_flee":
                    return InteractionType.ACTIONFLEE;
                case "wf_act_chase":
                    return InteractionType.ACTIONCHASE;
                case "wf_act_collisioncase":
                    return InteractionType.COLLISIONCASE;
                case "wf_act_collisionteam":
                    return InteractionType.COLLISIONTEAM;
                case "wf_act_move_to_dir":
                    return InteractionType.ACTIONMOVETODIR;
                case "conditionfurnishaveusers":
                case "wf_cnd_furnis_hv_avtrs":
                    return InteractionType.CONDITIONFURNISHAVEUSERS;
                case "wf_cnd_furnis_hv_prsn":
                case "wf_cnd_not_hv_avtrs":
                    return InteractionType.CONDITIONFURNISHAVENOUSERS;
                case "conditionstatepos":
                    return InteractionType.CONDITIONSTATEPOS;
                case "wf_cnd_stuff_is":
                    return InteractionType.WF_CND_STUFF_IS;
                case "wf_cnd_not_stuff_is":
                    return InteractionType.WF_CND_NOT_STUFF_IS;
                case "wf_cnd_not_match_snap":
                    return InteractionType.CONDITIONSTATEPOSNEGATIVE;
                case "conditiontimelessthan":
                    return InteractionType.CONDITIONTIMELESSTHAN;
                case "conditiontimemorethan":
                    return InteractionType.CONDITIONTIMEMORETHAN;
                case "conditiontriggeronfurni":
                case "wf_cnd_trggrer_on_frn":
                    return InteractionType.CONDITIONTRIGGERONFURNI;
                case "wf_cnd_not_trggrer_on":
                    return InteractionType.CONDITIONTRIGGERONFURNINEGATIVE;
                case "wf_cnd_has_furni_on":
                    return InteractionType.CONDITIONHASFURNIONFURNI;
                case "wf_cnd_not_furni_on":
                    return InteractionType.CONDITIONHASFURNIONFURNINEGATIVE;
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
                case "maniqui":
                    return InteractionType.MANNEQUIN;
                case "bgupdater":
                    return InteractionType.TONER;
                case "bot":
                    return InteractionType.BOT;
                case "adsbackground":
                    return InteractionType.ADSBACKGROUND;
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
                    return InteractionType.CONDITIONACTORINGROUP;
                case "wf_cnd_not_in_group":
                    return InteractionType.CONDITIONNOTINGROUP;
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
                    return InteractionType.WF_TRG_BOT_REACHED_STF;
                case "wf_trg_bot_reached_avtr":
                    return InteractionType.WF_TRG_BOT_REACHED_AVTR;
                case "wf_act_bot_clothes":
                    return InteractionType.WF_ACT_BOT_CLOTHES;
                case "wf_trg_trigger_self":
                    return InteractionType.WIRED_TRIGGER_SELF;
                case "wf_act_bot_teleport":
                    return InteractionType.WF_ACT_BOT_TELEPORT;
                case "wf_act_bot_follow_avatar":
                    return InteractionType.WF_ACT_BOT_FOLLOW_AVATAR;
                case "wf_act_bot_give_handitem":
                    return InteractionType.WF_ACT_BOT_GIVE_HANDITEM;
                case "wf_act_bot_move":
                    return InteractionType.WF_ACT_BOT_MOVE;
                case "wf_act_user_move":
                    return InteractionType.WF_ACT_USER_MOVE;
                case "wf_act_bot_talk_to_avatar":
                    return InteractionType.WF_ACT_BOT_TALK_TO_AVATAR;
                case "wf_act_bot_talk":
                    return InteractionType.WF_ACT_BOT_TALK;
                case "wf_cnd_has_handitem":
                    return InteractionType.WF_CND_HAS_HANDITEM;
                case "wf_act_join_team":
                    return InteractionType.WF_ACT_JOIN_TEAM;
                case "wf_act_leave_team":
                    return InteractionType.WF_ACT_LEAVE_TEAM;
                case "wf_act_give_score_tm":
                    return InteractionType.WF_ACT_GIVE_SCORE_TM;
                case "wf_cnd_actor_in_team":
                    return InteractionType.WF_CND_ACTOR_IN_TEAM;
                case "wf_cnd_not_in_team":
                    return InteractionType.WF_CND_NOT_IN_TEAM;
                case "wf_cnd_not_user_count":
                    return InteractionType.WF_CND_NOT_USER_COUNT;
                case "wf_cnd_user_count_in":
                    return InteractionType.WF_CND_USER_COUNT_IN;
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
