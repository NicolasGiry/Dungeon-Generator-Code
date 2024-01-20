using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class DetectWall : MonoBehaviour
{
    public TorchManager torchManager;
    public bool onWall = false;
    public Light light;

    private void Awake()
    {
        torchManager = GameObject.FindGameObjectWithTag("TorchManager").GetComponent<TorchManager>();
        //light = GetComponent<Light>();
    }

    void Start()
    {
        CheckWall();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckWall()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(-Vector3.forward), out hit, 1f))
        {
            if (!hit.transform.gameObject.CompareTag("Tile"))
            {
                Destroy(gameObject);
            } else
            {
                onWall = true;
                light.color = torchManager.torchColor;
            }
        } else
        {
            Destroy(gameObject);
        }
    }

    
}
