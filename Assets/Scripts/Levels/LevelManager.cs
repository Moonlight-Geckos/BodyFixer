using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform target;

    public GameObject particleWellDone1;
    public GameObject particleWellDone2;

    public GameObject particleBrilliant1;
    public GameObject particleBrilliant2;

    public GameObject particle;
    public Material material;
    public Texture2D[] textures = new Texture2D[3];
    void Start()
    {
        EventPool.finishedFixing.AddListener(ActivateEnd);
    }
    public void ActivateEnd(int percent)
    {
        if (percent > 75)
        {
            material.mainTexture = textures[0];
            particle.SetActive(true);
            particleBrilliant1.SetActive(true);
            particleBrilliant2.SetActive(true);

        }
        else if (percent >= 25)
        {
            material.mainTexture = textures[1];
            particle.SetActive(true);
            particleWellDone1.SetActive(true);
            particleWellDone2.SetActive(true);
        }
        else
        {
            material.mainTexture = textures[2];
            particle.SetActive(true);
        }
    }
    
}
