using System;

[Serializable]
public class ServerEnvironment
{
    [Popup("Local", "Dev", "Prod")]
    public string Environment;
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
                    return ProdHostUrlEu;
            }
        }
    }
    public string SocketUrl { get { return HostUrl + SocketPath; } }
    public string CGUrl { get { return HostUrl + CGPath; } }

}