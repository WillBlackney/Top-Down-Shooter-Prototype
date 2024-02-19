using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class WeaponToggler : MonoBehaviour
    {
        [SerializeField] private GameObject rangedWeaponAttachment;
        [SerializeField] private GameObject meleeWeaponAttachment;

        private SpriteRenderer rangedWeaponSR;
        private SpriteRenderer meleeWeaponSR;

        public SpriteRenderer RangedWeaponSR
        {
            get
            {
                if (!rangedWeaponSR)
                {
                    rangedWeaponSR = rangedWeaponAttachment.GetComponentInChildren<SpriteRenderer>();
                }
                return rangedWeaponSR;
            }
        }

        public SpriteRenderer MeleeWeaponSR
        {
            get
            {
                if (!meleeWeaponSR)
                {
                    meleeWeaponSR = meleeWeaponAttachment.GetComponentInChildren<SpriteRenderer>();
                }
                return meleeWeaponSR;
            }
        }

        private void Start()
        {
            RangedWeaponSR.enabled = false;
            MeleeWeaponSR.enabled = false;
            meleeWeaponAttachment.transform.position = rangedWeaponAttachment.transform.position;
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                RangedWeaponSR.enabled = true;
                MeleeWeaponSR.enabled = false;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                RangedWeaponSR.enabled = false;
                MeleeWeaponSR.enabled = true;
            }
        }
    }
}