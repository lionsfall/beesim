using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.VisualScripting;
using Dogabeey.SimpleJSON;

namespace Dogabeey
{
    public class World : MonoBehaviour, ISaveable
    {
        public static World Instance
        {
            get
            {
                return WorldManager.Instance.CurrentWorld;
            }
        }

        public string worldName;
        public bool mainWorld;
        public List<LevelScene> levelScenes;
        public int lastPlayedLevelIndex;

        private LevelScene currentLevel;

        public LevelScene CurrentLevel { get => currentLevel;
            set
            {
                currentLevel = value;
                //lastPlayedLevelIndex = levelScenes.IndexOf(currentLevel);
            }
        }

        public string SaveId => "World_" + worldName;

        protected void Awake()
        {
            SaveManager.Instance.Register(this);
            if(!Load())
            {
                lastPlayedLevelIndex = 0;
            }
            
        }

        public void PauseLevel(bool pause)
        {
            currentLevel.gameObject.SetActive(!pause);
        }

        public Dictionary<string, object> Save()
        {
            Dictionary<string, object> saveData = new Dictionary<string, object>
            {
                { "lastPlayedLevelIndex", lastPlayedLevelIndex }
            };

            return saveData;
        }

        public bool Load()
        {
            JSONNode saveData = SaveManager.Instance.LoadSave(this);

            if (saveData == null)
            {
                return false;
            }

            lastPlayedLevelIndex = (int) saveData["lastPlayedLevelIndex"];

            return true;
        }
    }

}
