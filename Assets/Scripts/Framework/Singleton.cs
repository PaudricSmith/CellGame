using UnityEngine;



public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{

    public static T One;


    protected virtual void Awake()
    {
        if (One == null)
        {
            One = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (One != this)
        {
            Destroy(gameObject);
        }
    }
}