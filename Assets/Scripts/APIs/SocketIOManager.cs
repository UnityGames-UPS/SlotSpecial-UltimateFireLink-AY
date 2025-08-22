using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using DG.Tweening;
using System.Linq;
using Newtonsoft.Json;
using Best.SocketIO;
using Best.SocketIO.Events;
using Newtonsoft.Json.Linq;
using System.Runtime.Serialization;
using Best.HTTP.Shared;

public class SocketIOManager : MonoBehaviour
{
    [SerializeField]
    private SlotBehaviour slotManager;

    [SerializeField]
    private UIManager uiManager;
    internal GameData InitialData = null;
    internal UiData UIData = null;
    internal Root ResultData = null;
    internal Player PlayerData = null;
    internal Root GambleData = null;
    internal Features InitFeature = null;
    internal List<List<int>> LineData = null;


    [SerializeField] internal List<string> bonusdata = null;
    //WebSocket currentSocket = null;
    internal bool isResultdone = false;

    protected string nameSpace = "playground"; //BackendChanges
    private Socket gameSocket; //BackendChanges

    private SocketManager manager;

    [SerializeField] internal JSFunctCalls JSManager;


    protected string SocketURI = null;
    [SerializeField] protected string TestSocketURI = "https://sl3l5zz3-5000.inc1.devtunnels.ms/";
    //protected string TestSocketURI = "https://mx2md3l5-5000.inc1.devtunnels.ms/";
    // protected string TestSocketURI = "https://fuzzy-space-happiness-6jv566jpv6jhrw5v-5000.app.github.dev/";
    // protected string TestSocketURI = "https://6m8kh503-5000.inc1.devtunnels.ms/";
    [SerializeField]
    private string testToken;

    protected string gameID = "SL-FLC";
    // protected string gameID = "";

    internal bool isLoaded = false;

    internal bool SetInit = false;

    private bool isConnected = false; //Back2 Start
    private bool hasEverConnected = false;
    private const int MaxReconnectAttempts = 5;
    private const float ReconnectDelaySeconds = 2f;

    private float lastPongTime = 0f;
    private float pingInterval = 2f;
    private float pongTimeout = 3f;
    private bool waitingForPong = false;
    private int missedPongs = 0;
    private const int MaxMissedPongs = 5;
    private Coroutine PingRoutine; //Back2 end

    private void Awake()
    {
        //Debug.unityLogger.logEnabled = false;
        isLoaded = false;
        SetInit = false;

    }

    private void Start()
    {
        //OpenWebsocket();
        OpenSocket();
    }

    void ReceiveAuthToken(string jsonData)
    {
        Debug.Log("Received data: " + jsonData);

        // Parse the JSON data
        var data = JsonUtility.FromJson<AuthTokenData>(jsonData);
        SocketURI = data.socketURL;
        myAuth = data.cookie;
        nameSpace = data.nameSpace; //BackendChanges

        // Proceed with connecting to the server using myAuth and socketURL
    }

    string myAuth = null;

    private void OpenSocket()
    {
        // Create and setup SocketOptions
        SocketOptions options = new SocketOptions(); //Back2 Start
        options.AutoConnect = false;
        options.Reconnection = false;
        options.Timeout = TimeSpan.FromSeconds(3); //Back2 end
        options.ConnectWith = Best.SocketIO.Transports.TransportTypes.WebSocket; //BackendChanges
#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("authToken");
        StartCoroutine(WaitForAuthToken(options));
#else
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = testToken
            };
        };
        options.Auth = authFunction;
        // Proceed with connecting to the server
        SetupSocketManager(options);
