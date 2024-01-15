// Zadawanie Zapytan do mysql
function ClearSweep::preprocess(%this, %client) {
    dbi.Select(ClearSweep, "process", "SELECT GeoDataID FROM `movable_objects` WHERE ObjectTypeID = 1070 OR ObjectTypeID = 1083 OR ObjectTypeID = 1147");
}

if (!isObject(ClearSweep)) {
    new ScriptObject(ClearSweep) {};
}

package ClearSweep {
    function ClearSweep::setup() {
        LiFx::registerCallback($LiFx::hooks::onConnectCallbacks, onConnectRequest, ClearSweep);
        // Rozpocznij cykliczne wywoływanie funkcji DeleteTestMovable co 30 sekund
        schedule(30000, 0, "ClearSweep::startDeleteTestMovable");
    }

    function ClearSweep::startDeleteTestMovable() {
        // Rozpocznij proces usuwania dla wszystkich GeoDataID
        ClearSweep::preprocess();

        // Zaplanuj ponowne wywołanie funkcji DeleteTestMovable co 30 sekund
        schedule(30000, 0, "ClearSweep::startDeleteTestMovable");
    }

    function ClearSweep::onConnectRequest(%this, %client) {
        ClearSweep.schedule(1000, "preprocess", %client);
    }

    function ClearSweep::preprocess(%this, %client) {
        dbi.Select(ClearSweep, "process", "SELECT GeoDataID FROM `movable_objects` WHERE ObjectTypeID = 1070 OR ObjectTypeID = 1083 OR ObjectTypeID = 1147");
    }

    function ClearSweep::process(%this, %rs) {
        if (%rs.ok()) {
            while (%rs.nextRecord()) {
                %geoDataID = %rs.getFieldValue("GeoDataID");

                // Wywołaj funkcję DeleteTestMovable dla każdego %geoDataID
                DeleteTestMovable(%geoDataID);
            }
        }
        dbi.remove(%rs);
        %rs.delete();
    }

    function ClearSweep::DeleteTestMovable(%geoDataID) {
        // Wywołaj funkcję DeleteTestMovable z %geoDataID
        echo("Deleting movable with GeoDataID: " @ %geoDataID);
        // Tutaj możesz umieścić kod, który chcesz wykonać dla każdego %geoDataID
    }

    function ClearSweep::version() {
        return "1.0.0";
    }
};

activatePackage(ClearSweep);
