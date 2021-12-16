using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FriendsDataHandler
{
    public static FriendsDataHandler Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new FriendsDataHandler();
            }
            return _Instance;
        }
    }
    private static FriendsDataHandler _Instance;

    public Dictionary<string, FriendData> Friends = new Dictionary<string, FriendData>();

    public void ClearFriends()
    {
        Friends.Clear();
    }
}