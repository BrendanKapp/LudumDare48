using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MinotaurState {center, player, attack, teleport};
public class MinotaurController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    public MinotaurState currentState;

    private Rigidbody rb;
    [SerializeField]
    private TerrainGenerator terrainGenerator;
    [SerializeField]
    private Transform player;
    private Animator animator;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }
    private void Update()
    {
        Attack();
    }
    private void Teleport()
    {
        print("Teleporting");
        int value = Random.value > 0.5f ? 0 : 4;
        if (Random.value > 0.5f)
        {
            transform.position = terrainGenerator.ConvertMapToWorld(2, 1, value);
        } else {
            transform.position = terrainGenerator.ConvertMapToWorld(value, 1, 2);
        }
        transform.position += Vector3.up * 1;
        transform.rotation = Quaternion.identity;
        transform.LookAt(player.position);
        currentState = MinotaurState.teleport;
        lastAttackTime = Time.time - 5f;
    }
    private void MoveCenter()
    {
        if ((transform.position - player.transform.position).y > -10)
        {
            // if in a non 2 file square, head to 2 file
            Vector3 location = terrainGenerator.ConvertWorldToMap(transform.position);
            Vector3 dir;
            if ((location.x < 1.5f && location.x > 2.5f) && (location.z < 1.5f && location.z > 2.5f))
            {
                Teleport();
                //dir = transform.position - terrainGenerator.ConvertMapToWorld(2, 1, (int)location.z);
            }
            dir = transform.position - terrainGenerator.ConvertMapToWorld(2, 1, 2); // lazy center
            dir.y = 0;
            rb.MovePosition(transform.position - dir.normalized * Time.deltaTime * speed);
            //transform.LookAt(player.position);
            LookTowardsPlayer();
            currentState = MinotaurState.center;
        } else {
            Teleport();
        }
    }
    private void MovePlayer()
    {
        RaycastHit hit;
        Vector3 toPlayer = player.transform.position - transform.position;
        if (Physics.Raycast(transform.position, toPlayer, out hit, Mathf.Infinity) && hit.collider.GetComponent<PlayerController>() != null)
        {
            toPlayer.y = 0;
            rb.MovePosition(transform.position + toPlayer.normalized * Time.deltaTime * speed);
            //transform.LookAt(player.position);
            LookTowardsPlayer();
            currentState = MinotaurState.player;
            print("Towards player");
        } else {
            MoveCenter();
        }
    }
    private float lastAttackTime;
    private float animationDelay = 5.5f;
    private void Attack()
    {
        if (Time.time - lastAttackTime < animationDelay) return;
        float distance = (player.transform.position - transform.position).sqrMagnitude;
        if (distance < 100)
        {
            LookTowardsPlayer();
            //print("Attack");
            lastAttackTime = Time.time;
            animator.SetTrigger("Strike");
            currentState = MinotaurState.attack;
        } else {
            MovePlayer();
        }
    }
    private void LookTowardsPlayer()
    {
        Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
        //print("rotating " + transform.rotation);
        //Vector3 direction = Vector3.Lerp(transform.position, player.position, 0.05f);
        //transform.LookAt(direction);
    }
    [SerializeField]
    private string explodeEffectName;
    [SerializeField]
    private Transform hitboxHitPoint;
    public void Hit()
    {
        print("hit");
        CameraRotate.screenShakeAmount += 70;
        HitboxController hitbox = ObjectPooler.PoolObject(explodeEffectName).GetComponent<HitboxController>();
        hitbox.transform.position = hitboxHitPoint.position;
        hitbox.Activate();
    }
    public void FootL()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.sqrMagnitude < 1000)
        {
            CameraRotate.screenShakeAmount += (int)(2000f / toPlayer.sqrMagnitude);
        }
    }
    public void FootR()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.sqrMagnitude < 1000)
        {
            CameraRotate.screenShakeAmount += (int)(2000f / toPlayer.sqrMagnitude);
        }
    }
}
