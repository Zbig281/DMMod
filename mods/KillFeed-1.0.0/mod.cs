/**
* <author>Christophe Roblin & Warped Ibun</author>
* <url>lifxmod.com</url>
* <credits></credits>
* <description>Life Is Feudal kill feed System</description>
*/

if (!isObject(LiFxKillFeed))
{
    new ScriptObject(LiFxKillFeed)
    {
    };
}

package LiFxKillFeed {
  function LiFxKillFeed::setup() {
    //$LiFx::hooks::onDeathCallbacks =  JettisonArray("onDeathCallbacks");
    //$LiFx::hooks::onKillCallbacks =  JettisonArray("onKillCallbacks");
    //$LiFx::hooks:: = JettisonArray("onSuicideCallbacks");
    //LiFx::registerCallback($LiFx::hooks::onKillCallbacks, onKnockout, LiFxKillFeed);
    LiFx::registerCallback($LiFx::hooks::onKillCallbacks, onKill, LiFxKillFeed);
    LiFx::registerCallback($LiFx::hooks::onSuicideCallbacks, onSuicide, LiFxKillFeed);
  }
  function LiFxKillFeed::version() {
    return "1.0.0";
  }
  function LiFxKillFeed::addCustomText(%this, %resultSet) {
    if(%resultSet.ok() && %resultSet.nextRecord()) {
      %RootContainerID = %resultSet.getFieldValue("RootContainerID");
      %Name = %resultSet.getFieldValue("Name") SPC %resultSet.getFieldValue("LastName");
      dbi.Select(LiFxKillFeed, "addFeature","INSERT INTO `custom_texts` (`Custom_text`) VALUES ('" @ %name @ "') RETURNING ID, " @ %RootContainerID @ " as RootContainerID");
    }
    dbi.remove(%resultSet);
    %resultSet.delete();
  }
  function LiFxKillFeed::addFeature(%this, %resultSet) {
    if(%resultSet.ok() && %resultSet.nextRecord()) {
      %ReturnID = %resultSet.getFieldValue("ID");
      %RootContainerID = %resultSet.getFieldValue("RootContainerID");
      dbi.Select(LiFxKillFeed, "addFeatureToItem","INSERT INTO `features` (`CustomtextID`) VALUES (" @ %ReturnID @ ") RETURNING ID, " @ %RootContainerID @ " as RootContainerID");
    }
    dbi.remove(%resultSet);
    %resultSet.delete();
  }
  
  function LiFxKillFeed::addFeatureToItem(%this, %resultSet) {
    if(%resultSet.ok() && %resultSet.nextRecord()) {
      %ReturnID = %resultSet.getFieldValue("ID");
      %RootContainerID = %resultSet.getFieldValue("RootContainerID");
      dbi.Update("UPDATE `items` SET FeatureID = " @ %ReturnID @ " WHERE ID = (SELECT ID FROM `items` WHERE ObjectTypeID = 1409 and ContainerID = " @ %RootContainerID @ " ORDER BY ID desc LIMIT 1)");
    }
    dbi.remove(%resultSet);
    %resultSet.delete();
  }
  function LiFxKillFeed::MessageAllWithCustomText(%this, %resultSet) {
    if(%resultSet.ok() && %resultSet.nextRecord())
    {
      LiFxUtility::messageAll(2480, %resultSet.getFieldValue("Message"));
    }
    dbi.remove(%resultSet);
    %resultSet.delete();
  }
  
function LiFxKillFeed::onKill(%this, %CharID, %KillerID, %isKnockout, %Tombstone) {
    if(!%isKnockout) {
        LiFx::debugEcho(%this SPC %CharID SPC %isKnockout SPC %Tombstone);
        if(%KillerID $= "4294967294") {
            dbi.Select(LiFxKillFeed, "MessageAllWithCustomText", "SELECT CONCAT(CONCAT((SELECT CONCAT(Name, ' ',LastName) FROM `character` where ID = " @ %CharID @ "), ' was just killed by an NPC, NPCs have now killed a total of '),' ' ,(SELECT COUNT(*) FROM chars_deathlog WHERE KillerID != CharID and IsKnockout = 0 and KillerID = " @ %KillerID @ "), ' players in the wilderness') as Message");
        } else {
            dbi.Select(LiFxKillFeed, "MessageAllWithCustomText", "SELECT CONCAT(CONCAT((SELECT CONCAT(Name, ' ',LastName) FROM `character` where ID = " @ %CharID @ "), ' was butchered, '), (SELECT CONCAT(Name, ' ',LastName) from `character` where ID = " @ %KillerID @ "),' has now comitted ' ,(SELECT COUNT(*) FROM chars_deathlog WHERE KillerID != CharID and IsKnockout = 0 and KillerID = " @ %KillerID @ "), ' total murders') as Message");  
        }
    }
}

// function LiFxKillFeed::onKnockout(%this, %CharID, %KillerID, %LargeBag) {
//   if(%isKnockout) {
//   LiFx::debugEcho(%this SPC %CharID SPC %KillerID SPC %Tombstone);
//   dbi.Select(LiFxKillFeed, "MessageAllWithCustomText", "SELECT CONCAT(CONCAT((SELECT CONCAT(Name, ' ',LastName) FROM `character` where ID = " @ %CharID @ "), ' was knocked out by '), (SELECT CONCAT(Name, ' ',LastName) from `character` where ID = " @ %KillerID @ "),' ' ,(SELECT COUNT(*) FROM chars_knockoutlog WHERE KnockoutID != CharID and CharID = " @ %CharID @ "), ' Total Knockouts taken') as Message");
//   }
// }

  function LiFxKillFeed::onSuicide(%this, %CharID, %isKnockout, %Tombstone) {    
    // SELECT CONCAT((SELECT CONCAT(Name, ' ',LastName) FROM `character` where ID = " @ %CharID @ "), ' Gave up on life and comitted suicide. ', (SELECT COUNT(*) FROM chars_deathlog WHERE KillerID = CharID and CharID = " @ %CharID @ "), ' suicides') as Message
    dbi.Select(LiFxKillFeed, "MessageAllWithCustomText", "SELECT CONCAT((SELECT CONCAT(Name, ' ',LastName) FROM `character` where ID = " @ %CharID @ "), ' Gave up on life and comitted suicide. ', (SELECT COUNT(*) FROM chars_deathlog WHERE KillerID = CharID and CharID = " @ %CharID @ "), ' Total suicides') as Message");
  
  }
};
activatePackage(LiFxKillFeed);
