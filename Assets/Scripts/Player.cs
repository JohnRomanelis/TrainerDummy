using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections;
using UnityEngine.SceneManagement;



public class Player : NetworkBehaviour
{
    private int s;
    private bool sendStream;

    private RawImage rawImage;

    Texture2D frameTexture;
    public override void OnStartServer()
    {
        rawImage = FindObjectOfType<RawImage>();
        if (rawImage != null)
        {
            Debug.Log("Found the RawImage");
        }
    }

    private void Awake()
    {
        // We need this so the player gameObject is not destroyed when we change scenes
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        if (isServer)
        {
            s = 0;            
        }
        else
        {
            s = 1;
        }
    }

    private void Update()
    {


        // Debug.Log(isLocalPlayer);

        if (isLocalPlayer && Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Sending Hola to the player");
            Hola(s);
        }

        if (isServer && Input.GetKeyDown(KeyCode.Z))
        {
            TooHigh();
        }

        if (isClient && Input.GetKeyDown(KeyCode.S))
        {
            sendStream = !sendStream;
        }

        if (isLocalPlayer && sendStream) // && Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("S pressed");
            SendImageFrame(); //TellServerToRequestStream();
        }

        if (isServer && Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("C was pressed!");
            GoToScene2();
        }
    }

  
    void SendImageFrame()
    {
        //Debug.Log("Before yield");
        //Wait till the last possible moment before screen rendering to hide the UI
        //yield return new WaitForEndOfFrame();
        //Debug.Log("After yield");

        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();
        int width = screenshot.width;
        int height = screenshot.height;
        
        byte[] bytes = screenshot.EncodeToJPG();



        Debug.Log("Sending Bytes");
        DisplayImage(bytes, width, height);
        
        Destroy(screenshot);
    }

    [Command]
    void DisplayImage(byte[] bytes, int width, int height)
    {
        Debug.Log("Receiving Bytes");

        if (bytes == null)
        {
            Debug.Log("Bytes is null");
        }
        else
        {
            Debug.Log("Received Bytes");
        }

        if (rawImage == null)
        {
            Debug.Log("rawImage is null");
        }
        else
        {
            Debug.Log("rawImage is ready");
        }

        Destroy(frameTexture);

        frameTexture = new Texture2D(width, height, TextureFormat.RGB24, false);
        frameTexture.LoadImage(bytes);

        rawImage.texture = frameTexture;

    }

    [Command]
    void Hola(int s)
    {
        Debug.Log("Received Hola from client!");
        Debug.Log($"Also value of 's' is {s}");
        ReplyHola();
    }

    [TargetRpc]
    void ReplyHola()
    {
        Debug.Log("Received Hola from server!");
    }
    
    [ClientRpc]
    void TooHigh()
    {
        Debug.Log("Too High!");
    }


    void GoToScene2()
    {   
        // We don't use the NetworkManager.ServerChangeScene because we 
        // don't want the server to change scene. 
        SceneManager.LoadScene("Scene2");
    }


}
