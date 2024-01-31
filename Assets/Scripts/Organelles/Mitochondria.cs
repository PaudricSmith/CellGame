using System.Collections;
using UnityEngine;



public class Mitochondria : MonoBehaviour
{

    private const float DestroyDelayTime = 2.0f;

    [SerializeField] private ParticleSystem mitochondriaParticles;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Trigger the event for the Player picking up the Nucleus
            EventManager.TriggerMitochondriaPickedUp();

            // Play particle system
            mitochondriaParticles.Play();

            // Hide the nucleus GameObject
            gameObject.GetComponent<SpriteRenderer>().color = Color.clear;

            // Delay and then destroy this object
            StartCoroutine(DestroyDelay(DestroyDelayTime));
        }
    }


    private IEnumerator DestroyDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        Destroy(gameObject);
    }
}
