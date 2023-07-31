using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (!instance)
            {
                GameObject singletonObj = new GameObject(typeof(T).Name);
                singletonObj.hideFlags = HideFlags.HideAndDontSave;
                instance = singletonObj.AddComponent<T>();
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (!instance)
        {
            instance = this as T;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    protected virtual void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }
}
