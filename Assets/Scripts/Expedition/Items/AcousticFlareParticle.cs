using Photon.Pun;
using UnityEngine;

public class AcousticFlareParticle : MonoBehaviourPun
{
    private float cooldown = 180f;

    private void FixedUpdate()
    {
        if (cooldown > 0) 
        {
            cooldown -= Time.fixedDeltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}