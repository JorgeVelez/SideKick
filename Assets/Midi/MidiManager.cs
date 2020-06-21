using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using System.ComponentModel;
using Midi;
using UnityEngine.EventSystems;

public class MidiManager : Singleton<MidiManager>
{

    public OutputDevice outDevice;

    public bool autoConnect = false;
    public bool showGui = false;
    public int outDeviceID = 2;
    public int inDeviceID = 1;

    Transform mainUI;

    Button btNoteOn;
    Button btNoteOff;
    Button btConnect;
    Slider slControl1;


    //recibir
    private const int SysExBufferSize = 128;
    private InputDevice inDevice = null;


    List<string> channelListBox = new List<string>();
    List<string> sysCommonListBox = new List<string>();
    List<string> sysRealtimeListBox = new List<string>();

    Text sysExRichTextBox;

    Button btRecibirOn;
    Button btRecibirOff;

    //sequence
    Button btLoadSequence;
    Button btStartSequence;
    Button btStopSequence;

    private bool scrolling = false;
    private bool playing = false;
    private bool closing = false;

    private Dictionary<Pitch, bool> pitchesPressed;

    private List<float> timeClocks = new List<float>();

    private float lastClock = 0;

    private float Bpm = 0;
    private float Bpm2 = 0;
    private float Bpm3 = 0;

    public Text bpmTx;
    public Text bpm2Tx;
    public Text bpm3Tx;



    public void ConectarOut(int devID)
    {
        outDeviceID = devID;

        if (OutputDevice.InstalledDevices.Count == 0)
        {
            Debug.Log("No MIDI output devices available." + "Error!");
        }
        else
        {
            try
            {
                outDevice = OutputDevice.InstalledDevices[outDeviceID];
                Debug.Log("output conectado a." + outDeviceID);
                outDevice.Open();

            }
            catch (Exception ex)
            {
                Debug.LogError("error: " + ex.Message.ToString());
            }
        }
    }

    public void ConectarIn(int devID)
    {
        inDeviceID = devID;
        if (InputDevice.InstalledDevices.Count == 0)
        {
            Debug.Log("No MIDI input devices available." + "Error!");
        }
        else
        {
            try
            {
                Debug.Log("input conectado a." + inDeviceID);
                inDevice = InputDevice.InstalledDevices[inDeviceID];
                inDevice.NoteOn += new InputDevice.NoteOnHandler(this.NoteOn);
                inDevice.NoteOff += new InputDevice.NoteOffHandler(this.NoteOff);
                inDevice.ControlChange += new InputDevice.ControlChangeHandler(this.controlChangeHandler);
                inDevice.SysEx += new InputDevice.SysExHandler(this.sysexHandler);
                inDevice.ProgramChange += new InputDevice.ProgramChangeHandler(this.programChangeHandler);
                inDevice.timeClock += new InputDevice.TimeClockHandler(this.timeCodeHandler);
                inDevice.Open();
                inDevice.StartReceiving(null, true);

                //startButton_Click();
            }
            catch (Exception ex)
            {
                Debug.Log("error: " + ex.Message.ToString());
            }
        }
    }

    public void NoteOn(NoteOnMessage msg)
    {
        lock (this)
        {
            Debug.Log("NoteOn channel " + msg.Channel + " Pitch " + msg.Pitch + " Velocity " + msg.Velocity);

            pitchesPressed[msg.Pitch] = true;
        }
    }

    public void controlChangeHandler(ControlChangeMessage msg)
    {
        lock (this)
        {
            Debug.Log("control channel " + msg.Channel + " control " + msg.Control + " Value " + msg.Value);

        }
    }

    public void programChangeHandler(ProgramChangeMessage msg)
    {
        lock (this)
        {
            Debug.Log(msg.Instrument);
        }
    }

    public void timeCodeHandler(TimeclockMessage msg)
    {
        lock (this)
        {
            float delta = msg.Time - lastClock;
            timeClocks.Add(delta);
            Debug.Log(msg.Time - lastClock);
            lastClock = msg.Time;

            if (timeClocks.Count > 48)
                timeClocks.RemoveAt(0);

            Bpm = Mathf.Round(60f / ((timeClocks.Average()) * 24f));

        }
    }


    void Update()
    {
        bpmTx.text = Bpm.ToString();
        bpm2Tx.text = Bpm2.ToString();
        bpm3Tx.text = Bpm3.ToString();
    }


    public void sysexHandler(SysExMessage msg)
    {
        lock (this)
        {
            Debug.Log(msg.Time);
        }
    }


