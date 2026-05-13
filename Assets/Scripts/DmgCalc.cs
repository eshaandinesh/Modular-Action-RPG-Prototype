using System.Collections.Generic;
using UnityEngine;
using Switch.Core;

public static class DmgCalc
{
    private static readonly Dictionary<ClassType, ClassType> WeaknessMap = new Dictionary<ClassType, ClassType>()
    {
        { ClassType.Tank, ClassType.Mage },       // tank is weak to mage
        { ClassType.Mage, ClassType.Assassin },   // mage is weak to assassin
        { ClassType.Assassin, ClassType.Tank }    // assassin is weak to tank
    };

    public static float GetCalculatedDamage(float baseDamage, ClassType attacker, ClassType defender)
    {
        if (WeaknessMap.ContainsKey(defender) && WeaknessMap[defender] == attacker)
        {
            return baseDamage * 1.5f; // super effective
        }
        else if (WeaknessMap.ContainsKey(attacker) && WeaknessMap[attacker] == defender)
        {
            return baseDamage * 0.5f; // not very effective
        }

        return baseDamage; // normal Damage
    }
}