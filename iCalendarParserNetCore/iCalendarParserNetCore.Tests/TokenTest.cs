using System;
using Xunit;

namespace iCalendarParserNetCore.Tests
{
    public class TokenTest
    {
        [Fact]
		public void testToken()
		{
			Token t1 = new Token("testing");
			Token t2 = new Token(TokenValue.Hyphen);
			Token t3 = new Token("x-vendor-specific-tag", ScannerState.ParseID);
			Assert.True(t1.TokenText == "testing" && t1.TokenVal == TokenValue.Value && !t1.isError());
			Assert.True(t2.TokenText == null && t2.TokenVal == TokenValue.Hyphen && !t2.isError());
			Assert.True(t3.TokenText == "x:vendorSpecificTag" && t3.TokenVal == TokenValue.Xtension && !t3.isError());
		}

		[Fact]
		public void testParseID()
		{
			Token t1 = new Token("BeGin", ScannerState.ParseID);
			Token t2 = new Token("a123-009", ScannerState.ParseID);
			Token t3 = new Token("x123", ScannerState.ParseID);
			Token t4 = new Token("123", ScannerState.ParseID);
			Assert.True(t1.TokenVal == TokenValue.Tbegin && !t1.isError());
			Assert.True(t2.TokenText == "a123009" && t2.TokenVal == TokenValue.ID && !t2.isError());
			Assert.True(t3.TokenText == "x123" && t3.TokenVal == TokenValue.ID && !t3.isError());
			Assert.True(t4.isError());
		}

		[Fact]
		public void testReservedWords()
		{
			Token t1 = new Token("class", ScannerState.ParseID);
			Token t2 = new Token("AttendeE", ScannerState.ParseID);
			Token t3 = new Token("daylight", ScannerState.ParseID);
			Token t4 = new Token("vtodO", ScannerState.ParseID);
			Assert.True(t1.TokenVal == TokenValue.Tclass && t1.isSymbolicProperty() && !t1.isError());
			Assert.True(t2.TokenVal == TokenValue.Tattendee && t2.isMailtoProperty() && !t2.isError());
			Assert.True(t3.TokenVal == TokenValue.Tdaylight && t3.isResourceProperty() && !t3.isError());
			Assert.True(t4.TokenVal == TokenValue.Tvtodo && t4.isBeginEndValue() && !t4.isError());
		}

		[Fact]
		public void testParseParms()
		{
			Token t1 = new Token("\"jklsdfjkldfs\"", ScannerState.ParseParms, true);
			Token t2 = new Token("a123-009", ScannerState.ParseParms);
			Assert.True(t1.TokenVal == TokenValue.QuotedString && !t1.isError());
			Assert.True(t2.TokenText == "a123-009" && t2.TokenVal == TokenValue.Parm && !t2.isError());
		}

		[Fact]
		public void testParseValue()
		{
			Token t2 = new Token("a123-009", ScannerState.ParseValue);
			Assert.True(t2.TokenText == "a123-009" && t2.TokenVal == TokenValue.Value && !t2.isError());
		}

		[Fact]
		public void testCamelCase()
		{
			string val = Token.CamelCase("stuff");
			Assert.True(val == "stuff");
			val = Token.CamelCase("1-23");
			Assert.True(val == "123");
			val = Token.CamelCase("x-attack");
			Assert.True(val == "xAttack");
			val = Token.CamelCase("x-gorilla-attack");
			Assert.True(val == "xGorillaAttack");
			val = Token.CamelCase("x-");
			Assert.True(val == "x");
			val = Token.CapsCamelCase("x-");
			Assert.True(val == "X");
			val = Token.CapsCamelCase("Valarm");
			Assert.True(val == "Valarm");
			val = Token.CapsCamelCase("valarm");
			Assert.True(val == "Valarm");
		}

		[Fact]
		public void testHtmlEncoding()
		{
			Token t1 = new Token("this is, a value", ScannerState.ParseValue);
			Assert.True(t1.TokenVal == TokenValue.Value);
			Assert.True(t1.TokenText == "this is, a value");
			t1 = new Token("&this is, <a value>", ScannerState.ParseValue);
			Assert.True(t1.TokenVal == TokenValue.Value);
			Assert.True(t1.TokenText == "&amp;this is, &lt;a value&gt;");
		}
	}
}
