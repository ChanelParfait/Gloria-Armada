using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct WeaponStats{
    // how frequently the weapon can fire
    public float fireInterval;
    // time taken to reload the weapon
    public float reloadTime; 
    // total ammo that can be stored at a time
    public int maxAmmo;
    public ProjectileStats projectileStats;

}
public enum WeaponCategories {Primary, Special, Additional};  
public class Weapon : MonoBehaviour
{
    // Base Weapon Class 
    [SerializeReference] protected GameObject projectile; 
    [SerializeReference] protected AudioClip fireSound;
    [SerializeField] protected WeaponStats weaponStats;
    public WeaponCategories weaponCategory;  
    public bool isPlayerWeapon = false;
    public bool isPatternFire = false;
    public bool canFire = true;
    protected AudioSource audioSource;
    public Vector3 spawnPosition;

    protected Perspective currentPerspective = Perspective.Side_On;
    
    // Events 
    public static UnityAction<float> OnAmmoChange;

    void OnEnable(){
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    void OnDisable(){
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    void UpdatePerspective(int _pers){
        currentPerspective = (Perspective)_pers;
    }


    // Start is called before the first frame  update
    void Start()
    {
        LevelManager lm = GameObject.Find("LevelManager").GetComponent<LevelManager>();
        if (lm != null){
            currentPerspective = lm.currentPerspective;
        }
        else {
            Debug.Log("Level Manager not found" + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public WeaponStats GetWeaponStats(){
        return weaponStats;
    }

    public ProjectileStats GetProjectileStats(){
        return weaponStats.projectileStats;
    }

    public virtual void SetupWeapon(){
        audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.clip = fireSound;
    }

    public virtual void Fire(Vector3 velocity){
        //Debug.Log("Fire Base Weapon");
        // Get spawn position and spawn projectile object
        GameObject clone = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation()); 
        clone.transform.localScale = weaponStats.projectileStats.size;
        
        
        //GameObject clone = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation); 
        // set stats of projectile
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats, velocity); 
        PlaySound();
    }

    
    public virtual void EnemyFire()
    {

        //Get spawn position and spawn projectile object
        GameObject clone = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation()); 
        
        //GameObject clone = Instantiate(projectile, gameObject.transform.position, gameObject.transform.rotation); 
        // set stats of projectile
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats); 
        PlaySound();
    }

    public virtual void EnemyPatternFire(){
        
    }

    public void UpdateStats(WeaponStats newStats){
        weaponStats = newStats;
    }

    public WeaponStats GetStats(){
        return weaponStats;
    }

    public virtual Vector3 GetSpawnPos()
    {
        // return the default spawn position
        return gameObject.transform.position + gameObject.transform.forward * 8;
    }

    public virtual Quaternion GetSpawnRotation()
    {
        // return the default spawn rotation
        return gameObject.transform.rotation;
    }
    
    public virtual void PlaySound()
    {
        audioSource.volume = Random.Range(0.8f, 1.2f);
        audioSource.pitch = Random.Range(0.8f, 1.2f);
        audioSource.Play();
    }

    public GameObject GetProjectile(){
        return projectile; 
    }

    public void SetProjectile(GameObject obj){
        projectile = obj;
    }


    public void SetFireSound(AudioClip clip){
        fireSound = clip;
    }
    
    public AudioClip GetFireSound(){
        return fireSound;
    }
}
