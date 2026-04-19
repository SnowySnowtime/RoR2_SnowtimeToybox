using System.Collections.Generic;
using EntityStates;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace SnowtimeToybox.FriendlyTurrets;

public class SnowtimeFriendlyTurret : FriendlyTurretBase<SnowtimeFriendlyTurret>
{
    public override string turretWhitelistString => "FriendTurret_Snowtime_Whitelist";
    public override string turretName => "snowsytimesnowtimesnowysnowstages turret"; // only used for logging atm .,.
    public override string[] riskierStats => ["baseDamage", "25", "baseRegen", "21", "baseArmor", "30", "levelDamage", "5", "levelRegen", "5", "levelArmor", "2"];
    
    public override void Initalization()
    {
        base.Initalization();
        string friendDir = @"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Snowtime/";
        
        broken = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_mdlFriendlyTurretSnowtimeBroken.prefab");
        body = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretSnowtimeBody.prefab");
        master = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(friendDir + "_FriendlyTurretSnowtimeMaster.prefab");
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/SnowtimePrimaryFamily.asset"));
        skillFamilies.Add(SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillFamily>(friendDir + "Skills/SnowtimeUtilityFamily.asset"));

        SkillDef cryoGuass = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/SnowtimeCryoGauss.asset");
        cryoGuass.activationState = new SerializableEntityStateType(typeof(SnowtimeCryoGaussFire));
        skillDefs.Add(cryoGuass);
        
        SkillDef silly = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<SkillDef>(friendDir + "Skills/SnowtimeShenanigans.asset");
        silly.activationState = new SerializableEntityStateType(typeof(Shenanigans));
        skillDefs.Add(silly);
        
        droneDef = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<DroneDef>(friendDir + "_FriendlyTurretSnowtime.asset");
        turretlingMaster = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Turretling/Variants/_TurretlingMaster_Snowtime.prefab");
    }

    public override void ContentAdditionFuncs()
    {
        base.ContentAdditionFuncs();
        ContentAddition.AddEntityState(typeof(SnowtimeCryoGaussFire), out _);
        ContentAddition.AddEffect(SnowtimeCryoGaussFire.muzzleflashEffectObject);
        ContentAddition.AddProjectile(SnowtimeCryoGaussFire.projectileObject);
        ContentAddition.AddEffect(SnowtimeCryoGaussFire.projectileGhostObject);
        ContentAddition.AddEffect(SnowtimeCryoGaussFire.projectileExplosionObject);
    }
}