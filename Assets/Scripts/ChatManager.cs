using System.IO;
using System.Net.Sockets;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ChatManager : MonoBehaviour
{
    private TcpClient twitchClient;
    private StreamReader reader;
    private StreamWriter writer;

    [SerializeField] private TMP_Text chatText;

    private readonly string username = "BoostMem";
    private readonly string password = "oauth:ow6e7eaz9lu9lliv6c32b73dkegvek";
    private readonly string channelName = "PWGood";

    private int num;

    private static Dictionary<string, int> chatDictionary = new();
    private static bool isReadingEnabled;

    public static Dictionary<string, int> ChatDictionary
    {
        get => chatDictionary;
        set => chatDictionary = value;
    }
    public static bool IsReadingEnabled
    {
        get => isReadingEnabled;
        set => isReadingEnabled = value;
    }

    void Start()
    {
        Connect();

        if (SceneManager.GetActiveScene().buildIndex == 0)
            isReadingEnabled = true;
        else
            isReadingEnabled = false;
    }

    public void Connect()
    {
        twitchClient = new TcpClient("irc.chat.twitch.tv", 6667);
        reader = new StreamReader(twitchClient.GetStream());
        writer = new StreamWriter(twitchClient.GetStream());

        writer.WriteLine("PASS " + password);
        writer.WriteLine("NICK " + username);
        writer.WriteLine("USER" + username + " 8 * :" + username);
        writer.WriteLine("JOIN #" + channelName.ToLower());
        writer.Flush();

        Debug.Log("Connected to Twitch IRC");
    }

    void Update()
    {
        if (twitchClient == null || !twitchClient.Connected) 
            Connect();

        if (isReadingEnabled)
            ReadChat();
    }

    void ReadChat() 
    {
        if (twitchClient.Available > 0) 
        {
            string message = reader.ReadLine();
            if (message.Contains("PRIVMSG"))
            {
                int splitPoint = message.IndexOf("!", 1);
                string chatName = message.Substring(1, splitPoint - 1);

                splitPoint = message.IndexOf(":", 1);
                string chatMessage = message.Substring(splitPoint + 1);

                if (SceneManager.GetActiveScene().buildIndex == 0)
                    if (chatName.Length + chatMessage.Length < 100) 
                        chatText.text = chatName + ": " + chatMessage;
                if (SceneManager.GetActiveScene().buildIndex == 2)
                    if (int.TryParse(chatMessage, out num) && num >= 1 && num <= 4)
                        chatDictionary.TryAdd(chatName, num);
            }
        }
    }
}