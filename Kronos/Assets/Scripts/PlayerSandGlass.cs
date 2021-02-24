using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSandGlass : MonoBehaviour
{

    // abilities[i]["header"] : CSV 파일 내에 "header" 열에 있는 i번째 행의 값을 가져온다.
    List<Dictionary<string, object>> abilities;

    enum dataHeader
    {
        tier,
        type,
        name,
        desc,
        cost,
        maxLevel,
        memo,
        level,
    }

    private void Start()
    {
        abilities = CSVReader.Read("abilities");



        var writer = new CsvFileWriter(Application.dataPath + "/Resources/abilities2.csv");

        List<string> columns = new List<string>();

        foreach (dataHeader h in System.Enum.GetValues(typeof(dataHeader)))
        {
            string temp = h.ToString();
            columns.Add(temp);
        }

        writer.WriteRow(columns);
        columns.Clear();

        for (int i = 0; i < abilities.Count; i++)
        {
            columns.Clear();

            foreach (dataHeader h in System.Enum.GetValues(typeof(dataHeader)))
            {
                string header = h.ToString();
                string temp = abilities[i][header].ToString();
                Debug.Log($"temp = {temp}");
                columns.Add(temp);
            }
            writer.WriteRow(columns);
            columns.Clear();
        }


        writer.Dispose();


    }




    public void DataSave()
    {
    }


}

