using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace SnowtimeToybox.Components
{
    [RequireComponent(typeof(CharacterBody))]
    public class BreadTurretWard : MonoBehaviour
    {
        public CharacterBody self;
        public GameObject BreadGraceWard = SnowtimeToyboxMod.FriendlyTurretBreadGraceWard;
        public GameObject BreadGraceWardInstance;

        public void Awake()
        {
            enabled = NetworkServer.active;
            self = GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            if (self.HasBuff(SnowtimeToyboxMod.BreadTurretBuffPassive))
            {
                //BreadGraceWardInstance = PrefabAPI.InstantiateClone(BreadGraceWard, self.baseNameToken + "_BreadGraceWard");
                if (BreadGraceWardInstance == null)
                {
                    BreadGraceWardInstance = Instantiate(BreadGraceWard);
                    BreadGraceWardInstance.GetComponent<TeamFilter>().teamIndex = self.teamComponent.teamIndex;
                    BreadGraceWardInstance.GetComponent<BuffWard>().Networkradius = 30f + self.radius;
                    BreadGraceWardInstance.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.gameObject);
                }
            }
            else
            {
                Object.Destroy(BreadGraceWardInstance);
                BreadGraceWardInstance = null;
            }
        }

        public void OnDestroy()
        {
            if (BreadGraceWardInstance)
            {
                Destroy(BreadGraceWardInstance);
                BreadGraceWardInstance = null;
            }
        }
    }
}