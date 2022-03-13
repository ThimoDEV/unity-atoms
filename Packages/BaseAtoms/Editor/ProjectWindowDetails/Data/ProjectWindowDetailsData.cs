///MIT License
///Copyright(c) 2019 InnoGames GmbH
///
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityAtoms.Editor
{
    [Serializable]
    public class ProjectWindowDetailsData
    {
        public List<ProjectWindowDetailData> detailsData = new List<ProjectWindowDetailData>();

        private const string SAVE_FILE_NOREZ = "ProjectWindowSettings";
        private const string SAVE_FILE_EDITORPREFS = "ProjectWindowDetail.Save";
        private const string SAVE_FILE_EDITORPREFS_DEFAULT = "Editor/Setting/ProjectWindowSettings.json";

        public bool GetVisible(string name)
        {
            foreach (var detail in detailsData)
            {
                if (detail.Name == name)
                    return detail.Visible;
            }
                
            return false;
        }

        public void SetValueOrCreateNew(string name, bool visible)
        {
            bool isFind = false;
            for (int i = 0; i < detailsData.Count; ++i)
            {
                if (detailsData[i].Name == name)
                {
                    detailsData[i].Visible = visible;
                    isFind = true;
                    break;
                }
            }

            if (!isFind)
            {
                detailsData.Add(new ProjectWindowDetailData(name, visible));
            }
        }

        public static void SaveSettings(ProjectWindowDetailsData data)
        {
            if (!PlayerPrefs.HasKey(SAVE_FILE_EDITORPREFS))
            {
                string[] allPath = AssetDatabase.FindAssets(SAVE_FILE_NOREZ);
                if (allPath.Length != 0)
                    PlayerPrefs.SetString(SAVE_FILE_EDITORPREFS, AssetDatabase.GUIDToAssetPath(allPath[0]).Replace("Assets/", ""));
            }

            string savePath = Path.Combine(Application.dataPath, PlayerPrefs.GetString(SAVE_FILE_EDITORPREFS, SAVE_FILE_EDITORPREFS_DEFAULT));

            string json = JsonUtility.ToJson(data, true);

            if (!File.Exists(savePath))
            {
                FileInfo file = new FileInfo(savePath);
                file.Directory.Create();
            }

            File.WriteAllText(savePath, json);
        }

        public static ProjectWindowDetailsData LoadSettings()
        {
            if (!PlayerPrefs.HasKey(SAVE_FILE_EDITORPREFS))
            {
                string[] allPath = AssetDatabase.FindAssets(SAVE_FILE_NOREZ);
                if (allPath.Length != 0)
                    PlayerPrefs.SetString(SAVE_FILE_EDITORPREFS, AssetDatabase.GUIDToAssetPath(allPath[0]).Replace("Assets/", ""));
            }

            string savePath = Path.Combine(Application.dataPath, PlayerPrefs.GetString(SAVE_FILE_EDITORPREFS, SAVE_FILE_EDITORPREFS_DEFAULT));

            if (!File.Exists(savePath))
            {
                string json = "{ \"detailsData\": [ { \"Name\": \"Asset Type\", \"Visible\": true }, { \"Name\": \"Guid\", \"Visible\": false }] }";

                ProjectWindowDetailsData data = JsonUtility.FromJson<ProjectWindowDetailsData>(json);
                ProjectWindowDetailsData.SaveSettings(data);
                return data;
            }
            else
            {
                string json = File.ReadAllText(savePath);
                return JsonUtility.FromJson<ProjectWindowDetailsData>(json);
            }
        }

        [Serializable]
        public class ProjectWindowDetailData
        {
            public string Name;
            public bool Visible;

            public ProjectWindowDetailData(string name, bool visible)
            {
                this.Name = name;
                this.Visible = visible;
            }
        }
    }
}

