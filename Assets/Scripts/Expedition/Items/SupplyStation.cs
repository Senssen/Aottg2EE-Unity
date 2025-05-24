using System.Collections;
using System.Collections.Generic;
using Characters;
using Photon.Pun.UtilityScripts;
using UnityEngine;

public class SupplyStation : MonoBehaviour
{
    private float timer = 0f;
    private readonly float TIMER_MAX = 1f;
    void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out Human _human)) {
            if (_human.NeedRefill(true) && Mathf.Abs(_human.GetVelocity().magnitude) < 0.1f) {
                timer += Time.deltaTime;

                if (timer >= TIMER_MAX) {
                    _human.Refill();
                    timer = 0f;
                }
            } else {
                timer = 0f;
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Human _)) {
            timer = 0f;
        } 
    }
}