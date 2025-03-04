using UnityEngine;

public class TrashGenerator : MonoBehaviour
{
    private int trashDensity;
    private int collected;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trashDensity = GameManager.Instance.trashDensity;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        
    }
}