    public void NoteOff(NoteOffMessage msg)
    {
        lock (this)
        {
            pitchesPressed.Remove(msg.Pitch);
        }
    }

    public void SendCtrlMsg(int valor, int cc = 11, Channel channel = Channel.Channel1)
    {
        //int ccVal = (int)(v * 127f);
        outDevice.SendControlChange(channel, (Control)cc, valor);
    }

    public void SendNoteOn(int nota, int pressure = 127, Channel channel = Channel.Channel1)
    {
        outDevice.SendNoteOn(channel, (Pitch)nota, pressure);
    }

    public void SendNoteOnWithOffSchedule(int nota, float delay, int pressure = 127, Channel channel = Channel.Channel1)
    {

        Clock clock = new Clock(120);
        clock.Schedule(new NoteOnMessage(outDevice, channel, (Pitch)nota, pressure, 0));
        clock.Schedule(new NoteOffMessage(outDevice, channel, (Pitch)nota, pressure, delay));
        clock.Start();
    }

    public void SendNoteOff(int nota, int pressure = 127, Channel channel = Channel.Channel1)
    {
        outDevice.SendNoteOff(channel, (Pitch)nota, pressure);

    }

    void OnApplicationQuit()
    {
        if (inDevice != null)
        {
            inDevice.StopReceiving();
            inDevice.Close();
        }

        if (outDevice != null)
        {
            if (outDevice.IsOpen)
            {
                outDevice.SilenceAllNotes();
                outDevice.Close();
            }
        }

    }

}

