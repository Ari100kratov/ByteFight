using System.Globalization;
using System.Net;
using System.Text;
using System.Xml;

namespace GameRuntime.Logic.User.Intellisense.Common;

public static partial class DocumentationFormatter
{
    /// <summary>
    /// Форматирует XML-документацию Roslyn в читаемый Markdown-подобный текст.
    /// </summary>
    public static string? Format(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
        {
            return null;
        }

        var doc = new XmlDocument();
        doc.LoadXml(xml);

        XmlNode? summaryNode = doc.SelectSingleNode("//summary");
        XmlNode? returnsNode = doc.SelectSingleNode("//returns");
        XmlNodeList? paramNodes = doc.SelectNodes("//param");

        var sb = new StringBuilder();

        IFormatProvider culture = CultureInfo.InvariantCulture;

        if (summaryNode != null)
        {
            sb.AppendLine(Normalize(summaryNode.InnerText));
        }

        if (paramNodes != null && paramNodes.Count > 0)
        {
            foreach (XmlNode param in paramNodes)
            {
                string name = param.Attributes?["name"]?.Value ?? "?";
                string desc = Normalize(param.InnerText);
                sb.AppendLine(culture, $"• {name}: {desc}");
            }
        }

        if (returnsNode != null)
        {
            sb.AppendLine(culture, $"Returns: {Normalize(returnsNode.InnerText)}");
        }

        string result = sb.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private static string Normalize(string text)
    {
        return string.Join(
            "\n",
            WebUtility.HtmlDecode(text)
                .Split('\n')
                .Select(line => line.Trim())
                .Where(line => !string.IsNullOrWhiteSpace(line)));
    }
}
