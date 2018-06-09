using UnityEngine;
using System.Collections.Generic;

public class MusicControllers : MonoBehaviour
{
    [Range(-3f, 1f)]
    public float warpMin;
    [Range(1f, 3f)]
    public float warpMax;
    [Range(1, 5)]
    public int numWarps;

    [Range(1f, 5f)]
    public float changeSpeed;

    public MomentDisplayController displayController;

    private AudioSource music;
    private readonly float defaultSpeed = 1f;
    Queue<float> targets;

	void Start ()
    {
        music = transform.GetComponentInChildren<AudioSource>();
        targets = new Queue<float>();

        if(displayController != null) displayController.MomentDisplayChangeEvent += OnMove;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (targets.Count == 0) return;

        WarpAudio();
	}

    private void WarpAudio()
    {
        float target = targets.Peek();
        float current = music.pitch;

        music.pitch = Mathf.Lerp(current, target, Time.deltaTime * changeSpeed);

        //If the target is hit remove target from queue
        if (Mathf.Abs(target - music.pitch) < .0001) targets.Dequeue();
    }

    public void OnMove(LevelMoment m)
    {
        if (targets.Count > 0) return;

        for(int i = 0; i < numWarps; i++)
        {
            targets.Enqueue(Random.Range(warpMin, warpMax));
        }
        targets.Enqueue(defaultSpeed);
    }
}
