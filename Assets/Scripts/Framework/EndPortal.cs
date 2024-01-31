using UnityEngine;



public class EndPortal : MonoBehaviour
{
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.TriggerPortalReached(gameObject.transform.localPosition);

            Destroy(gameObject);
        }
    }
}
