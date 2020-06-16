using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Linq;
using System.Text;

public delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

public class UsbDetector : Singleton<UsbDetector>
{
    public delegate void UsbDetectorAction();
    public event UsbDetectorAction OnUSBConnected;
    public event UsbDetectorAction OnUSBDisconnected;
    //OnDataReceived?.Invoke();

    public IntPtr interactionWindow;
    IntPtr hMainWindow;
    IntPtr oldWndProcPtr;
    IntPtr newWndProcPtr;
    WndProcDelegate newWndProc;
    bool isrunning = false;

    [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
    public static extern System.IntPtr GetForegroundWindow();

    [DllImport("user32.dll")]
    static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll")]
    static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    public const int DbtDevicearrival = 0x8000; // system detected a new device        
    public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
    public const int WmDevicechange = 0x0219; // device change event
    public const int DbtConfigChanged = 0x0018;
    public const int DbtDeviceSpecific = 0x8005;
    public const int DbtDevnodesChanged = 0x0007;
    private const int DbtDevtypDeviceinterface = 5;
    private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB 

    void Start()
    {
        if (isrunning) return;

        hMainWindow = GetForegroundWindow();
        newWndProc = new WndProcDelegate(wndProc);
        newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
        oldWndProcPtr = SetWindowLongPtr(hMainWindow, -4, newWndProcPtr);
        isrunning = true;
    }

    private static IntPtr StructToPtr(object obj)
    {
        var ptr = Marshal.AllocHGlobal(Marshal.SizeOf(obj));
        Marshal.StructureToPtr(obj, ptr, false);
        return ptr;
    }
    IntPtr wndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
    {

        switch (msg)
        {
            case /*WM_MINE*/0:
                onMyEvent(wParam, lParam);
                break;
            case WmDevicechange:
                switch ((int)wParam)
                {
                    case DbtDevicearrival:
                        
                         int devType = Marshal.ReadInt32(lParam, 4);
                        Debug.Log("<color=green>DbtDevicearrival " + devType + "</color>");
                        //if (devType == DBT_DEVTYP_VOLUME)
                         {
                            
                         }

                        break;

                    case DbtDeviceremovecomplete:
                        Debug.Log("<color=green>DbtDeviceremovecomplete " + msg + "</color>");
                        break;

                }
                break;
        }
        return CallWindowProc(IntPtr.Zero, hWnd, msg, wParam, lParam);
    }
    void onMyEvent(IntPtr wParam, IntPtr lParam)
    {
        //do stuff here
    }

    void OnDisable()
    {
        Debug.Log("Uninstall Hook");
        if (!isrunning) return;
        SetWindowLongPtr(hMainWindow, -4, oldWndProcPtr);
        hMainWindow = IntPtr.Zero;
        oldWndProcPtr = IntPtr.Zero;
        newWndProcPtr = IntPtr.Zero;
        newWndProc = null;
        isrunning = false;
    }
}