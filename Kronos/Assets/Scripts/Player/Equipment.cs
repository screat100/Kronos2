using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EquipmentType
{
    Weapon,
    Armor,
    Shield,
}

public class Equipment : MonoBehaviour
{
    
    [Header("Equipment Type")]
    public EquipmentType equipmentType;
    public BasicAttack basicAttack;
    public ShiftSkill shiftSkill;
    public WeaponSkillSet weaponSkillSet;
    public Parrying parrying;

    [Header("Equipment Level")]
    public int level;
    public float playtime;
    public float accumulatedPlaytime;

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

   

}
