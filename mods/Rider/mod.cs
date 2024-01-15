function serverCmdOnClickedRider(%client, %healingText, %charID)
{
  echo("serverCmdOnClickedRider called");
  echo("CharID: " @ %client.charID);
  EqClear::preprocess(%client.charID);

  // Dodanie konkretnych umiejętności (skill_type) dla postaci dostającej leczenie
  changeSkill(%client.charID, 36, 100);        // Footman 36
  changeSkill(%client.charID, 38, 100);        // Swordsman 38
  changeSkill(%client.charID, 40, 0);        // Huscarl 40
  changeSkill(%client.charID, 45, 0);          // Berserker 45 
  changeSkill(%client.charID, 44, 0);          // Vanguard 44
  changeSkill(%client.charID, 43, 0);          // Assaulter 43
  changeSkill(%client.charID, 49, 0);          // Ranger 49
  changeSkill(%client.charID, 48, 0);          // Archer 48
  changeSkill(%client.charID, 47, 0);          // Slinger 47
  changeSkill(%client.charID, 28, 100);          // Cavalry 28
  changeSkill(%client.charID, 29, 100);          // Knight 29
  changeSkill(%client.charID, 30, 100);          // Lancer 30

  // Dodanie broni do odpowiednich slotów postaci
  DMModRider::addWeaponToSlot(%client.charID, 836, 1);       // Głowa
  DMModRider::addWeaponToSlot(%client.charID, 837, 2);       // Klata
  DMModRider::addWeaponToSlot(%client.charID, 838, 3);       // Przedramię
  DMModRider::addWeaponToSlot(%client.charID, 839, 4);       // Ręce
  DMModRider::addWeaponToSlot(%client.charID, 910, 5);       // Nogi
  DMModRider::addWeaponToSlot(%client.charID, 840, 6);       // Buty
  //DMModRider::addWeaponToSlot(%client.charID, 1141, 13);      // Bron lewa góra 
  DMModRider::addWeaponToSlot(%client.charID, 598, 12);      // Bron prawa góra 
  DMModRider::addWeaponToSlot(%client.charID, 557, 11);      // Bron Lewy dół
  //DMModRider::addWeaponToSlot(%client.charID, 656, 10);    // Bron prawa dół   
  DMModRider::addWeaponToSlot(%client.charID, 1376, 14);     // Tabard

  // Dodanie przedmiotu do inventory postaci
  DMModRider::addWeaponToEQ(%client.charID, 1044);

  // Ustawienie atrybutów postaci, leczenie, skaleczenia
  dbi.Update("UPDATE `character` SET Strength = 30000000, Agility = 10000000, Intellect = 10000000, Willpower = 10000000, Constitution = 90000000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character` SET HardHP = 2000000000, SoftHP = 2000000000, HardStam = 2000000000, SoftStam = 2000000000, HungerRate = 10000 WHERE ID =" SPC %client.charID);
  dbi.Update("UPDATE `character_wounds` SET DurationLeft = 0 WHERE CharacterId = " SPC %client.charID);
  dbi.Update("DELETE FROM `character_effects` WHERE CharacterId =" SPC %client.charID);
  DMModRider::updateRandomGuildId(%client.charID);
  DMModRider::updateRandomCharacter(%client.charID);
  %client.schedule(100, "initPlayerManager"); 
  DMModRider.schedule(100, "rotatePlayer", %client, %transform);
  %player = %client.Player;
  %player.delete();
  %client.cmSendClientMessage(2475, "The Rider's items were obtained");
}

//Funkcja random GuildID wybierz 1/4
function DMModRider::updateRandomGuildId(%charID) {
  %randomGuildID = getRandom(1, 4);
  %newRoleID = 5;
  //bezposrednie zapytanie
  dbi.UPDATE("UPDATE `character` SET `GuildID` = " @ %randomGuildID @ ", `GuildRoleID` = " @ %newRoleID @ " WHERE `ID` = " @ %charID @ ";");
}

// Funkcje aktualizujące postać
function DMModRider::updateRandomCharacter(%charID) {
    dbi.Update("UPDATE `character` SET `GeoID`=117158105, `GeoAlt`=5125, `OffsetMmX`=44, `OffsetMmY`=-19, `OffsetMmZ`=16 WHERE ID =" SPC %charID);
}

// Funkcja do dodania przedmiotu do inventory postaci
function DMModRider::addWeaponToEQ(%charID, %objectTypeID)
{
    dbi.Update("INSERT INTO `items` (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT `RootContainerID` FROM `character` WHERE ID =" @ %charID @ "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
}

// Funkcja do dodawania broni do konkretnych slotów postaci
function DMModRider::addWeaponToSlot(%charID, %objectTypeID, %slotID)
{    
    dbi.Update("INSERT INTO items (ContainerID, ObjectTypeID, Quality, Quantity, Durability, CreatedDurability, FeatureID) VALUES ((SELECT EquipmentContainerID FROM `character` WHERE ID =" SPC %charID SPC "), " @ %objectTypeID @ ", 80, 1, 18000, 18000, NULL)");
    // Dodaj kod SQL do ustawienia slotu postaci na odpowiednią wartość
    dbi.Update("UPDATE `equipment_slots` SET ItemID = (SELECT LAST_INSERT_ID()), SkinID = NULL WHERE CharacterID =" SPC %charID SPC " AND Slot =" SPC %slotID);
}

activatePackage(DMModRider);
