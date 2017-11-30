
namespace SV.UPnPLite.Core
{
	using System.Text;
	using System.Xml.Linq;

    internal static class XDocumentExtensions
	{
		public static string ToStringWithDeclaration(this XDocument document)
		{
			var result = new StringBuilder();

			if (document.Declaration != null)
			{
				result.AppendLine(document.Declaration.ToString());
			}

			result.AppendLine(document.ToString());

			return result.ToString();
		}
	}
}
