public class RichTextRemover
{

	private static string[] RichText { get; } = new string[]
	{
		"b",
		"i",
		// TMP
		"u",
		"s",
		"sup",
		"sub",
		"allcaps",
		"smallcaps",
		"uppercase",
	};
	private static string[] RichTextDynamic { get; } = new string[]
	{
		"color",
		// TMP
		"align",
		"size",
		"cspace",
		"font",
		"indent",
		"line-height",
		"line-indent",
		"link",
		"margin",
		"margin-left",
		"margin-right",
		"mark",
		"mspace",
		"noparse",
		"nobr",
		"page",
		"pos",
		"space",
		"sprite index",
		"sprite name",
		"sprite",
		"style",
		"voffset",
		"width",
	};




	public static string RemoveRichText(string input)
	{

		foreach(string tag in RichTextDynamic)
			input = RemoveRichTextDynamicTag(input, tag);

		foreach (string tag in RichText)
			input = RemoveRichTextTag(input, tag);

		return input;

	}


	private static string RemoveRichTextDynamicTag (string input, string tag)
	{
		int index = -1;
		while (true)
		{
			index = input.IndexOf($"<{tag}=");
			//Debug.Log($"{{{index}}} - <noparse>{input}");
			if (index != -1)
			{
				int endIndex = input.Substring(index, input.Length - index).IndexOf('>');
				if (endIndex > 0)
					input = input.Remove(index, endIndex + 1);
				continue;
			}
			input = RemoveRichTextTag(input, tag, false);
			return input;
		}
	}
	private static string RemoveRichTextTag (string input, string tag, bool isStart = true)
	{
		while (true)
		{
			int index = input.IndexOf(isStart ? $"<{tag}>" : $"</{tag}>");
			if (index != -1)
			{
				input = input.Remove(index, 2 + tag.Length + (!isStart).GetHashCode());
				continue;
			}
			if (isStart)
				input = RemoveRichTextTag(input, tag, false);
			return input;
		}
	}

}