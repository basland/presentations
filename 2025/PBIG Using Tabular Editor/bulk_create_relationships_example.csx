// Example for Power BI Gebruikersdagen 2025
// Automating Power BI development with Tabular Editor
// Created by Bas Land, Kimura Data Intelligence B.V. and ThatFabricGuy.com 


// Loop through all tables to find PK_ columns
foreach (var pkTable in Model.Tables)
{
    foreach (var pkColumn in pkTable.Columns)
    {
        if (pkColumn.Name.StartsWith("PK_"))
        {
            // Extract the key name (without the "PK_" prefix)
            var keyName = pkColumn.Name.Substring(3);

            // Search for corresponding FK_ column in other tables
            foreach (var fkTable in Model.Tables)
            {
                if (fkTable == pkTable) continue; // Skip itself (dont create relationship with self
                
                foreach (var fkColumn in fkTable.Columns)
                {
                    if (fkColumn.Name == "FK_" + keyName)
                    {
                        // Check if relationship does not already exist
                        bool relationshipExists = Model.Relationships
                            .Any(r => r.ToColumn == pkColumn && r.FromColumn == fkColumn);

                        if (!relationshipExists)
                        {
                            // Create the relationship
                            var rel = Model.AddRelationship();
                            rel.FromColumn = fkColumn;  // Foreign key is always on the "From" side (fact table)
                            rel.ToColumn = pkColumn;    // Primary key is on the "To" side (dimension table)
                            rel.IsActive = true;
                            rel.CrossFilteringBehavior = CrossFilteringBehavior.OneDirection; // Single-direction filtering

                            // Hide the key columns, we don't want to use them in the report
                            pkColumn.IsHidden = true;
                            fkColumn.IsHidden = true;
                        }
                    }
                }
            }
        }
    }
}