#endif


        //#if UNITY_WEBGL && !UNITY_EDITOR
        //    string url = Application.absoluteURL;
        //    Debug.Log("Unity URL : " + url);
        //    ExtractUrlAndToken(url);

        //    Func<SocketManager, Socket, object> webAuthFunction = (manager, socket) =>
        //    {
        //      return new
        //      {
        //        token = testToken,
        //      };
        //    };
        //    options.Auth = webAuthFunction;
        //#else
        //        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        //        {
        //            return new
        //            {
        //                token = testToken,
        //            };
        //        };
        //        options.Auth = authFunction;
        //#endif
        //        // Proceed with connecting to the server
        //        SetupSocketManager(options);

    }

    public void ExtractUrlAndToken(string fullUrl)
    {
        Uri uri = new Uri(fullUrl);
        string query = uri.Query; // Gets the query part, e.g., "?url=http://localhost:5000&token=e5ffa84216be4972a85fff1d266d36d0"

        Dictionary<string, string> queryParams = new Dictionary<string, string>();
        string[] pairs = query.TrimStart('?').Split('&');

        foreach (string pair in pairs)
        {
            string[] kv = pair.Split('=');
            if (kv.Length == 2)
            {
                queryParams[kv[0]] = Uri.UnescapeDataString(kv[1]);
            }
        }

        if (queryParams.TryGetValue("url", out string extractedUrl) &&
            queryParams.TryGetValue("token", out string token))
        {
            Debug.Log("Extracted URL: " + extractedUrl);
            Debug.Log("Extracted Token: " + token);
            testToken = token;
            SocketURI = extractedUrl;
        }
        else
        {
            Debug.LogError("URL or token not found in query parameters.");
        }
    }
    private IEnumerator WaitForAuthToken(SocketOptions options)
    {
        // Wait until myAuth is not null
        while (myAuth == null)
        {
            Debug.Log("My Auth is null");
            yield return null;
        }
        while (SocketURI == null)
        {
            Debug.Log("My Socket is null");
            yield return null;
        }
        Debug.Log("My Auth is not null");
        // Once myAuth is set, configure the authFunction
        Func<SocketManager, Socket, object> authFunction = (manager, socket) =>
        {
            return new
            {
                token = myAuth

            };
        };
        options.Auth = authFunction;

        Debug.Log("Auth function configured with token: " + myAuth);

        // Proceed with connecting to the server
        SetupSocketManager(options);
    }

    private void SetupSocketManager(SocketOptions options)
    {
        // Create and setup SocketManager
#if UNITY_EDITOR
        this.manager = new SocketManager(new Uri(TestSocketURI), options);
#else
        this.manager = new SocketManager(new Uri(SocketURI), options);
#endif
        if (string.IsNullOrEmpty(nameSpace))
        {  //BackendChanges Start
            gameSocket = this.manager.Socket;
        }
        else
        {
            print("nameSpace: " + nameSpace);
            gameSocket = this.manager.GetSocket("/" + nameSpace);
        }
        // Set subscriptions
        gameSocket.On<ConnectResponse>(SocketIOEventTypes.Connect, OnConnected);
        gameSocket.On(SocketIOEventTypes.Disconnect, OnDisconnected); //Back2 Start
        gameSocket.On(SocketIOEventTypes.Error, OnError); //Back2 Start
        gameSocket.On<string>("game:init", OnListenEvent);
        gameSocket.On<string>("result", OnResult);
        gameSocket.On<bool>("socketState", OnSocketState);
        gameSocket.On<string>("internalError", OnSocketError);
        gameSocket.On<string>("alert", OnSocketAlert);
        gameSocket.On<string>("pong", OnPongReceived); //Back2 Start
        gameSocket.On<string>("AnotherDevice", OnSocketOtherDevice); //BackendChanges Finish
        manager.Open();
    }

    // Connected event handler implementation
    void OnConnected(ConnectResponse resp) //Back2 Start
    {
        Debug.Log("✅ Connected to server.");

        if (hasEverConnected)
        {
            uiManager.CheckAndClosePopups();
        }

        isConnected = true;
        hasEverConnected = true;
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        SendPing();
    } //Back2 end
    private void OnError()
    {
        Debug.LogError("Socket Error");
    }
    private void OnDisconnected() //Back2 Start
    {
        Debug.LogWarning("⚠️ Disconnected from server.");
        isConnected = false;
        ResetPingRoutine();
    } //Back2 end
    private void OnPongReceived(string data) //Back2 Start
    {
        Debug.Log("✅ Received pong from server.");
        waitingForPong = false;
        missedPongs = 0;
        lastPongTime = Time.time;
        Debug.Log($"⏱️ Updated last pong time: {lastPongTime}");
        Debug.Log($"📦 Pong payload: {data}");
    } //Back2 end

    private void OnError(string response)
    {
        Debug.LogError("Error: " + response);
    }

    private void OnListenEvent(string data)
    {
        // Debug.Log("Received some_event with data: " + data);
        ParseResponse(data);
    }

    private void OnSocketState(bool state)
    {
        if (state)
        {
            Debug.Log("my state is " + state);
        }
        else
        {

        }
    }
    private void OnSocketError(string data)
    {
        Debug.Log("Received error with data: " + data);
    }
    private void OnSocketAlert(string data)
    {
        Debug.Log("Received alert with data: " + data);
    }
    void OnResult(string data)
    {
        ParseResponse(data);
    }
    private void OnSocketOtherDevice(string data)
    {
        Debug.Log("Received Device Error with data: " + data);
        uiManager.ADfunction();
    }

    private void SendPing() //Back2 Start
    {
        ResetPingRoutine();
        PingRoutine = StartCoroutine(PingCheck());
    }
    void ResetPingRoutine()
    {
        if (PingRoutine != null)
        {
            StopCoroutine(PingRoutine);
        }
        PingRoutine = null;
    }

    private IEnumerator PingCheck()
    {
        while (true)
        {
            Debug.Log($"🟡 PingCheck | waitingForPong: {waitingForPong}, missedPongs: {missedPongs}, timeSinceLastPong: {Time.time - lastPongTime}");

            if (missedPongs == 0)
            {
                uiManager.CheckAndClosePopups();
            }

            // If waiting for pong, and timeout passed
            if (waitingForPong)
            {
                if (missedPongs == 2)
                {
                    uiManager.ReconnectionPopup();
                }
                missedPongs++;
                Debug.LogWarning($"⚠️ Pong missed #{missedPongs}/{MaxMissedPongs}");

                if (missedPongs >= MaxMissedPongs)
                {
                    Debug.LogError("❌ Unable to connect to server — 5 consecutive pongs missed.");
                    isConnected = false;
                    uiManager.DisconnectionPopup();
                    yield break;
                }
            }

            // Send next ping
            waitingForPong = true;
            lastPongTime = Time.time;
            Debug.Log("📤 Sending ping...");
            SendDataWithNamespace("ping");
            yield return new WaitForSeconds(pingInterval);
        }
    } //Back2 end

    private void AliveRequest()
    {
        SendDataWithNamespace("YES I AM ALIVE");
    }

    private void SendDataWithNamespace(string eventName, string json = null)
    {
        // Send the message
        if (gameSocket != null && gameSocket.IsOpen) //BackendChanges
        {
            if (json != null)
            {
                gameSocket.Emit(eventName, json);
                Debug.Log("JSON data sent: " + json);
            }
            else
            {
                gameSocket.Emit(eventName);
            }
        }
        else
        {
            Debug.LogWarning("Socket is not connected.");
        }
    }



    internal IEnumerator CloseSocket() //Back2 Start
    {
        uiManager.RaycastBlocker.SetActive(true);
        ResetPingRoutine();

        Debug.Log("Closing Socket");

        manager?.Close();
        manager = null;

        Debug.Log("Waiting for socket to close");

        yield return new WaitForSeconds(0.5f);

        Debug.Log("Socket Closed");

#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("OnExit"); //Telling the react platform user wants to quit and go back to homepage
#endif
    } //Back2 end


    internal void ReactNativeCallOnFailedToConnect() //BackendChanges
    {
#if UNITY_WEBGL && !UNITY_EDITOR
    JSManager.SendCustomMessage("onExit");
#endif
    }

    private void ParseResponse(string jsonObject)
    {

        Debug.Log(jsonObject);
        Root myData = JsonConvert.DeserializeObject<Root>(jsonObject);

        string id = myData.id;

        switch (id)
        {
            case "initData":
                {
                    InitialData = myData.gameData;
                    UIData = myData.uiData;
                    InitFeature = myData.features;
                    PlayerData = myData.player;
                    // bonusdata = GetBonusData(myData.gameData.spinBonus);

                    if (!SetInit)
                    {
                        Debug.Log(jsonObject);
                        List<string> LinesString = ConvertListListIntToListString(InitialData.lines);
                        //List<string> InitialReels = ConvertListOfListsToStrings(InitialData.Reel);
                        //InitialReels = RemoveQuotes(InitialReels);
                        PopulateSlotSocket();
                        SetInit = true;
                    }
                    else
                    {
                        RefreshUI();
                    }
                    break;
                }
            case "ResultData":
                {
                    Debug.Log(jsonObject);
                    // myData.message.GameData.FinalResultReel = ConvertListOfListsToStrings(myData.message.GameData.ResultReel);
                    // myData.message.GameData.FinalsymbolsToEmit = TransformAndRemoveRecurring(myData.message.GameData.symbolsToEmit);
                    ResultData = myData;
                    PlayerData = myData.player;
                    isResultdone = true;
                    break;
                }

            case "ExitUser":
                {
                    if (this.manager != null) //BackendChanges
                    {
                        Debug.Log("Dispose my Socket");
                        this.manager.Close();
                    }
                    // Application.ExternalCall("window.parent.postMessage", "onExit", "*");
#if UNITY_WEBGL && !UNITY_EDITOR
                        JSManager.SendCustomMessage("onExit");
#endif
                    break;
                }
        }
    }


    private void RefreshUI()
    {
        uiManager.InitialiseUIData(UIData.paylines);
    }

    private void PopulateSlotSocket()
    {
        uiManager.RaycastBlocker.SetActive(false);
        slotManager.shuffleInitialMatrix();

        slotManager.SetInitialUI();

        isLoaded = true;
#if UNITY_WEBGL && !UNITY_EDITOR
        JSManager.SendCustomMessage("OnEnter");
#endif
    }

    internal void AccumulateResult(string typex)
    {
        isResultdone = false;
        MessageData message = new MessageData();
        message.payload = new SentDeta();
        message.type = typex;
        Debug.Log(slotManager.BetCounter);
        message.payload.betIndex = slotManager.BetCounter;
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);
    }
    internal void AcumulateFreeSpin(int selectedButtonindex)
    {
        MessageData message = new MessageData();
        message.payload = new SentDeta();

        message.type = "FREESPIN";
        Debug.Log(selectedButtonindex + "     ()");
        message.payload.option = selectedButtonindex;
        message.payload.Event = "FREESPINOPTION";
        // Serialize message data to JSON
        string json = JsonUtility.ToJson(message);
        SendDataWithNamespace("request", json);
        //var message = new { type = "FREESPINOPTION", option = selectedButtonindex };
        //string json = JsonConvert.SerializeObject(message);
        //SendDataWithNamespace("request", json);
        //Debug.Log("Dev_Test:  " + json);
        //Debug.Log(JsonConvert.SerializeObject(message));

    }
    private List<string> RemoveQuotes(List<string> stringList)
    {
        for (int i = 0; i < stringList.Count; i++)
        {
            stringList[i] = stringList[i].Replace("\"", ""); // Remove inverted commas
        }
        return stringList;
    }

    private List<string> ConvertListListIntToListString(List<List<int>> listOfLists)
    {
        List<string> resultList = new List<string>();

        foreach (List<int> innerList in listOfLists)
        {
            // Convert each integer in the inner list to string
            List<string> stringList = new List<string>();
            foreach (int number in innerList)
            {
                stringList.Add(number.ToString());
            }

            // Join the string representation of integers with ","
            string joinedString = string.Join(",", stringList.ToArray()).Trim();
            resultList.Add(joinedString);
        }

        return resultList;
    }
    internal List<int> ConvertListStringToListInt(List<string> stringList)
    {
        List<int> intList = new List<int>();

        foreach (string str in stringList)
        {
            if (int.TryParse(str, out int number))
            {
                intList.Add(number);
            }
            else
            {
                throw new FormatException($"Invalid integer value: {str}");
            }
        }

        return intList;
    }

}

