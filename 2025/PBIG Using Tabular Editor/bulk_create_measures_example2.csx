// Example for Power BI Gebruikersdagen 2025
// Automating Power BI development with Tabular Editor
// Created by Bas Land, Kimura Data Intelligence B.V. and ThatFabricGuy.com

// Check if the table already exists
var measureTable = Model.Tables.FirstOrDefault(t => t.Name == "Calculations");

if (measureTable != null)
{
    // Loop through all tables to find m_ columns
    foreach (var table in Model.Tables)
    {
        foreach (var column in table.Columns.ToList()) // Convert to list to avoid modification issues
        {
            if (column.Name.StartsWith("m_"))
            {
                // Extract measure name (remove "m_" prefix)
                var measureName = column.Name.Substring(2);
                var daxExpression = "SUM ( " + table.Name + "[" + column.Name + "] )";

                // Check if the measure already exists
                var existingMeasure = measureTable.Measures.FirstOrDefault(m => m.Name == measureName);

                if (existingMeasure != null)
                {
                    existingMeasure.Expression = daxExpression;
                }
                else
                {
                    // Create new measure
                    var newMeasure = measureTable.AddMeasure(measureName, daxExpression);
                    newMeasure.DisplayFolder = table.Name;
                }

                // Hide the original column
                column.IsHidden = true;
            }
        }
    }
}