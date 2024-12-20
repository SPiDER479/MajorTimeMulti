using UnityEngine;
public class Market : MonoBehaviour
{
    [SerializeField] private Currency currency;
    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Resource"))
        {
            currency.coinCount += other.gameObject.GetComponent<Resource>().value;
            Destroy(other.gameObject);
        }
    }
}