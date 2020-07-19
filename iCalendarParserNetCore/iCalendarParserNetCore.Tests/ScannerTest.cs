using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace iCalendarParserNetCore.Tests
{
    public class ScannerTest
    {
		[Fact]
		public void testID()
		{
			string str = "bEgin legalid  a_nother    a123423   0illegal";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseID);
			Assert.True(t.TokenVal == TokenValue.Tbegin && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t.TokenText == "legalid" && t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t.TokenText == "a_nother" && t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t.TokenText == "a123423" && t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t == null);
		}

		[Fact]
		public void testTypical()
		{
			string str = "action ; stuff = 10:this is tim's value";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseID);
			Assert.True(t.TokenVal == TokenValue.Taction && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.SemiColon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t.TokenText == "stuff" && t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Equals && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseParms);
			Assert.True(t.TokenText == " 10" && t.TokenVal == TokenValue.Parm && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Colon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == @"this is tim's value" && t.TokenVal == TokenValue.Value && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t == null && scanner.isEOF());
		}

		[Fact]
		public void testSimple()
		{
			string str = ";:  ,x";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.SemiColon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Colon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Comma && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t == null);
		}

		[Fact]
		public void testEOF()
		{
			string str = "hello";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseID);
			Assert.True(t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t == null && scanner.isEOF());
		}

		[Fact]
		public void testParms()
		{
			string str = "\"quoted value ;;: with stuff\"  some other stuff";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseParms);
			Assert.True(t.TokenText == "&quot;quoted value ;;: with stuff&quot;" && t.TokenVal == TokenValue.QuotedString && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseParms);
			Assert.True(t.TokenText == "  some other stuff" && t.TokenVal == TokenValue.Parm && !t.isError());
		}

		[Fact]
		public void testValue()
		{
			string str = " sdfjlksdf kdskd f;;dfsd;;  3542 !@#$$^#^&*)&*(%%^   ";
			string encStr = " sdfjlksdf kdskd f;;dfsd;;  3542 !@#$$^#^&amp;*)&amp;*(%%^   ";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == encStr && t.TokenVal == TokenValue.Value && !t.isError());
		}

		[Fact]
		public void testValueFoldingPass()
		{
			string str = @" sdfjlksdf kdskd f;;dfsd;;  3542 !@#$$^#^&*)&*(%%^   
  sfddfs";
			string res = " sdfjlksdf kdskd f;;dfsd;;  3542 !@#$$^#^&amp;*)&amp;*(%%^    sfddfs";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == res && t.TokenVal == TokenValue.Value && !t.isError());
		}

		[Fact]
		public void testAnywhereFoldingPass()
		{
			string str = @"action 
 ; 
 stuff 
 = 10
 :this is 
 tim's value";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseID);
			Assert.True(t.TokenVal == TokenValue.Taction && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.SemiColon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseKey);
			Assert.True(t.TokenText == "stuff" && t.TokenVal == TokenValue.ID && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Equals && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseParms);
			Assert.True(t.TokenText == " 10" && t.TokenVal == TokenValue.Parm && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseSimple);
			Assert.True(t.TokenVal == TokenValue.Colon && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == "this is tim's value" && t.TokenVal == TokenValue.Value && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t == null && scanner.isEOF());
		}

		[Fact]
		public void testValueFoldingFail()
		{
			string str = @"fold
now";
			StringReader reader = new StringReader(str);
			Scanner scanner = new Scanner(reader);
			Token t;
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == "fold" && t.TokenVal == TokenValue.Value && !t.isError());
			t = scanner.GetNextToken(ScannerState.ParseValue);
			Assert.True(t.TokenText == "now" && t.TokenVal == TokenValue.Value && !t.isError());
		}
	}
}
