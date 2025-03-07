using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOneCombat : CombatBase
{
    protected override Dictionary<string, string> AttackMoves => new Dictionary<string, string>
    {
        { "j", "Jab" },
        { "k", "Cross" },
        { "l", "LowKick" },
        { "p", "Tackle" } // New Tackle Attack
    };
}


