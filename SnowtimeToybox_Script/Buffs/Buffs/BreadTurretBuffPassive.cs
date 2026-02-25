using R2API;
using RoR2;
using RoR2BepInExPack.GameAssetPaths.Version_1_39_0;
using R2API.ContentManagement;
using SnowtimeToybox;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static R2API.RecalculateStatsAPI;
using EntityStates.BrotherMonster;
using EntityStates.AffixVoid;
using R2API.AddressReferencedAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox.Buffs
{
    public class BreadTurretBuffPassively : BuffBase<BreadTurretBuffPassively>
    {
        public override BuffDef Buff => SnowtimeToyboxMod.BreadTurretBuffPassive;
        public GameObject BreadGraceWard = SnowtimeToyboxMod.FriendlyTurretBreadGraceWard;
        public GameObject BreadGraceWardInstance;

        public override void PostCreation()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddBreadTurretBuffNearby;
            //On.RoR2.CharacterBody.AddBuff_BuffDef += BreadSpawnGraceWard;
            On.RoR2.CharacterBody.FixedUpdate += BreadSpawnGraceWard;
        }

        private void AddBreadTurretBuffNearby(CharacterBody sender, StatHookEventArgs args)
        {
            if (!sender.HasBuff(Buff)) return;
            //args.attackSpeedMultAdd += 1.2f;
            //args.barrierDecayMult += 0.4f;
        }

        private void BreadSpawnGraceWard(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            if (!NetworkServer.active) return;
            if (self == null) return;
            if (self.HasBuff(Buff))
            {
                //BreadGraceWardInstance = PrefabAPI.InstantiateClone(BreadGraceWard, self.baseNameToken + "_BreadGraceWard");
                if (BreadGraceWardInstance == null)
                {
                    BreadGraceWardInstance = Object.Instantiate(BreadGraceWard);
                    BreadGraceWardInstance.GetComponent<TeamFilter>().teamIndex = self.teamComponent.teamIndex;
                    BreadGraceWardInstance.GetComponent<BuffWard>().Networkradius = 20f + self.radius;
                    BreadGraceWardInstance.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(self.gameObject);
                }
            }
            else
            {
                Object.Destroy(BreadGraceWardInstance);
                BreadGraceWardInstance = null;
            }
            orig(self);
        }

        public void OnDisable()
        {
            if (BreadGraceWardInstance)
            {
                Object.Destroy(BreadGraceWardInstance);
                BreadGraceWardInstance = null;
            }
        }
    }
}