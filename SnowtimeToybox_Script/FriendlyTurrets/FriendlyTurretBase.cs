using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurretChecks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SnowtimeToybox.FriendlyTurrets;

public abstract class FriendlyTurretBase<T> : FriendlyTurretBase where T : FriendlyTurretBase<T>
{
    //This, which you will see on all the -base classes, will allow both you and other modders to enter through any class with this to access internal fields/properties/etc as if they were a member inheriting this -Base too from this class.
    public static T instance { get; private set; }

    public FriendlyTurretBase()
    {
        if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting ItemBase was instantiated twice");
        instance = this as T;
    }
}
public abstract class FriendlyTurretBase
{
    public virtual GameObject broken { get; set;  }
    public virtual GameObject body { get; set;  }
    public virtual GameObject bodyRemoteOp { get; set;  }
    public virtual GameObject master { get; set;  }

    public List<SkillFamily> skillFamilies = [];
    public List<SkillDef> skillDefs = [];

    public virtual DroneDef droneDef { get; set;  }

    public virtual string turretWhitelistString { get; set;  }
    public virtual string turretName { get; set;  }

    public virtual GameObject turretlingMaster { get; set;  }
    public virtual InteractableSpawnCard interactableSpawnCard { get; set; }
    
    public virtual string[] riskierStats { get; set; }
    
    public virtual void Initalization()
    {
        Log.Debug($"initalizing {turretName} !!");
    }
    
    public virtual void ContentAdditionFuncs()
    {
        ContentAddition.AddDroneDef(droneDef);
        ContentAddition.AddBody(body);
        ContentAddition.AddBody(bodyRemoteOp);
        ContentAddition.AddMaster(master);
        foreach (var skillFamily in skillFamilies)
        {
            ContentAddition.AddSkillFamily(skillFamily);
        }
        foreach (var skillDef in skillDefs)
        {
            ContentAddition.AddSkillDef(skillDef);
        }
        ContentAddition.AddNetworkedObject(broken);
    }

