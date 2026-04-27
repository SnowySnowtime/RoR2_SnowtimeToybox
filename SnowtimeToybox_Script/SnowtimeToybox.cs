using BepInEx;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.SnowtimeToybox_FireHaloWeapon;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using On.RoR2.CharacterAI;
using R2API;
using RoR2;
using RoR2.Networking;
using RoR2.Skills;
using ShaderSwapper;
using SnowtimeToybox.Buffs;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurretChecks;
using SnowtimeToybox.FriendlyTurrets;
using SnowtimeToybox.Items;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using HG;
using IL.RoR2.ContentManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityHotReloadNS;
using Path = System.IO.Path;
using ReadOnlyContentPack = RoR2.ContentManagement.ReadOnlyContentPack;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete
[assembly: HG.Reflection.SearchableAttribute.OptIn]

namespace SnowtimeToybox
{
    // Dependencies and BepInPlugin initialization
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.SoundAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.DifficultyAPI.PluginGUID)]
    [BepInDependency("com.RiskOfBrainrot.RiskierRain", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskOfBrainrot.SwanSongExtended", BepInDependency.DependencyFlags.SoftDependency)]
    public class SnowtimeToyboxMod : BaseUnityPlugin
    {
        public const string Author = "SnowySnowtime";
        public const string Name = nameof(SnowtimeToyboxMod);
        public const string Version = "1.2.3";
        public const string GUID = Author + "." + Name;

        public static SnowtimeToyboxMod instance;

        public static DifficultyDef SnowtimeLegendaryDiffDef;
        public static DifficultyIndex SnowtimeLegendaryDiffIndex;
        public static SkillDef SnowtimePlasmaRifleSkillDef;
        public static GameObject MuzzleFlashObject;
        public static GameObject TracerObject;
        public static GameObject HitObject;
        public static GameObject OrbObject;

        public static CharacterBody.BodyFlags bodyFlags;
        
        // KEEP YOURSELF SAFE
        public static DamageAPI.ModdedDamageType HaloRicochetOnHit = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BorboSuperDebuffOnHit = DamageAPI.ReserveDamageType();
        
        // turretlings
        public static DroneDef FriendlyTurretTurretlingDef;
        public static InteractableSpawnCard FriendlyTurretTurretlingIsc;
        public static SkillFamily FriendlyTurretTurretlingPrimarySkillFamily;
        public static SkillFamily FriendlyTurretTurretlingSecondarySkillFamily;
        public static SkillFamily FriendlyTurretTurretlingUtilSkillFamily;
        public static SkillDef FriendlyTurretTurretlingPrimarySkillDef;
        public static SkillDef FriendlyTurretTurretlingSecondarySkillDef;
        public static SkillDef FriendlyTurretTurretlingUtilSkillDef;
        public static GameObject FriendlyTurretTurretlingBody;
        public static GameObject FriendlyTurretTurretlingBodyRemoteOp;
        public static GameObject FriendlyTurretTurretlingMaster;
        public static GameObject FriendlyTurretTurretlingBroken;
        // turretling variants
        public static GameObject AcanthiTurretlingBody;
        public static GameObject AcanthiTurretlingMaster;
        public static GameObject BorboTurretlingBody;
        public static GameObject BorboTurretlingMaster;
        public static GameObject BreadTurretlingBody;
        public static GameObject BreadTurretlingMaster;
        public static GameObject ShortcakeTurretlingBody;
        public static GameObject ShortcakeTurretlingMaster;
        public static GameObject SnowtimeTurretlingBody;
        public static GameObject SnowtimeTurretlingMaster;
        public static DroneDef DTTurretlingDef;
        public static GameObject DTTurretlingBody;
        public static GameObject DTTurretlingMaster;
        public static GameObject DTTurretlingBroken;
        public static SkillFamily DTTurretlingSkillFamily;
        public static SkillDef DTTurretlingSkillDef;
        public static SkillFamily ArtiPassiveFamily;
        public static SkillDef ArtiTurretSkill;
        public static SkillDef ArtiNoTurretSkill;
        public static DroneDef ArtiTurretlingDef;
        public static GameObject ArtiTurretlingBody;
        public static GameObject ArtiTurretlingMaster;
        public static GameObject ArtiTurretlingBroken;
        // turretling survivor
        public static SurvivorDef SwarmlingDef;
        public static GameObject SwarmlingBody;
        public static DroneDef SwarmlingMinionDef;
        public static GameObject SwarmlingMinionBody;
        public static GameObject SwarmlingMinionBroken;
        public static GameObject SwarmlingMinionMaster;
        public static SkillFamily Swarmling_PassiveFamily1;
        public static SkillFamily Swarmling_PassiveFamily2;
        public static SkillFamily Swarmling_PassiveFamily3;
        public static SkillFamily Swarmling_PassiveFamily4;
        public static SkillFamily Swarmling_PassiveFamily5;
        public static SkillFamily Swarmling_PassiveFamily6;
        public static SkillDef SwarmlingPassiveMinion;
        public static SkillFamily SwarmlingSpecialFamily;
        public static SkillDef SwarmlingSpecialSkill;
        public static SkillFamily SwarmlingUtilityFamily;
        public static SkillDef SwarmlingUtilitySkill;

        //public static DroneDef FriendlyTurretTestDroneDef;

        public static List<FriendlyTurretBase> friendlyTurretList = [];

        public static bool Legendary = false;
        // Copied from RiskierRain, sorry borbo :(
        public static bool ModLoaded(string modGuid) { return !string.IsNullOrEmpty(modGuid) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(modGuid); }
        public static bool riskierLoaded => ModLoaded("com.RiskOfBrainrot.RiskierRain");

        public static String assetDirectory;
        public static AssetBundle _stdifficultyAssetBundle;
        public static AssetBundle _stcharacterAssetBundle;
        public static AssetBundle _stitemAssetBundle;
        internal const string _stdifficultyAssetBundleName = "snowtimetoybox_difficulty";
        internal const string _stcharacterAssetBundleName = "snowtimetoybox_characters";
        internal const string _stitemAssetBundleName = "snowtimetoybox_items";

        public static ConfigEntry<bool> ToggleSpawnMessages { get; set; }
        public static ConfigEntry<bool> FriendlyTurretImmuneVoidDeath { get; set; }
        public static ConfigEntry<bool> TurretlingImmuneVoidDeath { get; set; }
        public static ConfigEntry<bool> FriendlyTurretFallImmunity { get; set; }
        public static ConfigEntry<bool> FriendlyTurretDrone { get; set; }
        public static ConfigEntry<int> FriendlyTurretRemoteOpPrice { get; set; }
        public static ConfigEntry<bool> FriendlyTurretShortcakeAggroType { get; set; }
        public static ConfigEntry<float> TurretlingSpawnChance { get; set; }
        public static ConfigEntry<float> TurretlingRainbowChance { get; set; }
        public static ConfigEntry<string> TurretlingRainbowBonusItems { get; set; }
        public static ConfigEntry<bool> TurretlingKillOriginalTurrets { get; set; }
        public static ConfigEntry<bool> TurretlingArtificerPassive { get; set; }
        
        public static ConfigEntry<string> TurretlingPassives { get; set; }
        public static ConfigEntry<float> TurretlingReviveCostMult { get; set; }
        public static ConfigEntry<float> TurretlingBaseDamage { get; set; }
        public static ConfigEntry<float> TurretlingBaseDamagePerLevel { get; set; }

        public void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ToggleSpawnMessages = Config.Bind("Friendly Turret Functions", "Spawn Message", true, "If true, the Friendly Turrets will give a message on every stage they spawn on, for insight on if and which turret spawned. Otherwise, friendly turrets are shy, and are also sad!");
            FriendlyTurretShortcakeAggroType = Config.Bind("Friendly Turret Functions", "Strawberry Shortcake Aggro Method", false, "If true, the Strawberry Shortcake Turret will spawn with a native increase to its aggro. Else, it only gains aggro for ~0.5s when its main skill fires.");
            FriendlyTurretImmuneVoidDeath = Config.Bind("Friendly Turret Flags", "Void Death Immunity", true, "If true, Friendly Turrets are immune to Void Death (Void Reaver implosions), this is because they are awful at avoiding them even with mods to make allies avoid them, and we get sad when they are detained.");
            FriendlyTurretFallImmunity = Config.Bind("Friendly Turret Flags", "Fall Damage Immunity", true, "If true, Friendly Turrets (and turretlings) are immune to fall damage, as navigating some maps can be a little difficult for them. Prevents any unexpected turret deaths, as we cant simply 'replace' them like Engineer can.");
            FriendlyTurretDrone = Config.Bind("Friendly Turret Flags", "Drone", false, "If true, Friendly Turrets (and turretlings) are flagged as drones. Probably comes with some oddities.");
            FriendlyTurretRemoteOpPrice = Config.Bind("Friendly Turret Functions", "Remote Operation Cost", 250, "Cost for becoming a Friendly Turret with Remote Operation.");
            TurretlingArtificerPassive = Config.Bind("Turretlings", "Turretling Passive", false, "If true, gives a turretling passive to those defined in Turretling Passive List.");
            TurretlingPassives = Config.Bind("Turretlings", "Turretling Passive List", "MageBody,Divineling;MercBody,Mercling;RailgunnerBody,Purity;BastionRobot,Ganymede;SeekerBody,Toastling;HuntressBody,Kottling;RocketSurvivorBody,Lil\'lusiveling;Loader,Scrapling;ArbiterBody,Nugget;CaptainBody,\'Paperweight\'", "internal names for bodies that should have turretlings ,.., (bodyname,turretlingname) turretlingname is the name given to these turretlings");
            TurretlingSpawnChance = Config.Bind("Turretlings", "Turretling Variant Spawn Chance ,,.", 100f, "chance to get a turretling when buying a friendly turret !!!");
            TurretlingImmuneVoidDeath = Config.Bind("Turretlings", "Void Death Immunity", false, "If true, All turretlings are immune to Void Death (Void Reaver implosions). Keep the scrunglies safe.");
            TurretlingReviveCostMult = Config.Bind("Turretlings", "turretling revive cost mult .,.", 0.6f, "price multiplier for reviving turretlings ,.. ,.");
            TurretlingKillOriginalTurrets = Config.Bind("Turretlings", "kill original turrets .,,.", false, "kills normal(gunner) turrets and replaces them with turretlings ,. ,.");
            TurretlingRainbowChance = Config.Bind("Turretlings", "turretling rainbow chance ,,.", 1f, "% chance to get a powerful and prideful rainbow turretling ,.,.");
            TurretlingRainbowBonusItems = Config.Bind("Turretlings", "turretling rainbow bonus items ,,.", "syringe,50,alienhead,5,extralife,1,moremissile,1,adaptivearmor,1,powercube,1,shockdamageaura,1", "give rainbow turretlings bonus items !!! follows (internalitemname),(count)");
            TurretlingBaseDamage = Config.Bind("Turretling Stats", "Base Damage", 12f, "Damage the turretling deals. Blaster deals 100%(1x) base damage, Pixi Launcher deal 200%(2x) base damage. Does not affect Turretling variants.");
            TurretlingBaseDamagePerLevel = Config.Bind("Turretling Stats", "Base Damage Per Level", 3f, "Base Damage increase per level. Does not affect Turretling variants.");
            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            Hooks.Hook();

            instance = this;

            var assetsFolderFullPath = Path.Combine(Path.GetDirectoryName(typeof(SnowtimeToyboxMod).Assembly.Location), "assetbundles");
            assetDirectory = assetsFolderFullPath;
            Debug.Log("Ran Start!");
            _stcharacterAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stcharacterAssetBundleName));
            base.StartCoroutine(_stcharacterAssetBundle.UpgradeStubbedShadersAsync());
            _stdifficultyAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stdifficultyAssetBundleName));
            _stitemAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stitemAssetBundleName));
            Debug.Log(_stcharacterAssetBundle);
            Debug.Log(_stdifficultyAssetBundle);
            Debug.Log(_stitemAssetBundle);

            AddDifficulty();
            AddCustomItems();
            AddCustomSkills();
            AddCustomAllies();
            AddCustomBuffs();

            ItemTag FriendTurret_Borbo_Whitelist = ItemAPI.AddItemTag("FriendTurret_Borbo_Whitelist");
            Log.Debug("FriendTurret_Borbo_Whitelist: " + FriendTurret_Borbo_Whitelist);

            ItemTag FriendTurret_Shortcake_Whitelist = ItemAPI.AddItemTag("FriendTurret_Shortcake_Whitelist");
            Log.Debug("FriendTurret_Shortcake_Whitelist: " + FriendTurret_Shortcake_Whitelist);

            ItemTag FriendTurret_Snowtime_Whitelist = ItemAPI.AddItemTag("FriendTurret_Snowtime_Whitelist");
            Log.Debug("FriendTurret_Snowtime_Whitelist: " + FriendTurret_Snowtime_Whitelist);

            ItemTag FriendTurret_Acanthi_Whitelist = ItemAPI.AddItemTag("FriendTurret_Acanthi_Whitelist");
            Log.Debug("FriendTurret_Acanthi_Whitelist: " + FriendTurret_Acanthi_Whitelist);

            ItemTag FriendTurret_Bread_Whitelist = ItemAPI.AddItemTag("FriendTurret_Bread_Whitelist");
            Log.Debug("FriendTurret_Acanthi_Whitelist: " + FriendTurret_Bread_Whitelist);

            ItemTag globalWhitelist = ItemAPI.AddItemTag("GlobalFriendTurret_Whitelist");
            Log.Debug("GlobalFriendTurret_Whitelist: " + globalWhitelist);

            ItemTag TurretlingNoneWhitelist = ItemAPI.AddItemTag("FriendTurret_None_Whitelist");
            Log.Debug("None FriendTurret_Whitelist: " + TurretlingNoneWhitelist);

            ItemTag TurretlingWhitelist = ItemAPI.AddItemTag("Turretling_Whitelist");
            Log.Debug("Turretling_Whitelist: " + TurretlingWhitelist);

            ItemCatalog.availability.CallWhenAvailable(AddCustomTagsToItems);
            EquipmentCatalog.availability.CallWhenAvailable(AddElitesToList);
        }
        
        Dictionary<string, string> itemStuff = new()
        {
            {"ITEM_LUNARSOAP_NAME", "FriendTurret_Acanthi_Whitelist"},
            {"SEEKINTHEVOID_STRAWPAIRY_NAME", "FriendTurret_Shortcake_Whitelist,FriendTurret_Acanthi_Whitelist"}, //seeking it ,.,.
            {"NT_ITEM_HYDRATOOTH_NAME", "FriendTurret_Acanthi_Whitelist"}, // nautilus
            {"NT_ITEM_SHIMMERINGNAUTILUS_NAME", "FriendTurret_Shortcake_Whitelist"},
            {"ROB_ITEM_FIRST_AID_SPRAY_NAME", "FriendTurret_Shortcake_Whitelist"}, // rob
            {"ROB_ITEM_PERFECT_APPLE_NAME", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_BORBOFUSE", "FriendTurret_Shortcake_Whitelist,FriendTurret_Snowtime_Whitelist"}, // swansong
            {"ITEM_CHOCYCOIN", "FriendTurret_Acanthi_Whitelist"}, 
            {"ITEM_REWORKRACK", "FriendTurret_Acanthi_Whitelist"}, 
            {"ITEM_ICHORVIOLET", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_DESIGNANOMALY", "FriendTurret_Shortcake_Whitelist"}, 
            {"ITEM_VOIDLASERTURBINE", "FriendTurret_Borbo_Whitelist"}, 
            {"ITEM_GOODEXECUTIONITEM", "FriendTurret_Snowtime_Whitelist"}, 
            {"ITEM_SANDSWEPT_AMBER_KNIFE", "FriendTurret_Acanthi_Whitelist"}, // sands, ,..,swept ,.,. 
            {"ITEM_SANDSWEPT_BLEEDING_WITNESS", "FriendTurret_Acanthi_Whitelist"},
            {"ITEM_SANDSWEPT_PRESERVED_ATOLL", "FriendTurret_Shortcake_Whitelist"},
            {"ITEM_SANDSWEPT_SMOULDERING_DOCUMENT", "FriendTurret_Acanthi_Whitelist"},
            {"Bork", "FriendTurret_Borbo_Whitelist,FriendTurret_Shortcake_Whitelist,FriendTurret_Acanthi_Whitelist"}, // idk ,.
            {"GuinsoosRageblade", "FriendTurret_Acanthi_Whitelist"},
            {"ImperialMandate", "FriendTurret_Acanthi_Whitelist"},
            {"KrakenSlayer", "FriendTurret_Acanthi_Whitelist,FriendTurret_Snowtime_Whitelist"},
            {"VV_ITEM_CRYOCANISTER_ITEM", "FriendTurret_Snowtime_Whitelist"}, //vanillas boi d.,
            {"SS2_ITEM_ARMEDBACKPACK_NAME", "FriendTurret_Shortcake_Whitelist"}, //star my storm ,.. 
            {"SS2_ITEM_STRANGECAN_NAME", "FriendTurret_Acanthi_Whitelist"},
            {"SS2_ITEM_ICETOOL_NAME", "FriendTurret_Snowtime_Whitelist"},
            {"ITEM_WoodenToolKit_Name", "FriendTurret_Bread_Whitelist"}, //bnr :plead
            {"SEEKINTHEVOID_COASTALCORAL_NAME", "GlobalFriendTurret_Whitelist"}, // globals - seekings itt ,..
            {"NT_ITEM_VISCOUSPOT_NAME", "GlobalFriendTurret_Whitelist"}, // globals - nauting it .,.,
            {"NT_ITEM_MOTHEROFPEARL_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"NT_ITEM_MOBIUSNODE_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"NT_ITEM_OSMIUMSHACKLES_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_GREENHERB_NAME", "GlobalFriendTurret_Whitelist"}, // globals - rob ,,.
            {"ROB_ITEM_REDHERB_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_MIXEDHERB_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_GOLDEN_APPLE_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ROB_ITEM_HEAVY_BOOT_NAME", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_EGG", "GlobalFriendTurret_Whitelist"}, // globals - swan ,.,
            {"ITEM_CUCKLER", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_DANGERCRIT", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_GAMMAKNIFE", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_FROZENSHELL", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_MASSANOMALY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BLOODANOMALY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BORBOBIGBATTERY", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_BORBOMANAFLOWER", "GlobalFriendTurret_Whitelist"}, 
            {"ITEM_SANDSWEPT_CROWNS_DIAMOND", "GlobalFriendTurret_Whitelist"}, // globals - sand my swpet, ,..
            {"Rabadons", "GlobalFriendTurret_Whitelist"}, // globals - idk ,. -- this is league of legends items :wilted_rose:
            {"VV_ITEM_ADZE_ITEM", "GlobalFriendTurret_Whitelist"}, // globals - vanillas ,,. boid .,.
            {"SS2_ITEM_HUNTERSSIGIL_NAME", "GlobalFriendTurret_Whitelist"}, // globals - stasrsing it ,,. by it ,.,. storm ,.,.
        };

        public void AddCustomTagsToItems()
        {
            Log.Debug("SnowtimeToybox is adding custom tags to items for Friendly Turrets/Drones...");

            ItemDef[] whitelistGlobalTurret = [
                RoR2Content.Items.Pearl,
                RoR2Content.Items.ShinyPearl,
                RoR2Content.Items.FallBoots,
                RoR2Content.Items.BonusGoldPackOnKill,
                RoR2Content.Items.FlatHealth,
                RoR2Content.Items.Knurl,
                // DLC3
                DLC3Content.Items.CookedSteak,
            ];
            ItemDef[] whitelistBorboVars = [
                // Base
                RoR2Content.Items.Syringe,
                RoR2Content.Items.BossDamageBonus,
                // DLC1
                DLC1Content.Items.PermanentDebuffOnHit,
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.MeteorAttackOnHighDamage,
                DLC2Content.Items.AttackSpeedPerNearbyAllyOrEnemy,
                // DLC3
            ];
            ItemDef[] whitelistShortcakeVars = [
                // Base
                RoR2Content.Items.Thorns,
                RoR2Content.Items.BarrierOnKill,
                RoR2Content.Items.HealWhileSafe,
                RoR2Content.Items.ArmorPlate,
                RoR2Content.Items.PersonalShield,
                RoR2Content.Items.Infusion,
                RoR2Content.Items.ChainLightning,
                RoR2Content.Items.BarrierOnOverHeal,
                RoR2Content.Items.Plant,
                RoR2Content.Items.BounceNearby,
                RoR2Content.Items.ShockNearby,
                // DLC1
                DLC1Content.Items.OutOfCombatArmor,
                DLC1Content.Items.HalfSpeedDoubleHealth,
                DLC1Content.Items.MissileVoid,
                DLC1Content.Items.ChainLightningVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                // DLC3
                DLC3Content.Items.ShieldBooster,
                DLC3Content.Items.ShockDamageAura,
            ];
            ItemDef[] whitelistSnowtimeVars = [
                // Base
                RoR2Content.Items.Syringe,
                RoR2Content.Items.IceRing,
                RoR2Content.Items.PersonalShield,
                RoR2Content.Items.SlowOnHit,
                // DLC1
                DLC1Content.Items.ElementalRingVoid,
                DLC1Content.Items.SlowOnHitVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                // DLC3
                DLC3Content.Items.ShieldBooster,
            ];
            ItemDef[] whitelistAcanthiVars = [
                // Base
                RoR2Content.Items.Tooth,
                RoR2Content.Items.BleedOnHit,
                RoR2Content.Items.Syringe,
                RoR2Content.Items.Clover,
                RoR2Content.Items.LunarBadLuck,
                RoR2Content.Items.DeathMark,
                RoR2Content.Items.Seed,
                RoR2Content.Items.Infusion,
                RoR2Content.Items.IncreaseHealing,
                RoR2Content.Items.NovaOnHeal,
                // DLC1
                DLC1Content.Items.BleedOnHitVoid,
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.TriggerEnemyDebuffs,
                // DLC3
                DLC3Content.Items.UltimateMeal,
            ];
            ItemDef[] whitelistBreadVars = [
                // Base
                RoR2Content.Items.WardOnLevel,
                RoR2Content.Items.Medkit,
                RoR2Content.Items.BarrierOnKill,
                RoR2Content.Items.BarrierOnOverHeal,
                RoR2Content.Items.SprintArmor,
                RoR2Content.Items.ArmorReductionOnHit,
                RoR2Content.Items.IncreaseHealing,
                // DLC1
                DLC1Content.Items.MoreMissile,
                // DLC2
                DLC2Content.Items.BoostAllStats,
                // DLC3
            ];
            ItemDef[] whitelistTurretlingVars = [
                // Base
                RoR2Content.Items.Syringe,
                // DLC1
                DLC1Content.Items.ElementalRingVoid,
                // DLC2
                // DLC3
                DLC3Content.Items.ShieldBooster,
            ];
            foreach (ItemDef item in whitelistGlobalTurret)
            {
                Log.Debug("Added " + item.name + " to global friendly turret item whitelist");
                ItemAPI.ApplyTagToItem("GlobalFriendTurret_Whitelist", item);
            }
            foreach (ItemDef item in whitelistBorboVars)
            {
                Log.Debug("Added " + item.name + " to borbo turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Borbo_Whitelist", item);
            }
            foreach (ItemDef item in whitelistShortcakeVars)
            {
                Log.Debug("Added " + item.name + " to Strawberry Shortcake Turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Shortcake_Whitelist", item);
            }
            foreach (ItemDef item in whitelistSnowtimeVars)
            {
                Log.Debug("Added " + item.name + " to Snowtime Turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Snowtime_Whitelist", item);
            }
            foreach (ItemDef item in whitelistAcanthiVars)
            {
                Log.Debug("Added " + item.name + " to acanthi turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Acanthi_Whitelist", item);
            }
            foreach (ItemDef item in whitelistBreadVars)
            {
                Log.Debug("Added " + item.name + " to bread turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_Bread_Whitelist", item);
            }
            AddCustomTagsToModdedItems();
        }
        public void AddCustomTagsToModdedItems()
        {
            Log.Debug("SnowtimeToybox is adding custom tags to Modded items for Friendly Turrets/Drones...");

            foreach (ItemDef itemDef in ItemCatalog.itemDefs)
            {
                string whitelistKey = "";

                if (itemStuff.TryGetValue(itemDef.nameToken, out string nameToken))
                {
                    whitelistKey = nameToken;
                } 
                else if (itemDef.nameToken.Length > 6 && itemStuff.TryGetValue(itemDef.nameToken[..^5], out string nameTokenNoName)) // without _NAME since some are weird .,.,
                {
                    whitelistKey = nameTokenNoName;
                }
                else if (itemStuff.TryGetValue(itemDef.name, out string itemDefName)) // base name since some use that ,.,.
                {
                    whitelistKey = itemDefName;
                }

                if (whitelistKey.IsNullOrWhiteSpace()) continue;
                
                foreach (string whitelist in whitelistKey.Split(','))
                {
                    Log.Debug($"adding {itemDef.nameToken} to {whitelist} whitelist");
                    ItemAPI.ApplyTagToItem(whitelist, itemDef);
                }
            }
        }
        
        public static List<EquipmentIndex> eliteDefsEquipInherit = [];
        public void AddElitesToList()
        {
            Log.Debug("SnowtimeToybox is listing Elite equipment for inheritance...");
            eliteDefsEquipInherit = [];
            foreach (var eliteDef in EliteCatalog.eliteDefs)
            {
                if (eliteDef.eliteEquipmentDef == null) return;
                if (eliteDef.eliteEquipmentDef?.equipmentIndex == null) return;
                eliteDefsEquipInherit.Add(eliteDef.eliteEquipmentDef.equipmentIndex);
                Log.Debug("Elite Equipment: " + eliteDef.eliteEquipmentDef + " Index: " + eliteDef.eliteEquipmentDef.equipmentIndex);
            }
        }

        public void AddCustomItems()
        {
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                item.Init(Config);
            }
        }
        public void AddCustomAllies()
        {
            bodyFlags = new CharacterBody.BodyFlags();
            if (FriendlyTurretImmuneVoidDeath.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            }
            if (FriendlyTurretFallImmunity.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
            if (FriendlyTurretDrone.Value)
            {
                bodyFlags |= CharacterBody.BodyFlags.Drone;
            }
            
            IEnumerable<Type> friendlyTurrets = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(FriendlyTurretBase)));
            foreach (Type friendlyTurret in friendlyTurrets)
            {
                FriendlyTurretBase turret = (FriendlyTurretBase)System.Activator.CreateInstance(friendlyTurret);
                turret.Initalization();
                turret.ContentAdditionFuncs();
                turret.StageInteractableFuncs();
                turret.PostInit();
            }

            // add turretling
            Log.Debug("Defining Turretling(s)...");
            string turretlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/";
            FriendlyTurretTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingBody.prefab");
            FriendlyTurretTurretlingBodyRemoteOp = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingBodyRemoteOp.prefab");
            // update stats and components
            FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().baseDamage = TurretlingBaseDamage.Value;
            FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().levelDamage = TurretlingBaseDamagePerLevel.Value;
            FriendlyTurretTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterBody>().baseDamage = TurretlingBaseDamage.Value;
            FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterBody>().levelDamage = TurretlingBaseDamagePerLevel.Value;
            FriendlyTurretTurretlingBodyRemoteOp.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            FriendlyTurretTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "_TurretlingMaster.prefab");
            FriendlyTurretTurretlingPrimarySkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingPrimaryFamily.asset");
            FriendlyTurretTurretlingPrimarySkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Primary.asset");
            FriendlyTurretTurretlingPrimarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingBlaster));
            FriendlyTurretTurretlingSecondarySkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingSecondaryFamily.asset");
            FriendlyTurretTurretlingSecondarySkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/Turretling_Secondary.asset");
            FriendlyTurretTurretlingSecondarySkillDef.activationState = new SerializableEntityStateType(typeof(TurretlingMissile));
            FriendlyTurretTurretlingUtilSkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(turretlingPath + "Skills/TurretlingUtilityFamily.asset");
            FriendlyTurretTurretlingUtilSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(turretlingPath + "Skills/TurretlingShenanigans.asset");
            FriendlyTurretTurretlingUtilSkillDef.activationState = new SerializableEntityStateType(typeof(Shenanigans));
            FriendlyTurretTurretlingDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(turretlingPath + "_Turretling.asset");
            FriendlyTurretTurretlingMaster.AddComponent<TurretlingRainbow>();
            ContentAddition.AddDroneDef(FriendlyTurretTurretlingDef);
            ContentAddition.AddEntityState(typeof(TurretlingDeath), out _);
            ContentAddition.AddEntityState(typeof(TurretlingBlaster), out _);
            ContentAddition.AddEntityState(typeof(TurretlingMissile), out _);
            ContentAddition.AddBody(FriendlyTurretTurretlingBody);
            ContentAddition.AddBody(FriendlyTurretTurretlingBodyRemoteOp);
            // erm
            ContentAddition.AddMaster(FriendlyTurretTurretlingMaster);
            ContentAddition.AddSkillFamily(FriendlyTurretTurretlingPrimarySkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretTurretlingPrimarySkillDef);
            ContentAddition.AddSkillFamily(FriendlyTurretTurretlingSecondarySkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretTurretlingSecondarySkillDef);
            ContentAddition.AddEffect(TurretlingDeath.deathfx);
            ContentAddition.AddEffect(SnowtimeOrbs.orbTurretlingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbTurretlingMissileImpactObject);
            // add turretling variants (spawned with a friendly turret)
            AcanthiTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Acanthi.prefab");
            AcanthiTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            AcanthiTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Acanthi.prefab");
            BorboTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Borbo.prefab");
            BorboTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            BorboTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Borbo.prefab");
            BreadTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Bread.prefab");
            BreadTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            BreadTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Bread.prefab");
            ShortcakeTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Shortcake.prefab");
            ShortcakeTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            ShortcakeTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Shortcake.prefab");
            SnowtimeTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingBody_Snowtime.prefab");
            SnowtimeTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            SnowtimeTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(turretlingPath + "Variants/_TurretlingMaster_Snowtime.prefab");
            ContentAddition.AddBody(AcanthiTurretlingBody);
            ContentAddition.AddMaster(AcanthiTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_acanthi);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_acanthi);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_acanthi);
            ContentAddition.AddEffect(SnowtimeOrbs.orbAcanthilingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbAcanthilingMissileImpactObject);
            ContentAddition.AddBody(BorboTurretlingBody);
            ContentAddition.AddMaster(BorboTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_borbo);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_borbo);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_borbo);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBorbolingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBorbolingMissileImpactObject);
            ContentAddition.AddBody(BreadTurretlingBody);
            ContentAddition.AddMaster(BreadTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_bread);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBreadlingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbBreadlingMissileImpactObject);
            ContentAddition.AddBody(ShortcakeTurretlingBody);
            ContentAddition.AddMaster(ShortcakeTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_shortcake);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_shortcake);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_shortcake);
            ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakelingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbShortcakelingMissileImpactObject);
            ContentAddition.AddBody(SnowtimeTurretlingBody);
            ContentAddition.AddMaster(SnowtimeTurretlingMaster);
            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_snowtime);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_snowtime);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_snowtime);
            ContentAddition.AddEffect(SnowtimeOrbs.orbSnowtimelingMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbSnowtimelingMissileImpactObject);

            ContentAddition.AddEffect(TurretlingBlaster.muzzlefx_rainbow);
            ContentAddition.AddEffect(TurretlingBlaster.hitfx_rainbow);
            ContentAddition.AddEffect(TurretlingBlaster.tracerfx_rainbow);
            ContentAddition.AddEffect(SnowtimeOrbs.orbRainbowMissileObject);
            ContentAddition.AddEffect(SnowtimeOrbs.orbRainbowMissileImpactObject);
            // Arti really quickly 
            ArtiTurretlingDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretling.asset");
            ArtiTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingBody.prefab");
            ArtiTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
            ArtiTurretlingBody.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
            ArtiTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingMaster.prefab");
            ArtiTurretlingMaster.AddComponent<TurretlingRainbow>();
            ArtiTurretlingBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_HolyTurretlingBroken.prefab");
            ArtiTurretlingBroken.AddComponent<PassiveTurretlingUpdateNamePerCharacter>();
            ArtiTurretlingBody.GetComponent<CharacterBody>().baseDamage = (TurretlingBaseDamage.Value / 1.5f);
            ArtiTurretlingBody.GetComponent<CharacterBody>().levelDamage = (TurretlingBaseDamagePerLevel.Value / 1.5f);
            ContentAddition.AddDroneDef(ArtiTurretlingDef);
            ContentAddition.AddBody(ArtiTurretlingBody);
            ContentAddition.AddMaster(ArtiTurretlingMaster);
            ContentAddition.AddBody(ArtiTurretlingBroken);
            // Operator
            DTTurretlingDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretling.asset");
            DTTurretlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingBody.prefab");
            DTTurretlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
            DTTurretlingBody.GetComponent<DroneCommandReceiver>().droneState = DroneCommandReceiver.DroneState.Idle;
            DTTurretlingMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingMaster.prefab");
            DTTurretlingMaster.AddComponent<TurretlingRainbow>();
            DTTurretlingBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/_DTTurretlingBroken.prefab");
            DTTurretlingSkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DTTurretlingSpecialFamily.asset");
            DTTurretlingSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DTTurretling_Special.asset");
            DTTurretlingSkillDef.activationState = new SerializableEntityStateType(typeof(DTTurretlingRainbowize));
            ContentAddition.AddDroneDef(DTTurretlingDef);
            ContentAddition.AddBody(DTTurretlingBody);
            ContentAddition.AddMaster(DTTurretlingMaster);
            ContentAddition.AddBody(DTTurretlingBroken);
            ContentAddition.AddSkillFamily(DTTurretlingSkillFamily);
            ContentAddition.AddSkillDef(DTTurretlingSkillDef);
            ContentAddition.AddEntityState(typeof(DTTurretlingDeath), out _);
            ContentAddition.AddEntityState(typeof(DTTurretlingRainbowize), out _);

            string swarmlingPath = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Survivor/";
            SwarmlingDef = _stcharacterAssetBundle.LoadAsset<SurvivorDef>(swarmlingPath + "Swarmling.asset");
            SwarmlingMinionDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(swarmlingPath + "_SwarmTurretling.asset");
            SwarmlingBody = _stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_TurretlingSurvivorBody.prefab");
            SwarmlingBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(TurretlingDeath));
            SwarmlingBody.AddComponent<TurretlingMissileTracker>();
            DroneTechRepairQueue repairQueueSwarmling = SwarmlingBody.AddComponent<DroneTechRepairQueue>();
            repairQueueSwarmling.healRate = 0.05f;
            SwarmlingMinionBody = _stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingBody.prefab");
            SwarmlingMinionBody.GetComponent<CharacterDeathBehavior>().deathState = new SerializableEntityStateType(typeof(DTTurretlingDeath));
            SwarmlingMinionBody.AddComponent<TurretlingMissileTracker>();
            SwarmlingMinionBody.AddComponent<EquipmentSlot>();
            SwarmlingMinionBody.GetComponent<CharacterBody>().baseDamage = (TurretlingBaseDamage.Value / 2f);
            SwarmlingMinionBody.GetComponent<CharacterBody>().levelDamage = (TurretlingBaseDamagePerLevel.Value / 2f);
            SwarmlingMinionBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingBroken.prefab");
            SwarmlingMinionMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(swarmlingPath + "_SwarmTurretlingMaster.prefab");
            SwarmlingMinionMaster.AddComponent<TurretlingRainbow>();
            SwarmlingMinionMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
            Swarmling_PassiveFamily1 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily1.asset");
            Swarmling_PassiveFamily2 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily2.asset");
            Swarmling_PassiveFamily3 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily3.asset");
            Swarmling_PassiveFamily4 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily4.asset");
            Swarmling_PassiveFamily5 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily5.asset");
            Swarmling_PassiveFamily6 = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/Swarm_PassiveFamily6.asset");
            SwarmlingSpecialFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/TurretlingSpecialFamilySurvivor.asset");
            SwarmlingUtilityFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(swarmlingPath + "Skills/TurretlingUtilityFamilySurvivor.asset");
            SwarmlingPassiveMinion = _stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Swarmling.asset");
            SwarmlingSpecialSkill = _stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Turretling_SpecialSurvivor.asset");
            SwarmlingSpecialSkill.activationState = new SerializableEntityStateType(typeof(TurretlingEnergyNova));
            SwarmlingUtilitySkill = _stcharacterAssetBundle.LoadAsset<SkillDef>(swarmlingPath + "Skills/Turretling_UtilitySurvivor.asset");
            SwarmlingUtilitySkill.activationState = new SerializableEntityStateType(typeof(TurretlingMiniBlinkState));
            ContentAddition.AddSurvivorDef(SwarmlingDef);
            ContentAddition.AddDroneDef(SwarmlingMinionDef);
            ContentAddition.AddBody(SwarmlingBody);
            ContentAddition.AddBody(SwarmlingMinionBody);
            ContentAddition.AddBody(SwarmlingMinionBroken);
            ContentAddition.AddMaster(SwarmlingMinionMaster);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily1);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily2);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily3);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily4);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily5);
            ContentAddition.AddSkillFamily(Swarmling_PassiveFamily6);
            ContentAddition.AddSkillFamily(SwarmlingSpecialFamily);
            ContentAddition.AddSkillFamily(SwarmlingUtilityFamily);
            ContentAddition.AddSkillDef(SwarmlingPassiveMinion);
            ContentAddition.AddSkillDef(SwarmlingSpecialSkill);
            ContentAddition.AddSkillDef(SwarmlingUtilitySkill);
            ContentAddition.AddEntityState(typeof(TurretlingEnergyNova), out _);
            ContentAddition.AddEntityState(typeof(TurretlingMiniBlinkState), out _);
            ContentAddition.AddEffect(TurretlingEnergyNova.novafx);

            ContentAddition.AddEntityState(typeof(Shenanigans), out _);
            
            // Add the Turretling to stages interactable spawncards, as it is a standard walking turret and NOT a Friendly Turret, as its internal name may imply
            FriendlyTurretTurretlingBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_mdlTurretlingBroken.prefab");
            FriendlyTurretTurretlingIsc = _stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/_iscBrokenTurretling.asset");
            ContentAddition.AddNetworkedObject(FriendlyTurretTurretlingBroken);
            var directorCardFriendlyTurretTurretling = new DirectorCard // Borbo Turret Interactable
            {
                spawnCard = FriendlyTurretTurretlingIsc,
                selectionWeight = 14, // the higher it is, the more common it is
                spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
                minimumStageCompletions = 0,
                preventOverhead = false
            };

            var directorCardHolderFriendlyTurretTurretling = new DirectorAPI.DirectorCardHolder
            {
                Card = directorCardFriendlyTurretTurretling,
                InteractableCategory = DirectorAPI.InteractableCategory.Drones
            };

            List<DirectorAPI.Stage> turretlingStageList = new List<DirectorAPI.Stage>();
            List<String> turretlingCustomStageList = new List<String>();

            // Stage 1
            turretlingStageList.Add(DirectorAPI.Stage.TitanicPlains);
            turretlingStageList.Add(DirectorAPI.Stage.DistantRoost);
            turretlingStageList.Add(DirectorAPI.Stage.SiphonedForest);
            turretlingStageList.Add(DirectorAPI.Stage.VerdantFalls);
            turretlingStageList.Add(DirectorAPI.Stage.ViscousFalls);
            // Stage 2
            turretlingStageList.Add(DirectorAPI.Stage.AbandonedAqueduct);
            turretlingStageList.Add(DirectorAPI.Stage.AphelianSanctuary);
            turretlingStageList.Add(DirectorAPI.Stage.PretendersPrecipice);
            // Stage 3
            turretlingStageList.Add(DirectorAPI.Stage.RallypointDelta);
            turretlingStageList.Add(DirectorAPI.Stage.ScorchedAcres);
            turretlingStageList.Add(DirectorAPI.Stage.IronAlluvium);
            turretlingStageList.Add(DirectorAPI.Stage.IronAuroras);
            // Stage 4
            turretlingStageList.Add(DirectorAPI.Stage.SirensCall);
            turretlingStageList.Add(DirectorAPI.Stage.SunderedGrove);
            turretlingStageList.Add(DirectorAPI.Stage.RepurposedCrater);
            turretlingStageList.Add(DirectorAPI.Stage.ConduitCanyon);
            // Stage 5
            turretlingStageList.Add(DirectorAPI.Stage.SkyMeadow);
            // Mods
            turretlingCustomStageList.Add("FBLScene");
            turretlingCustomStageList.Add("broadcastperch_wormsworms");
            turretlingCustomStageList.Add("tropics_wormsworms");
            turretlingCustomStageList.Add("tropicsnight_wormsworms");
            turretlingCustomStageList.Add("hollowsummit_wormsworms");
            turretlingCustomStageList.Add("hollowsummitnight_wormsworms");
            turretlingCustomStageList.Add("catacombs_DS1_Catacombs");
            turretlingCustomStageList.Add("snowtime_bloodgulch");
            turretlingCustomStageList.Add("snowtime_deathisland");
            turretlingCustomStageList.Add("snowtime_gephyrophobia");
            turretlingCustomStageList.Add("snowtime_gmconstruct");
            turretlingCustomStageList.Add("snowtime_gmflatgrass");
            turretlingCustomStageList.Add("snowtime_halo");
            turretlingCustomStageList.Add("snowtime_halo2");
            turretlingCustomStageList.Add("snowtime_highcharity");
            turretlingCustomStageList.Add("snowtime_icefields");
            turretlingCustomStageList.Add("snowtime_newmombasabridge");
            turretlingCustomStageList.Add("snowtime_odstmombasa");
            turretlingCustomStageList.Add("snowtime_plrhightower");
            turretlingCustomStageList.Add("snowtime_sandtrap");
            turretlingCustomStageList.Add("snowtime_sidewinder");

            foreach (DirectorAPI.Stage stage in turretlingStageList)
            {
                Log.Debug("Adding Turretlings to stage: " + stage);
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, stage);
            }
            foreach (string stage in turretlingCustomStageList)
            {
                Log.Debug("Adding Turretlings to stage: " + stage);
                DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurretTurretling, DirectorAPI.Stage.Custom, stage);
            }
            
            List<GameObject> turretlingBodies =
            [
                FriendlyTurretTurretlingBody,
                AcanthiTurretlingBody,
                BorboTurretlingBody,
                ShortcakeTurretlingBody,
                SnowtimeTurretlingBody,
                BreadTurretlingBody,
                DTTurretlingBody,
                FriendlyTurretTurretlingBodyRemoteOp,
                ArtiTurretlingBody
            ];
            foreach (var turretling in turretlingBodies)
            {
                turretling.AddComponent<TurretlingMissileTracker>();
                if (turretling.gameObject.name.Contains("RemoteOp")) return;
                turretling.AddComponent<EquipmentSlot>();
            }
            
            FriendlyTurretTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_None_Whitelist";
            BorboTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Borbo_Whitelist";
            SnowtimeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Snowtime_Whitelist";
            AcanthiTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Acanthi_Whitelist";
            BreadTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Bread_Whitelist";
            ShortcakeTurretlingMaster.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Shortcake_Whitelist";
            DTTurretlingBody.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Shortcake_Whitelist";
            ArtiTurretlingBody.AddComponent<FriendlyTurretInheritance>().whitelistedTag = "FriendTurret_Shortcake_Whitelist";
            
            foreach (var turretling in turretlingBodies)
            {
                turretling.GetComponent<CharacterBody>().bodyFlags |= bodyFlags;
            }
            
            if (TurretlingImmuneVoidDeath.Value)
            {
                FriendlyTurretTurretlingBody.GetComponent<CharacterBody>().bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.OverheatImmune | CharacterBody.BodyFlags.ResistantToAOE;
            }

            On.RoR2.PurchaseInteraction.GetInteractability += GetInteractabilityFriendlyTurrets;
            // i want die
            ContentAddition.AddEffect(BorboCheck.turretUseEffect);

            // update friendly turret targeting
            BaseAI.UpdateTargets += UpdateFriendlyTurretTargeting;
            
            //overlay amanger ,.,. 
            On.RoR2.CharacterModel.UpdateOverlays += CharacterModelOnUpdateOverlays;
        }
        
        private void CharacterModelOnUpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, RoR2.CharacterModel self)
        {
            orig(self);
            
            if (self && self.body)
            {
                if (self.body.HasBuff(BreadFriendlyTurret.BreadTurretBuffFortune))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("matBreadFortune"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/matBreadFortune.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
                
                if (self.body.HasBuff(AcanthiFriendlyTurret.AcanthiTurretDebuff))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("acanthidebuffoverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Acanthi/acanthidebuffoverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
                
                if (self.body.HasBuff(BorboFriendlyTurret.BorboTurretDebuff))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("borboturretdebuffoverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/borboturretdebuffoverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }

                if (self.body.HasBuff(BreadFriendlyTurret.BreadTurretBuffNearbyAllies))
                {
                    FriendlyTurretOverlayManager friendlyTurretOverlayManager = self.body.GetComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager) friendlyTurretOverlayManager = self.body.gameObject.AddComponent<FriendlyTurretOverlayManager>();
                    if (!friendlyTurretOverlayManager.hasOverlay("matBreadGraceOverlay"))
                    {
                        friendlyTurretOverlayManager.Body = self.body;
                        var temporaryOverlay = TemporaryOverlayManager.AddOverlay(self.gameObject);
                        temporaryOverlay.duration = float.PositiveInfinity;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(1f, 1f, 2f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<Material>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/matBreadGraceOverlay.mat");
                        temporaryOverlay.AddToCharacterModel(self);
                        friendlyTurretOverlayManager.Overlay.Add(temporaryOverlay);
                    }
                }
            }
        }

        private void UpdateFriendlyTurretTargeting(BaseAI.orig_UpdateTargets orig, RoR2.CharacterAI.BaseAI self)
        {
            // Default targeting type: Closest enemy(?)
            orig(self);
            CharacterBody body = self.body;
            string bodyname = body.baseNameToken;
            float target_maxdist = 250f;
            //Log.Debug(bodyname);
            // Target High Value Targets (enemies with greatest combinedhealth)
            if (bodyname.Contains("FRIENDLYTURRET_BORBO") || bodyname.Contains("FRIENDLYTURRET_SNOWTIME") || bodyname.Contains("FRIENDLYTURRET_ACANTHI"))
            {
                if (bodyname.Contains("FRIENDLYTURRET_SNOWTIME"))
                {
                    target_maxdist = 125f;
                }
                if (bodyname.Contains("FRIENDLYTURRET_ACANTHI"))
                {
                    target_maxdist = 80f;
                }
                //Log.Debug("Found appropriate turret AI: " + bodyname);
                InputBankTest inputBank = body.inputBank;
                TeamComponent teamComponent = body.teamComponent;
                if (body == null || inputBank == null || teamComponent == null)
                {
                    return;
                }
                BullseyeSearch bullseyeSearch = new BullseyeSearch
                {
                    searchOrigin = body.corePosition,
                    searchDirection = inputBank.aimDirection,
                    teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex),
                    maxDistanceFilter = target_maxdist,
                    filterByLoS = true,
                    sortMode = BullseyeSearch.SortMode.None
                };
                bullseyeSearch.RefreshCandidates();
                HurtBox hurtBox = null;
                float num = float.NegativeInfinity;
                foreach (HurtBox result in bullseyeSearch.GetResults())
                {
                    float maxCombinedHealth = result.healthComponent.fullCombinedHealth;
                    if (maxCombinedHealth > num)
                    {
                        num = maxCombinedHealth;
                        hurtBox = result;
                    }
                }
                if ((bool)hurtBox)
                {
                    self.currentEnemy.bestHurtBox = hurtBox;
                }
            }
            // Clean up the small fry (enemies with lowest current health)
            // Nothing uses this yet but I think it might be worth on some turrets, keeping for the future.
            if (bodyname.Contains("sometheoreticalturret"))
            {
                //Log.Debug("Found appropriate turret AI: " + bodyname);
                InputBankTest inputBank = body.inputBank;
                TeamComponent teamComponent = body.teamComponent;
                if (body == null || inputBank == null || teamComponent == null)
                {
                    return;
                }
                BullseyeSearch bullseyeSearch = new BullseyeSearch
                {
                    searchOrigin = body.corePosition,
                    searchDirection = inputBank.aimDirection,
                    teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex),
                    maxDistanceFilter = 250f,
                    filterByLoS = true,
                    sortMode = BullseyeSearch.SortMode.None
                };
                bullseyeSearch.RefreshCandidates();
                HurtBox hurtBox = null;
                float num = float.NegativeInfinity;
                foreach (HurtBox result in bullseyeSearch.GetResults())
                {
                    float currenthealth = result.healthComponent.health;
                    if (currenthealth < num)
                    {
                        num = currenthealth;
                        hurtBox = result;
                    }
                }
                if ((bool)hurtBox)
                {
                    self.currentEnemy.bestHurtBox = hurtBox;
                }
            }
        }

        string interactablesuffering = " (UnityEngine.GameObject)";
        string charactersuffering = "(Clone) (UnityEngine.GameObject)";
        string interactablesmaster;
        string charactersmaster;
        float initialprice = 0f;

        private Interactability GetInteractabilityFriendlyTurrets(On.RoR2.PurchaseInteraction.orig_GetInteractability orig, PurchaseInteraction self, Interactor activator)
        {
            //Log.Debug(self.displayNameToken);
            if (!self.displayNameToken.StartsWith("FRIENDLYTURRET_"))
            {
                return orig(self, activator);
            }

            CharacterBody[] minionBodies = activator.gameObject.GetComponent<CharacterBody>()?.GetMinionBodies();
            if (minionBodies == null) return orig(self, activator);
            
            foreach (CharacterBody body in minionBodies)
            {
                if (!body.baseNameToken.StartsWith("FRIENDLYTURRET_"))
                {
                    continue;
                }

                //Log.Debug("Minion: " + body.baseNameToken);
                interactablesmaster = self.GetComponent<SummonMasterBehavior>().masterPrefab.gameObject.ToString();
                if (interactablesmaster.EndsWith(interactablesuffering))
                {
                    interactablesmaster = interactablesmaster.Substring(0, interactablesmaster.LastIndexOf(interactablesuffering));
                }
                charactersmaster = body.master.gameObject.ToString();
                if (charactersmaster.EndsWith(charactersuffering))
                {
                    charactersmaster = charactersmaster.Substring(0, charactersmaster.LastIndexOf(charactersuffering));
                }
                //Log.Debug("Interactable: " + self.displayNameToken + " Minion CharacterBody: " + body.baseNameToken);
                //Log.Debug("Cleaned Interactable Summonable: " + interactablesmaster + " Minion Master: " + charactersmaster);
                //Log.Debug("Does Interactable Summon Master match CharacterBody Master?");
                if (charactersmaster.Contains(interactablesmaster))
                {
                    //Log.Debug("Previous query returned true");
                    return Interactability.Disabled;
                }
                // Implement cost reduction later.
                //networkUser.id.steamId

                //if (FriendlyTurretReducedCostForPartnersOrSelf.Value)
                //{
                //    if (initialprice == 0f)
                //    {
                //        initialprice = (self.cost / 2);
                //    }
                //}
            }

            return orig(self, activator);
        }
        
        public void AddDifficulty()
        {
            Log.Debug("Adding SnowtimeToybox Custom Difficulty...");
            if (riskierLoaded)
            {
                Log.Debug("2R4R detected, updating name to LASO and its description to match new gimmicks.");
                SnowtimeLegendaryDiffDef = new(3.5f, "SNOWTIME_LASO_NAME", "SNOWTIME_LEGENDARY_ICON", "SNOWTIME_LASO_DESC", new Color32(168, 50, 50, 255), "stLeg", false);
            }
            else
            {
                Log.Debug("2R4R not detected, adding Legendary.");
                SnowtimeLegendaryDiffDef = new(3.5f, "SNOWTIME_LEGENDARY_NAME", "SNOWTIME_LEGENDARY_ICON", "SNOWTIME_LEGENDARY_DESC", new Color32(100, 170, 255, 255), "stLeg", false);
            }
            SnowtimeLegendaryDiffDef.iconSprite = _stdifficultyAssetBundle.LoadAsset<Sprite>(@"Assets/SnowtimeMod/Assets/_difficulty/texSnowtimeLegendaryPLNK.png");
            SnowtimeLegendaryDiffDef.foundIconSprite = true;
            SnowtimeLegendaryDiffIndex = DifficultyAPI.AddDifficulty(SnowtimeLegendaryDiffDef);
        }
        
        public void AddCustomBuffs()
        {
            Log.Debug("Adding SnowtimeToybox Custom BuffDefs...");
            // Assets not implemented yet
            
            IEnumerable<Type> buffTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BuffBase)));
            foreach (Type buffType in buffTypes)
            {
                Log.Debug(buffType.Name);
                BuffBase buff = (BuffBase)System.Activator.CreateInstance(buffType);
                buff.Create();
            }
        }

        public void AddCustomSkills()
        {
            Log.Debug("Adding SnowtimeToybox Custom Skills...");

            // Effects First

            ContentAddition.AddEffect(FirePlasmaRifle.MuzzleFlashObject);
            ContentAddition.AddEffect(FirePlasmaRifle.TracerObject);
            ContentAddition.AddEffect(FirePlasmaRifle.HitObject);
            ContentAddition.AddEffect(SnowtimeHaloRicochetOrb.orbEffectObject);
            ContentAddition.AddEffect(_stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFXRico.prefab"));

            ArtiPassiveFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerHiddenPassiveFamily.asset");
            ArtiTurretSkill = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerTurretling.asset");
            ArtiNoTurretSkill = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/ArtificerNoTurretling.asset");
            ContentAddition.AddSkillFamily(ArtiPassiveFamily);
            ContentAddition.AddSkillDef(ArtiTurretSkill);
            ContentAddition.AddSkillDef(ArtiNoTurretSkill);

            if(TurretlingArtificerPassive.Value)
            {
                string[] bodyNames = TurretlingPassives.Value.Split(";");

                RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManagerOnonContentPacksAssigned;
                void ContentManagerOnonContentPacksAssigned(ReadOnlyArray<ReadOnlyContentPack> obj)
                {
                    foreach (var readOnly in obj)
                    {
                        foreach (var bodyPrefab in readOnly.bodyPrefabs)
                        {
                            try
                            {
                                foreach (string bodyNameAndTurretlingName in bodyNames)
                                {
                                    string bodyName = bodyNameAndTurretlingName.Split(",")[0];
                                    if (bodyName != bodyPrefab.name) continue;
                                    
                                    string turretlingName = bodyNameAndTurretlingName.Split(",")[1];
                                    Log.Debug($"{bodyName} has turretling friend .,,. Its fragile so be careful!");

                                    if (bodyPrefab.name == bodyName)
                                    {
                                        DroneTechRepairQueue RepairQueue = bodyPrefab.gameObject.AddComponent<DroneTechRepairQueue>();
                                        RepairQueue.healRate = 0.05f;
                                        GenericSkill Turretling = bodyPrefab.gameObject.AddComponent<GenericSkill>();
                                        Turretling._skillFamily = ArtiPassiveFamily;
                                        Turretling.skillName = "Turretling";

                                        Log.Debug($" body prefab name {bodyPrefab.name}");
                                        LanguageAPI.Add($"TURRETLING_{bodyPrefab.name.ToUpper()}_NAME", turretlingName);
                                    }
                                    else
                                    {
                                        Log.Warning($"unables to find body {bodyName} !!!");
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                Log.Warning(e);
                            }
                        }
                    }
                }
            }

            GameObject DroneTechBodyPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_Drone_Tech.DroneTechBody_prefab).WaitForCompletion();
            SkillLocator skillLocator = DroneTechBodyPrefab.GetComponent<SkillLocator>();
            SkillFamily skillFamily = skillLocator.primary.skillFamily;
            SkillDef SnowtimePlasmaRifleSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/entskilldefFirePlasmaRifle.asset");

            SnowtimePlasmaRifleSkillDef.activationState = new SerializableEntityStateType(typeof(FirePlasmaRifle));

            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);
            skillFamily.variants[^1] = new SkillFamily.Variant
            {
                skillDef = SnowtimePlasmaRifleSkillDef,
                viewableNode = new ViewablesCatalog.Node(SnowtimePlasmaRifleSkillDef.skillNameToken, false)
            };
            ContentAddition.AddEntityState(typeof(FirePlasmaRifle), out _);
            ContentAddition.AddSkillDef(SnowtimePlasmaRifleSkillDef);
            // done!

            // Turretlings!
            SkillDef DroneTechTurretlingSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/Turretling/DroneTechTurretling.asset");
            foreach (GenericSkill genericSkill in DroneTechBodyPrefab.GetComponents<GenericSkill>())
            {
                if(genericSkill.skillName == "Drone1")
                {
                    Log.Debug("Found Operator Passive SkillFamily 1!");
                    Array.Resize(ref genericSkill.skillFamily.variants, genericSkill.skillFamily.variants.Length + 1);
                    genericSkill.skillFamily.variants[^1] = new SkillFamily.Variant
                    {
                        skillDef = DroneTechTurretlingSkillDef,
                        viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingSkillDef.skillNameToken, false)
                    };
                }
                else if (genericSkill.skillName == "Drone2")
                {
                    Log.Debug("Found Operator Passive SkillFamily 2!");
                    Array.Resize(ref genericSkill.skillFamily.variants, genericSkill.skillFamily.variants.Length + 1);
                    genericSkill.skillFamily.variants[^1] = new SkillFamily.Variant
                    {
                        skillDef = DroneTechTurretlingSkillDef,
                        viewableNode = new ViewablesCatalog.Node(DroneTechTurretlingSkillDef.skillNameToken, false)
                    };
                }
            }
            ContentAddition.AddSkillDef(DroneTechTurretlingSkillDef);

            // this isnt going to be fun.
            On.RoR2.DroneRepairMaster.TickHealthRepairServer += DroneRepairMasterHookTickHealthRepairServer;
            On.DroneTechController.CommandFollowInternal += DroneTechControllerHookCommandFollowInternal;
            On.DroneTechController.CommandFollow_bool_GameObject += DroneTechControllerHookCommandFollowGameObject;
            On.DroneTechController.CommandFollow_bool_DroneInfo += DroneTechControllerHookCommandFollowDroneInfo;
            On.DroneTechController.BeginFollow_CharacterBody += DroneTechControllerHookBeginFollow;
            On.DroneTechController.BeginFollow_CharacterBody_int += DroneTechControllerHookBeginFollowInt;
            On.RoR2.DroneCommandReceiver.FixedUpdate += DroneCommandReceiverHookFixedUpdate;
            On.RoR2.DroneCommandReceiver.CommandFollow += DroneCommandReceiverHookCommandFollow;
            On.RoR2.DroneCommandReceiver.ActivateFollow += DroneCommandReceiverHookActivateFollow;
            On.RoR2.DroneCommandReceiver.CommandActivate += DroneCommandReceiverHookCommandActivate;
            On.EntityStates.DroneTech.CommandCarry.OnEnter += DroneTechHookOnEnter;
            On.RoR2.CharacterMaster.OnBodyStart += SnowtimeOnBodyStart;
            On.RoR2.Items.DroneUpgradeHiddenBodyBehavior.UpdateStack += DroneUpgradeHiddenBodyBehaviorHookUpdateStack;
            // i hate doing this, idk.
            On.RoR2.CharacterModel.IsUpgradedDrone += CharacterModelHookIsUpgradedDrone;
        }

        private bool CharacterModelHookIsUpgradedDrone(On.RoR2.CharacterModel.orig_IsUpgradedDrone orig, CharacterModel self)
        {
            if (self.gameObject.name.Contains("Turretling"))
            {
                return false;
            }
            else return orig(self);
        }

        private void DroneUpgradeHiddenBodyBehaviorHookUpdateStack(On.RoR2.Items.DroneUpgradeHiddenBodyBehavior.orig_UpdateStack orig, RoR2.Items.DroneUpgradeHiddenBodyBehavior self, int newStack)
        {
            if (self.body.gameObject.name.Contains("Turretling")) return;
            orig(self, newStack);
        }

        private void SnowtimeOnBodyStart(On.RoR2.CharacterMaster.orig_OnBodyStart orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);

            var charMaster = body.master;
            if (!charMaster.gameObject.GetComponent<PlayerCharacterMasterController>()) return;
            if(!charMaster.gameObject.GetComponent<TurretlingRainbow>())
            {
                Log.Debug("Added TurretlingRainbow to " + charMaster.gameObject.name + " | " + charMaster.playerCharacterMasterController.GetDisplayName());
                charMaster.gameObject.AddComponent<TurretlingRainbow>();
            }
        }

        private void DroneTechHookOnEnter(On.EntityStates.DroneTech.CommandCarry.orig_OnEnter orig, EntityStates.DroneTech.CommandCarry self)
        {
            if (self.gameObject.name.Contains("Turretling"))
            {
                Log.Debug(self.gameObject.name + " entered state CommandCarry.OnEnter");
                if ((bool)self.target && self.target.TryGetComponent<CharacterBody>(out var component))
                {
                    self.targetBody = component;
                    self.targetTransform = self.targetBody.modelLocator.modelChildLocator.FindChild(self.targetChildIndex);
                }
                if (!self.targetTransform)
                {
                    Debug.LogError("CommandCarry.OnEnter: No targetTransform! " + self.targetChildIndex);
                }
                if ((bool)self.rigidbodyMotor)
                {
                    self.rigidbodyMotor.enabled = false;
                }
                if ((bool)self.characterMotor)
                {
                    self.characterMotor.enabled = false;
                }
                if ((bool)self.modelLocator && (bool)self.modelLocator.modelBaseTransform && (bool)self.targetTransform)
                {
                    Transform modelBaseTransform = self.modelLocator.modelBaseTransform;
                    self.cachedModelPosition = modelBaseTransform.localPosition;
                    self.cachedModelRotation = modelBaseTransform.localRotation;
                    modelBaseTransform.parent = self.targetTransform;
                    modelBaseTransform.rotation = self.targetTransform.rotation;
                    modelBaseTransform.localPosition = new Vector3(0f, self.characterBody.radius, 0f);
                    self.transform.position = modelBaseTransform.transform.position;
                }
                if ((bool)self.characterBody.hurtBoxGroup)
                {
                    self.characterBody.hurtBoxGroup.hurtBoxesDeactivatorCounter++;
                }
                self.commandReceiver = GetComponent<DroneCommandReceiver>();
                if ((bool)self.commandReceiver)
                {
                    self.commandReceiver.droneState = DroneCommandReceiver.DroneState.Busy;
                }
                self.characterBody.fakeActorCounter++;
                if (!self.isAuthority && TryGetComponent<CharacterNetworkTransform>(out var t))
                {
                    self.networkTransform = t;
                    self.networkTransform.enabled = false;
                }
            }
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self);
        }

        private void DroneTechControllerHookCommandFollowInternal(On.DroneTechController.orig_CommandFollowInternal orig, DroneTechController self, bool shouldFollow, GameObject droneBody)
        {
            if (droneBody.gameObject.name.Contains("Turretling")) return;
            orig(self, shouldFollow, droneBody);
        }

        private void DroneTechControllerHookCommandFollowGameObject(On.DroneTechController.orig_CommandFollow_bool_GameObject orig, DroneTechController self, bool shouldFollow, GameObject droneBody)
        {
            if (droneBody.gameObject.name.Contains("Turretling")) return;
            orig(self, shouldFollow, droneBody);
        }

        private void DroneTechControllerHookCommandFollowDroneInfo(On.DroneTechController.orig_CommandFollow_bool_DroneInfo orig, DroneTechController self, bool shouldFollow, DroneInfo drone)
        {
            if (drone.characterBody.gameObject.name.Contains("Turretling")) return;
            orig(self, shouldFollow, drone);
        }

        // Double the rate that the Operator Turretlings respawn; in part due to their fragility.
        private void DroneRepairMasterHookTickHealthRepairServer(On.RoR2.DroneRepairMaster.orig_TickHealthRepairServer orig, DroneRepairMaster self, float healRate)
        {
            if (self.gameObject.name.Contains("Turretling"))
            {
                if (!NetworkServer.active)
                {
                    Debug.LogWarning("[Server] function 'System.Void RoR2.DroneRepairMaster::TickHealthRepairServer(System.Single)' called on client");
                }
                else if (self.HaveRepairBody && !(self.healthComponent == null) && NetworkServer.active)
                {
                    self.healthComponent.Heal((healRate * 2) * self.healthComponent.fullHealth * Time.fixedDeltaTime, default(ProcChainMask), nonRegen: false);
                    if (self.healthComponent.health >= self.healthComponent.fullHealth)
                    {
                        self.RespawnDrone();
                    }
                }
            }
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self,healRate);
        }
        private void DroneTechControllerHookBeginFollow(On.DroneTechController.orig_BeginFollow_CharacterBody orig, DroneTechController self, CharacterBody drone)
        {
            if (drone.gameObject.name.Contains("Turretling")) return;
            orig(self, drone);
        }
        private void DroneTechControllerHookBeginFollowInt(On.DroneTechController.orig_BeginFollow_CharacterBody_int orig, DroneTechController self, CharacterBody drone, int index)
        {
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self,drone,index);
        }
        private void DroneCommandReceiverHookFixedUpdate(On.RoR2.DroneCommandReceiver.orig_FixedUpdate orig, DroneCommandReceiver self)
        {
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self);
        }
        private void DroneCommandReceiverHookCommandFollow(On.RoR2.DroneCommandReceiver.orig_CommandFollow orig, DroneCommandReceiver self, bool shouldFollow)
        {
            Log.Debug("DroneCommandReceiver.CommandFollow fired on" + self.gameObject.name);
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self, shouldFollow);
        }
        private void DroneCommandReceiverHookActivateFollow(On.RoR2.DroneCommandReceiver.orig_ActivateFollow orig, DroneCommandReceiver self, bool occupySpace)
        {
            Log.Debug("DroneCommandReceiver.ActivateFollow fired on" + self.gameObject.name);
            if (self.gameObject.name.Contains("Turretling")) return;
            orig(self, occupySpace);
        }
        private static void DroneCommandReceiverHookCommandActivate(On.RoR2.DroneCommandReceiver.orig_CommandActivate orig, DroneCommandReceiver self)
        {
            Log.Debug("DroneCommandReceiver.CommandActivate fired on " + self.gameObject.name);
            if((bool)self.commandSkill && self.gameObject.name.Contains("Turretling"))
            {
                Log.Debug("Turretling: ADMIN OVERRIDE! Executing...");
                SerializableEntityStateType serializableEntityStateType  = self.commandSkill.activationState;
                self.commandSkill.stateMachine.SetInterruptState(EntityStateCatalog.InstantiateState(ref serializableEntityStateType), InterruptPriority.Vehicle);
            }
            if (self.gameObject.name.Contains("Turretling")) return;
            Log.Debug("Drone: ADMIN OVERRIDE! Executing...");
            orig(self);
        }

        public void CollectLanguageRootFolders(List<string> folders)
        {
            folders.Add(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(base.Info.Location), "Language"));
        }
        private void Destroy()
        {
            Language.collectLanguageRootFolders -= CollectLanguageRootFolders;
        }

        private void Update()
        {
#if DEBUG
            if (Input.GetKeyUp(KeyCode.F3))
            {
                UnityHotReload.LoadNewAssemblyVersion(typeof(SnowtimeToyboxMod).Assembly,
                    Path.Combine(Path.GetDirectoryName(Info.Location)!, "SnowtimeToybox.dll"));
            }
#endif
        }
        
        [ConCommand(commandName = "friendturretpos", flags = ConVarFlags.None, helpText = "get image friend !! find her lost girl friend inside me .,.,,. ")]
        public static void friendturretfindher(ConCommandArgs args)
        {
            /*
             * case "lakes":
               stagePositions.Add(new Vector3(139f, 59.07873f, -181.3314f), Quaternion.Euler(355f, 325f, 0)); //behind a waterfall on the map's edge (how is there not already a secret here??)
               break;
             */
            Log.Debug($"case \"{SceneManager.GetActiveScene().name}\":");
            Log.Debug($"    stagePositions.Add(new Vector3({args.senderBody.footPosition.x}f, {args.senderBody.footPosition.y}f, {args.senderBody.footPosition.z}f), Quaternion.Euler({args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.x}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.y}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.z}f));");
            Log.Debug($"    break;");
        }
        
        [ConCommand(commandName = "spawn_turret", flags = ConVarFlags.None, helpText = "spawn a turret !!!")]
        public static void spawnturret(ConCommandArgs args)
        {
            Log.Info($"tryings to find {args[0]}.,,. ");
            int indexTurret = -1;

            FriendlyTurretBase friend = friendlyTurretList.First(friendlyTurret => friendlyTurret.turretName.Contains(args[0], StringComparison.InvariantCultureIgnoreCase));

            Instantiate(friend.broken, args.senderBody.footPosition, args.senderBody.transform.rotation);
        }
        
        [ConCommand(commandName = "list_turret", flags = ConVarFlags.None, helpText = "list available turrets !!!")]
        public static void listturret(ConCommandArgs args)
        {
            foreach (FriendlyTurretBase friendlyTurret in friendlyTurretList)
            {
                Log.Info(friendlyTurret.turretName);
            }
        }
    }
}
