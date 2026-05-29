//Michal Mikuš, 3C, PVA, Felony Drive
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class CarRadio : MonoBehaviour
{
    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int MediaPlugin_Init();

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_Shutdown();

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern int MediaPlugin_Refresh();

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_GetTitle(StringBuilder buf, int bufSize);

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_GetArtist(StringBuilder buf, int bufSize);

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_GetAlbum(StringBuilder buf, int bufSize);

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_TogglePlayPause();

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_Next();

    [DllImport("MediaPlugin", CallingConvention = CallingConvention.Cdecl)]
    private static extern void MediaPlugin_Prev();

    public enum RadioStation
    {
        Mirror, InGame1,
    //    InGame2
    }

    public RadioStation currentStation = RadioStation.Mirror;
    
    public UIData UIData;
    public AudioSource radioSound;
    private string currentTrackInfo = "";
    private CancellationTokenSource _loopCts;
    private bool _isPluginInitialized = false;

    void Start()
    {
        if (MediaPlugin_Init() == 1) _isPluginInitialized = true;
        ChangeStation(currentStation);
    }
    private void StartMirrorLoop()
    {
        if (!_isPluginInitialized) return;

        
        _loopCts = new CancellationTokenSource();

       
        Task.Run(() => SpotifyPollingLoop(_loopCts.Token));
    }
    private void StopMirrorLoop()
    {
        if (_loopCts != null)
        {
            _loopCts.Cancel(); // Tohle okamžitě pošle signál do smyčky, ať skončí
            _loopCts.Dispose();
            _loopCts = null;
        }
    }
    public void PlayPause()
    {
        if (currentStation != RadioStation.Mirror) return;
        // Odhození do pozadí, aby Unity nečekalo na odpověď z Windows
        Task.Run(() => MediaPlugin_TogglePlayPause());
    }
    public void NextSong(int direction)
    {
        if (currentStation != RadioStation.Mirror) return;

        // Odhození do pozadí
        Task.Run(() =>
        {
            if (direction > 0)
                MediaPlugin_Next();
            else
                MediaPlugin_Prev();
        });
    }
    private void ChangeStation(RadioStation newStation)
    {
        switch (currentStation)
        {
          
            case RadioStation.Mirror:
                radioSound.Stop();
                currentTrackInfo = "Načítám informace z Mirroru...";
                UIData.radioChannel = "Mirror";

                StartMirrorLoop();
                
                break;

            case RadioStation.InGame1:
                StopMirrorLoop();
                UIData.radioChannel = "Stanice 1";
                currentTrackInfo = "Hraje interní stanice 1.";

                
               
                radioSound.clip = Resources.Load<AudioClip>("Radio/InGame1/song1");
                radioSound.Play();
                break;

            //case RadioStation.InGame2:
            //    UIData.radioChannel = "Stanice 2";
            //    currentTrackInfo = "Hraje interní stanice 2.";
            //    radioSound.Play();
            //    radioSound.clip = Resources.Load<AudioClip>("Radio/InGame2/" + "song1");
            //    break;
        }
    }
    public void NextStation(int direction)
    {
        var stations = (RadioStation[])System.Enum.GetValues(typeof(RadioStation));

        int nextIndex = (((int)currentStation + direction) % stations.Length + stations.Length) % stations.Length;

        currentStation = (RadioStation)nextIndex;
        ChangeStation(currentStation);
    }
    private async Task SpotifyPollingLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            int result = MediaPlugin_Refresh();

            if (result == 1)
            {
                StringBuilder titleBuf = new StringBuilder(512);
                StringBuilder artistBuf = new StringBuilder(512);
                MediaPlugin_GetTitle(titleBuf, 512);
                MediaPlugin_GetArtist(artistBuf, 512);

               
                string title = titleBuf.ToString();
                string artist = artistBuf.ToString();

                currentTrackInfo = artist + " - " + title;
            }

            try
            {
                // Čekání s podporou zrušení
                await Task.Delay(2000, token);
            }
            catch (TaskCanceledException)
            {
                // To je v pořádku, loop byl ukončen
                break;
            }
        }
        Debug.Log("Smyčka pro Mirror byla bezpečně ukončena.");
    }

    void Update()
    {
        if (UIData != null)
        {
            if (currentTrackInfo.Length > 28)
            {
                UIData.radioTrack = currentTrackInfo.Substring(0, 25) + "...";
            }
            else
            {
                UIData.radioTrack = currentTrackInfo;
            }
        }
    }
    void OnDestroy()
    {
        // Správné ukončení pluginu při zničení objektu/vypnutí hry
        StopMirrorLoop();
        if (_isPluginInitialized) MediaPlugin_Shutdown();
    }
}