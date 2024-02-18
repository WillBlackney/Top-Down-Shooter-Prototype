using MoreMountains.TopDownEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponToggler : MonoBehaviour
{
    [SerializeField] private GameObject rangedWeapon;
    [SerializeField] private GameObject meleeWeapon;
    private void Awake()
    {
        meleeWeapon.SetActive(false);
        meleeWeapon.SetActive(true);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)) 
        {
            meleeWeapon.SetActive(false);
            rangedWeapon.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            meleeWeapon.SetActive(true);
            rangedWeapon.SetActive(false);
        }
    }
}
