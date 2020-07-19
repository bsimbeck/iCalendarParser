
using System;
using System.Collections.Generic;
using System.Text;

namespace iCalendarParserNetCore
{
	/// <summary>
	/// Basic error handling mechanism for the Parser
	/// </summary>
	public class ParserError
    {
		private int linenumber;
		private string message;

		public ParserError(int _linenumber, string _message)
		{
			linenumber = _linenumber;
			message = _message;
		}

		public int Linenumber
		{
			get { return linenumber; }
		}

		public string Message
		{
			get { return message; }
		}

		public override string ToString()
		{
			return "Error on line: " + linenumber + ".  " + message;
		}

	}
}
