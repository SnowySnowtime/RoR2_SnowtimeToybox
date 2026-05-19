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
using UnityEngine.XR;
using RoR2BepInExPack.GameAssetPaths;
using RoR2.Projectile;

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
    [BepInDependency("_score.MiscFixes", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.RiskOfBrainrot.RiskierRain", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskOfBrainrot.SwanSongExtended", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    public class SnowtimeToyboxMod : BaseUnityPlugin
    {
        public const string Author = "SnowySnowtime";
        public const string Name = nameof(SnowtimeToyboxMod);
        public const string Version = "1.2.9";
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
        public static DamageAPI.ModdedDamageType SwarmlingArmorStripOnHit = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType SwarmlingNeedleImpale = DamageAPI.ReserveDamageType();

        public static List<FriendlyTurretBase> friendlyTurretList = [];

        public static bool Legendary = false;
        // Copied from RiskierRain, sorry borbo :(
        public static bool ModLoaded(string modGuid) { return !string.IsNullOrEmpty(modGuid) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(modGuid); }
        public static bool riskierLoaded => ModLoaded("com.RiskOfBrainrot.RiskierRain");
        public static bool scepterLoaded => ModLoaded("com.DestroyedClone.AncientScepter");

        public static String assetDirectory;
        public static AssetBundle _stdifficultyAssetBundle;
        public static AssetBundle _stcharacterAssetBundle;
        public static AssetBundle _stitemAssetBundle;
        internal const string _stdifficultyAssetBundleName = "snowtimetoybox_difficulty";
        internal const string _stcharacterAssetBundleName = "snowtimetoybox_characters";
        internal const string _stitemAssetBundleName = "snowtimetoybox_items";

        public static ConfigEntry<bool> ToggleLegendary { get; set; }
        public static ConfigEntry<bool> SwarmlingOSP { get; set; }
        public static ConfigEntry<float> SwarmlingBaseDamage { get; set; }
        public static ConfigEntry<float> SwarmlingDamagePerLevel { get; set; }
        public static ConfigEntry<float> SwarmlingBaseMaxHealth { get; set; }
        public static ConfigEntry<float> SwarmlingMaxHealthPerLevel { get; set; }
        public static ConfigEntry<float> SwarmlingBaseRegen { get; set; }
        public static ConfigEntry<float> SwarmlingRegenPerLevel { get; set; }
        public static ConfigEntry<float> SwarmlingBaseArmor { get; set; }
        public static ConfigEntry<float> SwarmlingMinionOffenseStatMult { get; set; }
        public static ConfigEntry<float> SwarmlingMinionDefenseStatMult { get; set; }
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
        public static ConfigEntry<float> TurretlingDemoChance { get; set; }
        public static ConfigEntry<float> TurretlingGibberishChance { get; set; }

        public void Awake()
        {
            instance = this;

            Log.Init(Logger);

            ToggleLegendary = Config.Bind("Difficulty", "Legendary", true, "If true, Legendary is enabled as a selectable difficulty.");
            SwarmlingOSP = Config.Bind("Survivors - Swarmling", "One-Shot Protection", false, "If true, enables one shot protection for the Swarmling.");
            SwarmlingBaseDamage = Config.Bind("Survivors - Swarmling", "Base Damage", 12f, "Base Damage.");
            SwarmlingDamagePerLevel = Config.Bind("Survivors - Swarmling", "Damage per Level", 2.4f, "Damage per Level.");
            SwarmlingBaseMaxHealth = Config.Bind("Survivors - Swarmling", "Base Health", 50f, "Base Health.");
            SwarmlingMaxHealthPerLevel = Config.Bind("Survivors - Swarmling", "Health per Level", 15f, "Health per Level.");
            SwarmlingBaseRegen = Config.Bind("Survivors - Swarmling", "Base Health Regen", 3f, "Base Health Regen.");
            SwarmlingRegenPerLevel = Config.Bind("Survivors - Swarmling", "Health Regen per Level", 0.5f, "Health Regen per Level.");
            SwarmlingBaseArmor = Config.Bind("Survivors - Swarmling", "Base Armor", 10f, "Base Armor.");
            SwarmlingMinionOffenseStatMult = Config.Bind("Survivors - Swarmling", "(Minion) Swarm Offensive Stat Divider", 3f, "Divides the 'Base Damage', 'Damage per Level' of the Swarm for balance purposes. Uses the base stats of the player at 1");
            SwarmlingMinionDefenseStatMult = Config.Bind("Survivors - Swarmling", "(Minion) Swarm Defensive Stat Divider", 1.25f, "Divides the 'Base Damage', 'Damage per Level' of the Swarm for balance purposes. Uses the base stats of the player at 1");
            ToggleSpawnMessages = Config.Bind("Friendly Turret Functions", "Spawn Message", true, "If true, the Friendly Turrets will give a message on every stage they spawn on, for insight on if and which turret spawned. Otherwise, friendly turrets are shy, and are also sad!");
            FriendlyTurretShortcakeAggroType = Config.Bind("Friendly Turret Functions", "Strawberry Shortcake Aggro Method", false, "If true, the Strawberry Shortcake Turret will spawn with a native increase to its aggro. Else, it only gains aggro for ~0.5s when its main skill fires.");
            FriendlyTurretImmuneVoidDeath = Config.Bind("Friendly Turret Flags", "Void Death Immunity", true, "If true, Friendly Turrets are immune to Void Death (Void Reaver implosions), this is because they are awful at avoiding them even with mods to make allies avoid them, and we get sad when they are detained.");
            FriendlyTurretFallImmunity = Config.Bind("Friendly Turret Flags", "Fall Damage Immunity", true, "If true, Friendly Turrets (and turretlings) are immune to fall damage, as navigating some maps can be a little difficult for them. Prevents any unexpected turret deaths, as we cant simply 'replace' them like Engineer can.");
            FriendlyTurretDrone = Config.Bind("Friendly Turret Flags", "Drone", false, "If true, Friendly Turrets (and turretlings) are flagged as drones. Probably comes with some oddities.");
            FriendlyTurretRemoteOpPrice = Config.Bind("Friendly Turret Functions", "Remote Operation Cost", 250, "Cost for becoming a Friendly Turret with Remote Operation.");
            TurretlingArtificerPassive = Config.Bind("Turretlings", "Turretling Passive", false, "If true, gives a turretling passive to those defined in Turretling Passive List.");
            TurretlingPassives = Config.Bind("Turretlings", "Turretling Passive List", "MageBody,Divineling;MercBody,Mercling;RailgunnerBody,Purity;BastionRobot,Ganymede;SeekerBody,Toastling;HuntressBody,Kottling;RocketSurvivorBody,Lil\'lusiveling;LoaderBody,Scrapling;ArbiterBody,Nugget;CaptainBody,\'Paperweight\';DemolisherBody,Demoling", "internal names for bodies that should have turretlings ,.., (bodyname,turretlingname) turretlingname is the name given to these turretlings");
            TurretlingSpawnChance = Config.Bind("Turretlings", "Turretling Variant Spawn Chance ,,.", 100f, "chance to get a turretling when buying a friendly turret !!!");
            TurretlingImmuneVoidDeath = Config.Bind("Turretlings", "Void Death Immunity", false, "If true, All turretlings are immune to Void Death (Void Reaver implosions). Keep the scrunglies safe.");
            TurretlingReviveCostMult = Config.Bind("Turretlings", "turretling revive cost mult .,.", 0.6f, "price multiplier for reviving turretlings ,.. ,.");
            TurretlingKillOriginalTurrets = Config.Bind("Turretlings", "kill original turrets .,,.", false, "kills normal(gunner) turrets and replaces them with turretlings ,. ,.");
            TurretlingRainbowChance = Config.Bind("Turretlings", "turretling rainbow chance ,,.", 1f, "% chance to get a powerful and prideful rainbow turretling ,.,.");
            TurretlingRainbowBonusItems = Config.Bind("Turretlings", "turretling rainbow bonus items ,,.", "syringe,50,alienhead,5,extralife,1,moremissile,1,adaptivearmor,1,powercube,1,shockdamageaura,1", "give rainbow turretlings bonus items !!! follows (internalitemname),(count)");
            TurretlingDemoChance = Config.Bind("Turretlings", "turretling demo chance ,.,,,.", 10f, "% chance to get a drunken gremlin ,.,.");
            TurretlingGibberishChance = Config.Bind("Turretlings", "turretling demo gibberish chance ,.,,,.", 100f, "how often for demolings to go fghrgjnbvrfbjftgnbfg ,.,.");
            TurretlingBaseDamage = Config.Bind("Turretling Stats", "Base Damage", 12f, "Damage the turretling deals. Blaster deals 100%(1x) base damage, Pixi Launcher deal 200%(2x) base damage. demoling grenade launcher does 300%(3x) base damage. Does not affect Turretling variants.");
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

            if(ToggleLegendary.Value == true)
            {
                AddDifficulty();
            }
            AddCustomItems();
            Content.AddCustomSkills();
            Content.AddCustomAllies();
            AddCustomBuffs();
            Content.AddCustomEffects();
            if (scepterLoaded)
            {
                Content.AddScepterSkills();
            }

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

            ItemCatalog.availability.CallWhenAvailable(AddCustomTagsToItems);
            EquipmentCatalog.availability.CallWhenAvailable(AddElitesToList);
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
            ItemDef[] whitelistTurretlingNone = [
                // Base
                RoR2Content.Items.BoostDamage,
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
            foreach (ItemDef item in whitelistTurretlingNone)
            {
                Log.Debug("Added " + item.name + " to bread turret's item whitelist");
                ItemAPI.ApplyTagToItem("FriendTurret_None_Whitelist", item);
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
            return;
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
            Log.Info($"case \"{SceneManager.GetActiveScene().name}\":");
            Log.Info($"    stagePositions.Add(new Vector3({args.senderBody.footPosition.x}f, {args.senderBody.footPosition.y}f, {args.senderBody.footPosition.z}f), Quaternion.Euler({args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.x}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.y}f, {args.senderBody.modelLocator.modelTransform.rotation.eulerAngles.z}f));");
            Log.Info($"    break;");
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
