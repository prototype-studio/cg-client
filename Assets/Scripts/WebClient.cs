using System;
using System.Text.Json;
using System.Threading.Tasks;
using Core;
using UnityEngine.Networking;

public static class WebClient
{
    public static async Task<TR> Post<T, TR>(string url, T message)
    {
        var json = JsonSerializer.Serialize(message, Constants.JsonOptions);
        var bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var responseJson = request.downloadHandler.text;
                var response = JsonSerializer.Deserialize<TR>(responseJson, Constants.JsonOptions);
                return response;
            }
            else
            {
                // Handle error gracefully
                string errorMsg = $"Request failed: {request.error}";
                var errorJson = request.downloadHandler.text;
                if (!string.IsNullOrEmpty(errorJson))
                {
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ErrorResponse>(errorJson, Constants.JsonOptions);
                        if (errorResponse?.ErrorMessage != null)
                            errorMsg = errorResponse.ErrorMessage;
                    }
                    catch { /* ignore */ }
                }
                throw new Exception(errorMsg);
            }
        }
    }
}