using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 싱글턴 클래스
/// 2020.06.06 김용현
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> : MonoBehaviour where T : Object
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                InitInstance();
            return _instance;
        }
    }
    /// <summary>
    /// 인스턴스 초기화.
    /// 상속 후 오버라이드 하여 다른 방법 사용가능.
    /// </summary>
    protected static void InitInstance()
    {
        _instance = FindObjectOfType<T>();
    }
    protected void OnDestroy()
    {
        _instance = null;
    }
}
