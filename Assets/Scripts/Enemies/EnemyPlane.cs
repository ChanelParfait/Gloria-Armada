using System.Collections;
using UnityEngine;

[System.Serializable]
internal class Vec3PID
{
    public PID x;
    public PID y;
    public PID z; 

    public Vec3PID(float p, float i, float d)
    {
        x = new PID(p, i, d);
        y = new PID(p, i, d);
        z = new PID(p, i, d);
    }

    public Vector3 Solve(Vector3 target, Vector3 current)
    {
        return new Vector3(x.Solve(target.x, current.x), y.Solve(target.y, current.y), z.Solve(target.z, current.z));
    }

    public Vector3 Solve(Vector3 error)
    {
        return new Vector3(x.Solve(error.x), y.Solve(error.y), z.Solve(error.z));

    }
}

public class EnemyPlane : EnemyBase
{
    [SerializeField] private float fireInterval = 1;
    [SerializeField] private float speed = 8f;
    public float referenceSpeed = 0;
    public Vector3 moveDir;
    public Vector3 orientation;

    [SerializeField] private Vec3PID pid = new(1f, 0.01f, 22f);

    protected GameObject targetObj;
    protected Camera cam;
    [SerializeField] protected Vector3 targetOffset;
    protected float randomOffsetComponent;
    protected Vector3 targetPos;

    protected Perspective currentPerspective;

    protected float timer = 0;
    protected float radarTimer = 0;
    protected float randFireTime;
    public GameObject deathExplosion;

    bool isLeaving;


    [SerializeField] private PowerupManager powerupManager;

    protected CameraUtils camUtils;

    [SerializeField] protected GameObject deathObj;

    public override float currentSpeed
    {
        get => speed;
        set => speed = value;
    }

    public override float currentFireRate
    {
        get => fireInterval;
        set => fireInterval = value;
    }

    private void OnEnable()
    {
        LevelManager.OnPerspectiveChange += UpdatePerspective;
    }

    private void OnDisable()
    {
        LevelManager.OnPerspectiveChange -= UpdatePerspective;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        weaponManager = gameObject.GetComponent<EnemyWeaponManager>();
        powerupManager = GameObject.FindObjectOfType<PowerupManager>();
        rb = GetComponent<Rigidbody>();
        GameObject lmObj = GameObject.FindGameObjectWithTag("LevelManager");
        if (lmObj != null)
        {
            LevelManager lm = GameObject.FindGameObjectWithTag("LevelManager").GetComponent<LevelManager>();
            currentPerspective = lm.currentPerspective;
            UpdatePerspective((int)currentPerspective);
            if (targetObj == null)
            {
                targetObj = GameObject.FindGameObjectWithTag("LevelManager");
            }
        }


        cam = Camera.main;
        camUtils = FindObjectOfType<CameraUtils>();
        randomOffsetComponent = Random.Range(-0.4f, 0.4f);
        randFireTime = Random.Range(1f, 2.0f);
        _ = StartCoroutine(Initialize());
        timer = fireInterval - 1;

    }


    virtual protected IEnumerator Initialize()
    {
        yield return new WaitForSeconds(0.1f);
        if (orientation == Vector3.zero && moveDir == Vector3.zero)
        {
            yield break;
        }
        rb.AddForce(referenceSpeed * Utilities.MultiplyComponents(orientation, moveDir), ForceMode.VelocityChange);
    }

    protected virtual void UpdatePerspective(int _pers)
    {
        currentPerspective = (Perspective)_pers;
        if (rb == null)
        {
            return;
        }
        rb.MoveRotation(Quaternion.Euler(0, -90, 0));
        //Switch case on perspective and teleport plane to the axes
        switch (currentPerspective)
        {
            case Perspective.Top_Down:
                rb.MovePosition(new Vector3(transform.position.x, 0, transform.position.z));
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                rb.constraints = RigidbodyConstraints.FreezePositionY;
                if (CompareTag("EnemyBoss"))
                {
                    _ = StartCoroutine(RotateTo(true));
                }
                break;
            case Perspective.Side_On:
                rb.MovePosition(new Vector3(transform.position.x, transform.position.y, 0));
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                rb.constraints = RigidbodyConstraints.FreezePositionZ;
                if (CompareTag("EnemyBoss"))
                {
                    _ = StartCoroutine(RotateTo(false));
                }
                break;
            case Perspective.Null:
                rb.constraints = RigidbodyConstraints.None;
                break;
        }
    }

