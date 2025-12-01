using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public GameObject slashParticle;
    public GameObject looseSlashParticle;

    public GameObject hitParticle;

    public GameObject dashGhost;
    public Material dashGhostMaterial;
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
        main.startRotationX = 90 * dir;
        main.startRotationY = 90;
        main.startRotationZ = 0;
        if (dir < 0)
        {
            main.flipRotation = 1;
        }
        particle.transform.position = new Vector3(transform.position.x, transform.position.y, -3);


        particle.transform.localScale = new Vector3(size + 0.5f, size / 2f, 1);
    }

    public void playHit(GameObject target)
    {
        GameObject particle;

        particle = Instantiate(hitParticle, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>().Play();
        var main = particle.GetComponent<ParticleSystem>().main;
        particle.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, -3);
        particle.transform.SetParent(target.transform);
    }
    
    // ---------------------------------------------------------
    // 1. THE SPAWNER
    // call this StartCoroutine(SpawnBodyGhosts(..., ...))
    // ---------------------------------------------------------
    public IEnumerator GhostTrail(float dashDuration, int numberOfGhosts, float fadeDuration)
    {
        Debug.Log("ghosting!");
        GameObject modelToCopy = dashGhost;
        Material ghostMaterial = dashGhostMaterial;
        float timePerSpawn = dashDuration / numberOfGhosts;
        float time = 0;

        while (time < dashDuration)
        {
            // 1. Make a full copy of the model hierarchy at the current position/rotation
            GameObject ghostObj = Instantiate(modelToCopy, modelToCopy.transform.position, modelToCopy.transform.rotation);
            
            // 2. Cleanup: Remove scripts or Animators from the ghost so it doesn't try to move/attack
            // (We destroy the Animator so the ghost freezes in place or resets to bind pose, depending on settings)
            var anim = ghostObj.GetComponent<Animator>();
            if (anim) Destroy(anim);

            // 3. Find all renderers in the copy (body, armor, weapon)
            Renderer[] ghostRenderers = ghostObj.GetComponentsInChildren<Renderer>();

            // 4. Create a unique material instance for this specific ghost so we can fade it
            Material instanceMat = new Material(ghostMaterial);

            // 5. Apply the ghost material to all parts of the copy
            foreach (Renderer r in ghostRenderers)
            {
                r.material = instanceMat;
            }

            // 6. Start fading this specific clone
            StartCoroutine(FadeAndDestroy(ghostObj, instanceMat, fadeDuration));

            // Wait for next spawn
            time += timePerSpawn;
            yield return new WaitForSeconds(timePerSpawn);
        }
    }

    // ---------------------------------------------------------
    // 2. THE FADER
    // ---------------------------------------------------------
    private IEnumerator FadeAndDestroy(GameObject ghostObj, Material ghostMat, float duration)
    {
        float time = 0;
        Color startColor = ghostMat.color;

        while (time < duration)
        {
            // Fade Alpha from 1 to 0
            float alpha = Mathf.Lerp(1f, 0f, time / duration);
            ghostMat.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            time += Time.deltaTime;
            yield return null;
        }

        // Destroy the copied object
        Destroy(ghostObj);
    }
}
