#r "System.Net.Http"
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

// You need to signin to https://platform.openai.com/ and create an API key for your profile then paste that key 
// into the apiKey constant below
const string apiKey = "<<<your super secret token here>>>";
const string uri = "https://api.openai.com/v1/chat/completions";
const string developerMessage = "You are to assume to role of an experienced Power BI consultant. You will be asked to generate descriptions of tables, columns and measures in a Power BI semantic model. These descriptions will be exposed to Power BI end users. The descriptions should be clear and concise and follow standard business language.";

using (var client = new HttpClient())
{
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

    // Loop through each table in the model.
    foreach (var t in Model.Tables)
    {
        // --- TABLE DESCRIPTION ---
        // Build a comma-separated list of the table's columns.
        string columnsList = string.Join(", ", t.Columns.Select(c => c.Name));
        // Build the prompt for the table description.
        string tablePrompt = "Write a description for the table " + t.Name + " that contains columns: " + columnsList + ".";
        // Create the JSON request body.
        var tableRequestObj = new {
            model = "gpt-4o-mini",
            messages = new object[] {
                new { role = "developer", content = developerMessage },
                new { role = "user", content = tablePrompt }
            }
        };
        string tableBody = JsonConvert.SerializeObject(tableRequestObj);
        
        // Send the POST request for the table description.
        var tableResponse = client.PostAsync(uri, new StringContent(tableBody, Encoding.UTF8, "application/json")).Result;
        tableResponse.EnsureSuccessStatusCode();
        string tableResult = tableResponse.Content.ReadAsStringAsync().Result;
        JObject tableObj = JObject.Parse(tableResult);
        string tableDescription = tableObj["choices"][0]["message"]["content"].ToString().Trim();
        
        // Assign the description to the table.
        t.Description = tableDescription;

        // column descriptions here
        foreach (var col in t.Columns)
        {
            // Build the prompt for the column description.
            string colPrompt = "Write a description for the column " + col.Name + " in table " + t.Name + ".";
            var colRequestObj = new {
                model = "gpt-4o-mini",
                messages = new object[] {
                    new { role = "developer", content = developerMessage },
                    new { role = "user", content = colPrompt }
                }
            };
            string colBody = JsonConvert.SerializeObject(colRequestObj);
            var colResponse = client.PostAsync(uri, new StringContent(colBody, Encoding.UTF8, "application/json")).Result;
            colResponse.EnsureSuccessStatusCode();
            string colResult = colResponse.Content.ReadAsStringAsync().Result;
            JObject colObj = JObject.Parse(colResult);
            string colDescription = colObj["choices"][0]["message"]["content"].ToString().Trim();
            // Assign the description to the column.
            col.Description = colDescription;
        }

        // --- MEASURE DESCRIPTIONS ---
        foreach (var m in t.Measures)
        {
            // Build the prompt for the measure description.
            string measurePrompt = "Write a description for the measure " + m.Name + " in table " + t.Name + " with the following DAX code: " + m.Expression + ".";
            var measureRequestObj = new {
                model = "gpt-4o-mini",
                messages = new object[] {
                    new { role = "developer", content = developerMessage },
                    new { role = "user", content = measurePrompt }
                }
            };
            string measureBody = JsonConvert.SerializeObject(measureRequestObj);
            var measureResponse = client.PostAsync(uri, new StringContent(measureBody, Encoding.UTF8, "application/json")).Result;
            measureResponse.EnsureSuccessStatusCode();
            string measureResult = measureResponse.Content.ReadAsStringAsync().Result;
            JObject measureObj = JObject.Parse(measureResult);
            string measureDescription = measureObj["choices"][0]["message"]["content"].ToString().Trim();
            // Append the DAX code for end users.
            m.Description = measureDescription + "\n=====\n" + m.Expression;
        }
    }
}