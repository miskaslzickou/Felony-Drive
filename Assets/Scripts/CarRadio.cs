using UnityEngine;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CarRadio : MonoBehaviour
{
    [System.Serializable]
    public class RadioStation
    {
        public string name;
        public AudioClip clip;
    }

    delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

    public UIData uiData;
    public AudioSource radioSound;
    private string currentTrackInfo = "";

    private bool isSearching = false;
    private float updateTimer = 0f;
    private float checkInterval = 2f;

    void Start()
    {

    }

    async void FetchSpotifyTrackAsync()
    {
        isSearching = true;
        string result = await Task.Run(() => GetSpotifyTrackNative());
        currentTrackInfo = result;
        isSearching = false;
    }

    string GetSpotifyTrackNative()
    {
        Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");
        if (spotifyProcesses.Length == 0) return "RÁDIO VYPNUTO";

        HashSet<uint> pids = new HashSet<uint>();
        foreach (var p in spotifyProcesses)
        {
            pids.Add((uint)p.Id);
        }

        string trackInfo = "NIC NEHRAJE";

        EnumWindows((hWnd, lParam) =>
        {
            GetWindowThreadProcessId(hWnd, out uint windowPid);

            if (pids.Contains(windowPid))
            {
                int length = GetWindowTextLength(hWnd);
                if (length > 0)
                {
                    StringBuilder sb = new StringBuilder(length + 1);
                    GetWindowText(hWnd, sb, sb.Capacity);
                    string title = sb.ToString();

                    if (title != "Spotify" && title != "Spotify Premium" && title != "GDI+ Window (Spotify.exe)" && title != "Default IME"&& title != "MSCTFIME UI")
                    {
                        trackInfo = title;
                        return false;
                    }
                    else
                        trackInfo = "";
                }
            }
            return true;
        }, IntPtr.Zero);

        return trackInfo;
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if (updateTimer >= checkInterval)
        {
            updateTimer = 0f;
            if (!isSearching)
            {
                FetchSpotifyTrackAsync();
            }
        }

        if (uiData != null)
        {
            uiData.radioTrack = currentTrackInfo;
        }
    }
}