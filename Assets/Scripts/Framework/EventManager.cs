using System;
using UnityEngine;



public class EventManager : MonoBehaviour
{

    // Define an event to be called
    public static event Action OnNucleusPickedUp;
    public static event Action OnMitochondriaPickedUp;
    public static event Action OnGogliPickedUp;
    public static event Action<Vector3> OnPortalReached;



    // The method to be called when an event happens
    public static void TriggerNucleusPickedUp() => OnNucleusPickedUp?.Invoke();
    public static void TriggerMitochondriaPickedUp() => OnMitochondriaPickedUp?.Invoke();
    public static void TriggerGogliPickedUp() => OnGogliPickedUp?.Invoke();
    public static void TriggerPortalReached(Vector3 portalCenter) => OnPortalReached?.Invoke(portalCenter);
}
