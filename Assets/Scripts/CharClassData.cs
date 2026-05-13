using UnityEngine;

namespace Switch.Core
{
    public enum ClassType { Base, Tank, Mage, Assassin }

    [CreateAssetMenu(fileName = "New Class Data", menuName = "Switch/Character Class Data")]
    public class CharClassData : ScriptableObject
    {
        public ClassType classType;
        public float maxHealth;
        public float damage;
        public float speed;
        public float defense;
        
        [Header("Combat")]
        public float attackRange = 2f; // NEW: Every class gets its own range!
    }
}