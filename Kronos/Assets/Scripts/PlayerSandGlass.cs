using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSandGlass : MonoBehaviour
{

    // abilities[i]["header"] : CSV 파일 내에 "header" 열에 있는 i번째 행의 값을 가져온다.
    protected List<Dictionary<string, object>> abilities;
    protected List<Dictionary<string, object>> abilities_text;

    enum dataHeader
        /*
         * abilities.csv의 헤더들
         */
    {
        tier,
        type,
        cost,
        maxLevel,
        level,
    }

    enum dataHeader_text
        /*
         * abilities_text.csv의 헤더들
         */
    {
        name,
        desc,
    }

    protected int[] SumLevelOfType;

    protected enum typeOfAbility
    {
        attack,
        health,
        skill,
        critical,
        shield,
        utility,
    }

    private void Awake()
    {
        abilities = CSVReader.Read("abilities");
        abilities_text = CSVReader.Read("abilities_text");

        SumLevelOfType = new int[6];
    }


    public void DataSave()
        /*
         * 현재 List에 저장된 데이터들을 .csv 파일에 저장
         * columns에 각 행의 내용들을 add하고 writer.WriteRow()를 사용해 행을 저장
         */
    {
        var writer = new CsvFileWriter(Application.dataPath + "/Resources/abilities.csv");

        List<string> columns = new List<string>();

        // 첫 행은 data_header
        foreach (dataHeader h in System.Enum.GetValues(typeof(dataHeader)))
        {
            string temp = h.ToString();
            columns.Add(temp);
        }

        writer.WriteRow(columns);
        columns.Clear();

        // 이후 행들은 실제 데이터 삽입
        for (int i = 0; i < abilities.Count; i++)
        {
            columns.Clear();

            foreach (dataHeader h in System.Enum.GetValues(typeof(dataHeader)))
            {
                string header = h.ToString();
                string temp = abilities[i][header].ToString();
                columns.Add(temp);
            }
            writer.WriteRow(columns);
            columns.Clear();
        }

        writer.Dispose();
        Debug.Log("data saved");
    }

    protected void RenewSumLevelOfType()
        /*
         * 각 계열 능력들의 스킬 합을 구해서 저장
         */
    {
        for(int i=0; i<SumLevelOfType.Length; i++)
        {
            SumLevelOfType[i] = 0;
        }

        for (int i=0; i<abilities.Count; i++)
        {
            switch(abilities[i]["type"].ToString()) 
            {
                case "attack":
                    SumLevelOfType[0] += int.Parse(abilities[i]["level"].ToString());
                    break;

                case "health":
                    SumLevelOfType[1] += int.Parse(abilities[i]["level"].ToString());
                    break;

                case "skill":
                    SumLevelOfType[2] += int.Parse(abilities[i]["level"].ToString());
                    break;

                case "critical":
                    SumLevelOfType[3] += int.Parse(abilities[i]["level"].ToString());
                    break;

                case "shield":
                    SumLevelOfType[4] += int.Parse(abilities[i]["level"].ToString());
                    break;

                case "utility":
                    SumLevelOfType[5] += int.Parse(abilities[i]["level"].ToString());
                    break;

                default:
                    break;
            }
        }
    }

}

