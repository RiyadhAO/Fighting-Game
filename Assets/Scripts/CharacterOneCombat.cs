using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterOneCombat : CombatBase
{
    protected override Dictionary<KeyCode, string> AttackMoves => new Dictionary<KeyCode, string>
    {
        { KeyCode.E, "Jab" },
        { KeyCode.K, "Uppercut" },
        { KeyCode.L, "Hook" }
    };
}


