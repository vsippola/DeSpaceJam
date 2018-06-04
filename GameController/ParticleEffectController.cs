using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    public ParticleSystem[] particles;

    [Range(1f, 10f)]
    public float jumpSpeed = 5f;

    public float targetSpeed = 1f;
    public bool slowing;

    private void Awake()
    {
        slowing = false;
        targetSpeed = 1f;
    }

    void Update ()
    {
        if (!slowing) return;

        SlowAnimations();
	}

    public void BumpSpeed()
    {
        foreach(ParticleSystem part in particles)
        {
            ParticleSystem.MainModule main = part.main;
            main.simulationSpeed = jumpSpeed;
        }

        slowing = true;
    }


    private void SlowAnimations()
    {
        bool changed = false;

        foreach(ParticleSystem part in particles)
        {
            ParticleSystem.MainModule main = part.main;
            float speed = main.simulationSpeed;

            if(Mathf.Abs(speed) > .001)
            {
                changed = true;

                main.simulationSpeed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime);
            }
        }

        slowing = changed;
    }
}
