using Settings;
using UnityEngine;

public class LoadModEntities : MonoBehaviour
{
    [SerializeField] private GameObject ExpeditionUIPrefab;
    void Start()
    {
        Instantiate(ExpeditionUIPrefab);
    }
}
