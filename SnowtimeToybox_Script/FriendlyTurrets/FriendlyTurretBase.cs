using System;
using System.Collections.Generic;
using EntityStates.SnowtimeToybox_FriendlyTurret;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using SnowtimeToybox.Components;
using SnowtimeToybox.FriendlyTurretChecks;
using UnityEngine;

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
    public virtual GameObject master { get; set;  }

    public List<SkillFamily> skillFamilies = [];
    public List<SkillDef> skillDefs = [];

    public virtual DroneDef droneDef { get; set;  }

    public virtual string turretWhitelistString { get; set;  }
    public virtual string turretName { get; set;  }

    public virtual GameObject turretlingMaster { get; set;  }
    
    public virtual string[] riskierStats { get; set; }
    
    public virtual void Initalization()
    {
        Log.Debug($"initalizing {turretName} !!");
    }
    
    public virtual void ContentAdditionFuncs()
    {
        ContentAddition.AddBody(body);
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
    }
}