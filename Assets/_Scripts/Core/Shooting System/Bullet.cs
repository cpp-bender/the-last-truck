using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("DEPENDENCIES"), Space(5f)]
    public Rigidbody rb;

    [Header("BULLET SETTINGS"), Space(5f)]
    public BulletProperties bulletProp;

    public float Damage { get => damage; set => damage = value; }
    public float damage;

    public PoolTag Tag { get => tag; set => tag = value; }
    public PoolTag tag;

    private void OnEnable()
    {
        StartCoroutine(GoToPool(bulletProp.LifeTime));
        GoForward();
    }

    protected virtual void OnDisable()
    {
        rb.velocity = Vector3.zero;
        transform.forward = Vector3.forward;
        transform.rotation = Quaternion.identity;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.ENEMY))
        {
            GiveDamage(other);
            StartCoroutine(GoToPool(0f));
        }
        else if (other.CompareTag(Tags.GROUND))
        {
            ConnectWithGround(other);
        }
    }

    protected virtual void GiveDamage(Collider other)
    {
        other.GetComponent<BaseEnemy>().TakeDamage(Damage);
    }

    protected virtual void ConnectWithGround(Collider other)
    {
        StartCoroutine(GoToPool(0f));
    }

    private IEnumerator GoToPool(float time)
    {
        yield return new WaitForSeconds(time);

        if (gameObject.activeSelf)
            ObjectPooler.Instance.EnqueueToPool(Tag, gameObject);
    }

    private void GoForward()
    {
        rb.AddForce(transform.forward * bulletProp.Speed, ForceMode.Impulse);
    }
}
