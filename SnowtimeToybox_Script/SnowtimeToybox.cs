using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using EntityStates;
using EntityStates.SnowtimeToybox_FireHaloWeapon;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using ShaderSwapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using SnowtimeToybox.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityHotReloadNS;
using Path = System.IO.Path;
using SceneDirector = On.RoR2.SceneDirector;

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
        public const string Version = "1.0.0";
        public const string GUID = Author + "." + Name;
        // public static ConfigEntry<bool> ToggleBloodGulch { get; set; }

        public static SnowtimeToyboxMod instance;

        public static DifficultyDef SnowtimeLegendaryDiffDef;
        public static DifficultyIndex SnowtimeLegendaryDiffIndex;
        public static SkillDef SnowtimePlasmaRifleSkillDef;
        public static GameObject MuzzleFlashObject;
        public static GameObject TracerObject;
        public static GameObject HitObject;
        public static GameObject OrbObject;

        // friend walker turrets!
        public static DroneDef FriendlyTurretBorboDef;
        public static InteractableSpawnCard FriendlyTurretBorboIsc;
        public static SkillFamily FriendlyTurretBorboSkillFamily;
        public static SkillDef FriendlyTurretBorboSkillDef;
        public static GameObject FriendlyTurretBorboBody;
        public static GameObject FriendlyTurretBorboMaster;
        public static GameObject FriendlyTurretBorboBroken;
        //public static DroneDef FriendlyTurretTestDroneDef;
        
        public static List<GameObject> friendlyTurretList = [];

        public static bool Legendary = false;
        // Copied from RiskierRain, sorry borbo :(
        public static bool ModLoaded(string modGuid) { return !string.IsNullOrEmpty(modGuid) && BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey(modGuid); }
        public static bool riskierLoaded => ModLoaded("com.RiskOfBrainrot.RiskierRain");

        public void Awake()
        {
            instance = this;

            Log.Init(Logger);

            //ToggleBloodGulch = Config.Bind("Stage 1 Toggles", "Blood Gulch", true, "If true, Blood Gulch is added to the loop, otherwise it shall not appear");

            Language.collectLanguageRootFolders += CollectLanguageRootFolders;

            Run.onRunStartGlobal += (Run run) =>
            {
                Legendary = false;
                if (run.selectedDifficulty == SnowtimeLegendaryDiffIndex)
                {
                    Legendary = true;
                    CharacterMaster.onStartGlobal += CharacterMaster_OnStartGlobal;
                }
            };

            Run.onRunDestroyGlobal += (Run run) =>
            {
                Legendary = false;
                CharacterMaster.onStartGlobal -= CharacterMaster_OnStartGlobal;
            };

            GlobalEventManager.onServerDamageDealt += SnowtimeDamageTypes.GlobalEventManager_onServerDamageDealt;
            
            On.RoR2.SceneDirector.Start += SceneDirectorOnStart;
        }

        private void SceneDirectorOnStart(SceneDirector.orig_Start orig, RoR2.SceneDirector self)
        {
            orig(self);
            Log.Debug("hehe !");
            
            if (!NetworkServer.active) return;
            
            Vector3 position;
            Quaternion rotation;

            Dictionary<Vector3, Quaternion> stagePositions = new Dictionary<Vector3, Quaternion>();
            string currStage = SceneManager.GetActiveScene().name;
            switch (currStage)
            {
                case "blackbeach":
                    stagePositions.Add(new Vector3(-60, -51.2f, -231), Quaternion.Euler(0, 0, 0)); //high cliff in the middle of the map
                    break;
                case "blackbeach2":
                    stagePositions.Add(new Vector3(-101.4f, 1.5f, 20.1f), Quaternion.Euler(0, 292.8f, 0)); //between two knocked-over pillars near the gate
                    break;
                case "golemplains":
                    stagePositions.Add(new Vector3(190.3899f, -86.776f, -135.183f), Quaternion.Euler(0f, 59.69361f, 0f));
                    break;
                case "golemplains2":
                    stagePositions.Add(new Vector3(9.8f, 127.5f, -251.8f), Quaternion.Euler(0, 5, 0)); //on the cliff where the middle giant ring meets the ground
                    break;
                case "goolake":
                    stagePositions.Add(new Vector3(53.9f, -45.9f, -219.6f), Quaternion.Euler(0, 190, 0)); //on the clifftop near the ancient gate
                    break;
                case "foggyswamp":
                    stagePositions.Add(new Vector3(-83.74f, -83.35f, 39.09f), Quaternion.Euler(0, 104.27f, 0)); //on the wall / dam across from where two newt altars spawn
                    break;
                case "frozenwall":
                    stagePositions.Add(new Vector3(-230.7f, 132, 239.4f), Quaternion.Euler(0, 167, 0)); //on cliff near water, next to the lone tree
                    break;
                case "wispgraveyard":
                    stagePositions.Add(new Vector3(-341.5f, 79, 0.5f), Quaternion.Euler(0, 145, 0)); //small cliff outcrop above playable area, same large island with artifact code
                    break;
                case "dampcavesimple":
                    stagePositions.Add(new Vector3(157.5f, -43.1f, -188.9f), Quaternion.Euler(0, 318.4f, 0)); //on the overhang above rex w/ 3 big rocks
                    break;
                case "shipgraveyard":
                    stagePositions.Add(new Vector3(20.5f, -23.7f, 185.1f), Quaternion.Euler(0, 173.6f, 0)); //in the cave entrance nearest to the cliff, on the spire below the land bridge
                    break;
                case "rootjungle":
                    stagePositions.Add(new Vector3(-196.6f, 190.1f, -204.5f), Quaternion.Euler(0, 80, 0)); //top of the highest root in the upper / back area
                    break; 
                case "skymeadow":
                    stagePositions.Add(new Vector3(65.9f, 127.4f, -293.9f), Quaternion.Euler(0, 194.8f, 0)); //on top of the tallest rock spire, opposite side of map from the moon
                    break;
                case "snowyforest":
                    stagePositions.Add(new Vector3(-38.7f, 112.7f, 153.1f), Quaternion.Euler(0, 54.1f, 0)); //on top of a lone elevated platform on a tree
                    break;
                case "ancientloft":
                    stagePositions.Add(new Vector3(-133.4f, 33f, -280f), Quaternion.Euler(0, 354.5f, 0)); //on a branch under the main platform in the back corner of the map
                    break;
                case "sulfurpools":
                    stagePositions.Add(new Vector3(-33.6f, 36.8f, 164.1f), Quaternion.Euler(0, 187f, 0)); //in the corner, atop of one of the columns
                    break;
                case "FBLScene":
                    stagePositions.Add(new Vector3(58.3f, 372f, -88.8f), Quaternion.Euler(0, 0, 0)); //overlooking the shore
                    break;
                case "drybasin":
                    stagePositions.Add(new Vector3(149.4f, 65.7f, -212.7f), Quaternion.Euler(0, 0, 0)); //in a cranny near collapsed aqueducts
                    break;
                case "lakes":
                    stagePositions.Add(new Vector3(139f, 59.07873f, -181.3314f), Quaternion.Euler(355f, 325f, 0)); //behind a waterfall on the map's edge (how is there not already a secret here??)
                    break;
                default:
                    Log.Debug("no custom pos !!! too bad ,..");
                    return;
            }        
            GameObject turret = friendlyTurretList[Run.instance.runRNG.RangeInt(0, friendlyTurretList.Count)];
            KeyValuePair<Vector3, Quaternion> stagePos = stagePositions.ElementAt(Run.instance.runRNG.RangeInt(0, stagePositions.Count));
            GameObject term = Instantiate(turret, stagePos.Key, stagePos.Value);
            Log.Debug($"turret name = {turret.name} !!!!");

            NetworkServer.Spawn(term);
        }

        public static String assetDirectory;
        public static AssetBundle _stdifficultyAssetBundle;
        public static AssetBundle _stcharacterAssetBundle;
        internal const string _stdifficultyAssetBundleName = "snowtimetoybox_difficulty";
        internal const string _stcharacterAssetBundleName = "snowtimetoybox_characters";

        private void Start()
        {
            instance = this;

            var assetsFolderFullPath = Path.Combine(Path.GetDirectoryName(typeof(SnowtimeToyboxMod).Assembly.Location), "assetbundles");
            assetDirectory = assetsFolderFullPath;
            Debug.Log("Ran Start!");
            _stcharacterAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stcharacterAssetBundleName));
            base.StartCoroutine(_stcharacterAssetBundle.UpgradeStubbedShadersAsync());
            _stdifficultyAssetBundle = AssetBundle.LoadFromFile(Path.Combine(assetsFolderFullPath, _stdifficultyAssetBundleName));
            Debug.Log(_stcharacterAssetBundle);
            Debug.Log(_stdifficultyAssetBundle);

            AddDifficulty();
            //AddEffectDef();
            AddCustomSkills();
            AddCustomAllies();
            AddCustomBuffs();
        }

        public void AddCustomAllies()
        {
            Log.Debug(GUID);
            Log.Debug("Adding SnowtimeToybox Friend Drones...");
            // Entity States need to be added before the rest of the content is loaded.
            Log.Debug("Adding Friendly Turret Entity States");
            // borbo turret borbo turret
            // Add Borbo Turret
            Log.Debug("Defining Friendly Turret based on Borbo (2R4R)");
            FriendlyTurretBorboBody = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorboBody.prefab");
            FriendlyTurretBorboMaster = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorboMaster.prefab");
            FriendlyTurretBorboSkillFamily = _stcharacterAssetBundle.LoadAsset<SkillFamily>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/BorboPrimaryFamily.asset");
            FriendlyTurretBorboSkillDef = _stcharacterAssetBundle.LoadAsset<SkillDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Skills/BorboBlast.asset");
            // I am being PEDANTIC but i dont care!
            FriendlyTurretBorboSkillDef.activationState = new SerializableEntityStateType(typeof(ChargeBorboLaser));
            FriendlyTurretBorboBroken = _stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_mdlFriendlyTurretBorboBroken.prefab");
            FriendlyTurretBorboDef = _stcharacterAssetBundle.LoadAsset<DroneDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/_FriendlyTurretBorbo.asset");
            // Add the Content
            Log.Debug("Adding Borbo Turret Assets");
            ContentAddition.AddEntityState(typeof(FireBorboLaser), out _);
            ContentAddition.AddEntityState(typeof(ChargeBorboLaser), out _);
            ContentAddition.AddBody(FriendlyTurretBorboBody);
            ContentAddition.AddMaster(FriendlyTurretBorboMaster);
            ContentAddition.AddSkillFamily(FriendlyTurretBorboSkillFamily);
            ContentAddition.AddSkillDef(FriendlyTurretBorboSkillDef);
            ContentAddition.AddEffect(FireBorboLaser.effectPrefabObject);
            ContentAddition.AddEffect(FireBorboLaser.hitEffectPrefabObject);
            ContentAddition.AddEffect(FireBorboLaser.tracerEffectPrefabObject);
            // ContentAddition.AddDroneDef();

            Log.Debug("Adding Friendly Turrets Interactables to Stages");
            ContentAddition.AddNetworkedObject(FriendlyTurretBorboBroken);
            // i want die
            friendlyTurretList.Add(FriendlyTurretBorboBroken);
        }


        private void CharacterMaster_OnStartGlobal(CharacterMaster obj)
        {
            if (obj.teamIndex != TeamIndex.Player)
            {
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.AlienHead, 1);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostAttackSpeed, 3);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostHp, 4);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.PersonalShield, 5);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.BoostDamage, 10);
                if (obj.inventory) obj.inventory.GiveItemPermanent(RoR2Content.Items.ShinyPearl, 1);
            }
        }

        public void AddDifficulty()
        {
            Log.Debug(GUID);
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

        public static BuffDef BorboTurretDebuff;

        public void AddCustomBuffs()
        {
            Log.Debug(GUID);
            Log.Debug("Adding SnowtimeToybox Custom BuffDefs...");
            BorboTurretDebuff = _stcharacterAssetBundle.LoadAsset<BuffDef>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Borbo/Buff/BorboTurretDebuff.asset");
            Log.Debug(BorboTurretDebuff);
            ContentAddition.AddBuffDef(BorboTurretDebuff);
            
            IEnumerable<Type> buffTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BuffBase)));
            foreach (Type buffType in buffTypes)
            {
                BuffBase buff = (BuffBase)System.Activator.CreateInstance(buffType);
                buff.Create();
            }
            
        }

        public void AddCustomSkills()
        {
            Log.Debug(GUID);
            Log.Debug("Adding SnowtimeToybox Custom Skills...");

            // Effects First

            ContentAddition.AddEffect(FirePlasmaRifle.MuzzleFlashObject);
            ContentAddition.AddEffect(FirePlasmaRifle.TracerObject);
            ContentAddition.AddEffect(FirePlasmaRifle.HitObject);
            ContentAddition.AddEffect(SnowtimeHaloRicochetOrb.orbEffectObject);
            ContentAddition.AddEffect(_stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFXRico.prefab"));

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
    }
}
