function DMModTank::preprocess(%charID) {
    echo("Character ID: " @ %charID);

    // Dodaj komunikaty debugujące
    echo("Clearing equipment for character ID: " @ %charID);
    
    // Usuń rekordy związane z graczem z tabeli 'equipment_slots' i ustaw ItemID na NULL
    %eqQuery = "UPDATE equipment_slots SET ItemID = NULL WHERE CharacterID = " @ %charID;
    echo("Executing query: " @ %eqQuery);
    dbi.query(DMModTank, "process", %eqQuery);
    
    // Usuń rekordy związane z graczem z tabeli 'items'
    %itemQuery = "DELETE FROM items WHERE ContainerID IN (SELECT RootContainerID FROM `character` WHERE ID = " @ %charID @ ")";
    echo("Before executing query: " @ %itemQuery);
    dbi.query(DMModTank, "process", %itemQuery);
    echo("After executing query: " @ %itemQuery);
}

function serverCmdOnClickedTank(%client, %healingText, %charID)
{
  echo("serverCmdOnClickedTank called");
  echo("CharID: " @ %client.charID);
  EqClear::preprocess(%client.charID);

// Dodanie konkretnych umiejętności (skill_type) dla postaci dostającej leczenie
  changeSkill(%client.charID, 36, 100);        // Footman 36
  changeSkill(%client.charID, 38, 100);        // Swordsman 38
  changeSkill(%client.charID, 40, 100);        // Huscarl 40
  changeSkill(%client.charID, 45, 0);          // Berserker 45 
  changeSkill(%client.charID, 44, 0);          // Vanguard 44
  changeSkill(%client.charID, 43, 1);          // Assaulter 43
  changeSkill(%client.charID, 49, 0);          // Ranger 49
  changeSkill(%client.charID, 48, 0);          // Archer 48
  changeSkill(%client.charID, 47, 1);          // Slinger 47

  // Dodanie broni do odpowiednich slotów postaci
  DMModTank::addWeaponToSlot(%client.charID, 803, 1);       // Głowa
  DMModTank::addWeaponToSlot(%client.charID, 804, 2);       // Klata
  DMModTank::addWeaponToSlot(%client.charID, 805, 3);       // Przedramię
  DMModTank::addWeaponToSlot(%client.charID, 806, 4);       // Ręce
  DMModTank::addWeaponToSlot(%client.charID, 807, 5);       // Nogi
  DMModTank::addWeaponToSlot(%client.charID, 808, 6);       // Buty
  DMModTank::addWeaponToSlot(%client.charID, 615, 13);      // Bron lewa góra 
  DMModTank::addWeaponToSlot(%client.charID, 616, 12);      // Bron prawa góra 
  DMModTank::addWeaponToSlot(%client.charID, 556, 11);      // Bron Lewy dół
  //DMModTank::addWeaponToSlot(%client.charID, 656, 10);    // Bron prawa dół   
  DMModTank::addWeaponToSlot(%client.charID, 1376, 14);     // Tabard

  // Dodanie przedmiotu do inventory postaci
  DMModTank::addWeaponToEQ(%client.charID, 592);


  // Ustawienie atrybutów postaci, leczenie, skaleczenia
  dbi.Update("UPDATE `character` SET Strength = 30000000, Agility = 10000000, Intellect = 10000000, Willpower = 10000000, Constitution = 90000000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character` SET HardHP = 2000000000, SoftHP = 2000000000, HardStam = 2000000000, SoftStam = 2000000000, HungerRate = 10000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character_wounds` SET DurationLeft = 0 WHERE CharacterId = " SPC %client.charID);
  dbi.Update("DELETE FROM `character_effects` WHERE CharacterId =" SPC %client.charID);
  DMModTank::updateRandomGuildId(%client.charID);
  DMModTank::updateRandomCharacter(%client.charID);
  %client.schedule(100, "initPlayerManager"); 
  DMModTank.schedule(100, "rotatePlayer", %client, %transform);
  %player = %client.Player;
  %player.delete();
  %client.cmSendClientMessage(2475, "The Tank's items were obtained"); //system chat 2475
}

function NPCMaster::version() {
    return "1.0.0";
}

//Funkcja random GuildID wybierz 1/4
function DMModTank::updateRandomGuildId(%charID) {
  %randomGuildID = getRandom(1, 4);
  %newRoleID = 5;
  //bezposrednie zapytanie
  dbi.UPDATE("UPDATE `character` SET `GuildID` = " @ %randomGuildID @ ", `GuildRoleID` = " @ %newRoleID @ " WHERE `ID` = " @ %charID @ ";");
}

// Funkcja aktualizująca postać na podstawie losowego wyboru
function DMModTank::updateRandomCharacter(%charID) {
    // Losowo wybierz funkcję z tablicy
    %randIndex = mFloor(getRandom() * 4); // 4, ponieważ masz cztery funkcje
    %selectedFunction = "DMModTank::updateCharacter" @ (%randIndex + 1);

    // Wywołaj oryginalną funkcję
    call(%selectedFunction, %charID);

    // Dodaj dodatkowe operacje w zależności od wybranej funkcji
    switch (%randIndex) {
        case 0:
            // Dodatkowe operacje dla updateCharacter1
            dbi.Update("UPDATE `character` SET `GeoID`=117162664, `GeoAlt`=5125, `OffsetMmX`=349, `OffsetMmY`=-628, `OffsetMmZ`=22 WHERE ID =" SPC %charID);
        case 1:
            // Dodatkowe operacje dla updateCharacter2
            dbi.Update("UPDATE `character` SET `GeoID`=117152428, `GeoAlt`=5125, `OffsetMmX`=326, `OffsetMmY`=-81, `OffsetMmZ`=17 WHERE ID =" SPC %charID);
        case 2:
            // Dodatkowe operacje dla updateCharacter3
            dbi.Update("UPDATE `character` SET `GeoID`=117158069, `GeoAlt`=5125, `OffsetMmX`=-96, `OffsetMmY`=-7, `OffsetMmZ`=16 WHERE ID =" SPC %charID);
        case 3:
            // Dodatkowe operacje dla updateCharacter4
            dbi.Update("UPDATE `character` SET `GeoID`=117158046, `GeoAlt`=5125, `OffsetMmX`=638, `OffsetMmY`=-948, `OffsetMmZ`=23 WHERE ID =" SPC %charID);
    }
}

// Funkcja do dodania przedmiotu do inventory postaci
function DMModTank::addWeaponToEQ(%charID, %objectTypeID)
{
    dbi.Update("INSERT INTO `items` (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT `RootContainerID` FROM `character` WHERE ID =" @ %charID @ "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
}

// Funkcja do dodawania broni do konkretnych slotów postaci
function DMModTank::addWeaponToSlot(%charID, %objectTypeID, %slotID)
{    
    dbi.Update("INSERT INTO items (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT EquipmentContainerID FROM `character` WHERE ID =" SPC %charID SPC "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
    // Dodaj kod SQL do ustawienia slotu postaci na odpowiednią wartość
    dbi.Update("UPDATE `equipment_slots` SET ItemID = (SELECT LAST_INSERT_ID()), SkinID = NULL WHERE CharacterID =" SPC %charID SPC " AND Slot =" SPC %slotID);
}

activatePackage(DMModTank);
