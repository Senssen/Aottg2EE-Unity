using Characters;
using Photon.Pun;
using UnityEngine;

public class CollectGas : MonoBehaviour
{
    [SerializeField] 
    private RoleItems.SupplyItem _itemType;
    public bool DroppedByDead = false;

    private float timer = 0f;
    private readonly float delay = 5f * 60f; // 5 minutes in seconds

    private void FixedUpdate()
    {
        timer += Time.fixedDeltaTime;
        if (timer >= delay)
        {
            // Code to execute after 5 minutes
            Destroy(gameObject);

            timer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.gameObject.GetPhotonView().IsMine)
            {
                GameObject HumanObj = PhotonExtensions.GetMyHuman();
                Human human = HumanObj.GetComponent<Human>();

                if (_itemType == RoleItems.SupplyItem.Gas)
                {
                    if (human.Stats.CurrentGas == human.Stats.MaxGas)
                        return;

                    if (DroppedByDead)
                    {
                        human.Stats.CurrentGas = human.Stats.MaxGas * 0.3f;
                    }
                    else
                    {
                        human.Stats.CurrentGas = human.Stats.MaxGas;
                    }
                }
                else if (_itemType == RoleItems.SupplyItem.Weapon)
                {
                    BaseUseable Weapon = human.Weapon;
                    if (Weapon is BladeWeapon)
                    {
                        var weapon = (BladeWeapon)Weapon;
                        if (weapon.BladesLeft == weapon.MaxBlades)
                            return;
                        
                        if (weapon.BladesLeft < weapon.MaxBlades)
                            weapon.BladesLeft++;
                    }
                    else if (Weapon is AHSSWeapon || Weapon is APGWeapon)
                    {
                        var weaponAHSS = (AHSSWeapon)Weapon;
                        if (weaponAHSS.AmmoLeft == weaponAHSS.MaxAmmo)
                            return;

                        if (weaponAHSS.AmmoLeft < weaponAHSS.MaxAmmo)
                            weaponAHSS.AmmoLeft++;
                    }
                    else if (Weapon is ThunderspearWeapon)
                    {
                        var weaponTS = (ThunderspearWeapon)Weapon;
                        if (weaponTS.AmmoLeft == weaponTS.MaxAmmo)
                            return;

                        if (weaponTS.AmmoLeft < weaponTS.MaxAmmo)
                            weaponTS.AmmoLeft++;
                    }
                }
            }
            
            Destroy(gameObject);
        }
    }
}