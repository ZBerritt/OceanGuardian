using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine.SceneManagement;

public class FacilityManager : MonoBehaviour
{
    public static FacilityManager Instance;

    [SerializeField] private Canvas canvas;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        canvas.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleWorkshopMenu()
    {
        canvas.enabled = true;
        return; 
    }
}
