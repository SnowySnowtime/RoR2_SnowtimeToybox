using EntityStates;
using HG;
using RoR2;
using RoR2.Items;
using SnowtimeToybox;
using System.Linq;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem.PlaybackState;

namespace EntityStates.SnowtimeToybox_FriendlyTurret
{
    public class FireBreadBeam : BaseState
    {
        public static float baseDuration = 5f;

        public static float durationMultiplierPerTier = 0.5f;

        public static float healCoefficient = 5f;

        public static float healMultiplierPerTier = 0.5f;

        public static GameObject healBeamObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/Skills/BreadFortuneBeamL.prefab");
        public static GameObject healBeamObjectR = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/FriendlyTurrets/FriendlyTurretTestIngame/Bread/Skills/BreadFortuneBeamR.prefab");
        public static GameObject healBeamPrefab = healBeamObject;
        public static GameObject healBeamPrefabR = healBeamObjectR;

        public HurtBox target;
        public Transform modelTransform;

        private HealBeamController healBeamController;

        private float duration;

        private float healingBonus;

        private float durationBonus;

        private float lineWidthRefVelocity;

        public override void OnEnter()
        {
            base.OnEnter();
            durationBonus = 1f;
            healingBonus = 1f;
            duration = baseDuration;
            float healRate = healCoefficient * damageStat * healingBonus / duration + healingBonus;
            Ray aimRay = GetAimRay();

            /// straight up lasering it
            modelTransform = GetModelTransform();
            if (!modelTransform)
            {
                return;
            }
            ChildLocator component = modelTransform.GetComponent<ChildLocator>();
            if (!component)
            {
                return;
            }
            Transform transformL = component.FindChild("Muzzle_L");
            Transform transformR = component.FindChild("Muzzle_R");

            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            bullseyeSearch.teamMaskFilter = TeamMask.none;
            if ((bool)base.teamComponent)
            {
                bullseyeSearch.teamMaskFilter.AddTeam(base.teamComponent.teamIndex);
            }
            bullseyeSearch.filterByLoS = false;
            bullseyeSearch.maxDistanceFilter = 50f;
            bullseyeSearch.maxAngleFilter = 180f;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
            bullseyeSearch.RefreshCandidates();
            bullseyeSearch.FilterOutGameObject(base.gameObject);
            target = bullseyeSearch.GetResults().FirstOrDefault();
            if (NetworkServer.active && (bool)transform && (bool)target)
            {
                GameObject gameObjectWaow = Object.Instantiate(healBeamPrefab, transformL);
                healBeamController = gameObjectWaow.GetComponent<HealBeamController>();
                healBeamController.healRate = healRate;
                healBeamController.target = target;
                healBeamController.ownership.ownerObject = base.gameObject;
                NetworkServer.Spawn(gameObjectWaow);
                TransferDebuffOnHitUtils.FireProjectile(base.characterBody, target.healthComponent.body.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if ((base.fixedAge >= duration || !target) && base.isAuthority)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            //PlayCrossfade("Gesture", "Empty", 0.2f);
            if ((bool)healBeamController)
            {
                healBeamController.BreakServer();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            HurtBoxReference.FromHurtBox(target).Write(writer);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            HurtBoxReference hurtBoxReference = default(HurtBoxReference);
            hurtBoxReference.Read(reader);
            target = hurtBoxReference.ResolveGameObject()?.GetComponent<HurtBox>();
        }
    }
}