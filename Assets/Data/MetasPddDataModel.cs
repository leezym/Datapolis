using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;


[System.Serializable]
public class MetaData
{
    public string numeracion;
    public string meta;
    public string cumplimiento;
    public string responsable;
}

[System.Serializable]
public class AreaData
{
    public string area;
    public List<MetaData> datos;
}

[System.Serializable]
public class AreasWrapper
{
    public List<AreaData> areas;
}

public class MetasPddDataModel
{
    public static MetasPddDataModel Instance { get; private set; }

    private List<AreaData> data;
    private string filePath;

    public MetasPddDataModel(string filePath)
    {
        this.filePath = filePath;
        data = new List<AreaData>();
        Instance = this;
    }

    // Initialization method that loads and stores the JSON data
    public void Initialize()
    {
        if (File.Exists(filePath))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                AreasWrapper wrapper = JsonUtility.FromJson<AreasWrapper>("{\"areas\":" + json + "}");
                data = wrapper.areas ?? new List<AreaData>();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error loading JSON: " + e.Message);
                data = new List<AreaData>();
            }
        }
        else
        {
            Debug.LogWarning("JSON file not found: " + filePath + ". Initializing with empty data.");
            data = new List<AreaData>();
        }
    }

    // Method to add or update data by taking a JSON object (MetaData) and area
    public void AddOrUpdateMeta(string area, string metaJson)
    {
        try
        {
            MetaData newMeta = JsonUtility.FromJson<MetaData>(metaJson);
            if (newMeta == null)
            {
                Debug.LogError("Invalid JSON for MetaData.");
                return;
            }

            AreaData areaData = data.Find(a => a.area == area);
            if (areaData == null)
            {
                areaData = new AreaData { area = area, datos = new List<MetaData>() };
                data.Add(areaData);
            }

            MetaData existingMeta = areaData.datos.Find(m => m.numeracion == newMeta.numeracion);
            if (existingMeta != null)
            {
                // Update
                existingMeta.meta = newMeta.meta;
                existingMeta.cumplimiento = newMeta.cumplimiento;
                existingMeta.responsable = newMeta.responsable;
            }
            else
            {
                // Add
                areaData.datos.Add(newMeta);
            }

            SaveData();
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error adding/updating meta: " + e.Message);
        }
    }
// Example usage of the MetasPddDataModel class
/*
using UnityEngine;

public class ExampleUsage : MonoBehaviour
{
    private MetasPddDataModel dataModel;

    void Start()
    {
        // Initialize the data model with the path to the JSON file
        dataModel = new MetasPddDataModel("Assets/Data/metas_pdd.json");
        dataModel.Initialize();

        // Example: Add or update a meta
        string newMetaJson = "{\"numeracion\":\"999\",\"meta\":\"Example meta\",\"cumplimiento\":\"50%\",\"responsable\":\"Example Dept\"}";
        dataModel.AddOrUpdateMeta("example_area", newMetaJson);

        // Example: Delete a meta by numeracion
        dataModel.DeleteMeta("1");

        // Access data
        List<AreaData> areas = dataModel.GetData();
        foreach (AreaData area in areas)
        {
            Debug.Log("AreaData: " + area.area);
            foreach (MetaData meta in area.datos)
            {
                Debug.Log("MetaData: " + meta.numeracion + " - " + meta.meta);
            }
        }
    }
}
*/

    // Method to delete specific elements from the stored data based on a key (numeracion)
    public bool DeleteMeta(string numeracion)
    {
        foreach (AreaData areaData in data)
        {
            MetaData meta = areaData.datos.Find(m => m.numeracion == numeracion);
            if (meta != null)
            {
                areaData.datos.Remove(meta);
                return true;
            }
        }
        Debug.LogWarning("MetaData with numeracion " + numeracion + " not found.");
        return false;
    }

    // Save data to JSON file
    private void SaveData()
    {
        try
        {
            AreasWrapper wrapper = new AreasWrapper { areas = data };
            string json = JsonUtility.ToJson(wrapper);
            // Remove the wrapper
            json = json.Substring(9, json.Length - 10); // Remove {"areas": and }
            File.WriteAllText(filePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving data: " + e.Message);
        }
    }

    // Get data for access
    public List<AreaData> GetData()
    {
        return data;
    }

    // Save data to a local JSON file
    public void SaveToFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        try
        {
            AreasWrapper wrapper = new AreasWrapper { areas = data };
            string json = JsonUtility.ToJson(wrapper);
            File.WriteAllText(path, json);
        }
        catch (Exception e)
        {
            Debug.LogError("Error saving data: " + e.Message);
        }
    }

    // Load data from a local JSON file
    public void LoadFromFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                AreasWrapper wrapper = JsonUtility.FromJson<AreasWrapper>(json);
                data = wrapper.areas ?? new List<AreaData>();
                Debug.Log("Data loaded from: " + path);
            }
            catch (Exception e)
            {
                Debug.LogError("Error loading data: " + e.Message);
                data = new List<AreaData>();
            }
        }
        else
        {
            Debug.LogWarning("Save file not found: " + path + ". Initializing with empty data.");
            data = new List<AreaData>();
        }
    }
}