    public virtual void StageInteractableFuncs()
    {
        Log.Debug(interactableSpawnCard);
        var directorCardFriendlyTurret = new DirectorCard
        {
            spawnCard = interactableSpawnCard,
            selectionWeight = 0,
            spawnDistance = DirectorCore.MonsterSpawnDistance.Standard,
            minimumStageCompletions = 670,
            preventOverhead = false
        };

        var directorCardHolderFriendlyTurret = new DirectorAPI.DirectorCardHolder
        {
            Card = directorCardFriendlyTurret,
            InteractableCategory = DirectorAPI.InteractableCategory.Drones
        };

        List<DirectorAPI.Stage> friendlyTurretStageList = new List<DirectorAPI.Stage>();
        List<String> friendlyTurretCustomStageList = new List<String>();

        // Stage 1
        friendlyTurretStageList.Add(DirectorAPI.Stage.TitanicPlains);
        friendlyTurretStageList.Add(DirectorAPI.Stage.DistantRoost);
        friendlyTurretStageList.Add(DirectorAPI.Stage.SiphonedForest);
        friendlyTurretStageList.Add(DirectorAPI.Stage.VerdantFalls);
        friendlyTurretStageList.Add(DirectorAPI.Stage.ViscousFalls);
        // Stage 2
        friendlyTurretStageList.Add(DirectorAPI.Stage.AbandonedAqueduct);
        friendlyTurretStageList.Add(DirectorAPI.Stage.AphelianSanctuary);
        friendlyTurretStageList.Add(DirectorAPI.Stage.PretendersPrecipice);
        // Stage 3
        friendlyTurretStageList.Add(DirectorAPI.Stage.RallypointDelta);
        friendlyTurretStageList.Add(DirectorAPI.Stage.ScorchedAcres);
        friendlyTurretStageList.Add(DirectorAPI.Stage.IronAlluvium);
        friendlyTurretStageList.Add(DirectorAPI.Stage.IronAuroras);
        // Stage 4
        friendlyTurretStageList.Add(DirectorAPI.Stage.SirensCall);
        friendlyTurretStageList.Add(DirectorAPI.Stage.SunderedGrove);
        friendlyTurretStageList.Add(DirectorAPI.Stage.RepurposedCrater);
        friendlyTurretStageList.Add(DirectorAPI.Stage.ConduitCanyon);
        // Stage 5
        friendlyTurretStageList.Add(DirectorAPI.Stage.SkyMeadow);
        // Mods
        friendlyTurretCustomStageList.Add("FBLScene");
        friendlyTurretCustomStageList.Add("broadcastperch_wormsworms");
        friendlyTurretCustomStageList.Add("tropics_wormsworms");
        friendlyTurretCustomStageList.Add("tropicsnight_wormsworms");
        friendlyTurretCustomStageList.Add("hollowsummit_wormsworms");
        friendlyTurretCustomStageList.Add("hollowsummitnight_wormsworms");
        friendlyTurretCustomStageList.Add("catacombs_DS1_Catacombs");
        friendlyTurretCustomStageList.Add("snowtime_bloodgulch");
        friendlyTurretCustomStageList.Add("snowtime_deathisland");
        friendlyTurretCustomStageList.Add("snowtime_gephyrophobia");
        friendlyTurretCustomStageList.Add("snowtime_gmconstruct");
        friendlyTurretCustomStageList.Add("snowtime_gmflatgrass");
        friendlyTurretCustomStageList.Add("snowtime_halo");
        friendlyTurretCustomStageList.Add("snowtime_halo2");
        friendlyTurretCustomStageList.Add("snowtime_highcharity");
        friendlyTurretCustomStageList.Add("snowtime_icefields");
        friendlyTurretCustomStageList.Add("snowtime_newmombasabridge");
        friendlyTurretCustomStageList.Add("snowtime_odstmombasa");
        friendlyTurretCustomStageList.Add("snowtime_plrhightower");
        friendlyTurretCustomStageList.Add("snowtime_sandtrap");
        friendlyTurretCustomStageList.Add("snowtime_sidewinder");

        foreach (DirectorAPI.Stage stage in friendlyTurretStageList)
        {
            //Log.Debug("Adding Friendly Turrets to stage: " + stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurret, stage);
        }
        foreach (string stage in friendlyTurretCustomStageList)
        {
            //Log.Debug("Adding Friendly Turrets to stage: " + stage);
            DirectorAPI.Helpers.AddNewInteractableToStage(directorCardHolderFriendlyTurret, DirectorAPI.Stage.Custom, stage);
        }
    }
    
    public virtual void PostInit()
    {
        master.AddComponent<TurretlingBabySpawner>().turretlingPrefab = turretlingMaster;
        broken.AddComponent<BorboCheck>().purchaseInteraction = broken.GetComponent<PurchaseInteraction>();
        master.AddComponent<FriendlyTurretInheritance>().whitelistedTag = turretWhitelistString;

        body.AddComponent<EquipmentSlot>();

        CharacterBody charBody = body.GetComponent<CharacterBody>();
        if (SnowtimeToyboxMod.riskierLoaded)
        {
            for (int i = 0; i < riskierStats.Length; i += 2)
            {
                charBody.SetFieldValue(riskierStats[i], float.Parse(riskierStats[i + 1]));
            }
        }

        charBody.bodyFlags |= SnowtimeToyboxMod.bodyFlags;
        SnowtimeToyboxMod.friendlyTurretList.Add(this);
        droneDef.remoteOpCost = SnowtimeToyboxMod.FriendlyTurretRemoteOpPrice.Value;
        Log.Debug("Updated Friendly Turret " + charBody.name + " Remote Operation prices to: " +  droneDef.remoteOpCost);

        ExplicitPickupDropTable dtTripleDroneShopBlacklist = Addressables.LoadAssetAsync<ExplicitPickupDropTable>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_DLC3_TripleDroneShop.dtTripleDroneShopBlacklist_asset).WaitForCompletion();
        Array.Resize(ref dtTripleDroneShopBlacklist.pickupEntries, dtTripleDroneShopBlacklist.pickupEntries.Length + 1);
        Log.Debug("Adding " + droneDef.name + " to " + dtTripleDroneShopBlacklist.name);
        dtTripleDroneShopBlacklist.pickupEntries[^1] = new ExplicitPickupDropTable.PickupDefEntry
        {
            pickupDef = droneDef,
            pickupWeight = 8008132
        };
    }
}