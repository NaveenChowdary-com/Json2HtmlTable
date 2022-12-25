using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace Json2HtmlTable_Core
{
    public class Json2Html
    {
        private readonly string border = "";
        public Json2Html(bool IsWithTableBorder = false, string? css = null)
        {
            if (IsWithTableBorder)
                border = " border=1";

            if (string.IsNullOrEmpty(css))
                html.Append("<style> td{border:1px solid black}th{border:1px solid red}</style>");
            else
                html.Append("<style>" + css + "</style>");
        }

        private StringBuilder html = new StringBuilder();

        public string GetJson2Html(string json)
        {
            try
            {
                object? dynamicJson = JsonConvert.DeserializeObject<object>(json);

                if (dynamicJson?.GetType() == typeof(JArray))
                {
                    JArray jArray = (JArray)dynamicJson;
                    foreach (JToken jItem in jArray)
                    {
                        if (jItem.Type != JTokenType.Object)
                        {
                            if (jItem.Type != JTokenType.Array)
                            {
                                html.Append("<table><tbody><tr><td>" + ((JValue)jItem).Value + "</td></tr><tbody></table>");
                            }
                            else
                            {
                                GetArrayJson2Html(jItem);
                            }
                        }
                        else
                        {
                            GetObjectJson2Html((JObject)jItem);
                        }
                        html.Append("<br />");
                    }
                }
                else if (dynamicJson?.GetType() == typeof(JObject))
                {
                    GetObjectJson2Html((JObject)dynamicJson);
                }
            }
            catch (Exception)
            {
            }
            return html.ToString();
        }

        private void GetObjectJson2Html(JObject jObject)
        {
            try
            {
                html.Append("<table" + border + "><tbody>");
                foreach (JProperty jProperty in jObject.Properties())
                {
                    html.Append("<tr><td><b>" + jProperty.Name + "</b></td><td>");
                    foreach (JToken jToken in jProperty)
                    {
                        if (jToken.Type == JTokenType.Array)
                            GetArrayJson2Html(jToken);
                        else if (jToken.Type == JTokenType.Object)
                        {
                            html.Append("<table" + border + "><thead><tr>");
                            JObject jObj = (JObject)jToken;
                            var jObjProList = jObj.Properties().Select(x => x.Name);
                            foreach (string strProperty in jObjProList) html.Append("<th>" + strProperty + "</th>");
                            html.Append("</tr></thead><tbody><tr>");
                            foreach (JProperty jProp in jObj.Properties())
                            {
                                if (jProp.Value.Type == JTokenType.Array)
                                {
                                    html.Append("<td>");
                                    GetArrayJson2Html(jProp.Value);
                                    html.Append("</td>");
                                }
                                else if (jProp.Value.Type == JTokenType.Object)
                                {
                                    html.Append("<td>");
                                    GetObjectJson2Html((JObject)jProp.Value);
                                    html.Append("</td>");
                                }
                                else
                                    html.Append("<td>" + jProp.Value + "</td>");
                            }
                            html.Append("</tr></tbody></table>");
                        }
                        else
                            html.Append(jToken);
                    }
                    html.Append("</td></tr>");
                }
                html.Append("</tbody></table>");
            }
            catch (Exception)
            {
            }
        }

        private void GetArrayJson2Html(JToken jToken)
        {
            try
            {
                html.Append("<table" + border + "><thead><tr>");

                foreach (var _jToken in jToken)
                {
                    if (_jToken is null)
                    {
                        continue;
                    }

                    if (_jToken.Type != JTokenType.Object && _jToken.Type != JTokenType.Array)
                    {
                        html.Append("<td>" + ((JValue)_jToken).Value + "</td>");
                    }
                    else
                    {
                        foreach (JProperty jProperty in _jToken.Cast<JProperty>())
                        {
                            html.Append("<th>" + jProperty.Name + "</th>");
                        }
                        break;
                    }
                }

                html.Append("</tr></thead><tbody>");
                foreach (JToken j_Token in (JArray)jToken)
                {
                    html.Append("<tr>");
                    foreach (JProperty jProperty in j_Token.Cast<JProperty>())
                    {
                        if (jProperty.HasValues)
                        {
                            if (jProperty.Value.Type == JTokenType.Array)
                            {
                                html.Append("<td>");
                                GetArrayJson2Html(jProperty.Value);
                                html.Append("</td>");
                            }
                            else if (jProperty.Value.Type == JTokenType.Object)
                            {
                                html.Append("<td>");
                                GetObjectJson2Html((JObject)jProperty.Value);
                                html.Append("</td>");
                            }
                            else
                                html.Append("<td>" + jProperty.Value + "</td>");
                        }
                    }
                    html.Append("</tr>");
                }
                html.Append("</tbody></table>");
            }
            catch (Exception)
            {
            }
        }
    }
}