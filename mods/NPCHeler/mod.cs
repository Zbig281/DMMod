function serverCmdPlayerHeal(%client, %healingText, %charID)
{
    echo("serverCmdHealPlayer called");

    // Wykonaj leczenie
    dbi.Update("UPDATE `character` SET HardHP = 2000000000, SoftHP = 2000000000, HardStam = 2000000000, SoftStam = 2000000000, HungerRate = 10000 WHERE ID =" SPC %client.charID);
    dbi.Update("UPDATE `character_wounds` SET DurationLeft = 0 WHERE CharacterId = " SPC %client.charID);
    dbi.Update("DELETE FROM `character_effects` WHERE CharacterId =" SPC %client.charID);

    // Usuń obiekt gracza
    %player = %client.Player;
    %player.delete();

    // Zaplanuj inicjalizację gracza po usunięciu obiektu gracza
    %client.schedule(100, "initPlayerManager");
}
activatePackage(PlayerHeal);