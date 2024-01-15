function serverCmdOnClickedArcher(%client, %healingText, %charID)
{
  echo("serverCmdOnClickedArcher called");
  echo("CharID: " @ %client.charID);
  EqClear::preprocess(%client.charID);


  // Dodanie konkretnych umiejętności (skill_type) dla postaci dostającej leczenie
  changeSkill(%client.charID, 40, 0);         // Footman 36
  changeSkill(%client.charID, 38, 0);         // Swordsman 38
  changeSkill(%client.charID, 36, 0);         // Huscarl
  changeSkill(%client.charID, 45, 0);         // Berserker 45
  changeSkill(%client.charID, 44, 0);         // Vanguard 44
  changeSkill(%client.charID, 43, 0);         // Assaulter 43
  changeSkill(%client.charID, 47, 100);       // Slinger 47
  changeSkill(%client.charID, 48, 100);       // Archer 48
  changeSkill(%client.charID, 49, 100);       // Ranger 49

  // Dodanie broni do odpowiednich slotów postaci
  DMModArcher::addWeaponToSlot(%client.charID, 857, 1);     // Głowa
  DMModArcher::addWeaponToSlot(%client.charID, 858, 2);     // Klata
  DMModArcher::addWeaponToSlot(%client.charID, 859, 3);     // Przedramię
  DMModArcher::addWeaponToSlot(%client.charID, 860, 4);     // Ręce
  DMModArcher::addWeaponToSlot(%client.charID, 861, 5);     // Nogi
  DMModArcher::addWeaponToSlot(%client.charID, 862, 6);     // Buty
  DMModArcher::addWeaponToSlot(%client.charID, 603, 13);    // Bron lewa góra
  DMModArcher::addArrowToSlot(%client.charID, 656, 12);     // Bron prawa góra
  DMModArcher::addWeaponToSlot(%client.charID, 560, 11);    // Bron Lewy dół
  //DMModArcher::addWeaponToSlot(%client.charID, 656, 10);  // Bron prawa dół
  DMModArcher::addWeaponToSlot(%client.charID, 1376, 14);   // Tabard

  // Dodanie broni do EQ postaci
  DMModArcher::addWeaponToEQ(%client.charID, 592);

  // Ustawienie atrybutów postaci, leczenie, skaleczenia
  dbi.Update("UPDATE `character` SET Strength = 40000000, Agility = 50000000, Intellect = 10000000, Willpower = 10000000, Constitution = 40000000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character` SET HardHP = 2000000000, SoftHP = 2000000000, HardStam = 2000000000, SoftStam = 2000000000, HungerRate = 10000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character_wounds` SET DurationLeft = 0 WHERE CharacterId = " SPC %client.charID);
  dbi.Update("DELETE FROM `character_effects` WHERE CharacterId =" SPC %client.charID);
  DMModArcher::updateRandomGuildId(%client.charID);        
  DMModArcher::updateRandomCharacter(%client.charID);
  %client.schedule(100, "initPlayerManager");
  DMModArcher.schedule(100, "rotatePlayer", %client, %transform);
  %player = %client.Player;
  %player.delete();
  %client.cmSendClientMessage(2475, "The Archer's items were obtained");
}

//Funkcja random GuildID wybierz 1/4
function DMModArcher::updateRandomGuildId(%charID) {
  %randomGuildID = getRandom(1, 4);
  %newRoleID = 5;
  //bezposrednie zapytanie
  dbi.UPDATE("UPDATE `character` SET `GuildID` = " @ %randomGuildID @ ", `GuildRoleID` = " @ %newRoleID @ " WHERE `ID` = " @ %charID @ ";");
}

// Funkcja aktualizująca postać na podstawie losowego wyboru
function DMModArcher::updateRandomCharacter(%charID) {
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
function DMModArcher::addWeaponToEQ(%charID, %objectTypeID)
{
    dbi.Update("INSERT INTO `items` (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT `RootContainerID` FROM `character` WHERE ID =" @ %charID @ "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
}
// Funkcja do dodawania broni do konkretnych slotów postaci
function DMModArcher::addWeaponToSlot(%charID, %objectTypeID, %slotID)
{
    dbi.Update("INSERT INTO items (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT EquipmentContainerID FROM `character` WHERE ID =" SPC %charID SPC "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
    // Dodaj kod SQL do ustawienia slotu postaci na odpowiednią wartość
    dbi.Update("UPDATE `equipment_slots` SET ItemID = (SELECT LAST_INSERT_ID()), SkinID = NULL WHERE CharacterID =" SPC %charID SPC " AND Slot =" SPC %slotID);
}
// Funkcja do dodawania strzał do konkretnych slotów postaci
function DMModArcher::addArrowToSlot(%charID, %objectTypeID, %slotID)
{
    dbi.Update("INSERT INTO items (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT EquipmentContainerID FROM `character` WHERE ID =" SPC %charID SPC "), " @ %objectTypeID @ ", 80, 100, 18000, 18000, NULL)");
    // Dodaj kod SQL do ustawienia slotu postaci na odpowiednią wartość
    dbi.Update("UPDATE `equipment_slots` SET ItemID = (SELECT LAST_INSERT_ID()), SkinID = NULL WHERE CharacterID =" SPC %charID SPC " AND Slot =" SPC %slotID);
}

activatePackage(DMModArcher);