    private IEnumerator RotateTo(bool toHorizontal)
    {
        float initZ = transform.rotation.eulerAngles.z;
        float finalZ = toHorizontal ? 90 : 0;
        //Lerp from initial to final rotation over 1 second
        for (float t = 0; t < 1; t += Time.deltaTime)
        {
            Vector3 newRot = transform.rotation.eulerAngles;
            newRot.z = Mathf.Lerp(initZ, finalZ, Utilities.EaseInOut(t));
            transform.rotation = Quaternion.Euler(newRot);
            yield return null;
        }
        //transform.rotation.eulerAngles.Set(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, finalZ);
    }

    protected virtual Vector3 GetTargetOffset()
    {
        return currentPerspective switch
        {
            Perspective.Top_Down => new Vector3((camUtils.height / 2) - 30.0f, 0, camUtils.width / 2 * randomOffsetComponent),
            Perspective.Side_On => new Vector3((camUtils.width / 2) - 30.0f, camUtils.height / 2 * randomOffsetComponent, 0),
            Perspective.Null => Vector3.zero,
            _ => targetObj.transform.position,
        };
    }

    // Update is called once per frame
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer >= fireInterval * randFireTime)
        {
            randFireTime = Random.Range(0.5f, 2.0f);
            timer = 0;
            Fire();
        }
    }

    public void Flee(bool flee = true)
    {
        isLeaving = true;
    }

    void GetFleePosition()
    {
        Vector3 fleePos = transform.position + Vector3.right * 100;
        Vector3 offset = Vector3.zero;
        if (currentPerspective == Perspective.Top_Down)
        {
            offset = new Vector3(-camUtils.height * 4, 0, 4 * Random.Range(-camUtils.width, camUtils.width));
        }
        else if (currentPerspective == Perspective.Side_On)
        {
            offset = new Vector3(-camUtils.width * 4, 4 * Random.Range(0, camUtils.height), 0);
        }
        fleePos += offset;

        targetPos = fleePos;
    }

    protected virtual void FixedUpdate()
    {
        if (!isLeaving){
            targetOffset = GetTargetOffset();
            Vector3 targetObjPos = targetObj.transform.position;
            targetPos = targetObjPos + targetOffset;
        }
        else {
            GetFleePosition();
        }

        if (rb.angularVelocity.magnitude > 0.1f)
        {
            rb.useGravity = true;
        }
        else
        {
            MoveEnemy();
            radarTimer += Time.deltaTime;
            if (radarTimer > 0.2f)
            {
                AvoidGround();
                radarTimer = 0;
            }
        }
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
    }

    protected override void Die()
    {
        // Destroy Self and emit death explosion
        _ = Instantiate(deathExplosion, transform.position, Quaternion.identity);
        if (powerupManager != null)
        {
            powerupManager.SpawnPowerUp(transform.position, rb.velocity);
        }
        if (deathObj != null)
        {
            GameObject deadObj = Instantiate(deathObj, transform.position, transform.rotation);
            foreach (Rigidbody rb in deadObj.GetComponentsInChildren<Rigidbody>())
            {
                //Add force to the rigid body
                rb.AddForce(GetComponent<Rigidbody>().velocity, ForceMode.VelocityChange);
                // Translate the angular velocity of the parent by the localPosition of the child to get the correct velocity
                Vector3 angularVelocity = GetComponent<Rigidbody>().angularVelocity;
                Vector3 pointOffset = rb.transform.localPosition;
                Vector3 linearVelocityAtPoint = Vector3.Cross(angularVelocity, pointOffset);
                rb.AddForce(linearVelocityAtPoint, ForceMode.VelocityChange);
            }
        }
        base.Die();
    }

    protected virtual void MoveEnemy()
    {
        _ = targetPos - transform.position;
        //Scale the error by the screen width
        Vector3 moveDir = pid.Solve(targetPos, transform.position);
        rb.AddForce(20.0f * speed * moveDir.normalized);
    }

    private void AvoidGround()
    {
        //Raycast down to check for ground
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100.0f))
        {
            //If the distance to the ground is less than 10 units, add a force upwards
            if (hit.distance < 20.0f)
            {
                rb.AddForce((20 - hit.distance) * 20.0f * Vector3.up);
            }
        }
    }

    protected virtual void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Terrain"))
        {
            //Get the normal of the collision
            Vector3 normal = col.contacts[0].normal;
            //Get dot product of the normal and the velocity
            Rigidbody rb = GetComponent<Rigidbody>();
            float dot = Vector3.Dot(rb.velocity.normalized, normal) * rb.velocity.magnitude;
            dot = Mathf.Clamp01(dot);

            //Reduce health by a minimum of 1health, max of MaxLife based on dot
            int damage = (int)Mathf.Lerp(1, maxHealth, dot);

            TakeDamage(damage);
        }
    }

}
