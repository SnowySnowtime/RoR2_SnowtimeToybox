using System.Linq;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SnowtimeToybox.Components
{
    [RequireComponent(typeof(InputBankTest))]
    [RequireComponent(typeof(CharacterBody))]
    [RequireComponent(typeof(TeamComponent))]
    public class TurretlingMissileTracker : MonoBehaviour, ITargetTracker
    {
        public GameObject trackingPrefab;

        public float maxTrackingDistance = 40f;

        public float maxTrackingAngle = 20f;

        public float trackerUpdateFrequency = 10f;

        private HurtBox trackingTarget;

        private CharacterBody characterBody;

        private TeamComponent teamComponent;

        private InputBankTest inputBank;

        private float trackerUpdateStopwatch;

        private Indicator indicator;

        private readonly BullseyeSearch search = new BullseyeSearch();

        private void Awake()
        {
            if (trackingPrefab == null)
            {
                trackingPrefab = Addressables.LoadAssetAsync<GameObject>(RoR2BepInExPack.GameAssetPaths.Version_1_39_0.RoR2_Base_Engi.EngiMissileTrackingIndicator_prefab).WaitForCompletion();
            }
            indicator = new Indicator(base.gameObject, trackingPrefab);
        }

        private void Start()
        {
            characterBody = GetComponent<CharacterBody>();
            inputBank = GetComponent<InputBankTest>();
            teamComponent = GetComponent<TeamComponent>();
        }

        public HurtBox GetTrackingTarget()
        {
            return trackingTarget;
        }

        private void OnEnable()
        {
            indicator.active = true;
        }

        private void OnDisable()
        {
            indicator.active = false;
        }

        private void FixedUpdate()
        {
            MyFixedUpdate(Time.fixedDeltaTime);
        }

        private void MyFixedUpdate(float deltaTime)
        {
            trackerUpdateStopwatch += deltaTime;
            if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
            {
                trackerUpdateStopwatch -= 1f / trackerUpdateFrequency;
                _ = trackingTarget;
                Ray aimRay = new Ray(inputBank.aimOrigin, inputBank.aimDirection);
                SearchForTarget(aimRay);
                indicator.targetTransform = (trackingTarget ? trackingTarget.transform : null);
            }
        }

        private void SearchForTarget(Ray aimRay)
        {
            search.teamMaskFilter = TeamMask.all;
            search.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            search.filterByLoS = true;
            search.searchOrigin = aimRay.origin;
            search.searchDirection = aimRay.direction;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.maxDistanceFilter = maxTrackingDistance;
            search.maxAngleFilter = maxTrackingAngle;
            search.RefreshCandidates();
            search.FilterOutGameObject(base.gameObject);
            trackingTarget = search.GetResults().FirstOrDefault();
        }
    }

}