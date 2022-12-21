namespace Imbalance
{
	public static class Extensions
	{
		public static string ToImbalance(this int c)
		{
			c &= 255;
			string text = ".«";
			if ((c & 1) == 1)
			{
				text = "1" + text;
			}
			else
			{
				text = "2" + text;
				c -= 2;
			}
			c >>= 1;
			for (int i = 1; i < 15; i += 2)
			{
				switch ((c & 1) + (c & 2))
				{
					case 0:
						text = "12" + text;
						break;
					case 1:
						text = "11" + text;
						break;
					case 2:
						text = "22" + text;
						break;
					case 3:
						text = "21" + text;
						break;
				}
				c >>= 2;
			}
			string text2 = text;
			string text3 = text;
			if (text2.StartsWith("2"))
			{
				text2 = text2.Substring(1);
			}
			if (text3.StartsWith("1"))
			{
				text3 = text3.Substring(1);
			}
			while (text2.StartsWith("12"))
			{
				text2 = text2.Substring(2);
			}
			while (text3.StartsWith("21"))
			{
				text3 = text3.Substring(2);
			}
			text2 = "«" + text2;
			text3 = "»" + text3;
			return (text2.Length >= text3.Length) ? text3 : text2;
		}
	}
}