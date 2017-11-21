using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using ApiAiSDK;
using ApiAiSDK.Unity;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

using System.Text;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class BotEngine : MonoBehaviour {
    private ApiAiUnity apiAiUnity;

    public LoadMarkerAtLatLong filterAgeThing;
    public GameObject chart;

    [SerializeField]
    private Text Recognitions;

    private DictationRecognizer DictationRecognizer;

    public string getStringInBetween(string outText, string targetBegin, string targetEnd) {
        int from = outText.IndexOf(targetBegin) + targetBegin.Length;
        int to = outText.LastIndexOf(targetEnd);

        return outText.Substring(from, to - from);
    }

    // Use this for initialization
    void Start () {
        Debug.Log("API AI Initialized.");

        const string ACCESS_TOKEN = "c1cb6e5396d04d6e8b1215ac0cfd55ac";

        var config = new AIConfiguration(ACCESS_TOKEN, SupportedLanguage.English);

        apiAiUnity = new ApiAiUnity();
        apiAiUnity.Initialize(config);

        //SendText("Do you know what is the definition of insanity is?");
        Recognitions.text = "";

        DictationRecognizer = new DictationRecognizer();

        DictationRecognizer.DictationResult += (text, confidence) =>
        {
            Debug.LogFormat("Dictation result: {0}", text);
            Recognitions.text = text;
            SendText(text);
        };

        DictationRecognizer.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
        };

        DictationRecognizer.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
        };

        DictationRecognizer.Start();

        WindowsVoice.speak("Welcome to Visual Flow!");
    }

    public void SendText(string text) {
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        };

        try {
            var response = apiAiUnity.TextRequest(text);
            if (response != null) {
                ProcessOutput(response);
            } else {
                Debug.LogError("Response is null");
            }
        }
        catch (Exception ex) {
            Debug.LogException(ex);
        }
    }

    public void ProcessOutput(ApiAiSDK.Model.AIResponse res) {
        string outText = JsonConvert.SerializeObject(res);
        Debug.Log(outText);

        string action = res.Result.Action;
        string speech = res.Result.Fulfillment.Speech;

        switch (action) {
            case "input.filterage":
                filterAgeThing.geoMarkers.RemoveHalf();
                WindowsVoice.speak(speech);
                break;
            case "input.speech":
                WindowsVoice.speak(speech);
                break;
            case "input.charton":
                WindowsVoice.speak(speech);
                chart.SetActive(true);
                break;
            case "input.chartoff":
                WindowsVoice.speak(speech);
                chart.SetActive(false);
                break;
            default:
                break;
        }
    }
}
