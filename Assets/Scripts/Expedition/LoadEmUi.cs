using UnityEngine.SceneManagement;
using UnityEngine;

public class LoadEmUi : MonoBehaviour
{
    [SerializeField] private GameObject EMUIPrefab;
    void Start() { 
        Instantiate(EMUIPrefab); 
    }
}
