using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Json2HtmlTable
{
    public class Json2Html
    {
        private readonly string border = "";
        public Json2Html(bool IsWithTableBorder = false,string css = null)
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
                JObject jObject = JsonConvert.DeserializeObject<JObject>(json);
                html += "<table"+ border + "><tbody>";
                foreach (JProperty jProperty in jObject.Properties())
                {
                    html += "<tr><td><b>" + jProperty.Name + "</b></td><td>";
                    foreach (JToken jToken in jProperty)
                    {
                        if (jToken.Type == JTokenType.Array)
                            GetArrayJson2Html(jToken);
                        else if (jToken.Type == JTokenType.Object)
                        {
                            html += "<table"+ border + "><thead><tr>";
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
                                    html = GetJson2Html(jProp.Value.ToString()) + "</td>";
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
                html += "<table"+ border + "><tbody><tr>";
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
                                html = GetJson2Html(jProperty.Value.ToString()) + "</td>";
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
