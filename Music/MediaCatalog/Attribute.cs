using System;

namespace Microsoft.Samples.MediaCatalog
{
	public class Attribute
	{
		private readonly string name;
		private readonly object val;

		public Attribute(string attributeName, object attributeValue)
		{
			name = attributeName;
			val = attributeValue;
		}

		public string Name
		{
			get { return name; }
		}

		public object Value
		{
			get { return val; }
		}
	}
}
