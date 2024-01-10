using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunsConfig : MonoBehaviour
{
    [Header("Disparar")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawner;
    [SerializeField] private float fireRate;
    [SerializeField] private int speedBullet;
    Vector3 finalTarget;
    private float nexFireTime;

    [Header("Recoil")]
    [SerializeField] private float recoilForce;
    [SerializeField] private float recoilDuration;
    [SerializeField] private Rigidbody2D recoilRigidbody;
    private Vector3 originalPosition;
    private bool isRecoiling = false;

    [Header("Dispercion")]
    [SerializeField] private float bulletSpreadAngle;

    [Header("ApuntarAlMouse")]
    //[SerializeField] private Camera camara;
    [SerializeField] private SpriteRenderer sr;
    Vector3 targetRotation;
    private float angle;
    [SerializeField] private Transform player;
    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        originalPosition = recoilRigidbody.position;
    }

    private void Update()
    {
        RotateTowardsMouse();
        CheckFiring();
    }
    private void CalculateAngleTowardsMouse()
    {

    }
    private void RotateTowardsMouse()
    {
        targetRotation = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(targetRotation.y, targetRotation.x) * Mathf.Rad2Deg;
        //print(angle);
        transform.rotation = Quaternion.Euler(0, 0, angle);
        //print(angle);
        
        if (angle > 90 || angle < -90)
        {
            sr.flipY = true;
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
        else
        {
            sr.flipY = false;
            transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        }
    }
    private void Recoil()
    {
        if (!isRecoiling)
        {
            StartCoroutine(RecoilCoroutine());
        }
    }
    IEnumerator RecoilCoroutine()
    {
    isRecoiling = true;

    float recoilAngle = angle;
    //print(angle);
    Vector2 recoilDirection;

    if (recoilAngle > 90 || recoilAngle < -90)
    {
        recoilDirection = Quaternion.Euler(0, 0, angle) * transform.right;
    }
    else
    {
        recoilDirection = Quaternion.Euler(0, 0, angle) * -transform.right;
    }
    
    recoilRigidbody.AddForce(recoilDirection * recoilForce, ForceMode2D.Impulse);

    yield return new WaitForSeconds(recoilDuration);
    
    isRecoiling = false;
    }

    private void CheckFiring()
    {
        if(Input.GetMouseButton(0) && Time.time >= nexFireTime)
        {
            GameObject bullet = Instantiate(bulletPrefab);
            bullet.transform.position = bulletSpawner.position;
            bullet.transform.rotation = transform.rotation;
            targetRotation.z = 0;
            finalTarget = (targetRotation - transform.position).normalized;

            float randomSpread = Random.Range(-bulletSpreadAngle, bulletSpreadAngle);
            Quaternion spreadRotation = Quaternion.Euler(0, 0, randomSpread);
            Vector2 spreadDirection = spreadRotation * finalTarget;
            bullet.GetComponent<Rigidbody2D>().AddForce(spreadDirection * speedBullet, ForceMode2D.Impulse);

            Recoil();
            
            nexFireTime = Time.time + fireRate;
        }
    }
}
