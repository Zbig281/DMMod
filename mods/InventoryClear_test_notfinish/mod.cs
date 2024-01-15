function EqClear::preprocess(%charID) {
    echo("Character ID: " @ %charID);

    // Dodaj komunikaty debugujące
    echo("Clearing equipment for character ID: " @ %charID);
    
    // Usuń rekordy związane z graczem z tabeli 'equipment_slots' i ustaw ItemID na NULL
    %eqQuery = "UPDATE equipment_slots SET ItemID = NULL WHERE CharacterID = " @ %charID;
    echo("Executing query: " @ %eqQuery);
    dbi.query(EqClear, "process", %eqQuery);
    
    // Usuń rekordy związane z graczem z tabeli 'items'
    %itemQuery = "DELETE FROM items WHERE ContainerID IN (SELECT RootContainerID FROM `character` WHERE ID = " @ %charID @ ")";
    echo("Before executing query: " @ %itemQuery);
    dbi.query(EqClear, "process", %itemQuery);
    echo("After executing query: " @ %itemQuery);


}

if (!isObject(EqClear)) {
    new ScriptObject(EqClear) {};
}

package EqClear 
{
    function EqClear::process(%rs) {
        if (%rs.ok()) {
            while (%rs.nextRecord()) {
                %charID = %rs.getFieldValue("ID");
                // Wywołaj funkcję preprocess dla każdego %charID
                EqClear::preprocess(%charID);
            }
        }
        dbi.remove(%rs);
        %rs.delete();
        echo("Deleted records count: " @ %rs.getRecordCount());
    }

    function EqClear::version() {
        return "1.0.0";
    }
};
activatePackage(EqClear);
