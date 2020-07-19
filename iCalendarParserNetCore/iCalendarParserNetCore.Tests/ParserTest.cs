using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace iCalendarParserNetCore.Tests
{
    public class ParserTest
    {
		private const string testcaseDir = "ICalParser\\testcases\\";
		private const string resultDir = "ICalParser\\testcases\\results\\";
		private static string[] testcases = new string[]
		{
		"20030115mtg",
		"20030122mtg",
		"20030205mtg",
		"20030212mtg",
		"20030226mtg",
		"20030312mtg",
		"20030326mtg",
		"Philosophers'Birthdays",
		"Home",
		"ComplexEvent",
		"ComplexerEvents",
		"allday",
		"DVDs",
		"RecurExept",
		"RecurAnomoly",
		"Mac32Events",
		};
		private const string icalExt = ".ics";
		private const string rdfExt = ".rdf";
		private const string rqlExt = ".rql";
		private const string tripleExt = ".trp";
		private int tally = 0;

		[Fact]
		public void testParser()
		{
			string icalString =
	@"BEGIN:VCALENDAR
METHOD:REQUEST
BEGIN:VEVENT
SEQUENCE:2
ATTENDEE;CN=Libby Miller:mailto:libby.miller@bristol.ac.uk
DTSTAMP:20030109T123909Z
SUMMARY:IRC Meet
UID:EB825E41-23CE-11D7-B93D-003065B0C95E
ORGANIZER;CN=Damian Steer:mailto:pldms@mac.com
DTSTART;
 TZID=/softwarestudio.org/Olson_20011030_5/Europe/London:20030115T180000
DURATION:PT1H
BEGIN:VALARM
ATTACH;VALUE=URI:Ping
TRIGGER;VALUE=DURATION:-PT10M
ACTION:AUDIO
END:VALARM
END:VEVENT

BEGIN:VTIMEZONE
TZID:/softwarestudio.org/Olson_20011030_5/Europe/London
X-LIC-LOCATION:Europe/London
BEGIN:STANDARD
TZOFFSETFROM:+0100
TZOFFSETTO:+0000
TZNAME:GMT
DTSTART:19701025T020000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=10
END:STANDARD
BEGIN:DAYLIGHT
TZOFFSETFROM:+0000
TZOFFSETTO:+0100
TZNAME:BST
DTSTART:19700329T010000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=3
END:DAYLIGHT
END:VTIMEZONE

END:VCALENDAR
";

			RDFEmitter emitter = new RDFEmitter();
			Parser parser = new Parser(new StringReader(icalString), emitter);
			parser.Parse();
			Console.WriteLine(emitter.Rdf);

		}

		[Fact]
		public void testError()
		{
			string icalString =
	@"BEGIN:VCALENDAR
METHOD:REQUEST
BEGIN:VEVENT
SEQUENCE:2
ATTENDEE;CN--------<error here>Libby Miller:mailto:libby.miller@bristol.ac.uk
DTSTAMP:20030109T123909Z
SUMMARY:IRC Meet
UID:EB825E41-23CE-11D7-B93D-003065B0C95E
ORGANIZER;CN=Damian Steer:mailto:pldms@mac.com
DTSTART;
 TZID=/softwarestudio.org/Olson_20011030_5/Europe/London:20030115T180000
DURATION:PT1H
BEGIN:VALARM
ATTACH;VALUE=URI:Ping
TRIGGER;VALUE=DURATION:-PT10M
ACTION:AUDIO
END:VALARM
END:VEVENT

BEGIN:VTIMEZONE
TZID:/softwarestudio.org/Olson_20011030_5/Europe/London
X-LIC-LOCATION:Europe/London
BEGIN:STANDARD
TZOFFSETFROM:+0100
TZOFFSETTO:+0000
TZNAME:GMT
DTSTART:19701025T020000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=10
END:STANDARD
BEGIN:DAYLIGHT
TZOFFSETFROM:+0000
TZOFFSETTO:+0100
TZNAME:BST
DTSTART:19700329T010000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=3
END:DAYLIGHT
END:VTIMEZONE

END:VCALENDAR
";

			RDFEmitter emitter = new RDFEmitter();
			Parser parser = new Parser(new StringReader(icalString), emitter);
			parser.Parse();
			Console.WriteLine(emitter.Rdf);
			Assert.True(parser.HasErrors);
		}

		[Fact]
		public void testRDFQLParser()
		{
			string icalString =
	@"BEGIN:VCALENDAR
METHOD:REQUEST
BEGIN:VEVENT
SEQUENCE:2
ATTENDEE;CN=Libby Miller:mailto:libby.miller@bristol.ac.uk
DTSTAMP:20030109T123909Z
SUMMARY:IRC Meet
UID:EB825E41-23CE-11D7-B93D-003065B0C95E
ORGANIZER;CN=Damian Steer:mailto:pldms@mac.com
DTSTART;VALUE=DATE:20030115
DURATION:PT1H
BEGIN:VALARM
ATTACH;VALUE=URI:Ping
TRIGGER;VALUE=DURATION:-PT10M
ACTION:AUDIO
END:VALARM
END:VEVENT

BEGIN:VTIMEZONE
TZID:/softwarestudio.org/Olson_20011030_5/Europe/London
X-LIC-LOCATION:Europe/London
BEGIN:STANDARD
TZOFFSETFROM:+0100
TZOFFSETTO:+0000
TZNAME:GMT
DTSTART:19701025T020000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=10
END:STANDARD
BEGIN:DAYLIGHT
TZOFFSETFROM:+0000
TZOFFSETTO:+0100
TZNAME:BST
DTSTART:19700329T010000
RRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=3
END:DAYLIGHT
END:VTIMEZONE

END:VCALENDAR
";

			RQLEmitter emitter = new RQLEmitter();
			Parser parser = new Parser(new StringReader(icalString), emitter);
			parser.Parse();
			Console.WriteLine(emitter.Rql);

		}

		[Fact]
		public void testParser_testsuite()
		{
			System.IO.Directory.CreateDirectory(resultDir);
			for (int i = 0; i < testcases.Length; ++i)
			{
				RDFEmitter emitter = new RDFEmitter();
				RQLEmitter rqlEmitter = new RQLEmitter();
				StreamReader reader = new StreamReader(testcaseDir + testcases[i] + icalExt);
				StreamReader rqlReader = new StreamReader(testcaseDir + testcases[i] + icalExt);
				Parser parser = new Parser(reader, emitter);
				Parser rqlParser = new Parser(rqlReader, rqlEmitter);
				parser.Parse();
				rqlParser.Parse();
				StreamWriter writer = new StreamWriter(resultDir + testcases[i] + rdfExt);
				StreamWriter rqlWriter = new StreamWriter(resultDir + testcases[i] + rqlExt);
				writer.WriteLine(emitter.Rdf);
				rqlWriter.WriteLine(String.Format(rqlEmitter.Rql, "icaltest"));
				writer.Close();
				reader.Close();
				rqlWriter.Close();
				rqlReader.Close();
			}
		}

		[Fact]
		public void testTripleEmitter()
		{
			System.IO.Directory.CreateDirectory(resultDir);
			for (int i = 0; i < testcases.Length; ++i)
			{
				TripleEmitter emitter = new TripleEmitter();
				StreamReader reader = new StreamReader(testcaseDir + testcases[i] + icalExt);
				Parser parser = new Parser(reader, emitter);
				parser.Parse();
				StreamWriter writer = new StreamWriter(resultDir + testcases[i] + tripleExt);
				foreach (Triple t in emitter.Triples)
				{
					Assert.True(t.GetObject() != null);
					Assert.True(t.GetPredicate() != null);
					Assert.True(t.GetSubject() != null);
					writer.WriteLine(t.ToString());
				}
				writer.Close();
				reader.Close();
			}
		}

		
		public void persister(string rql)
		{
			tally++;
			Console.Out.WriteLine("PERSISTER OUTPUT----------------------: " + tally);
			Console.Out.WriteLine(rql);
		}

		[Fact]
		public void testRQLPersister()
		{
			tally = 0;
			RQLEmitter rqlEmitter = new RQLEmitter(new Persister(persister));
			StreamReader rqlReader = new StreamReader(testcaseDir + "DVDs.ics");
			Parser rqlParser = new Parser(rqlReader, rqlEmitter);
			rqlParser.Parse();
			rqlReader.Close();
			Assert.True(tally > 0);
		}
	}
}
