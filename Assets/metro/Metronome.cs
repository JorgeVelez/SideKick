using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Metronome : MonoBehaviour
{
    public double bpm = 140.0F;

    double nextTick = 0.0F; // The next tick in dspTime
    double sampleRate = 0.0F; 
    bool ticked = false;

    private AudioSource asource;


    void Start() {
        asource = GetComponent<AudioSource>();

        double startTick = AudioSettings.dspTime;
        sampleRate = AudioSettings.outputSampleRate;

        nextTick = startTick + (60.0 / bpm);
    }

    void LateUpdate() {
        if ( !ticked && nextTick >= AudioSettings.dspTime ) {
            ticked = true;
            OnTick( );
        }
    }

    void OnTick() {
        Debug.Log( "Tick" );
        asource.Play();

    }

    void FixedUpdate() {
        double timePerTick = 60.0f / bpm;
        double dspTime = AudioSettings.dspTime;

        while ( dspTime >= nextTick ) {
            ticked = false;
            nextTick += timePerTick;
        }

    }
}