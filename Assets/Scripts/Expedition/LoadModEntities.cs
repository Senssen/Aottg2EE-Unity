using Settings;
using UnityEngine;

public class LoadModEntities : MonoBehaviour
{
    [SerializeField] private GameObject ExpeditionUIPrefab;
    [SerializeField] private GameObject DynamicWeatherPrefab;
    void Start()
    {
        Instantiate(ExpeditionUIPrefab);
        Instantiate(DynamicWeatherPrefab);
    }
}
