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
        print("Teleporting at " + Time.time);
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
        lastAttackTime = Time.time - animationDelay + 0.5f;
    }
    private float lastSeenPlayer;
    private float teleportAfterTime = 5f;
    private void MoveCenter()
    {
         if (Time.time - lastSeenPlayer < teleportAfterTime)
        {
            // if in a non 2 file square, head to 2 file
            Vector3 dir;
            dir = transform.position - terrainGenerator.ConvertMapToWorld(2, 1, 2); // lazy center
            dir.y = 0;
            rb.MovePosition(transform.position - dir.normalized * Time.deltaTime * speed);
            LookTowardsPlayer(1);
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
            LookTowardsPlayer(1);
            currentState = MinotaurState.player;
            lastSeenPlayer = Time.time;
        } else {
            MoveCenter();
        }
    }
    private float lastAttackTime;
    private float animationDelay = 3.5f;
    private void Attack()
    {
        if (Time.time - lastAttackTime < animationDelay) return;
        float distance = (player.transform.position - transform.position).sqrMagnitude;
        if (distance < 100)
        {
            LookTowardsPlayer(5);
            AudioSource roarSound = Sound.GetSound("Roar");
            roarSound.pitch = Random.Range(0.2f, 0.5f);
            roarSound.volume = Random.value / 2f + 0.5f;
            roarSound.transform.position = transform.position;
            roarSound.Play();
            lastAttackTime = Time.time;
            animator.SetTrigger("Strike");
            currentState = MinotaurState.attack;
        } else {
            MovePlayer();
        }
    }
    private void LookTowardsPlayer(float multiplier)
    {
        Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * multiplier);
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
        AudioSource hitEffectSound = Sound.GetSound("HitEffect");
        hitEffectSound.pitch = Random.Range(0.2f, 0.5f);
        hitEffectSound.volume = Random.value / 2f + 0.5f;
        hitEffectSound.Play();
        hitEffectSound.transform.position = hitbox.transform.position;
        hitbox.Activate();
    }
    public void FootL()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.sqrMagnitude < 1000)
        {
            CameraRotate.screenShakeAmount += (int)(2000f / toPlayer.sqrMagnitude);
        }
        if (toPlayer.sqrMagnitude < 400)
        {
            AudioSource footstepSound = Sound.GetSound("RockSmash" + Random.Range(1, 4));
            footstepSound.pitch = Random.Range(0.2f, 0.5f);
            footstepSound.volume = Random.value / 2f + 0.5f;
            footstepSound.transform.position = transform.position;
            footstepSound.Play();
        }
    }
    public void FootR()
    {
        Vector3 toPlayer = player.transform.position - transform.position;
        if (toPlayer.sqrMagnitude < 1000)
        {
            CameraRotate.screenShakeAmount += (int)(2000f / toPlayer.sqrMagnitude);
        }
        if (toPlayer.sqrMagnitude < 400)
        {
            AudioSource footstepSound = Sound.GetSound("RockSmash" + Random.Range(1, 4));
            footstepSound.pitch = Random.Range(0.2f, 0.5f);
            footstepSound.volume = Random.value / 2f + 0.5f;
            footstepSound.transform.position = transform.position;
            footstepSound.Play();
        }
    }
}
