using System;
using UnityEngine;

[Serializable]
public class ServerEnvironment
{
    [Popup("Local", "Dev", "Prod")]
    public string Environment;
    
    //[HideInInspector]
    public string Region;
    
    private string LocalHostUrl = "http://localhost:5000";
    private string DevHostUrl = "https://lul2.herokuapp.com";
    private string ProdHostUrlUs = "http://eq-1786457703.us-east-1.elb.amazonaws.com";
    private string ProdHostUrlEu = "http://eq-1685188041.eu-central-1.elb.amazonaws.com";
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
                case "Dev":
                    return DevHostUrl;
                case "Prod":
                default:
                    {
                        switch(Region)
                        {
                            case "eu":
                                return ProdHostUrlEu;
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
                case "Dev":
                default:
                    return "b0ss";
                case "Prod":
                    return PlayerPrefs.GetString("unic0rn");
            }
        }
    }
}