[Serializable]
public class MessageData
{
    public string type;

    public SentDeta payload;

}
[Serializable]
public class SentDeta
{
    public int betIndex;
    public string Event;
    public int option;
}

[Serializable]
public class GameData
{
    public List<List<int>> lines { get; set; }
    public List<double> bets { get; set; }
}
[Serializable]
public class Features
{
    public List<FreespinOption> freespinOptions { get; set; }
    public List<int> jackpotMultipliers { get; set; }
    public List<BonusTrigger> bonusTrigger { get; set; }

    public FreeSpin freeSpin { get; set; }
    public Bonus bonus { get; set; }
}
public class BonusTrigger
{
    public List<int> count { get; set; }
    public int rows { get; set; }
}
[Serializable]
public class FreespinOption
{
    public int count { get; set; }
    public List<int> multiplier { get; set; }
}
public class FreeSpin
{
    public bool isFreeSpin { get; set; }
    public int count { get; set; }
}
[Serializable]
public class Root
{
    //Result Data
    public bool success { get; set; }
    public List<List<string>> matrix { get; set; }
    public string name { get; set; }
    public Payload payload { get; set; }

    public Features features { get; set; }
    //Initial Data
    public string id { get; set; }
    public GameData gameData { get; set; }
    public UiData uiData { get; set; }
    public Player player { get; set; }
    //Bonus Data

}
[Serializable]
public class Scatter
{
    public double amount { get; set; }
}
[Serializable]
public class Jackpot
{
    public bool isTriggered { get; set; }
    public double amount { get; set; }
}
[Serializable]
public class Payload
{
    public double winAmount { get; set; }
    public List<Win> wins { get; set; }
    //gamble
    public bool playerWon { get; set; }
    public double currentWinning { get; set; }
    public Cards cards { get; set; }
    public double balance { get; set; }

}
[Serializable]
public class Cards
{
    public int dealerCard { get; set; }
    public int playerCard { get; set; }
}
[Serializable]
public class Win
{
    public int line { get; set; }
    public List<int> positions { get; set; }
    public double amount { get; set; }
}


[Serializable]
public class FreeSpins
{
    public int count { get; set; }
    public bool isFreeSpin { get; set; }
}
[Serializable]
public class Bonus
{
    public bool isTriggered { get; set; }
    public List<List<string>> matrix { get; set; }
    public int scatterCount { get; set; }
    public int spinCount { get; set; }
    public List<ScatterValue> scatterValues { get; set; }
}

public class ScatterValue
{
    public int value { get; set; }
    public List<int> index { get; set; }
}


[Serializable]
public class UiData
{
    public Paylines paylines { get; set; }
}

[Serializable]
public class Paylines
{
    public List<Symbol> symbols { get; set; }
}

[Serializable]
public class Player
{
    public double balance { get; set; }
}


[Serializable]
public class Symbol
{
    public int id { get; set; }
    public string name { get; set; }
    public List<int> multiplier { get; set; }
    public string description { get; set; }
}

[Serializable]
public class PlayerData
{
    public double Balance { get; set; }
    public double haveWon { get; set; }
    public double currentWining { get; set; }
}
[Serializable]
public class AuthTokenData
{
    public string cookie;
    public string socketURL;
    public string nameSpace; //BackendChanges
}