using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTwoCombat : CombatBase
{
    protected override Dictionary<string, string> AttackMoves => new Dictionary<string, string>
    {
        { "b", "Jab" },
        { "m", "FrontKick" },
        { "n", "Swing" },
        { "v", "Tackle" },
        { "buttonWest", "Jab" },   // Gamepad Y (Xbox) / Triangle (PS)
        { "buttonSouth", "FrontKick" },   // Gamepad A (Xbox) / Cross (PS)
        { "buttonNorth", "Swing" },    // Gamepad X (Xbox) / Square (PS)
        { "buttonEast", "Tackle"}
    };
}

