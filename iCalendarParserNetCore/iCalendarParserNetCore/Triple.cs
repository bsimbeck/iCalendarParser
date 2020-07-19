using System;
using System.Collections.Generic;
using System.Text;

namespace iCalendarParserNetCore
{

    public interface ITriple
    {
        string GetSubject();
        string GetObject();
        string GetPredicate();
        bool IsResource();
    }

    public class Triple :ITriple
    {
        string subj, obj, pred;
        bool objIsResource;

        public Triple(string _pred, string _subj, string _obj, bool _objIsResource)
        {
            pred = _pred;
            subj = _subj;
            obj = _obj;
            objIsResource = _objIsResource;
        }

		public string GetSubject()
		{
			return subj;
		}

		public string GetObject()
		{
			return obj;
		}

		public string GetPredicate()
		{
			return pred;
		}

		public bool IsResource()
		{
			return objIsResource;
		}

		public override string ToString()
		{
			string rval = "[" + GetPredicate() + "] [" + GetSubject() + "] ";
			if (IsResource())
			{
				rval += "[" + GetObject() + "]";
			}
			else
			{
				rval += "'" + GetObject() + "'";
			}
			return rval;
		}
	}
}
