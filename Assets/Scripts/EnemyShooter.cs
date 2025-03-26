using System;
using System.Collections;
using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public GameObject bulletPrefab;
    private Vector3 _bulletSpawnPoint;

    public float fireRate = 1.0f;
    public float bulletSpeed = 1.0f;
    public float angleStep = 36f;
    public float currentAngle = 0f;

    public int spiralBulletCount = 32;
    public int multidirectionalBulletCount = 16;

    public void SummonEnemy()
    {
        StartCoroutine(StartBoss());
    }

    private IEnumerator StartBoss()
    {
        yield return new WaitForSeconds(1.0f);
        yield return InvokeBulletMultidirectional();
        yield return new WaitForSeconds(2.0f);
        yield return InvokeBulletsSpiral();
        yield return new WaitForSeconds(2.0f);
        yield return InvokeBulletMultidirectional();
        yield return new WaitForSeconds(3.0f);
        yield return InvokeBulletMultidirectional();
    }

    private IEnumerator InvokeBulletsSpiral()
    {
        for (var i = 0; i < spiralBulletCount; i++)
        {
            _bulletSpawnPoint = transform.position;
            var angle = currentAngle + i * angleStep;
            var radians = angle * Mathf.Deg2Rad;

            var direction = new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));

            // var offset = _bulletSpawnPoint + (direction.normalized * 0.2f);
            var offset = _bulletSpawnPoint;

            var bullet = Instantiate(bulletPrefab, offset, Quaternion.identity);
            bullet.transform.localScale *= 0.5f;
            var rigidBody = bullet.GetComponent<Rigidbody>();
            rigidBody.useGravity = false;
            rigidBody.linearVelocity = direction * bulletSpeed;
            currentAngle += angleStep / 2;

            yield return new WaitForSeconds(fireRate);
        }
    }

    private IEnumerator InvokeBulletMultidirectional()
    {
        for (var i = 0; i < multidirectionalBulletCount; i++)
        {
            _bulletSpawnPoint = transform.position;
            var angle = currentAngle + i * angleStep;
            var radians = angle * Mathf.Deg2Rad;

            var x = Mathf.Cos(radians);
            var z = Mathf.Sin(radians);

            for (var j = 0; j < 4; j++)
            {
                var direction = Quaternion.Euler(0, j * 90, 0) * new Vector3(x, 0, z);
                // var offset = _bulletSpawnPoint + (direction.normalized * 0.5f);
                var offset = _bulletSpawnPoint;
                var bullet = Instantiate(bulletPrefab, offset, Quaternion.identity);
                bullet.transform.localScale *= 0.5f;
                var rigidBody = bullet.GetComponent<Rigidbody>();
                rigidBody.useGravity = false;

                rigidBody.linearVelocity = direction * bulletSpeed;
                currentAngle += angleStep / 2;
            }

            yield return new WaitForSeconds(fireRate);
        }
    }
}