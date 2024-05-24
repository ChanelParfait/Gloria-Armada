using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class EnemyBase : Actor
{
    
    // Events
    public static UnityAction<EnemyBase> OnEnemyDeath;
    protected EnemyWeaponManager weaponManager; 
 
    [SerializeField] protected Rigidbody rb;

    // How much the score increases on enemy death
    public int scoreValue = 10; 
    public bool fire = false; 

    [SerializeField] private bool onFire = false;

    private Coroutine burnCoroutine;
    [SerializeField] private GameObject fireEffectPrefab; // Reference to the fire particle effect prefab

    private GameObject currentFireEffect;

    protected virtual void Awake(){
        LoadFireEffectPrefab();
    }

    // Start is called before the first frame update
    override protected void Start()
    {
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        base.Start();
    }


     void Update(){

        /*if(fire){
            Fire();
            fire = false;
        }*/
     }

    protected void Setup(){
        
    }



    public virtual void Fire(){
        // Fire a Weapon
        if (weaponManager != null)
        {
            weaponManager.FireActiveWeapon(); 
        }
        
    }

    protected override void Die(){ 
        //Debug.Log("Enemy Died: " + name + "UniqueID: " + GetInstanceID());  
        OnEnemyDeath?.Invoke(this);
        base.Die();
    }

    private void LoadFireEffectPrefab()
    {
        fireEffectPrefab = Resources.Load<GameObject>("FireDamagePrefab");
        if (fireEffectPrefab == null)
        {
            Debug.LogError("FireEffectPrefab not found in Resources folder");
        }
    }

    public void SetOnFire(bool onFire = true, float burnDamage = 1, float burnDuration = 5, float burnTime = 1){
        //If not already on fire, set on fire
        if (!this.onFire && onFire){
            this.onFire = onFire;
            if (fireEffectPrefab != null && currentFireEffect == null)
            {
                currentFireEffect = Instantiate(fireEffectPrefab, transform);
            }
            burnCoroutine = StartCoroutine(BurnDamageOverTime(1, 5, 1));
        }
        //Possible to forcefully extuinguish fire before it reaches duration (not used at the moment)
        else if (this.onFire && !onFire){
            this.onFire = onFire;
            if (burnCoroutine != null){
                StopCoroutine(burnCoroutine);
            }
            if (currentFireEffect != null)
            {
                Destroy(currentFireEffect);
            }
        }
        //Re-applying burn damage with new values (enemy gets hit with burn twice in a row)
        else if (this.onFire && onFire){
            //Stop the existing coroutine and start a new one with the new values
            if (burnCoroutine != null){
                StopCoroutine(burnCoroutine);
            }
            burnCoroutine = StartCoroutine(BurnDamageOverTime(burnDamage, burnDuration, burnTime));
        }
        // If not on fire and not setting on fire, do nothing
    }

    public IEnumerator BurnDamageOverTime(float burnDamage, float burnDuration, float burnTime)
    {
        float elapsed = 0f;
        while (elapsed < burnDuration)
        {
            TakeDamage(burnDamage);
            Debug.Log("Burn damage applied. Current health: " + CurrentHealth);
            elapsed += burnTime;
            yield return new WaitForSeconds(burnTime);    
        }
        if (currentFireEffect != null)
        {
                Destroy(currentFireEffect);
        }
        onFire = false;
    }
}
