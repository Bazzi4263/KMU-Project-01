using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

// enum값들을 모아둠
public class Define
{
    public enum Sound
    {
        Bgm,
        Effect,
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum TILE
    {
        [EnumMember(Value = "1")]
        GROUND = 1,
        [EnumMember(Value = "100")]
        WATER = 100,
        [EnumMember(Value = "0")]
        CASTLE = 0,
        [EnumMember(Value = "2")]
        FOREST = 2,
        [EnumMember(Value = "3")]
        Dungeon = 3,
        [EnumMember(Value = "4")]
        Unknown = 4,
        [EnumMember(Value = "123")]
        UNDERGROUND = 123,
        [EnumMember(Value = "124")]
        UNDERWATER = 124,
        [EnumMember(Value = "125")]
        MOUNTAIN = 125
    }

    public enum PlayerType
    {
        None = 0,
        Archer,
        Knight,
        Magician,
        Thief,
        Warrior,
        Priest,
        Fighter,
        End
    }

    [Serializable]
    public class MonsterType
    {
        public GrassLand grassLand;
        public Desert desert;
        public SnowLand snowLand;

        public MonsterType(GrassLand _grassLand, Desert _desert, SnowLand _snowLand)
        {
            grassLand = _grassLand;
            desert = _desert;
            snowLand = _snowLand;
        }

        public enum GrassLand
        {
            NONE,
            GreenSlime,
            Ghost,
            BlueSlime,
            BlueSwordSlime,
            GreenSwordSlime,
            Mushroom,
            Wasp,
            Minotaur
        }
        public enum Desert
        {
            NONE,
            DesertSlime,
            DesertSwordSlime,
            Lamia,
            BlackLamia,
            Scorpion,
            BlackScorpion,
            Worm,
            Genius
        }

        public enum SnowLand
        {
            NONE,
            SnowSlime,
            SnowSwordSlime,
            SnowGhost,
            Skeleton,
            SkeletonWarrior,
            Zombie,
            Succubus,
            BlackMagician
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum STAGE
    {
        [EnumMember(Value = "0")]
        GRASSLAND,
        [EnumMember(Value = "1")]
        DESERT,
        [EnumMember(Value = "2")]
        SNOWLAND,
        [EnumMember(Value = "3")]
        TOWN
    }

    #region UI
    public enum UIEvent
    {
        Click,
        Drag,
        BeginDrag,
        EndDrag,
        On,
        Exit
    }

    public enum Buttons
    {
        StartButton,
        EndButton,
        SettingButton,
        BagButton,
        RunButton,
        SkillButton,
        CancelButton,
        MinimapButton,
        CharButton,
        OutButton
    }

    public enum Texts
    {
        DialogText,
        TimeText,
        GoldText
    }

    public enum Images
    {
        StartIcon,
        EndIcon,
        SettingIcon,
        SkillIcon,
        OutIcon,
        TimeIcon,
        GoldIcon,
        PortionIcon,
        ExpIcon,
        Image
    }
    #endregion

