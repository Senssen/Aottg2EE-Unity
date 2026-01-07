using Photon.Pun;
using Projectiles;
using UnityEngine;

public class AcousticFlareParticle : MonoBehaviourPun
{
    private float cooldown = AcousticFlareController._maxLife;

    private void FixedUpdate()
    {
        if (photonView.AmOwner)
        {    
            if (cooldown > 0)
                cooldown -= Time.fixedDeltaTime;
            else
                PhotonNetwork.Destroy(gameObject);
        }
    }
}