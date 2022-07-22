using UnityEngine.UI;

namespace _Project.Scripts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UnityEngine;
    using Microsoft.AspNetCore.SignalR.Client;

    public class SignalRConnector
    {
        private HubConnection _connexionImage;
        private HubConnection _connexionSession;
        private string _session;
        public async Task InitAsync()
        {
            _connexionImage = new HubConnectionBuilder()
                .WithUrl("https://localhost:7187/Image")
                .Build();
            _connexionImage.On<byte[]/*, string*/>("ReceiveImage", (image/*, _session*/) => 
            {
                Debug.Log(image);
                //Debug.Log(_session);
            });
            
            
            _connexionSession = new HubConnectionBuilder()
                .WithUrl("https://localhost:7187/Session")
                .Build();
            _connexionSession.On<string>("ReceiveSessionId", session =>
            {
                _session = session;
                Debug.Log(session);
            });
            
            _connexionSession.On<string>("ReceiveTimerCanBegin", session =>
            {
                StartTimer(30);
            });
            
            
            
            await StartConnectionAsync();
        }

       
        public async Task SendImageAsync(RawImage image)
        {
            try
            {
                byte[] img = ((Texture2D) image.texture).EncodeToJPG();
                Image test = new Image(img, _session);
                await _connexionImage.InvokeAsync("SendImage",
                    test.image/*, test.sessionId*/);
                Debug.Log(test.sessionId);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error {ex.Message}");
            }
        }

        public async Task Accept()
        {
            try
            {
                await _connexionImage.InvokeAsync("CGUAccepted");
                Debug.Log("cgu accepted");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error {ex.Message}");
            }
            
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

        private IEnumerable StartTimer(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
    }
}