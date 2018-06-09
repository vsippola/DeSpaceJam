using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerForeground : MonoBehaviour
{
    public delegate void OnFadeEvent();
    public event OnFadeEvent FadeInEvent;
    public event OnFadeEvent FadeOutEvent;

    public void FadeIn()
    {
        if (FadeInEvent != null) FadeInEvent();
    }

    public void FadeOut()
    {
        if (FadeOutEvent != null) FadeOutEvent();
    }
}
