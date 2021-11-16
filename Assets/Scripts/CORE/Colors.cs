using UnityEngine;

public class Colors
{
    public static string COLOR_GOOD = "#49FD5E"; // green
    public static string COLOR_BAD = "#ED5945"; // red
    public static string COLOR_TEXT = "white"; // white
    public static string COLOR_HIGHLIGHT = "#FFCF33"; // yellow/orange
    public static string COLOR_HIGHLIGHT_ALLY = "#F76BFF"; // yellow/orange

    public static Color AsColor(string colorHex)
    {
        Color parsedColor;
        ColorUtility.TryParseHtmlString(colorHex, out parsedColor);
        return parsedColor;
    }
}