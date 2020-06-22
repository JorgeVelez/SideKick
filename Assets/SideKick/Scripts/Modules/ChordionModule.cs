using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Midi;
using System;

public class ChordionModule : MonoBehaviour
{
    public Transform scalesParent;
    public GameObject scalesPrefab;

    List<Button> scaleButtons = new List<Button>();

    List<string> chordTypes = new List<string>() { "ChordsM", "Chordsm", "Chords7", "ChordsDim", "ChordsAug"};

    int octave = 3;

    void Start()
    {
        scalesPrefab.SetActive(false);
        foreach (ScalePattern sp in Scale.Patterns)
        {
            GameObject go = Instantiate(scalesPrefab, scalesParent);
            go.SetActive(true);
            go.GetComponentInChildren<Text>().text = sp.Name;
            ScalePattern leScale = sp;
            go.GetComponentInChildren<Button>().onClick.AddListener(() => SetupScale(sp, go.GetComponentInChildren<Button>()));
            scaleButtons.Add(go.GetComponentInChildren<Button>());

        }

        for (int i = 0; i < 8; i++)
        {
            transform.Find( "octaves/octave").gameObject.SetActive(false);
                GameObject go = Instantiate(transform.Find("octaves/octave").gameObject,transform.Find("octaves"));
                go.SetActive(true);
            int octaveIndex = i;
                go.GetComponent<SKButton>().onTouchUp.AddListener(() => ChangeOctave(octaveIndex));
                go.GetComponentInChildren<Text>().text = i.ToString();
        }
        ChangeOctave(octave);

        for (int i = 0; i < chordTypes.Count; i++)
        {
            transform.Find(chordTypes[i] + "/Button").gameObject.SetActive(false);
            for (Pitch pitch = Pitch.C0; pitch < Pitch.C2; ++pitch)
            {
                GameObject go = Instantiate(transform.Find(chordTypes[i] + "/Button").gameObject, transform.Find(chordTypes[i]));
                go.SetActive(true);
                Pitch lePitch = pitch;
                int chordTypeIndex = i;
                go.GetComponent<SKButton>().onTouchDown.AddListener(() => PlayChord(lePitch, chordTypeIndex));
                go.GetComponent<SKButton>().onTouchUp.AddListener(() => StopChord(lePitch, chordTypeIndex));
                go.GetComponentInChildren<Text>().text = lePitch.NotePreferringFlats().ToString();
            }
        }

        for (int i = 0; i < 24; i++)
        {
            transform.Find("strum/area").gameObject.SetActive(false);
            GameObject go = Instantiate(transform.Find("strum/area").gameObject, transform.Find("strum"));
            go.SetActive(true);
            int strumIndex = i;
            go.GetComponent<SKHover>().onTouchOver.AddListener(() => PlayNote(strumIndex));
       }

        //chordPrefab.SetActive(false);
        //for (Pitch pitch = Pitch.C2; pitch < Pitch.C4; ++pitch)
        //{
        //    GameObject go = Instantiate(chordPrefab, majorChordsParent);
        //    go.SetActive(true);
        //    Pitch lePitch = pitch;
        //    go.GetComponent<SKButton>().onTouchDown.AddListener(() => PlayChord(lePitch, 0));
        //    go.GetComponent<SKButton>().onTouchUp.AddListener(() => StopChord(lePitch, 0));
        //    go.GetComponentInChildren<Text>().text = lePitch.NotePreferringFlats().ToString();
        //}

    }

    

    private void SetupScale(ScalePattern sp, Button bt)
    {
        foreach (Button item in scaleButtons)
        {
            item.image.color = new Color(.377f, .377f, .377f);
        }

        bt.image.color = new Color(0, .5f, 1);
    }

    private void PlayNote(int strumIndex)
    {
        MidiManager.Instance.SendNoteOnWithOffSchedule((octave * 12), 100);
    }

    private void PlayChord( Pitch rootPitch, int notesequence)
    {
        Chord chord = new Chord(rootPitch.NotePreferringFlats(),Chord.Patterns[notesequence], 0);
        Pitch[] pitches = chord.GetPitches(rootPitch);
        for (var i = 0; i < chord.GetPitches(rootPitch).Length; ++i)
        {
            MidiManager.Instance.SendNoteOn((int)pitches[i]+(octave*12));
        }
    }

    private void StopChord(Pitch rootPitch, int notesequence)
    {
        Chord chord = new Chord(rootPitch.NotePreferringFlats(), Chord.Patterns[notesequence], 0);
        Pitch[] pitches = chord.GetPitches(rootPitch);
        for (var i = 0; i < pitches.Length; ++i)
        {
            MidiManager.Instance.SendNoteOff((int)pitches[i] + (octave * 12));
        }
    }

    private void ChangeOctave( int _octave)
    {
        foreach (Transform child in transform.Find("octaves"))
        {
            child.GetComponentInChildren<Image>().color = child.GetComponentInChildren<SKButton>().colouOriginal;
        }
        octave = _octave;
        transform.Find("octaves").GetChild(octave + 1).GetComponentInChildren<Image>().color = Color.magenta;

    }
}
