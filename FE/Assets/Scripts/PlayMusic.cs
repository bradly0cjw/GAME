using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayMusic : MonoBehaviour
{
    AudioSource audio = null;  // add 'public' for method 1 listed below

    // Start is called before the first frame update
    void Start()
    {
        // method 1: add an AudioSource in inspector, then assign clip to it
        audio = GetComponent<AudioSource>();

        // method 2: set all from scratch by code
        //this.AddComponent<AudioSource>();
        //audio = GetComponent<AudioSource>();
        //audio.clip = Resources.Load<AudioClip>("Music/Colors of the wind");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))  // left mouse button
        {
            if (! audio.isPlaying) 
                audio.Play();
        }

        if (Input.GetMouseButtonDown(2))  // middle mouse button
        {
            if (audio.isPlaying)
                audio.Pause();
        }

        if (Input.GetMouseButtonDown(1))  // right mouse button
        {
            if (audio.isPlaying)
                audio.Stop();
        }

    }
}
