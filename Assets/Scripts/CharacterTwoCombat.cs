using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterTwoCombat : CombatBase
{
    protected override Dictionary<KeyCode, string> AttackMoves => new Dictionary<KeyCode, string>
    {
        { KeyCode.L, "Jab" },
        { KeyCode.K, "Roundhouse" },
        { KeyCode.M, "Elbow" }
    };
}