    public enum BattleState 
    { 
        START, 
        PLAYERTURN, 
        SELECTINGTARGET,
        SELECTINGDEADTARGET, //죽은대상 선택중
        IDLE, 
        ENEMYTURN, 
        WON, 
        LOST, 
        RUN,
        TURNPROCESSING,
        SELECTINGPPTARGET // PP포션의 사용 대상 플레이어를 선택하고 있는 상태
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Target
    {
        [EnumMember(Value = "0")]
        OneEnemy = 0,
        [EnumMember(Value = "1")]
        AllEnemy = 1,
        [EnumMember(Value = "2")]
        Self = 2,
        [EnumMember(Value = "3")]
        OnePlayer = 3,
        [EnumMember(Value = "4")]
        AllPlayer = 4,
        [EnumMember(Value = "5")]
        OneDeadPlayer = 5

    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum actionType
    {
        [EnumMember(Value = "0")]
        Attack = 0,
        [EnumMember(Value = "1")]
        Debuff = 1,  
        [EnumMember(Value = "2")]
        Buff = 2,
        [EnumMember(Value = "3")]
        Item = 3,
        [EnumMember(Value = "4")]
        Recover = 4,
        [EnumMember(Value = "5")]
        Combined = 5,
        [EnumMember(Value = "6")]
        Revive = 6
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum itemType
    {
        [EnumMember(Value = "0")]
        Recover = 0,
        [EnumMember(Value = "1")]
        InstantDmg = 1,
        [EnumMember(Value = "2")]
        Buff = 2,
        [EnumMember(Value = "3")]
        Debuff = 3,
        [EnumMember(Value = "4")]
        Permanant = 4
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum artefactType
    {
        [EnumMember(Value = "0")]
        Pemanant,
        [EnumMember(Value = "1")]
        Buff,
        [EnumMember(Value = "2")]
        Debuff,
        [EnumMember(Value = "3")]
        Other
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum grade
    {
        [EnumMember(Value = "0")]
        Normal,
        [EnumMember(Value = "1")]
        Rare,
        [EnumMember(Value = "2")]
        Epic,
        [EnumMember(Value = "3")]
        Unique,
        [EnumMember(Value = "4")]
        Legendary
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum artefactTarget
    {
        [EnumMember(Value = "0")]
        AllPlayer,
        [EnumMember(Value = "1")]
        AllEnemy,
        [EnumMember(Value = "2")]
        ParticularPlayer,
        [EnumMember(Value = "3")]
        ParticularEnemy
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum buffType
    {
        [EnumMember(Value = "0")]
        Buff = 0,
        [EnumMember(Value = "1")]
        Debuff = 1,
        [EnumMember(Value = "2")]
        Instant = 2,
        [EnumMember(Value = "3")]
        Permanant = 3,
        [EnumMember(Value = "4")]
        Shield = 4,
        [EnumMember(Value = "5")]
        Charge = 5,
        [EnumMember(Value = "6")]
        Bind = 6
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum buffTarget
    {
        [EnumMember(Value = "0")]
        Caster = 0,
        [EnumMember(Value = "1")]
        Target = 1,
        [EnumMember(Value = "2")]
        AllPlayer = 2,
        [EnumMember(Value = "3")]
        AllEnemy = 3,
    }

    public enum characterStatus
    {   
        None,
        Cloak,
        Taunt,
        Inactive,
        DebuffImmune,
        Dodge
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum affectedStat
    {
        [EnumMember(Value = "0")]
        hp = 0,
        [EnumMember(Value = "1")]
        speed = 1,
        [EnumMember(Value = "2")]
        armor = 2,
        [EnumMember(Value = "3")]
        attack_power = 3,
        [EnumMember(Value = "6")]
        pp = 6,
        [EnumMember(Value = "7")]
        all_stat = 7,
        [EnumMember(Value = "8")]
        other = 8
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum attackType
    {
        [EnumMember(Value = "0")]
        melee = 0,
        [EnumMember(Value = "1")]
        ranged = 1,
        [EnumMember(Value = "2")]
        nonattack = 2
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum battleType
    {
        [EnumMember(Value = "0")]
        normal,
        [EnumMember(Value = "1")]
        dungeon,
        [EnumMember(Value = "2")]
        town
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Scenes
    {
        [EnumMember(Value = "0")]
        UnKnownScene,
        [EnumMember(Value = "1")]
        StartScene,
        [EnumMember(Value = "2")]
        MapScene,
        [EnumMember(Value = "3")]
        BattleScene,
        [EnumMember(Value = "4")]
        TownScene,
        [EnumMember(Value = "5")]
        TestScene
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ParticleTypes
    {
        [EnumMember(Value = "0")]
        None,
        [EnumMember(Value = "1")]
        Blood,
        [EnumMember(Value = "2")]
        Green,
        [EnumMember(Value = "3")]
        Blue,
        [EnumMember(Value = "4")]
        Yellow,
        [EnumMember(Value = "5")]
        Purple,
        [EnumMember(Value = "6")]
        Red,
    }

}
