using System.Collections.Generic;
using EntityStates;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using SnowtimeToybox.Components;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurrets;

public class BreadFriendlyTurret : FriendlyTurretBase<BreadFriendlyTurret>
{
    public override string turretWhitelistString => "FriendTurret_Bread_Whitelist";
    public override string turretName => "breads turret"; // only used for logging atm .,.
    public override GameObject turretlingMaster { get; set; }
    public override string[] riskierStats => ["baseDamage", "20", "baseRegen", "25", "baseArmor", "60", "levelDamage", "4", "levelRegen", "5", "levelArmor", "15"];
    
    public static BuffDef BreadTurretBuffPassive;
    public static BuffDef BreadTurretBuffFortune;
    public static BuffDef BreadTurretBuffNearbyAllies;

    public static GameObject FriendlyTurretBreadBeamL;
    public static GameObject FriendlyTurretBreadBeamR;
    public static GameObject FriendlyTurretBreadGraceWard;

    public override void Initalization()
    {
        base.Initalization();
        string friendDir = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/";
        
        broken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_mdlFriendlyTurretBreadBroken.prefab");
        body = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBreadBody.prefab");
        bodyRemoteOp = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBreadBodyRemoteOp.prefab");
        master = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretBreadMaster.prefab");
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/BreadPrimaryFamily.asset"));
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/BreadUtilityFamily.asset"));
        
        SkillDef beam = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/BreadBeam.asset");
        beam.activationState = new SerializableEntityStateType(typeof(FireBreadBeam));
        skillDefs.Add(beam);
        
        SkillDef silly = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/BreadShenanigans.asset");
        silly.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        skillDefs.Add(silly);
        
        FriendlyTurretBreadBeamL = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "Skills/BreadFortuneBeamL.prefab");
        FriendlyTurretBreadBeamR = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "Skills/BreadFortuneBeamR.prefab");
        FriendlyTurretBreadGraceWard = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "Buff/BreadsGraceWard.prefab");
        
        BreadTurretBuffPassive = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/BreadsPassive.asset");
        BreadTurretBuffFortune = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/BreadsFortune.asset");
        BreadTurretBuffNearbyAllies = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<BuffDef>(friendDir + "Buff/BreadsGrace.asset");
        
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretBread.asset");
        interactableSpawnCard = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<InteractableSpawnCard>(friendDir + "_iscBrokenFriendlyTurretBread.asset");
        turretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/_TurretlingMaster_Bread.prefab");
    }

    public override void ContentAdditionFuncs()
    {
        base.ContentAdditionFuncs();
        ContentAddition.AddEntityState(typeof(FireBreadBeam), out _);
        FriendlyTurretBreadBeamL.RegisterNetworkPrefab();
        FriendlyTurretBreadBeamR.RegisterNetworkPrefab();
        FriendlyTurretBreadGraceWard.RegisterNetworkPrefab();
        body.AddComponent<BreadTurretWard>();
        bodyRemoteOp.AddComponent<BreadTurretWard>();
    }
    public override void StageInteractableFuncs()
    {
        base.StageInteractableFuncs();
    }
}