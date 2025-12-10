using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthManager : MonoBehaviour
{
    public GameObject notifyPrefab;

    public Animator anim;

    public Timeline linkedTimeline;
    public UITimeline linkedUITimeline;

    public float HP = 20;
    public float maxHP = 20;
    public float stagger = 10;
    public float maxStagger = 10;

    private float damageMod = 1;

    public Block activeBlock;

    public Dodge activeDodge;
    // Start is called before the first frame update

    public Image healthBar;
    public Transform staggerMarker;
    public Transform staggerStartPos;
    public Transform staggerEndPos;

    public Renderer staggerGlowRenderer;
    public Color emissionColor = new Color(1f, 0.5f, 0f);
    public float minIntensity = 1f;
    public float maxIntensity = 5f;
    private Material targetMaterial;

    public bool dead = false; 

    public AudioSource hitSound;
    public AudioSource blockSound;
    public AudioSource dodgeSound;
    public AudioSource staggerSound;

    void Start(){
        if (staggerGlowRenderer != null)
        {
            targetMaterial = staggerGlowRenderer.material;
            targetMaterial.EnableKeyword("_EMISSION");
        }
    }
    public void EndTurn()
    {
        damageMod = 1;
        if (stagger <= 0) {
            stagger = maxStagger;
            anim.SetTrigger("UnStaggered");
        } 
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (healthBar != null)
        {
            float targetFill = HP / maxHP;
            healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, targetFill, Time.deltaTime * 5f);
        }

        if (staggerMarker != null && staggerStartPos != null && staggerEndPos != null)
        {
            float ratio = Mathf.Clamp01(stagger / maxStagger);
            Vector3 targetPos = Vector3.Lerp(staggerEndPos.position, staggerStartPos.position, ratio);
            
            staggerMarker.position = Vector3.Lerp(staggerMarker.position, targetPos, Time.deltaTime * 10f);
            if (targetMaterial != null)
            {
                float currentIntensity = Mathf.Lerp(maxIntensity, minIntensity, ratio);
                targetMaterial.SetColor("_EmissionColor", emissionColor * currentIntensity);
                if (stagger <= 0){
                    targetMaterial.SetColor("_EmissionColor", emissionColor * currentIntensity*2);
                }
            }
        }
    }

    public void Hit(Attack atk)
    {
        int baseDamage = atk.value;
        if (activeBlock) //block-attack clash
        {
            bool blocked = ((Block)activeBlock).blockHit(atk,this);
            if (blocked) blockSound.Play();
        }
        else if (activeDodge)//dodge-attack clash
        {
            if (baseDamage <= activeDodge.value)
            {
                dodgeSound.Play();
                GameObject n = Instantiate(notifyPrefab);
                n.GetComponent<Notify>().DisplayNotify("Dodged", this.gameObject.transform.position, Color.yellow, 3f);
                return;
            }
            else
            {

                //staggering blow
                this.takeDamage(baseDamage);
                this.takeStagger(baseDamage);
                activeDodge.ClashLose();
                atk.ClashWin();

                activeDodge = null;
                
            }
        }
        else
        {
            //normal hit
            this.takeDamage(baseDamage);
        }
    }

    public void takeDamage(int damage){
        if (dead) return;
        HP -= damage * damageMod;
        GameObject n = Instantiate(notifyPrefab);
        hitSound.pitch = Random.Range(0.9f, 1.1f);
        hitSound.Play();
        n.GetComponent<Notify>().DisplayNotify((damage * damageMod).ToString(), this.gameObject.transform.position, Color.red, 3f);
        if (HP <= 0){
            staggerSound.Play();
            linkedTimeline.Kill();
            linkedUITimeline.Kill();
            StartCoroutine(TriggerDeathWithDelay(0.09f));
        }
    }

    public void takeStagger(int damage){
        if (dead) return;
        stagger -= damage;
        GameObject n = Instantiate(notifyPrefab);
        n.GetComponent<Notify>().DisplayNotify((damage).ToString(), this.gameObject.transform.position, new Color(1.0f, 0.6470588f, 0f, 1.0f),2.5f);
        if (stagger <= 0 && damageMod != 2){
            staggerSound.Play();
            n = Instantiate(notifyPrefab);
            n.GetComponent<Notify>().DisplayNotify(("STAGGERED").ToString(), this.gameObject.transform.position, new Color(1.0f, 0.6470588f, 0f, 1.0f), 5f);
            damageMod = 2;
            linkedTimeline.Stagger();
            linkedUITimeline.Stagger();
            gameObject.GetComponent<EffectManager>().playStagger(gameObject);
            StartCoroutine(TriggerStaggerWithDelay(0.1f));
        }

         
    }

    private IEnumerator TriggerStaggerWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (HP > 0) {
            anim.SetTrigger("Staggered");
        }
    }

    private IEnumerator TriggerDeathWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!dead){
            anim.SetTrigger("Dead");
            dead = true;
        }
    }
}
