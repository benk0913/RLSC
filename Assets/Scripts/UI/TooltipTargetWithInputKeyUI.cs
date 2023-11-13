using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTargetWithInputKeyUI : TooltipTargetUI
{
    public string KeymapKey = "Exit";
    private string originalText;
    
    void Start()
    {
        originalText = Text;
        Text = originalText +"("+InputMap.Map[KeymapKey].ToString()+")";
    }
}