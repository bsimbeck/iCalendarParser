using System;
using System.IO;
using System.Text;

namespace iCalendarParserNetCore
{
    public class ICalUtil
	{
		/// <summary>
		/// From RFC2445
		/// 
		/// Lines of text SHOULD NOT be longer than 75 octets, excluding the line
		/// break. Long content lines SHOULD be split into a multiple line
		/// representations using a line "folding" technique. That is, a long
		/// line can be split between any two characters by inserting a CRLF
		/// immediately followed by a single linear white space character (i.e.,
		/// SPACE, US-ASCII decimal 32 or HTAB, US-ASCII decimal 9). Any sequence
		/// of CRLF followed immediately by a single linear white space character
		/// is ignored (i.e., removed) when processing the content type.
		/// 
		/// For example the line:
		/// 
		///		DESCRIPTION:This is a long description that exists on a long line.
		///	
		/// Can be represented as:
		/// 
		///   DESCRIPTION:This is a lo
		///		ng description
		///		that exists on a long line.
		///		
		/// </summary>
		/// <param name="iCal"></param>
		/// <returns></returns>
		public static string FoldLines(StringBuilder iCal)
        {
			StringReader reader = new StringReader(iCal.ToString());
			StringBuilder folded = new StringBuilder();
			string line = reader.ReadLine();
			while (line != null)
			{
				int start = 0;
				int len = line.Length;

				// I chose 72 instead of the max 75 cause I wanted to, so :P
				while (len > 72)
				{
					// Indent if not first line
					if (start != 0)
						folded.Append(" ");
					folded.AppendFormat("{0}\r\n", line.Substring(start, 72));
					start += 72;
					len -= 72;
				}
				// Indent if not first line
				if (start != 0)
					folded.Append(" ");
				folded.AppendFormat("{0}\r\n", line.Substring(start, len));
				line = reader.ReadLine();
			}
			return folded.ToString();
		}
    }
}
