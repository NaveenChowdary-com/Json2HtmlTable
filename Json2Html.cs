using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Json2HtmlTable
{
    public class Json2Html
    {
        private readonly string border = "";
        public Json2Html(bool IsWithTableBorder = false, string css = null)
        {
            if (IsWithTableBorder)
                border = " border=1";

            if (string.IsNullOrEmpty(css))
                html = "<style> td{border:1px solid black}th{border:1px solid red}</style>";
            else
                html = "<style>" + css + "</style>";
        }

        private string html = "";

        public string GetJson2Html(string json)
        {
            try
            {
                var dynamicJson = JsonConvert.DeserializeObject<dynamic>(json);

                if (dynamicJson.GetType() == typeof(JArray))
                {
                    JArray jArray = (JArray)dynamicJson;
                    foreach (JToken jItem in jArray)
                    {
                        GetObjectJson2Html(jItem.ToObject<JObject>());
                        html += "<br />";
                    }
                }
                else if (dynamicJson.GetType() == typeof(JObject))
                {
                    GetObjectJson2Html((JObject)dynamicJson);
                }
            }
            catch (Exception)
            {
            }
            return html;
        }

        private string GetObjectJson2Html(JObject jObject)
        {
            try
            {
                html += "<table" + border + "><tbody>";
                foreach (JProperty jProperty in jObject.Properties())
                {
                    html += "<tr><td><b>" + jProperty.Name + "</b></td><td>";
                    foreach (JToken jToken in jProperty)
                    {
                        if (jToken.Type == JTokenType.Array)
                            GetArrayJson2Html(jToken);
                        else if (jToken.Type == JTokenType.Object)
                        {
                            html += "<table" + border + "><thead><tr>";
                            JObject jObj = jToken as JObject;
                            List<string> jObjProList = jObj.Properties().Select(x => x.Name).ToList();
                            foreach (string strProperty in jObjProList) html += "<th>" + strProperty + "</th>";
                            html += "</tr></thead><tbody><tr>";
                            foreach (JProperty jProp in jObj.Properties())
                            {
                                if (jProp.Value.Type == JTokenType.Array)
                                {
                                    html += "<td>";
                                    html = GetArrayJson2Html(jProp.Value) + "</td>";
                                }
                                else if (jProp.Value.Type == JTokenType.Object)
                                {
                                    html += "<td>";
                                    html = GetObjectJson2Html((JObject)jProp.Value) + "</td>";
                                }
                                else
                                    html += "<td>" + jProp.Value + "</td>";
                            }
                            html += "</tr></tbody></table>";
                        }
                        else
                            html += jToken;
                    }
                    html += "</td></tr>";
                }
                html += "</tbody></table>";
            }
            catch (Exception)
            {
            }
            return html;
        }

        private string GetArrayJson2Html(JToken jToken)
        {
            try
            {
                html += "<table" + border + "><tbody><tr>";
                JToken _jToken = jToken.First;
                if (_jToken != null) foreach (JProperty jProperty in _jToken) html += "<th>" + jProperty.Name + "</th>";
                html += "</tr></thead><tbody>";
                foreach (JToken j_Token in (jToken as JArray))
                {
                    html += "<tr>";
                    foreach (JProperty jProperty in j_Token)
                    {
                        if (jProperty.HasValues)
                        {
                            if (jProperty.Value.Type == JTokenType.Array)
                            {
                                html += "<td>";
                                html = GetArrayJson2Html(jProperty.Value) + "</td>";
                            }
                            else if (jProperty.Value.Type == JTokenType.Object)
                            {
                                html += "<td>";
                                html = GetObjectJson2Html((JObject)jProperty.Value) + "</td>";
                            }
                            else
                                html += "<td>" + jProperty.Value + "</td>";
                        }
                    }
                    html += "</tr>";
                }
                html += "</tbody></table>";
            }
            catch (Exception)
            {
            }
            return html;
        }
    }
}
