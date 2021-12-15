public class FriendData
{
    public string mainActorName;
    public string userId;
    public ulong steamId;
    public bool isOnline;

    // From here below will be available only if isOnline is true.
    public string currentName;
    public int level;
    public string classJob;
}