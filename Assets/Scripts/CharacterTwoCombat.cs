using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTwoCombat : CombatBase
{
    protected override Dictionary<KeyCode, string> AttackMoves => new Dictionary<KeyCode, string>
    {
        { KeyCode.M, "Jab" },
        { KeyCode.K, "Roundhouse" },
        { KeyCode.L, "Elbow" }
    };
}

