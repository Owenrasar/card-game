using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public GameObject slashParticle;
    public GameObject looseSlashParticle;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void playSlash(int size, int dir, bool isLost)
    {
        Debug.Log("dir is: " + dir);
        GameObject particle;
        if (isLost)
        {
            particle = Instantiate(looseSlashParticle, transform.position, Quaternion.identity);
        }
        else
        {
            particle = Instantiate(slashParticle, transform.position, Quaternion.identity);
        }
        particle.GetComponent<ParticleSystem>().Play();
        var main = particle.GetComponent<ParticleSystem>().main;
        main.startRotationX = 90*dir;
        main.startRotationY = 90;
        main.startRotationZ = 0;
        if (dir < 0)
        {
            main.flipRotation = 1;
        }
        particle.transform.position = new Vector3(transform.position.x, transform.position.y, -3);

        if (size > 1)
        {
            particle.transform.localScale = new Vector3(size+0.5f,size-1.5f,1);
        } else {
            particle.transform.localScale = new Vector3(size+0.5f,1f,1);
        }
    }
}
