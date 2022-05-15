using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BaseEnemy)), RequireComponent(typeof(Rigidbody2D))]
public class SimpleEnemyMovement : MonoBehaviour
{
    [Header("Movement")]

    public float movementMinRange;
    public float movementSpeed;

    [Space(10)]

    [Header("Vision")]

    [Range(0, 360)]
    public float visionAngle;
    public float visionRadius;
    public LayerMask layersToIgnore;
    public TargetType targetType;

    [Space(10)]

    [Header("Dodging")]

    [Tooltip("X is the minimum time, Y is the maximum time")]
    public Vector2 timeToReact;
    public float dodgeSpeed;
    public float dodgeCooldown;
    private float dodgeNextFire;

    public float angleSafetyNetFromProjectiles = 5;
    public bool isInDanger;

    private Vector2 dodgeEscapeVector;
    private Projectile mostDangerousProjectile;

    private List<BaseEntity> nearbyEntities = new List<BaseEntity>();
    private List<BaseEntity> visibleEntities = new List<BaseEntity>();

    private List<Projectile> nearbyProjectiles = new List<Projectile>();
    private List<Projectile> nearbyEnemyProjectiles = new List<Projectile>();
    private List<Projectile> nearbyHarmfulProjectiles = new List<Projectile>();

    private BaseEnemy enemyComponent;
    public Transform target;
    private Rigidbody2D rb;
    private Vector2 movementDirection;
    private bool isTargetInSight;
    private Vector3 directionToTarget;
    public Vector3 targetLastKnownPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        enemyComponent = GetComponent<BaseEnemy>();
        dodgeNextFire = dodgeCooldown;
        targetLastKnownPosition = Random.insideUnitCircle * visionAngle;
    }

    // Update is called once per frame
    void Update()
    {
        FindTarget();

        if (isInDanger)
        {
            if(mostDangerousProjectile != null)
            {
                Vector2 directionToProjectile = (mostDangerousProjectile.transform.position - transform.position).normalized;
                dodgeEscapeVector = GetRandomPerpendicularDirectionFromVector(directionToProjectile);

                if (dodgeNextFire <= 0)
                {
                    DashToDodge();
                }
            }
            else
            {
                isInDanger = false;
            }                 
        }
        
        FindHarmfulProjectiles();

        if (target != null)
        {          
            movementDirection = (target.position - transform.position).normalized;
        }

        dodgeNextFire -= Time.deltaTime;
    }

    #region Vision
    public void FindVisibleTargets()
    {
        visibleEntities.Clear();

        nearbyEntities = Physics2D.OverlapCircleAll(transform.position, visionRadius).Where(x => x.GetComponent<BaseEntity>())
            .Where(x => enemyComponent.enemyTags.Contains(x.transform.tag)).Select(x => x.GetComponent<BaseEntity>()).ToList();

        for (int i = 0; i < nearbyEntities.Count; i++)
        {
            Transform tempTarget = nearbyEntities[i].transform;
            directionToTarget = (tempTarget.position - transform.position).normalized;

            if (Vector3.Angle(transform.up, directionToTarget) < visionAngle / 2)
            {
                //Target is within view angle
                float distance = Vector3.Distance(transform.position, tempTarget.position);

                if (Physics2D.Raycast(transform.position, directionToTarget, distance, ~layersToIgnore))
                {
                    visibleEntities.Add(tempTarget.GetComponent<BaseEntity>());
                }
            }
        }
    }

    private void FindTarget()
    {
        // 1. View radius finding all nearby enemies
        // 2. Measure angle between entity and the direction this object is looking
        // 3. Compare to vision angle property and if angle measured < field of view then object is in view in terms of angle
        // 4. Shoot a raycast to the targets within angle view and check if they are obstructed

        if (target != null)
        {
            TrackTarget();
        }
        else
        {
            FindVisibleTargets();

            if (visibleEntities.Count > 0)
            {
                if (targetType == TargetType.Closest)
                {
                    target = visibleEntities.OrderByDescending(x => Vector2.Distance(transform.position, x.transform.position)).First().transform;
                }
                else if (targetType == TargetType.MostHealth)
                {
                    target = visibleEntities.OrderByDescending(x => x.health).First().transform;
                }
            }
            else
            {
                target = null;
            }
        }
    }

    private void TrackTarget()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > visionRadius || Vector3.Angle(transform.up, directionToTarget) > visionAngle / 2 || !isTargetInSight)
        {
            Debug.Log("Enemy out of sights, last position recorded");
            targetLastKnownPosition = target.position;
            target = null;
        }
        else
        {
            targetLastKnownPosition = Random.insideUnitCircle * visionAngle;
        }
    }

    public Vector2 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees -= transform.eulerAngles.z;
        }
        return new Vector2(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

    public virtual void RotateToTarget()
    {
        //Calculate the angle the enemy needs to be rotated to.
        float angle = Mathf.Atan2(movementDirection.y, movementDirection.x) * Mathf.Rad2Deg;

        //Apply the calculated angle to the enemy.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public virtual void RotateToPosition(Vector2 position)
    {
        Vector2 directionToPosition = (position - (Vector2)transform.position).normalized;
        float angle = Mathf.Atan2(directionToPosition.y, directionToPosition.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }
    #endregion

    #region Dodging

    public void DashToDodge()
    {
        dodgeNextFire = dodgeCooldown;
        rb.AddForce(dodgeEscapeVector * dodgeSpeed, ForceMode2D.Impulse);
    }

    public Vector2 GetRandomPerpendicularDirectionFromVector(Vector2 direction)
    {
        int num = Random.Range(0, 2);

        if (num == 1)
        {
            return new Vector2(direction.y, -direction.x);
        }
        else
        {
            return new Vector2(-direction.y, direction.x);
        }
    }

    public void FindNearbyProjectiles()
    {
        nearbyProjectiles = Physics2D.OverlapCircleAll(transform.position, visionRadius).Where(x => x.transform.tag == "Projectile").Select(x => x.GetComponent<Projectile>()).ToList();
    }

    public void FindHarmfulProjectiles()
    {
        FindNearbyProjectiles();

        nearbyEnemyProjectiles = nearbyProjectiles.Where(x => enemyComponent.enemyTags.Contains(x.entityShotFrom.transform.tag)).ToList();

        nearbyHarmfulProjectiles.Clear();

        for (int i = 0; i < nearbyEnemyProjectiles.Count; i++)
        {
            if (IsProjectileHeadedTowardsThis(nearbyEnemyProjectiles[i]))
            {
                nearbyHarmfulProjectiles.Add(nearbyEnemyProjectiles[i]);
                PrioritizeHarmfulProjectiles();
            }
        }
    }

    public void PrioritizeHarmfulProjectiles()
    {
        nearbyHarmfulProjectiles.OrderBy(x => TimeToProjectileIntercept(x));
        mostDangerousProjectile = nearbyHarmfulProjectiles[0];

        if(TimeToProjectileIntercept(mostDangerousProjectile) < timeToReact.y)
        {
            isInDanger = true;
        }
        else
        {
            isInDanger = false;
        }
    }

    public float TimeToProjectileIntercept(Projectile projectile)
    {
        float distance = Vector3.Distance(projectile.transform.position, transform.position);

        float time = distance / projectile.GetComponent<Rigidbody2D>().velocity.magnitude;

        return time;
    }

    public bool IsProjectileHeadedTowardsThis(Projectile projectile)
    {
        Vector3 projVelocity = projectile.GetComponent<Rigidbody2D>().velocity;
        Vector3 dirFromProjToObject = (transform.position - projectile.transform.position).normalized;

        if(Vector3.Angle(projVelocity.normalized, dirFromProjToObject.normalized) <= angleSafetyNetFromProjectiles)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #endregion

    private void FixedUpdate()
    {
        if(target != null)
        {
            MoveToTarget();
        }
        else if(target == null)
        {
            MoveCautiously();
        }
    }

    public virtual void MoveToTarget()
    {
        if (isTargetInSight)
        {
            RotateToTarget();

            if (Vector2.Distance(transform.position, target.position) > movementMinRange)
            {
                rb.AddForce(movementDirection * movementSpeed);
            }
            else
            {
                rb.AddForce(-movementDirection * movementSpeed);
            }
        }
    }

    public virtual void MoveCautiously()
    {
        RotateToPosition(targetLastKnownPosition);
        movementDirection = (targetLastKnownPosition - transform.position).normalized;
        Debug.DrawLine(transform.position, targetLastKnownPosition, Color.red);

        rb.AddForce(movementDirection * movementSpeed / 2);
    }
}

public enum TargetType
{
    Closest,
    MostHealth
}
