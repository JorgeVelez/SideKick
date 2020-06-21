using System.Threading;
using Midi;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class AppManager : MonoBehaviour
{

    Transform MainUI;

    Dropdown MidiInDD;
    Dropdown MidiOutDD;

    void Start()
    {
        MainUI = GameObject.Find("Canvas").transform;

        MidiInDD = MainUI.Find("Header/MidiInDD").GetComponent<Dropdown>();
        MidiOutDD = MainUI.Find("Header/MidiOutDD").GetComponent<Dropdown>();


        MidiInDD.onValueChanged.AddListener(MidiInDDOnValueChanged);
        MidiInDD.options.Clear();       
        MidiInDD.options.Add(new Dropdown.OptionData("Choose Input Device"));
        foreach (InputDevice device in InputDevice.InstalledDevices)
        {
            MidiInDD.options.Add(new Dropdown.OptionData(device.Name));
        }
        MidiInDD.RefreshShownValue();



        MidiOutDD.onValueChanged.AddListener(MidiOutDDOnValueChanged);
        MidiOutDD.options.Clear();
        MidiOutDD.options.Add(new Dropdown.OptionData("Choose Output Device"));

        foreach (OutputDevice device in OutputDevice.InstalledDevices)
        {
            MidiOutDD.options.Add(new Dropdown.OptionData(device.Name));
        }
        MidiOutDD.RefreshShownValue();

        if (PlayerPrefs.HasKey("MidiOutSelectedDevice"))
        {
            foreach (Dropdown.OptionData od in MidiOutDD.options)
            {
                if (od.text == PlayerPrefs.GetString("MidiOutSelectedDevice"))
                {
                    MidiOutDD.value = MidiOutDD.options.IndexOf(od);
                }
            }
        }

        if (PlayerPrefs.HasKey("MidiInSelectedDevice"))
        {
            foreach (Dropdown.OptionData od in MidiInDD.options)
            {
                if (od.text == PlayerPrefs.GetString("MidiInSelectedDevice"))
                {
                    MidiInDD.value = MidiInDD.options.IndexOf(od);
                }
            }
        }


    }

    private void MidiOutDDOnValueChanged(int val)
    {
        if (val > 0)
        {

            Debug.Log("midi out selected " + OutputDevice.InstalledDevices[val-1].Name);
            PlayerPrefs.SetString("MidiOutSelectedDevice", OutputDevice.InstalledDevices[val - 1].Name);
            MidiManager.Instance.ConectarOut(val - 1);
        }
    }

    private void MidiInDDOnValueChanged(int val)
    {
        if (val > 0)
        {
            Debug.Log("midi in selected " + InputDevice.InstalledDevices[val-1].Name);

            PlayerPrefs.SetString( "MidiInSelectedDevice", InputDevice.InstalledDevices[val - 1].Name);
            MidiManager.Instance.ConectarIn(val - 1);

        }
    }
}



