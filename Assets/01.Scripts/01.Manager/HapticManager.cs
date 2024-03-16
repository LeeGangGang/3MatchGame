using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_IOS
using System.Runtime.InteropServices;
#endif

public class HapticManager : MonoBehaviour
{
    public static HapticManager Inst;

#if UNITY_ANDROID && !UNITY_EDITOR
    public AndroidJavaClass UnityPlayer = null;
    public AndroidJavaObject CurrentActivity = null;
    public AndroidJavaObject AndroidVibrator = null;
#elif UNITY_IOS
    [DllImport("__Internal")]
    public static extern void Vibrate(int n);
#endif

    public readonly string VibrateFuncName = "vibrate";
    
    void Awake()
    {
        if (Inst == null)
            Inst = this;
        else if (Inst != this)
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        Init();
    }

    void Init()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        CurrentActivity = UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidVibrator = CurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#elif UNITY_IOS && !UNITY_EDITOR
        
#endif
    }

    public void Enter(long _ms)
    {
        //if (Data.Inst.IsHapticOff)
        //    return;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call(VibrateFuncName, _ms);
#elif UNITY_IOS && !UNITY_EDITOR
        int n = 1519;
        if (1000 > _ms && _ms > 10)
            n = 1520;
        else if (_ms >= 1000)
            n = 1521;
            
        Vibrate(n);
#endif
    }

    public void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("cancel");
#elif UNITY_IOS && !UNITY_EDITOR

#endif
    }
}
