using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;
using TMPro;
using Unity.VisualScripting;

public class SignalRSender : MonoBehaviour
{

    
    public RawImage image1;
    public RawImage image2;
    public TMP_Text currentTimer;

    [SerializeField] private int maxTimer;
    
    private SignalRConnector _connector;
    private HubConnection _connexionImage;
    private HubConnection _connexionSession;
    private string _session;
    private bool _cguAccepted;
    private int _timer;
    private Coroutine _timerCoroutine;
    
    //Testing
    private bool _timerStarted;
    private bool _qrCodeScanned;
    
    
    async void Start()
    {
        image1.enabled = false;
        image2.enabled = false;
        _timerStarted = false;
        _qrCodeScanned = false;
        _cguAccepted = false;
        _timerCoroutine = null;
        await InitAsync();
    }

    

    public async void OnClick()
    {
       await SendImageAsync(image1);
    }
    public async void OnClick2()
    {
        await SendImageAsync(image2);
    }

    public void AcceptCGUs()
    {
        _cguAccepted = true;
    }

    public void ScanQRCode()
    {
        _qrCodeScanned = true;
        StartCoroutine(StartTimer());
    }
    
    
    private async Task StartConnectionAsync()
    {
        try
        {
            await _connexionImage.StartAsync();
            await _connexionSession.StartAsync();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error {ex.Message}");
        }
    }
    
    public async Task InitAsync()
    {
        
        //Initialise les connexions pour les Hubs
        _connexionImage = new HubConnectionBuilder()
            .WithUrl("https://localhost:7187/Image")
            .Build();
        _connexionImage.On<byte[], string>("ReceiveImage", (imag, sess) =>
        {
            Debug.Log("OUIIIIIi");
        });
        _connexionSession = new HubConnectionBuilder()
            .WithUrl("https://localhost:7187/Session")
            .Build();
        
        //Si je reçois l'id, alors je le stock
        _connexionSession.On<string>("ReceiveSessionId", session =>
        {
            _session = session;
            Debug.Log(session);
        });
        
        //Si on m'autorise à commencer le timer, alors je lance une coroutine
        _connexionSession.On<string>("ReceiveTimerCanBegin", session =>
        {
            _timerCoroutine = StartCoroutine( StartTimer());
        });
        
        //Tant que je n'ai pas reçu la confirmation que les CGUs ont été acceptées, je continue le timer
        _connexionSession.On("ReceiveCGUsAccepted", () =>
        {
            image1.enabled = true;
            image2.enabled = true;
            StopCoroutine(_timerCoroutine);
        });
        
        await StartConnectionAsync();
    }
    
    private IEnumerator StartTimer()
    {
        _timer = 0;
        while (_timer<maxTimer)
        {
            currentTimer.text = _timer.ToString();
            _timer++;
            Debug.Log(_timer);
            
           
            /*if (_cguAccepted)
            {
                image1.enabled = true;
                image2.enabled = true;
                yield break;
            }*/
            
            yield return new WaitForSeconds(1);
        }
        //Si le timer est terminé, alors je préviens le back
        _connexionSession.InvokeAsync<string>("SendEndTimer", _session);
        yield return null;
    }
    
    public async Task SendImageAsync(RawImage image)
    {
        try
        {
            byte[] img = ((Texture2D) image.texture).EncodeToJPG();
            
            //J'envoie l'image et l'id de session au back
            await _connexionImage.InvokeAsync("SendImage", img, _session);
            Debug.Log("L'image envoyé appartient à la session :" + _session);
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error {ex.Message}");
        }
    }
    
    
}
