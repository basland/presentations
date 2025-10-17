# Imports
from delta.tables import *

def upsert(spark_session, dataframe, table_name, table_keys):
    if DeltaTable.isDeltaTable(spark_session, f"Tables/{table_name}"):
        # perform upsert
        delta_table = DeltaTable.forName(spark_session, table_name)

        merge_condition = ' AND '.join([f"target.{key} = source.{key}" for key in table_keys])

        # Perform the merge
        delta_table.alias('target').merge(
            source=dataframe.alias('source'),
            condition=merge_condition
        ).whenMatchedUpdateAll() \
         .whenNotMatchedInsertAll() \
         .execute()
    else: 
        # copy df to delta 
        dataframe.write.format("delta").saveAsTable(table_name)