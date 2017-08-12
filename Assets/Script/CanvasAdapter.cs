using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasAdapter : MonoBehaviour
{
    [SerializeField]
    private Transform infoBarRoot;
    public static Transform InfoBarRoot
    {
        get
        {
            return instance.infoBarRoot;
        }
    }

    public static Transform Transform
    {
        get
        {
            return instance.transform;
        }
    }

    private static CanvasAdapter instance;

    void Awake()
    {
        instance = this;
    }
}
