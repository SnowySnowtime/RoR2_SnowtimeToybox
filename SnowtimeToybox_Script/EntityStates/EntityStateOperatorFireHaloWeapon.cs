using EntityStates;
using IL.RoR2.UI;
using R2API;
using RoR2.ContentManagement;
using RoR2;
using RoR2.Skills;
using SnowtimeToybox;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SnowtimeToybox_FireHaloWeapon
{
    public class FirePlasmaRifle : GenericBulletBaseState
    {
        public new string muzzleName = "NanoPistolOrigin";
        public static GameObject MuzzleFlashObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleMuzzleFlashVFX.prefab");
        public static GameObject TracerObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/TracerPlasmaRifle.prefab");
        public static GameObject HitObject = SnowtimeToyboxMod._stcharacterAssetBundle.LoadAsset<GameObject>(@"Assets/SnowtimeMod/Assets/Characters/DroneTech/PlasmaRifle/PlasmaRifleImpactVFX.prefab");
        public new GameObject muzzleFlashPrefab = MuzzleFlashObject;
        public new GameObject tracerEffectPrefab = TracerObject;
        public new GameObject hitEffectPrefab = HitObject;
        public new string fireSoundString = "Play_Plasma_Rifle_Fire";
        public new float procCoefficient = 0.7f;
        public new float damageCoefficient = 0.6f;
        public new float bulletRadius = 1.25f;
        public new float maxDistance = 128f;
        public new float baseDuration = 0.04f;
        public float bounceDamageCoefficient = 0.4f;
        public float RecoilY = 0.8f;
        public float RecoilX = 0.2f;
        public float orbSpeed = 100f;
        public bool shouldBounce = true;
        public new bool useSmartCollision = true;
        public new Transform muzzleTransform;
        public float additionalStocks = 26;

        private DroneTechController controller;
        private GameObject currentDrone;

        public override void OnEnter()
        {
            this.controller = this.GetComponent<DroneTechController>();
            base.OnEnter();
            this.duration = this.baseDuration / (this.attackSpeedStat / 4f);
            additionalStocks = 23f + (this.attackSpeedStat * 3.5f);
            this.skillLocator.primary.maxStock = (int)additionalStocks;
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Skill;

        public override void PlayFireAnimation()
        {
            Animator modelAnimator = this.GetModelAnimator();
            string layerName = (!(bool)(Object)modelAnimator ? 0 : (modelAnimator.GetBool("isCarried") ? 1 : (modelAnimator.GetBool("inLeap") ? 1 : 0))) != 0 ? "FullBody, Override" : "Gesture, Override";
            this.PlayAnimation(layerName, "FireNanoPistol");
        }

        public override void DoFireEffects()
        {
            string soundString = this.fireSoundString;
            int num = (int)Util.PlaySound(soundString, this.gameObject);
            EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, this.gameObject, this.muzzleName, false);
        }

        public override void FireBullet(Ray aimRay)
        {
            TrajectoryAimAssist.ApplyTrajectoryAimAssist(ref aimRay, this.maxDistance, this.gameObject);
            if ((bool)(Object)this.controller && (bool)(Object)this.controller.CurrentDrone?.characterBody)
                this.currentDrone = this.controller.CurrentDrone.characterBody.gameObject;
            this.recoilAmplitudeY = RecoilY;
            this.recoilAmplitudeX = RecoilX;

            base.FireBullet(aimRay);
        }

        public override void ModifyBullet(BulletAttack bulletAttack)
        {
            base.ModifyBullet(bulletAttack);
            bulletAttack.damage = this.damageCoefficient * this.characterBody.damage;
            bulletAttack.damageType = new DamageTypeCombo((DamageTypeCombo)DamageType.IgniteOnHit, DamageTypeExtended.Generic, DamageSource.Primary);
            DamageAPI.AddModdedDamageType(bulletAttack, SnowtimeToyboxMod.HaloRicochetOnHit);
            bulletAttack.muzzleName = this.muzzleName;
            bulletAttack.tracerEffectPrefab = this.tracerEffectPrefab;
            bulletAttack.hitEffectPrefab = this.hitEffectPrefab;
            bulletAttack.weapon = this.gameObject;
            bulletAttack.maxDistance = this.maxDistance;
            bulletAttack.filterCallback = new BulletAttack.FilterCallback(this.IgnoreAlliesAndCurrentDrone);
            bulletAttack.falloffModel = BulletAttack.FalloffModel.DefaultBullet;
        }

        private bool IgnoreAlliesAndCurrentDrone(BulletAttack bullet, ref BulletAttack.BulletHit hit)
        {
            HurtBox component = hit.collider.GetComponent<HurtBox>();
            if ((bool)(Object)component && (bool)(Object)component.healthComponent && ((Object)component.healthComponent.gameObject == (Object)bullet.weapon || (Object)component.healthComponent.gameObject == (Object)this.currentDrone) || (Object)hit.entityObject == (Object)this.gameObject || (Object)hit.entityObject == (Object)bullet.weapon)
                return false;
            return !(bool)(Object)component || !(bool)(Object)component.healthComponent || FriendlyFireManager.ShouldDirectHitProceed(component.healthComponent, this.teamComponent.teamIndex);
        }
    }
}