using System;
using RoR2;
using RoR2.EntityLogic;
using SnowtimeToybox;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using On.EntityStates.AffixVoid;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace SnowtimeToybox.FriendlyTurretChecks
{
    public class FriendlyTurretOverlayManager : MonoBehaviour
    {
        private TemporaryOverlayInstance temporaryOverlay;
        public List<TemporaryOverlayInstance> Overlay = [];
        public RoR2.CharacterBody Body;
        
        public void Start()
        {
            
        }

        public void FixedUpdate()
        {
            foreach (TemporaryOverlayInstance overlayInstance in Overlay.ToList())
            {
                switch (overlayInstance.originalMaterial.name)
                {
                    case "matBreadFortune":
                        if (!Body.HasBuff(SnowtimeToyboxMod.BreadTurretBuffFortune))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;
                    
                    case "borboturretdebuffoverlay":
                        if (!Body.HasBuff(SnowtimeToyboxMod.BorboTurretDebuff))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;
                    
                    case "acanthidebuffoverlay":
                        if (!Body.HasBuff(SnowtimeToyboxMod.AcanthiTurretDebuff))
                        {
                            Overlay.Remove(overlayInstance);
                            overlayInstance.Destroy();
                        }
                        break;
                }
            }
            if (Overlay.Count == 0)
            {
                Destroy(this);
                return;
            }
        }
    }
}