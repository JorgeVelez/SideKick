using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.ComponentModel;
using Midi;
using UnityEngine.EventSystems;

public class MidiManager : MonoBehaviour 
{
	private static MidiManager instance;  

	public static OutputDevice outDevice;

	public bool autoConnect=false;
	public bool showGui=false;
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


	List<string> channelListBox=new List<string>();
	List<string> sysCommonListBox=new List<string>();
	List<string> sysRealtimeListBox=new List<string>();

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

	private MidiManager() {

	}

	public static MidiManager Instance 	
	{ 		
		get 		
		{ 			
			if (instance == null)
				instance = new MidiManager (); 			
			return instance; 		
		}  	
	}

	void Start () {
        
		//mainUI = transform.Find ("Canvas");

		////recibir
		//btRecibirOn = mainUI.Find ("btRecibirOn").gameObject.GetComponent<Button> ();
		//btRecibirOff = mainUI.Find ("btRecibirOff").gameObject.GetComponent<Button> ();

		//btRecibirOn.onClick.AddListener (startButton_Click);
		//btRecibirOff.onClick.AddListener (stopButton_Click);

		////mandar
		//btNoteOn = mainUI.Find ("btNoteOn").gameObject.GetComponent<Button> ();
		//btNoteOff = mainUI.Find ("btNoteOff").gameObject.GetComponent<Button> ();
		//btConnect = mainUI.Find ("btConnect").gameObject.GetComponent<Button> ();

		//slControl1 = mainUI.Find ("slControl1").gameObject.GetComponent<Slider> ();
		////slControl1.onValueChanged.AddListener (SendCtrlMsg);

		////btNoteOn.onClick.AddListener (onBtNoteOn);
		////btNoteOff.onClick.AddListener (onBtNoteOff);

		//btConnect.onClick.AddListener (Conectar);

		pitchesPressed = new Dictionary<Pitch, bool>();


		Debug.Log ("In devices --------------------------");

		foreach (InputDevice device in InputDevice.InstalledDevices)
		{
			Debug.Log (device.Name + " index: " + InputDevice.InstalledDevices.IndexOf(device));
		}

		Debug.Log ("Out devices --------------------------");
		foreach (OutputDevice device in OutputDevice.InstalledDevices)
		{
			Debug.Log (device.Name + " index: " + OutputDevice.InstalledDevices.IndexOf(device));
		}

		if (autoConnect)
			Conectar ();

		if (!showGui)
			mainUI.gameObject.SetActive (false);
	}
	
	void Conectar () {
		if(OutputDevice.InstalledDevices.Count == 0)
		{
			Debug.Log ("No MIDI output devices available."+ "Error!");
		}else{
			try
			{
				//btConnect.gameObject.SetActive(false);

				outDevice = OutputDevice.InstalledDevices[outDeviceID];
                Debug.Log("output conectado a." + outDeviceID);
                outDevice.Open();

			}
			catch(Exception ex)
			{Debug.LogError ("error: "+ex.Message.ToString());
			}
		}

		if(InputDevice.InstalledDevices.Count == 0)
		{
			Debug.Log ("No MIDI input devices available."+ "Error!");
		}else{
			try
			{
                Debug.Log("input conectado a." + inDeviceID);
                inDevice =InputDevice.InstalledDevices[inDeviceID];
				inDevice.NoteOn += new InputDevice.NoteOnHandler(this.NoteOn);
				inDevice.NoteOff += new InputDevice.NoteOffHandler(this.NoteOff);
                inDevice.ControlChange += new InputDevice.ControlChangeHandler(this.controlChangeHandler);
                inDevice.SysEx += new InputDevice.SysExHandler(this.sysexHandler);
                inDevice.ProgramChange += new InputDevice.ProgramChangeHandler(this.programChangeHandler);
                inDevice.timeCode += new InputDevice.TimeCodeHandler(this.timeCodeHandler);
                inDevice.Open();
                inDevice.StartReceiving(null, true);

                //startButton_Click();
            }
			catch(Exception ex)
			{
				Debug.Log ("error: "+ex.Message.ToString());
			}
		}
	}

	public void NoteOn(NoteOnMessage msg)
	{
		lock (this)
		{
            Debug.Log("NoteOff channel " + msg.Channel + " Pitch " + msg.Pitch + " Velocity " + msg.Velocity);

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

    public void timeCodeHandler(TimecodeMessage msg)
    {
        lock (this)
        {
            Debug.Log(msg.Time);
        }
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
            Debug.Log("NoteOff channel " + msg.Channel + " Pitch " + msg.Pitch + " Velocity " + msg.Velocity);
            pitchesPressed.Remove(msg.Pitch);
		}
	}

	public void SendCtrlMsg (int valor, int cc=11, Channel channel=Channel.Channel1) {
		//int ccVal = (int)(v * 127f);
		outDevice.SendControlChange(channel, (Control)cc, valor);
	}

	public void SendNoteOn (int nota, int pressure=127, Channel channel=Channel.Channel1) {
		outDevice.SendNoteOn(channel,  (Pitch)nota, pressure);
	}

	public void SendNoteOnWithOffSchedule (int nota, float delay,  int pressure=127, Channel channel=Channel.Channel1) {

		Clock clock = new Clock(120);
		clock.Schedule(new NoteOnMessage(outDevice, channel,  (Pitch)nota, pressure, 0));
		clock.Schedule(new NoteOffMessage(outDevice, channel,  (Pitch)nota, pressure, delay));
		clock.Start();
	}

	public void SendNoteOff (int nota, int pressure=127, Channel channel=Channel.Channel1) {
		outDevice.SendNoteOff(channel, (Pitch)nota, pressure);

	}
		
	// MIDI IN

	void OnApplicationQuit()
	{
		if(inDevice != null)
		{
			inDevice.Close();
		}

        if (outDevice != null)
        {
            if (outDevice.IsOpen)
            {
                //outDevice.SilenceAllNotes();
                outDevice.Close();
            }
        }

	}

	private void startButton_Click()
	{
		channelListBox.Clear();

		try
		{
			inDevice.StartReceiving(null);
		}
		catch(Exception ex)
		{
			Debug.Log ("error: "+ex.Message.ToString());
		}
	}

	private void stopButton_Click()
	{
		try
		{
			inDevice.StopReceiving();
		}
		catch(Exception ex)
		{
			Debug.Log ("error: "+ex.Message.ToString());
		}
	}

}
