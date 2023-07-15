using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

public class NetworkService {
    private const string xmlApi = "https://api.openweathermap.org/data/2.5/weather?q=Warsaw&mode=xml&appid=77baa391b2670e4ce261f2653c7ed7e2";
    private const string webImage = "http://upload.wikimedia.org/wikipedia/commons/c/c5/Moraine_Lake_17092005.jpg";
    private const string localApi = "http://localhost/unity-uia/api.php";

    public IEnumerator GetWeatherXML(Action<string> callback) {
        return CallAPI(xmlApi, null, callback);
        //Method calls another 'IEnumerator' class, so it has no 'yield' (as it's nested deeper)
    }

    public IEnumerator LogWeather(string name, float cloudValue, Action<string> callback) {
        //Define a form with values to send.
        WWWForm form = new WWWForm();
        form.AddField("message", name);
        form.AddField("cloud_value", cloudValue.ToString());
        form.AddField("timestamp", DateTime.UtcNow.Ticks.ToString());

        return CallAPI(localApi, form, callback);
    }

    //Method for HTTP request. It calls callback once request is finished
    private IEnumerator CallAPI(string url, WWWForm form, Action<string> callback) {
        //POST using WWWForm or
        //GET without
        using (UnityWebRequest request = (form == null) ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, form)) {

            yield return request.Send();

            if (request.isNetworkError) {
                Debug.LogError("network problem: " + request.error);
            }
            else if (request.responseCode != (long) System.Net.HttpStatusCode.OK) {
                Debug.LogError("responser error: " + request.responseCode);
            }
            else {
                callback(request.downloadHandler.text);
            }
        }
    }

    public IEnumerator DownloadImage(Action<Texture2D> callback) {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(webImage);
        
        yield return request.Send();
        //Retrive downloaded image using the DownloadHandler utility
        callback(DownloadHandlerTexture.GetContent(request));
    }
}
