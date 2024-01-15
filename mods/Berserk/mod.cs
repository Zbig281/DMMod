function serverCmdOnClickedBerserk(%client, %healingText, %charID)
{  
  echo("serverCmdOnClickedBerserk called");
  echo("CharID: " @ %client.charID);
  EqClear::preprocess(%client.charID);

    // Dodanie konkretnych umiejętności (skill_type) dla postaci dostającej leczenie
    changeSkill(%client.charID, 40, 0);          // Footman
    changeSkill(%client.charID, 38, 0);          // Swordsman
    changeSkill(%client.charID, 36, 1);          // Huscarl
    changeSkill(%client.charID, 43, 100);        // Berserker
    changeSkill(%client.charID, 44, 100);        // Vanguard
    changeSkill(%client.charID, 45, 100);        // Assaulter
    changeSkill(%client.charID, 49, 0);          // Ranger
    changeSkill(%client.charID, 48, 0);          // Archer
    changeSkill(%client.charID, 47, 1);          // Slinger

    // Dodanie broni do odpowiednich slotów postaci
    DMModBerserk::addWeaponToSlot(%client.charID, 633, 1);      // Głowa
    DMModBerserk::addWeaponToSlot(%client.charID, 634, 2);      // Klata
    DMModBerserk::addWeaponToSlot(%client.charID, 635, 3);      // Przedramię
    DMModBerserk::addWeaponToSlot(%client.charID, 636, 4);      // Ręce
    DMModBerserk::addWeaponToSlot(%client.charID, 637, 5);      // Nogi
    DMModBerserk::addWeaponToSlot(%client.charID, 638, 6);      // Buty
    DMModBerserk::addWeaponToSlot(%client.charID, 577, 13);     // Bron lewa góra
    DMModBerserk::addWeaponToSlot(%client.charID, 581, 12);     // Bron prawa góra
    //DMModBerserk::addWeaponToSlot(%client.charID, 656, 11);   // Bron Lewy dół
    //DMModBerserk::addWeaponToSlot(%client.charID, 656, 10);   // Bron prawa dół
    DMModBerserk::addWeaponToSlot(%client.charID, 1376, 14);   // Tabard

    // Dodanie broni do odpowiednich slotów postaci
    DMModBerserk::addWeaponToEQ(%client.charID, 592);

    // Ustawienie atrybutów postaci, leczenie, skaleczenia
    dbi.Update("UPDATE `character` SET Strength = 60000000, Agility = 10000000, Intellect = 10000000, Willpower = 10000000, Constitution = 60000000 WHERE ID =" SPC %client.charID);
    dbi.Update("UPDATE `character` SET HardHP = 2000000000, SoftHP = 2000000000, HardStam = 2000000000, SoftStam = 2000000000, HungerRate = 10000 WHERE ID =" SPC %client.charID);
    dbi.Update("UPDATE `character_wounds` SET DurationLeft = 0 WHERE CharacterId = " SPC %client.charID);
    dbi.Update("DELETE FROM `character_effects` WHERE CharacterId =" SPC %client.charID);  
    DMModBerserk::updateRandomGuildId(%client.charID);      
    DMModBerserk::updateRandomCharacter(%client.charID);
    %client.schedule(100, "initPlayerManager"); 
    DMModBerserk.schedule(100, "rotatePlayer", %client, %transform);
    %player = %client.Player;
    %player.delete();
    %client.cmSendClientMessage(2475, "The Berserk's items were obtained");
}


//Funkcja random GuildID wybierz 1/4
function DMModBerserk::updateRandomGuildId(%charID) {
  %randomGuildID = getRandom(1, 4);
  %newRoleID = 5;
  //bezposrednie zapytanie
  dbi.UPDATE("UPDATE `character` SET `GuildID` = " @ %randomGuildID @ ", `GuildRoleID` = " @ %newRoleID @ " WHERE `ID` =" SPC %charID);
}

// Funkcja aktualizująca postać na podstawie losowego wyboru
function DMModBerserk::updateRandomCharacter(%charID) {
    // Losowo wybierz funkcję z tablicy
    %randIndex = mFloor(getRandom() * 4); // 4, ponieważ masz cztery funkcje
    %selectedFunction = "DMModArcher::updateCharacter" @ (%randIndex + 1);

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
function DMModBerserk::addWeaponToEQ(%charID, %objectTypeID)
{
    dbi.Update("INSERT INTO `items` (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT `RootContainerID` FROM `character` WHERE ID =" @ %charID @ "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
}
// Funkcja do dodawania broni do konkretnych slotów postaci
function DMModBerserk::addWeaponToSlot(%charID, %objectTypeID, %slotID)
{
    // Dodaj kod SQL do dodania przedmiotu do slotu postaci
    dbi.Update("INSERT INTO items (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT EquipmentContainerID FROM `character` WHERE ID =" SPC %charID SPC "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
    // Dodaj kod SQL do ustawienia slotu postaci na odpowiednią wartość
    dbi.Update("UPDATE `equipment_slots` SET ItemID = (SELECT LAST_INSERT_ID()), SkinID = NULL WHERE CharacterID =" SPC %charID SPC " AND Slot =" SPC %slotID);
}

activatePackage(DMModBerserk);
