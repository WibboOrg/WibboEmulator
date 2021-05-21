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
        onewaygate,
        loveshuffler,
        habbowheel,
        DICE,
        bottle,
        TELEPORT,
        ARROW,
        rentals,
        pet,
        pool,
        roller,
        fbgate,
        iceskates,
        normslaskates,
        lowpool,
        haloweenpool,
        WALLPAPER,
        FLOOR,
        LANDSCAPE,
        football,
        footballgoalgreen,
        footballgoalyellow,
        footballgoalblue,
        footballgoalred,
        footballcountergreen,
        footballcounteryellow,
        footballcounterblue,
        footballcounterred,
        banzaigateblue,
        banzaigatered,
        banzaigateyellow,
        banzaigategreen,
        banzaifloor,
        banzaiscoreblue,
        banzaiscorered,
        banzaiscoreyellow,
        banzaiscoregreen,
        banzaitele,
        banzaipuck,
        banzaipyramid,
        banzaiblo,
        banzaiblob,
        ChronoTimer,
        freezeexit,
        freezeredcounter,
        freezebluecounter,
        freezeyellowcounter,
        freezegreencounter,
        freezeyellowgate,
        freezeredgate,
        freezegreengate,
        freezebluegate,
        freezetileblock,
        freezetile,
        jukebox,
        puzzlebox,
        triggertimer,
        triggerroomenter,
        triggergameend,
        triggergamestart,
        triggerrepeater,
        triggerrepeaterlong,
        triggeronusersay,
        triggercommand,
        WIRED_TRIGGER_SELF,
        triggercollisionuser,
        triggerscoreachieved,
        triggerstatechanged,
        triggerwalkonfurni,
        triggerwalkofffurni,
        triggercollision,
        actiongivescore,
        actionposreset,
        actionmoverotate,
        actionresettimer,
        actionshowmessage,
        actionteleportto,
        wf_act_endgame_team,
        wf_act_call_stacks,
        actiontogglestate,
        actionkickuser,
        actionflee,
        actionchase,
        collisioncase,
        collisionteam,
        actiongivereward,
        actionmovetodir,
        conditionfurnishaveusers,
        wf_cnd_stuff_is,
        wf_cnd_not_stuff_is,
        conditionstatepos,
        conditionstateposNegative,
        conditiontimelessthan,
        conditiontimemorethan,
        conditiontriggeronfurni,
        conditiontriggeronfurniNegative,
        conditionhasfurnionfurni,
        conditionhasfurnionfurniNegative,
        conditionfurnishavenousers,
        conditionactoringroup,
        conditionnotingroup,
        ringplate,
        colortile,
        colorwheel,
        floorswitch1,
        firegate,
        glassfoor,
        specialrandom,
        specialunseen,
        wire,
        wireCenter,
        wireCorner,
        wireSplitter,
        wireStandard,
        GIFT,
        MANNEQUIN,
        TONER,
        bot,
        adsbackground,
        BADGE_DISPLAY,
        BADGE_TROC,
        tvyoutube,
        pilemagic,
        superwired,
        superwiredcondition,
        jmphorse,
        GUILD_ITEM,
        GUILD_GATE,
        pressurepad,
        gnomebox,
        monstergraine,
        highscore,
        highscorepoints,
        vendingenablemachine,
        wf_trg_bot_reached_stf,
        wf_trg_bot_reached_avtr,
        wf_act_bot_clothes,
        wf_act_bot_teleport,
        wf_act_bot_follow_avatar,
        wf_act_bot_give_handitem,
        wf_act_bot_move,
        wf_act_bot_talk_to_avatar,
        wf_act_bot_talk,
        wf_cnd_has_handitem,
        wf_act_join_team,
        wf_act_leave_team,
        wf_act_give_score_tm,
        wf_cnd_actor_in_team,
        wf_cnd_not_in_team,
        wf_cnd_not_user_count,
        wf_cnd_user_count_in,
        wf_act_user_move,
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
                    return InteractionType.vendingenablemachine;
                case "alert":
                    return InteractionType.ALERT;
                case "onewaygate":
                    return InteractionType.onewaygate;
                case "loveshuffler":
                    return InteractionType.loveshuffler;
                case "habbowheel":
                    return InteractionType.habbowheel;
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
                    return InteractionType.pet;
                case "pool":
                case "water":
                    return InteractionType.pool;
                case "roller":
                    return InteractionType.roller;
                case "fbgate":
                    return InteractionType.fbgate;
                case "iceskates":
                    return InteractionType.iceskates;
                case "normalskates":
                    return InteractionType.normslaskates;
                case "lowpool":
                    return InteractionType.lowpool;
                case "haloweenpool":
                    return InteractionType.haloweenpool;
                case "football":
                case "ball":
                    return InteractionType.football;
                case "footballgoalgreen":
                case "green_goal":
                    return InteractionType.footballgoalgreen;
                case "footballgoalyellow":
                case "yellow_goal":
                    return InteractionType.footballgoalyellow;
                case "footballgoalred":
                case "red_goal":
                    return InteractionType.footballgoalred;
                case "footballgoalblue":
                case "blue_goal":
                    return InteractionType.footballgoalblue;
                case "footballcountergreen":
                    return InteractionType.footballcountergreen;
                case "footballcounteryellow":
                    return InteractionType.footballcounteryellow;
                case "footballcounterblue":
                    return InteractionType.footballcounterblue;
                case "footballcountered":
                    return InteractionType.footballcounterred;
                case "banzaigateblue":
                case "bb_blue_gate":
                    return InteractionType.banzaigateblue;
                case "banzaigatered":
                case "bb_red_gate":
                    return InteractionType.banzaigatered;
                case "banzaigateyellow":
                case "bb_yellow_gate":
                    return InteractionType.banzaigateyellow;
                case "banzaigategreen":
                case "bb_green_gate":
                    return InteractionType.banzaigategreen;
                case "banzaifloor":
                case "bb_patch":
                    return InteractionType.banzaifloor;
                case "banzaiscoreblue":
                    return InteractionType.banzaiscoreblue;
                case "banzaiscorered":
                    return InteractionType.banzaiscorered;
                case "banzaiscoreyellow":
                    return InteractionType.banzaiscoreyellow;
                case "banzaiscoregreen":
                    return InteractionType.banzaiscoregreen;
                case "banzaitele":
                case "bb_teleport":
                    return InteractionType.banzaitele;
                case "banzaipuck":
                    return InteractionType.banzaipuck;
                case "banzaipyramid":
                    return InteractionType.banzaipyramid;
                case "wf_blob2":
                    return InteractionType.banzaiblo;
                case "wf_blob":
                    return InteractionType.banzaiblob;
                case "freezetimer":
                case "banzaicounter":
                case "counter":
                    return InteractionType.ChronoTimer;
                case "freezeexit":
                    return InteractionType.freezeexit;
                case "freezeredcounter":
                    return InteractionType.freezeredcounter;
                case "freezebluecounter":
                    return InteractionType.freezebluecounter;
                case "freezeyellowcounter":
                    return InteractionType.freezeyellowcounter;
                case "freezegreencounter":
                    return InteractionType.freezegreencounter;
                case "freezeyellowgate":
                    return InteractionType.freezeyellowgate;
                case "freezeredgate":
                    return InteractionType.freezeredgate;
                case "freezegreengate":
                    return InteractionType.freezegreengate;
                case "freezebluegate":
                    return InteractionType.freezebluegate;
                case "freezetileblock":
                    return InteractionType.freezetileblock;
                case "freezetile":
                    return InteractionType.freezetile;
                case "jukebox":
                    return InteractionType.jukebox;
                case "triggertimer":
                case "wf_trg_attime":
                    return InteractionType.triggertimer;
                case "wf_trg_collision":
                    return InteractionType.triggercollision;
                case "triggerroomenter":
                case "wf_trg_enterroom":
                    return InteractionType.triggerroomenter;
                case "triggergameend":
                case "wf_trg_gameend":
                    return InteractionType.triggergameend;
                case "triggergamestart":
                case "wf_trg_gamestart":
                    return InteractionType.triggergamestart;
                case "triggerrepeater":
                case "wf_trg_timer":
                    return InteractionType.triggerrepeater;
                case "wf_trg_period_long":
                    return InteractionType.triggerrepeaterlong;
                case "triggeronusersay":
                case "wf_trg_onsay":
                    return InteractionType.triggeronusersay;
                case "wf_trg_cmd":
                    return InteractionType.triggercommand;
                case "wf_trg_cls_user":
                    return InteractionType.triggercollisionuser;
                case "triggerscoreachieved":
                case "wf_trg_atscore":
                    return InteractionType.triggerscoreachieved;
                case "triggerstatechanged":
                case "wf_trg_furnistate":
                    return InteractionType.triggerstatechanged;
                case "triggerwalkonfurni":
                case "wf_trg_onfurni":
                    return InteractionType.triggerwalkonfurni;
                case "triggerwalkofffurni":
                case "wf_trg_offfurni":
                    return InteractionType.triggerwalkofffurni;
                case "actiongivescore":
                case "wf_act_givepoints":
                    return InteractionType.actiongivescore;
                case "actionposreset":
                case "wf_act_matchfurni":
                    return InteractionType.actionposreset;
                case "actionmoverotate":
                case "wf_act_moverotate":
                    return InteractionType.actionmoverotate;
                case "actionresettimer":
                    return InteractionType.actionresettimer;
                case "actionshowmessage":
                case "wf_act_saymsg":
                    return InteractionType.actionshowmessage;
                case "wf_act_give_reward":
                    return InteractionType.actiongivereward;
                case "superwired":
                    return InteractionType.superwired;
                case "superwiredcondition":
                    return InteractionType.superwiredcondition;
                case "actionteleportto":
                case "wf_act_moveuser":
                    return InteractionType.actionteleportto;
                case "wf_act_endgame_team":
                    return InteractionType.wf_act_endgame_team;
                case "wf_act_call_stacks":
                    return InteractionType.wf_act_call_stacks;
                case "actiontogglestate":
                case "wf_act_togglefurni":
                    return InteractionType.actiontogglestate;
                case "wf_act_kick_user":
                case "wf_act_kickuser":
                case "wf_act_kick":
                    return InteractionType.actionkickuser;
                case "wf_act_flee":
                    return InteractionType.actionflee;
                case "wf_act_chase":
                    return InteractionType.actionchase;
                case "wf_act_collisioncase":
                    return InteractionType.collisioncase;
                case "wf_act_collisionteam":
                    return InteractionType.collisionteam;
                case "wf_act_move_to_dir":
                    return InteractionType.actionmovetodir;
                case "conditionfurnishaveusers":
                case "wf_cnd_furnis_hv_avtrs":
                    return InteractionType.conditionfurnishaveusers;
                case "wf_cnd_furnis_hv_prsn":
                case "wf_cnd_not_hv_avtrs":
                    return InteractionType.conditionfurnishavenousers;
                case "conditionstatepos":
                    return InteractionType.conditionstatepos;
                case "wf_cnd_stuff_is":
                    return InteractionType.wf_cnd_stuff_is;
                case "wf_cnd_not_stuff_is":
                    return InteractionType.wf_cnd_not_stuff_is;
                case "wf_cnd_not_match_snap":
                    return InteractionType.conditionstateposNegative;
                case "conditiontimelessthan":
                    return InteractionType.conditiontimelessthan;
                case "conditiontimemorethan":
                    return InteractionType.conditiontimemorethan;
                case "conditiontriggeronfurni":
                case "wf_cnd_trggrer_on_frn":
                    return InteractionType.conditiontriggeronfurni;
                case "wf_cnd_not_trggrer_on":
                    return InteractionType.conditiontriggeronfurniNegative;
                case "wf_cnd_has_furni_on":
                    return InteractionType.conditionhasfurnionfurni;
                case "wf_cnd_not_furni_on":
                    return InteractionType.conditionhasfurnionfurniNegative;
                case "ringplate":
                    return InteractionType.ringplate;
                case "colortile":
                    return InteractionType.colortile;
                case "colorwheel":
                    return InteractionType.colorwheel;
                case "floorswitch1":
                case "floorswitch2":
                    return InteractionType.floorswitch1;
                case "firegate":
                    return InteractionType.firegate;
                case "glassfoor":
                    return InteractionType.glassfoor;
                case "specialrandom":
                case "wf_xtra_random":
                    return InteractionType.specialrandom;
                case "specialunseen":
                case "wf_xtra_unseen":
                    return InteractionType.specialunseen;
                case "wire":
                    return InteractionType.wire;
                case "wireCenter":
                    return InteractionType.wireCenter;
                case "wireCorner":
                    return InteractionType.wireCorner;
                case "wireSplitter":
                    return InteractionType.wireSplitter;
                case "wireStandard":
                    return InteractionType.wireStandard;
                case "puzzlebox":
                    return InteractionType.puzzlebox;
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
                    return InteractionType.bot;
                case "adsbackground":
                    return InteractionType.adsbackground;
                case "badge_display":
                    return InteractionType.BADGE_DISPLAY;
                case "badge_troc":
                    return InteractionType.BADGE_TROC;
                case "tvyoutube":
                    return InteractionType.tvyoutube;
                case "pilemagic":
                    return InteractionType.pilemagic;
                case "jmphorse":
                    return InteractionType.jmphorse;
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
                    return InteractionType.conditionactoringroup;
                case "wf_cnd_not_in_group":
                    return InteractionType.conditionnotingroup;
                case "pressurepad":
                case "arrowplate":
                case "pressure_pad":
                    return InteractionType.pressurepad;
                case "gnomebox":
                    return InteractionType.gnomebox;
                case "monstergraine":
                    return InteractionType.monstergraine;
                case "highscore":
                    return InteractionType.highscore;
                case "hightscorepoints":
                    return InteractionType.highscorepoints;
                case "wf_trg_bot_reached_stf":
                    return InteractionType.wf_trg_bot_reached_stf;
                case "wf_trg_bot_reached_avtr":
                    return InteractionType.wf_trg_bot_reached_avtr;
                case "wf_act_bot_clothes":
                    return InteractionType.wf_act_bot_clothes;
                case "wf_trg_trigger_self":
                    return InteractionType.WIRED_TRIGGER_SELF;
                case "wf_act_bot_teleport":
                    return InteractionType.wf_act_bot_teleport;
                case "wf_act_bot_follow_avatar":
                    return InteractionType.wf_act_bot_follow_avatar;
                case "wf_act_bot_give_handitem":
                    return InteractionType.wf_act_bot_give_handitem;
                case "wf_act_bot_move":
                    return InteractionType.wf_act_bot_move;
                case "wf_act_user_move":
                    return InteractionType.wf_act_user_move;
                case "wf_act_bot_talk_to_avatar":
                    return InteractionType.wf_act_bot_talk_to_avatar;
                case "wf_act_bot_talk":
                    return InteractionType.wf_act_bot_talk;
                case "wf_cnd_has_handitem":
                    return InteractionType.wf_cnd_has_handitem;
                case "wf_act_join_team":
                    return InteractionType.wf_act_join_team;
                case "wf_act_leave_team":
                    return InteractionType.wf_act_leave_team;
                case "wf_act_give_score_tm":
                    return InteractionType.wf_act_give_score_tm;
                case "wf_cnd_actor_in_team":
                    return InteractionType.wf_cnd_actor_in_team;
                case "wf_cnd_not_in_team":
                    return InteractionType.wf_cnd_not_in_team;
                case "wf_cnd_not_user_count":
                    return InteractionType.wf_cnd_not_user_count;
                case "wf_cnd_user_count_in":
                    return InteractionType.wf_cnd_user_count_in;
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
