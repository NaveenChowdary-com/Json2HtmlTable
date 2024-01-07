using System.Text;
using System.Text.Json;

namespace Json2HtmlTable_Core
{
	public class Json2Html
	{
		private StringBuilder htmlTable = new();

		public string GetJson2Html(string jsonTxt, bool setDefaultStyle = false)
		{
			if (string.IsNullOrWhiteSpace(jsonTxt))
			{
				return "Invalid Json";
			}

			try
			{
				var readerOptionsDoc = new JsonDocumentOptions
				{
					AllowTrailingCommas = true,
					CommentHandling = JsonCommentHandling.Skip
				};

				var jsonObj = JsonDocument.Parse(jsonTxt, readerOptionsDoc);

				ProcessJsonElement(jsonObj.RootElement);

			}
			catch (Exception)
			{
				htmlTable = new("Invalid Json");
			}
			finally
			{
			}

			if (setDefaultStyle)
				htmlTable.Append("<style>table,td,th{border:1px solid #000;border-collapse:collapse;padding:0;margin:0}table table{border-style:hidden;width:100%}td{padding: 1px;}</style>");

			return htmlTable.ToString();
		}

		private void ProcessJsonElement(JsonElement jsonElement)
		{
			switch (jsonElement.ValueKind)
			{
				case JsonValueKind.Object:
					ProcessObject(jsonElement);
					break;
				case JsonValueKind.Array:
					ProcessArray(jsonElement);
					break;
				case JsonValueKind.String:
					htmlTable.Append("<td>" + jsonElement.GetString() + "</td>");
					break;
				case JsonValueKind.Number:
					htmlTable.Append("<td>" + jsonElement.GetDouble() + "</td>");
					break;
				case JsonValueKind.True:
				case JsonValueKind.False:
					htmlTable.Append("<td>" + jsonElement.GetBoolean() + "</td>");
					break;
				case JsonValueKind.Null:
					htmlTable.Append("<td>null</td>");
					break;
				default:
					htmlTable.Append("<td></td>");
					break;
			}
		}

		private void ProcessObject(JsonElement jsonElementObject)
		{
			var properties = jsonElementObject.EnumerateObject();

			if (properties.Any())
			{
				htmlTable.Append("<table>");
				foreach (var property in properties)
				{
					if (property.Value.ValueKind != JsonValueKind.Undefined)
					{
						htmlTable.Append("<tr>");

						htmlTable.Append("<td>" + property.Name + "</td>");

						if (property.Value.ValueKind == JsonValueKind.Array || property.Value.ValueKind == JsonValueKind.Object)
							htmlTable.Append("<td>");

						ProcessJsonElement(property.Value);

						if (property.Value.ValueKind == JsonValueKind.Array || property.Value.ValueKind == JsonValueKind.Object)
							htmlTable.Append("</td>");

						htmlTable.Append("</tr>");

					}
				}
				htmlTable.Append("</table>");
			}
		}

		private void ProcessArray(JsonElement jsonElementArray)
		{
			if (jsonElementArray.GetArrayLength() > 0)
			{
				htmlTable.Append("<table><tr>");
				foreach (var element in jsonElementArray.EnumerateArray())
				{
					if (element.ValueKind == JsonValueKind.Array || element.ValueKind == JsonValueKind.Object)
						htmlTable.Append("<td>");

					ProcessJsonElement(element);

					if (element.ValueKind == JsonValueKind.Array || element.ValueKind == JsonValueKind.Object)
						htmlTable.Append("</td>");
				}
				htmlTable.Append("</tr></table>");
			}
		}

	}
}
