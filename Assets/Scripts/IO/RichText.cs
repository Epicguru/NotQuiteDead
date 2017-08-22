using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class RichText
{
    private static StringBuilder str = new StringBuilder();
    private const string OPEN_TAG = "<", CLOSE_TAG = ">";
    private const string HASHTAG = "#";
    private const string COLOUR_START = "<color=", COLOUR_END= "</color>";
    private const string SIZE_START = "<size=", SIZE_END= "</size>";
    private const string BOLD_START = "<b>", BOLD_END= "</b>";
    private const string ITALICS_START = "<i>", ITALICS_END= "</i>";

    public static string InColour(string text, Color colour)
    {
        str.Length = 0;
        str.Append(COLOUR_START);
        str.Append(HASHTAG);
        str.Append(ColorUtility.ToHtmlStringRGBA(colour));
        str.Append(CLOSE_TAG);
        str.Append(text);
        str.Append(COLOUR_END);

        return str.ToString();
    }

    public static string InSize(string text, float size)
    {
        str.Length = 0;
        str.Append(SIZE_START);
        str.Append(size);
        str.Append(CLOSE_TAG);
        str.Append(text);
        str.Append(SIZE_END);

        return str.ToString();
    }

    public static string InBold(string text)
    {
        str.Length = 0;
        str.Append(BOLD_START);
        str.Append(text);
        str.Append(BOLD_END);

        return str.ToString();
    }

    public static string InItalics(string text)
    {
        str.Length = 0;
        str.Append(ITALICS_START);
        str.Append(text);
        str.Append(ITALICS_END);

        return str.ToString();
    }
}
