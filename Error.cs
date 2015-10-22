

using System;

namespace PathEdit
{
	public class Error : Exception
	{
		public Error(string message) : base(message)
		{
		}

		public Error(string format, params object[] args)
			: base(String.Format(format, args))
		{
		}
	}
}