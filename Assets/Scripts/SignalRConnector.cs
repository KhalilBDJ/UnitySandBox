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
        private HubConnection _connection;
        public Action<RawImage> OnImageReceived;
        public async Task InitAsync()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7187/Image")
                .Build();
            _connection.On<Byte[]>("ReceiveImage", image => 
            {
                Debug.Log(image);
            });
            await StartConnectionAsync();
        }
        public async Task SendImageAsync(RawImage image)
        {
            try
            {
                Byte[] img = ((Texture2D) image.texture).EncodeToJPG();
                await _connection.InvokeAsync<Byte[]>("SendImage",
                    img);
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
                await _connection.StartAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error {ex.Message}");
            }
        }
    }
}