/*
 * 
 *  OutputDevice outputDevice;
    InputDevice inputDevice;

    Clock clock;

    int beatsPerMinute = 180;

    Arpeggiator arpeggiator;
    Drummer drummer;

    bool done = false;
    bool isRunning = false;


    void Start()
    {



    }
    void Start2()
    {
        if (InputDevice.InstalledDevices.Count == 0)
        {
            Debug.Log("No input devices.");
        }
        else
        {
            Debug.Log("Input Devices:");
            foreach (var device in InputDevice.InstalledDevices)
            {
                Debug.Log(device.Name);
            }
        }
        // Print a table of the output device names, or "No output devices" if there are none.
        if (OutputDevice.InstalledDevices.Count == 0)
        {
            Debug.Log("No output devices.");
        }
        else
        {
            Debug.Log("Output Devices:");
            foreach (var device in OutputDevice.InstalledDevices)
            {
                Debug.Log(device.Name);
            }
        }

        outputDevice = OutputDevice.InstalledDevices[1];
        if (outputDevice == null)
        {
            Debug.Log("No output devices, so can't run this example.");
            return;
        }
        outputDevice?.Open();

        inputDevice = InputDevice.InstalledDevices[0];
        if (inputDevice == null)
        {
            Debug.Log("No input devices, so can't run this example.");
            return;
        }

        inputDevice?.Open();

        clock = new Clock(beatsPerMinute);
        clock.Start();

        //arpeggiator = new Arpeggiator(inputDevice, outputDevice, clock);
        //drummer = new Drummer(clock, outputDevice, 4);


        inputDevice?.StartReceiving(null, true);

        Summarizer summarizer = new Summarizer(inputDevice);

        isRunning = true;

        //Debug.Log("Playing an arpeggiated C chord and then bending it down.");
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        //outputDevice.SendPitchBend(Channel.Channel1, 8192);
        //// Play C, E, G in half second intervals.
        //outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
        //Thread.Sleep(500);
        //outputDevice.SendNoteOn(Channel.Channel1, Pitch.E4, 80);
        //Thread.Sleep(500);
        //outputDevice.SendNoteOn(Channel.Channel1, Pitch.G4, 80);
        //Thread.Sleep(500);

        //// Now apply the sustain pedal.
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);

        //// Now release the C chord notes, but they should keep ringing because of the sustain
        //// pedal.
        //outputDevice.SendNoteOff(Channel.Channel1, Pitch.C4, 80);
        //outputDevice.SendNoteOff(Channel.Channel1, Pitch.E4, 80);
        //outputDevice.SendNoteOff(Channel.Channel1, Pitch.G4, 80);

        //// Now bend the pitches down.
        //for (var i = 0; i < 17; ++i)
        //{
        //    outputDevice.SendPitchBend(Channel.Channel1, 8192 - i * 450);
        //    Thread.Sleep(200);
        //}

        //// Now release the sustain pedal, which should silence the notes, then center
        //// the pitch bend again.
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        //outputDevice.SendPitchBend(Channel.Channel1, 8192);

        //Debug.Log("Playing the first two bars of Mary Had a Little Lamb...");
        //var clock = new Clock(120);
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 0));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 1));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.D4, 80, 1));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.D4, 80, 2));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.C4, 80, 2));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.C4, 80, 3));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.D4, 80, 3));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.D4, 80, 4));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 4));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 5));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 5));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 6));
        //clock.Schedule(new NoteOnMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 6));
        //clock.Schedule(new NoteOffMessage(outputDevice, Channel.Channel1, Pitch.E4, 80, 7));
        //clock.Start();
        //Thread.Sleep(5000);
        //clock.Stop();

        //Debug.Log("Playing sustained chord runs up the keyboard...");
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
        //PlayChordRun(outputDevice, new Chord("C"), 100);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
        //PlayChordRun(outputDevice, new Chord("F"), 100);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
        //PlayChordRun(outputDevice, new Chord("G"), 100);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 127);
        //PlayChordRun(outputDevice, new Chord("C"), 100);
        //Thread.Sleep(2000);
        //outputDevice.SendControlChange(Channel.Channel1, Control.SustainPedal, 0);

        //// Close the output device.
        //outputDevice.Close();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyUp(KeyCode.A))
            {
                outputDevice.SendPercussion(Percussion.BassDrum2 + UnityEngine.Random.Range(0, 45), 90);

            }
        }


        if (isRunning)
        {
            Debug.LogLine("BPM = {0}, Playing = {1}, Arpeggiator Mode = {2}",
                clock.BeatsPerMinute, clock.IsRunning, arpeggiator.Status);
            Debug.LogLine("Escape : Quit");
            Debug.LogLine("Down : Slower");
            Debug.LogLine("Up: Faster");
            Debug.LogLine("Left: Previous Chord or Scale");
            Debug.LogLine("Right: Next Chord or Scale");
            Debug.LogLine("Space = Toggle Play");
            Debug.LogLine("Enter = Toggle Scales/Chords");

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Debug.Log("down.");

                clock.BeatsPerMinute -= 2;
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                clock.BeatsPerMinute += 2;
            }
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                arpeggiator.Change(1);
            }
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                arpeggiator.Change(-1);
            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (clock.IsRunning)
                {
                    clock.Stop();
                    inputDevice?.StopReceiving();
outputDevice.SilenceAllNotes();
                }
                else
                {
                    clock.Start();
                    inputDevice?.StartReceiving(clock);
                }
            }

            if (Input.GetKeyDown(KeyCode.A))
            {
                outputDevice.SendNoteOn(Channel.Channel1, Pitch.C4, 80);
                outputDevice.SendNoteOn(Channel.Channel1, Pitch.E4, 80);
                outputDevice.SendNoteOn(Channel.Channel1, Pitch.G4, 80);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                outputDevice.SendNoteOff(Channel.Channel1, Pitch.C4, 80);
                outputDevice.SendNoteOff(Channel.Channel1, Pitch.E4, 80);
                outputDevice.SendNoteOff(Channel.Channel1, Pitch.G4, 80);
            }
            //if (Input.GetKeyDown(KeyCode.Return))
            //{
            //    arpeggiator.ToggleMode();
            //}

            //foreach (KeyCode kcode in Enum.GetValues(typeof(KeyCode)))
            //{
            //    if (Input.GetKeyDown(kcode) && (int)kcode > 96 && (int)kcode < 123)
            //    {
            //        Debug.Log(kcode.ToString());

            //        // We've hit a QUERTY key which is meant to simulate a MIDI note, so
            //        // send the Note On to the output device and tell the arpeggiator.
            //        var noteOn = new NoteOnMessage(outputDevice, 0, (Pitch)(int)kcode, 100,
            //            clock.Time);
            //        clock.Schedule(noteOn);
            //        arpeggiator.NoteOn(noteOn);
            //        // We don't get key release events for the console, so schedule a
            //        // simulated Note Off one beat from now.
            //        var noteOff = new NoteOffMessage(outputDevice, 0, (Pitch)(int)kcode, 100,
            //            clock.Time + 1);
            //        CallbackMessage.CallbackType noteOffCallback = beatTime => { arpeggiator.NoteOff(noteOff); };
            //        clock.Schedule(new CallbackMessage(beatTime => arpeggiator.NoteOff(noteOff),
            //            noteOff.Time));
            //    }
            //}


        }
    }

    void OnApplicationQuit()
{

    clock?.Stop();
    inputDevice?.StopReceiving();
    outputDevice?.SilenceAllNotes();

    outputDevice.Close();
    if (inputDevice != null)
    {
        inputDevice.Close();
        inputDevice.RemoveAllEventHandlers();
    }
}

private void PlayChordRun(OutputDevice outputDevice, Chord chord, int millisecondsBetween)
{
    var previousNote = (Pitch)(-1);
    for (var pitch = Pitch.A0; pitch < Pitch.C8; ++pitch)
    {
        if (chord.Contains(pitch))
        {
            if (previousNote != (Pitch)(-1))
            {
                outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
            }
            outputDevice.SendNoteOn(Channel.Channel1, pitch, 80);
            Thread.Sleep(millisecondsBetween);
            previousNote = pitch;
        }
    }
    if (previousNote != (Pitch)(-1))
    {
        outputDevice.SendNoteOff(Channel.Channel1, previousNote, 80);
    }
}
}


public class Arpeggiator
{
    private readonly Clock _clock;
    private readonly OutputDevice _outputDevice;
    private readonly Dictionary<Pitch, List<Pitch>> _lastSequenceForPitch;
    private int _currentChordPattern;
    private int _currentScalePattern;
    private InputDevice _inputDevice;
    private bool _playingChords;

    public Arpeggiator(InputDevice inputDevice, OutputDevice outputDevice, Clock clock)
    {
        _inputDevice = inputDevice;
        _outputDevice = outputDevice;
        _clock = clock;
        _currentChordPattern = 0;
        _currentScalePattern = 0;
        _playingChords = false;
        _lastSequenceForPitch = new Dictionary<Pitch, List<Pitch>>();

        if (inputDevice != null)
        {
            inputDevice.NoteOn += NoteOn;
            inputDevice.NoteOff += NoteOff;
        }
    }

    /// <summary>
    ///     String describing the arpeggiator's current configuration.
    /// </summary>
    public string Status
    {
        get
        {
            lock (this)
            {
                if (_playingChords)
                {
                    return "Chord: " + Chord.Patterns[_currentChordPattern].Name;
                }
                return "Scale: " + Scale.Patterns[_currentScalePattern].Name;
            }
        }
    }

    /// <summary>
    ///     Toggle between playing chords and playing scales.
    /// </summary>
    public void ToggleMode()
    {
        lock (this)
        {
            _playingChords = !_playingChords;
        }
    }

    /// <summary>
    ///     Changes the current chord or scale, whichever is the current mode.
    /// </summary>
    public void Change(int delta)
    {
        lock (this)
        {
            if (_playingChords)
            {
                _currentChordPattern = _currentChordPattern + delta;
                while (_currentChordPattern < 0)
                {
                    _currentChordPattern += Chord.Patterns.Length;
                }
                while (_currentChordPattern >= Chord.Patterns.Length)
                {
                    _currentChordPattern -= Chord.Patterns.Length;
                }
            }
            else
            {
                _currentScalePattern = _currentScalePattern + delta;
                while (_currentScalePattern < 0)
                {
                    _currentScalePattern += Scale.Patterns.Length;
                }
                while (_currentScalePattern >= Scale.Patterns.Length)
                {
                    _currentScalePattern -= Scale.Patterns.Length;
                }
            }
        }
    }

    public void NoteOn(NoteOnMessage msg)
    {
        lock (this)
        {
            var pitches = new List<Pitch>();
            if (_playingChords)
            {
                var chord = new Chord(msg.Pitch.NotePreferringSharps(),
                    Chord.Patterns[_currentChordPattern], 0);
                var p = msg.Pitch;
                for (var i = 0; i < chord.NoteSequence.Length; ++i)
                {
                    p = chord.NoteSequence[i].PitchAtOrAbove(p);
                    pitches.Add(p);
                }
            }
            else
            {
                var scale = new Scale(msg.Pitch.NotePreferringSharps(),
                    Scale.Patterns[_currentScalePattern]);
                var p = msg.Pitch;
                for (var i = 0; i < scale.NoteSequence.Length; ++i)
                {
                    p = scale.NoteSequence[i].PitchAtOrAbove(p);
                    pitches.Add(p);
                }
                pitches.Add(msg.Pitch + 12);
            }
            _lastSequenceForPitch[msg.Pitch] = pitches;
            for (var i = 1; i < pitches.Count; ++i)
            {
                _clock.Schedule(new NoteOnMessage(_outputDevice, msg.Channel,
                    pitches[i], msg.Velocity, msg.Time + i));
            }
        }
    }

    public void NoteOff(NoteOffMessage msg)
    {
        if (!_lastSequenceForPitch.ContainsKey(msg.Pitch))
        {
            return;
        }
        var pitches = _lastSequenceForPitch[msg.Pitch];
        _lastSequenceForPitch.Remove(msg.Pitch);
        for (var i = 1; i < pitches.Count; ++i)
        {
            _clock.Schedule(new NoteOffMessage(_outputDevice, msg.Channel,
                pitches[i], msg.Velocity, msg.Time + i));
        }
    }
}

public class Drummer
{
    private readonly int _beatsPerMeasure;
    private readonly Clock _clock;
    private readonly List<Message> _messagesForOneMeasure;
    private OutputDevice _outputDevice;

    public Drummer(Clock clock, OutputDevice outputDevice, int beatsPerMeasure)
    {
        _clock = clock;
        _outputDevice = outputDevice;
        _beatsPerMeasure = beatsPerMeasure;
        _messagesForOneMeasure = new List<Message>();
        for (var i = 0; i < beatsPerMeasure; ++i)
        {
            var percussion = i == 0 ? Percussion.PedalHiHat : Percussion.MidTom1;
            var velocity = i == 0 ? 100 : 40;
            _messagesForOneMeasure.Add(new PercussionMessage(outputDevice, percussion,
                velocity, i));
        }
        _messagesForOneMeasure.Add(new CallbackMessage(
            CallbackHandler, 0));
        clock.Schedule(_messagesForOneMeasure, 0);
    }

    private void CallbackHandler(float time)
    {
        // Round up to the next measure boundary.
        var timeOfNextMeasure = time + _beatsPerMeasure;
        _clock.Schedule(_messagesForOneMeasure, timeOfNextMeasure);
    }
}

public class Summarizer
{
    private InputDevice inputDevice;
    private Dictionary<Pitch, bool> pitchesPressed;

    public Summarizer(InputDevice inputDevice)
    {
        this.inputDevice = inputDevice;
        pitchesPressed = new Dictionary<Pitch, bool>();
        inputDevice.NoteOn += new InputDevice.NoteOnHandler(this.NoteOn);
        inputDevice.NoteOff += new InputDevice.NoteOffHandler(this.NoteOff);
        inputDevice.SysEx += new InputDevice.SysExHandler(this.sysexHandler);
        inputDevice.ControlChange += new InputDevice.ControlChangeHandler(this.CCHandler);
        PrintStatus();
    }

    private void PrintStatus()
    {
        // Print the currently pressed notes.
        List<Pitch> pitches = new List<Pitch>(pitchesPressed.Keys);
        pitches.Sort();
        Debug.Log("Notes: ");
        for (int i = 0; i < pitches.Count; ++i)
        {
            Pitch pitch = pitches[i];
            if (i > 0)
            {
                Debug.Log(", ");
            }
            Debug.Log(pitch.NotePreferringSharps());
            if (pitch.NotePreferringSharps() != pitch.NotePreferringFlats())
            {
                Debug.Log(pitch.NotePreferringFlats());
            }
        }
        // Print the currently held down chord.
        List<Chord> chords = Chord.FindMatchingChords(pitches);
        Debug.Log("Chords: ");
        for (int i = 0; i < chords.Count; ++i)
        {
            Chord chord = chords[i];
            if (i > 0)
            {
                Debug.Log(", ");
            }
            Debug.Log(chord);
        }

    }

    public void NoteOn(NoteOnMessage msg)
    {
        lock (this)
        {
            pitchesPressed[msg.Pitch] = true;
            Debug.Log("NoteOn channel " + msg.Channel + " Pitch " + msg.Pitch + " Velocity " + msg.Velocity);

            PrintStatus();
        }
    }

    public void NoteOff(NoteOffMessage msg)
    {
        lock (this)
        {
            pitchesPressed.Remove(msg.Pitch);
            Debug.Log("NoteOff channel " + msg.Channel + " Pitch " + msg.Pitch + " Velocity " + msg.Velocity);
            PrintStatus();
        }
    }

    public void sysexHandler(SysExMessage msg)
    {
        lock (this)
        {
            Debug.Log("sysex " + msg.Time);

        }
    }

    public void CCHandler(ControlChangeMessage msg)
    {
        lock (this)
        {
            Debug.Log("CCHandler channel " + msg.Channel + " val " + msg.Value);
        }
    }*/
