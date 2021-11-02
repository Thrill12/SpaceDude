using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraZoom : MonoBehaviour
{
    private CinemachineVirtualCamera cam;

    public float minCamSize = 15;
    public float maxCamSize = 50f;

    private void Start()
    {
        cam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            if(cam.m_Lens.OrthographicSize > minCamSize)
            {
                cam.m_Lens.OrthographicSize--;
            }            
        }
        else if(Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if(cam.m_Lens.OrthographicSize < maxCamSize)
            {
                cam.m_Lens.OrthographicSize++;
            }          
        }
    }
}
