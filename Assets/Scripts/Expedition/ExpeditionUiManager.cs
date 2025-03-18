using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpeditionUiManager : MonoBehaviour
{
    [SerializeField]
    private GameObject CanvasObj;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CloseEmMenu()
    {
        CanvasObj.SetActive(false);
    }

    public void OpenEmMenu()
    {
        CanvasObj.SetActive(true);
    }
}