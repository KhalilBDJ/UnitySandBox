using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using _Project.Scripts;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.AspNetCore.SignalR.Client;

public class SignalRSender : MonoBehaviour
{

    public RawImage image1;
    public RawImage image2;
    
    private SignalRConnector _connector;

    async void Start()
    {
        _connector = new SignalRConnector();
        await _connector.InitAsync();
    }

    public async void OnClick()
    {
       await SendImage(image1);
    }
    public async void OnClick2()
    {
        await SendImage(image2);
    }

    private async Task SendImage(RawImage image)
    {
        await _connector.SendImageAsync(image);
    }
}
