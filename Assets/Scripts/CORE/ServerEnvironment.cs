using System;
using UnityEngine;

[Serializable]
public class ServerEnvironment
{
    [Popup("Local", "Dev", "Prod")]
    public string Environment;
    
    //[HideInInspector]
    public string Region = "us";
    
    private string LocalHostUrl = "http://localhost:5000";
#if UNITY_EDITOR
    private string DevHostUrl = "https://lul2.herokuapp.com";
#endif
    private string ProdHostUrlUs = "http://eq-1786457703.us-east-1.elb.amazonaws.com";
    private string SocketPath = "/socket.io/";
    private string CGPath = "/update-content";
    public string HostUrl
    {
        get
        { 
            switch (Environment)
            {
                case "Local":
                    return LocalHostUrl;
#if UNITY_EDITOR
                case "Dev":
                    return DevHostUrl;
#endif
                case "Prod":
                default:
                    {
                        switch(Region)
                        {
                            default:
                                return ProdHostUrlUs;
                        }
                    }
            }
        }
    }
    public string SocketUrl { get { return HostUrl + SocketPath; } }
    public string CGUrl { get { return HostUrl + CGPath; } }

    public string unic0rn
    {
        get
        {
            switch (Environment)
            {
                
                case "Local":
                    return "kekw";
                default:
#if UNITY_EDITOR
                case "Dev":
                    return "b0ss";
#endif
                case "Prod":
                    return PlayerPrefs.GetString("unic0rn");
            }
        }
    }
}