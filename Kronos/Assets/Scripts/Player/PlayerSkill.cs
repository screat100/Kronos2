using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Skill
{
    public Sprite icon;
    public string skillName;
    public int coolTime;
    public string desc;
    public string details;
    public string function;

    public Skill()
    {

    }
}

public class PlayerSkill : MonoBehaviour
{
    protected List<Dictionary<string, object>> skills_dict;
    List<Skill> skills;





    void Start()
    {
        skills_dict = CSVReader.Read("CSV/skills");

        skills = new List<Skill>();

        for(int i=0; i< skills_dict.Capacity; i++)
        {
            Skill temp = new Skill();
            temp.skillName = skills_dict[i]["skillName"].ToString();
            // temp.icon = (¸µÅ©);
            temp.coolTime = int.Parse(skills_dict[i]["coolTime"].ToString());
            temp.desc = skills_dict[i]["desc"].ToString();
            temp.details = skills_dict[i]["details"].ToString();
        }
    }

}
