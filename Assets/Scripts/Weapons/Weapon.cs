using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting.Dependencies.NCalc;
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
    // time that the weapon can be continuously fired for
    public float fireTime;
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
        // Get spawn position and rotation to spawn projectile
        GameObject clone = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation()); 
        // Set projectile Scale
        clone.transform.localScale = weaponStats.projectileStats.size;
        // Set stats of projectile and provide player velocity
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats, velocity); 
        // Play firing audio
        PlaySound();
    }

    
    public virtual void EnemyFire()
    {

        //Get spawn position and spawn projectile object
        GameObject clone = Instantiate(projectile, GetSpawnPos(), GetSpawnRotation()); 
        // set stats of projectile
        clone.GetComponent<Projectile>().Launch(weaponStats.projectileStats); 
        PlaySound();
    }

    // Function runs when holding down the weapon key 
    // Intended for special weapons with hold features
    public virtual void Hold(){
        // for base weapon do nothing
    }

    // Function for when the weapon key is released
    // Intended for special weapons with hold features
    public virtual void Release(){
        // for base weapon do nothing
    }

    // function to stop weapon from firing
    public virtual void StopFiring(){
        // for base weapon do nothing 
    }

    public virtual Vector3 GetSpawnPos()
    {
        // Default spawn position is 8 units in front of the weapon
        return gameObject.transform.position + gameObject.transform.forward * 8;
    }

    public virtual Quaternion GetSpawnRotation()
    {
        // Default spawn rotation is the rotation of the weapon
        return gameObject.transform.rotation;
    public virtual void EnemyPatternFire(){
        
    }

    public void UpdateStats(WeaponStats newStats){
        weaponStats = newStats;
    }

    public WeaponStats GetStats(){
        return weaponStats;
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
