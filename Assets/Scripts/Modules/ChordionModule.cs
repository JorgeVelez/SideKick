using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Midi;

public class ChordionModule : MonoBehaviour
{
    public Transform scalesParent;
    public GameObject scalesPrefab;

    public Transform majorChordsParent;
    public GameObject chordPrefab;
    void Start()
    {
        scalesPrefab.SetActive(false);
        foreach (ScalePattern sp in Scale.Patterns)
        {
            GameObject go = Instantiate(scalesPrefab, scalesParent);
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = sp.Name;
        }

        for (Pitch pitch = Pitch.C2; pitch < Pitch.B5; ++pitch)
        {
            GameObject go = Instantiate(chordPrefab, majorChordsParent);
            go.SetActive(true);
            Pitch lePitch = pitch;
            go.GetComponentInChildren<Button>().onClick.AddListener(()=> PlayChord(lePitch));
            go.GetComponentInChildren<Text>().text = lePitch.ToString();
        }

        
    }

    private void PlayChord( Pitch rootPitch)
    {
        Chord chord = new Chord(rootPitch.NotePreferringFlats(),Chord.Patterns[0], 0);
        for (var pitch = Pitch.A0; pitch < Pitch.C8; ++pitch)
        {
            if (chord.Contains(pitch))
            {
                    MidiManager.Instance.outDevice.SendNoteOn(Channel.Channel1, pitch, 127);
            }
        }
    }
}
