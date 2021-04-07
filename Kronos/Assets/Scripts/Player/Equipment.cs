using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    public enum EquipmentType
    {
        Weapon,
        Armor,
        Shield,
    }

    [Header("Equipment Status")]
    public int HP;
    public int attack;
    public int shield;
    public int attackSpeed;
    public int moveSpeed;
    public int criticalProb;
    public int criticalDamage;
    public float avoidanceRate;
    public float coolTimeDecreaseRate;
    
    public float stamina;
    public float time;

    public class Weapon : MonoBehaviour
    {
        
    }

    public class Armor : MonoBehaviour
    {

    }

    public class Shield : MonoBehaviour
    {

    }

    public EquipmentType equipmentType;


}
