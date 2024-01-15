function HorseClear::preprocess(%this, %client) {
    // Zmodyfikuj zapytanie SQL, aby uwzględniało ObjectTypeID = 776 w tabeli horses
    dbi.Select(HorseClear, "process", "SELECT ID FROM `horses` WHERE ObjectTypeID = 776");
}

function HorseClear::process(%this, %rs) {
    if (%rs.ok()) {
        while (%rs.nextRecord()) {
            %horseID = %rs.getFieldValue("ID");

            // Wywołaj funkcję DeleteTestHorse dla każdego %horseID
            HorseClear::DeleteTestHorse(%horseID);
        }
    }
    dbi.remove(%rs);
    %rs.delete();

    // Zaplanuj ponowne sprawdzenie po 5 minutach
    schedule(300000, 0, "HorseClear::startDeleteTestHorse");
}

function HorseClear::DeleteTestHorse(%ID) {
    // Pobierz czas do usunięcia konia
    %timeToDeletion = HorseClear::calculateTimeToDeletion(%ID);

    // Wyślij wiadomość o zbliżającym się usunięciu konia do wszystkich graczy
    HorseClear::sendMessageAboutDeletion(%ID, %timeToDeletion);

    // Tutaj umieść kod usuwający rekord o danym ID z tabeli `horses`
    // echo("Deleting horse with ID: " @ %ID);
    // Przykładowy kod usuwający rekord z tabeli
    // dbi.Delete("horses", "ID = " @ %ID);
}

function HorseClear::calculateTimeToDeletion(%ID) {
    // Tutaj można dodać logikę obliczania czasu do usunięcia konia na podstawie %ID
    // W przykładzie zastosowano prosty odliczanie czasu od 5 minut
    %timeToDeletion = 300;
    return %timeToDeletion;
}

function HorseClear::sendMessageAboutDeletion(%ID, %timeToDeletion) {
    // Ustal format wiadomości w zależności od czasu do usunięcia
    %message = HorseClear::formatDeletionMessage(%timeToDeletion);

    // Wyślij wiadomość do wszystkich graczy na serwerze
    HorseClear::messageAll(2480, %message);
}

function HorseClear::formatDeletionMessage(%timeToDeletion) {
    if (%timeToDeletion > 60) {
        %minutes = mFloor(%timeToDeletion / 60);
        %message = %minutes SPC "minutes to horse deletion!";
    } else {
        %message = %timeToDeletion SPC "seconds to horse deletion!";
    }
    return %message;
}

function HorseClear::version() {
    return "1.0.0";
}

function HorseClear::setup() {
    LiFx::registerCallback($LiFx::hooks::onConnectCallbacks, onConnectRequest, HorseClear);
    // Rozpocznij sprawdzanie co 5 minut
    schedule(300000, 0, "HorseClear::startDeleteTestHorse");
}

function HorseClear::onConnectRequest(%this, %client) {
    // Sprawdź, czy gracz podczas połączenia nie powinien już być usunięty
    HorseClear::DeleteTestHorse(%client.getID());
}

// Uruchomienie pakietu HorseClear
activatePackage(HorseClear);
