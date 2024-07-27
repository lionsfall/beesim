using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Dogabeey
{
    public class WorldManager : SingletonComponent<WorldManager>
    {
        [Header("References")]
        public List<World> worlds;
        public Transform levelContainer;
        public ParticleSystem winParticle;

        private World currentWorld;

        public World CurrentWorld
        {
            get
            {
                return currentWorld;
            }
            set
            {

                currentWorld = value;
                
                EventManager.TriggerEvent(Const.GameEvents.CURRENT_WORLD_CHANGED, new EventParam());
            }
        }

        private void OnEnable()
        {
            EventManager.StartListening(Const.GameEvents.LEVEL_COMPLETED, OnLevelCompleted);
            EventManager.StartListening(Const.GameEvents.LEVEL_FAILED, OnLevelFailed);
        }
        private void OnDisable()
        {
            EventManager.StopListening(Const.GameEvents.LEVEL_COMPLETED, OnLevelCompleted);
            EventManager.StopListening(Const.GameEvents.LEVEL_FAILED, OnLevelFailed);
        }
        void OnLevelCompleted(EventParam param)
        {
            winParticle.Play();
            DOVirtual.DelayedCall(1, () =>
            ScreenManager.Instance.Show(Screens.WinScreen));
        }
        void OnLevelFailed(EventParam param)
        {
            DOVirtual.DelayedCall(1, () =>
            ScreenManager.Instance.Show(Screens.LoseScreen));
        }

        private void Start()
        {
            Application.targetFrameRate = 60;
            CurrentWorld = worlds[0];
            LoadCurrentLevel();
        }
        private void Update()
        {
            
        }

        public void LoadLevel(LevelScene levelScene)
        {
            EndCurrentLevel();
            World.Instance.CurrentLevel = Instantiate(levelScene, levelContainer);
        }
        public void LoadCurrentLevel()
        {
            LoadLevel(FindCurrentLevel());
        }
        public void EndCurrentLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                Destroy(World.Instance.CurrentLevel.gameObject);
                World.Instance.CurrentLevel = null;

            }
        }
        public void LoadNextLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                LoadLevel(FindNextLevel());
            }
        }
        public void ResetCurrentLevel()
        {
            if (World.Instance.CurrentLevel != null)
            {
                LoadLevel(FindCurrentLevel());
            }
        }
        private LevelScene FindCurrentLevel()
        {
            return World.Instance.levelScenes[World.Instance.lastPlayedLevelIndex % World.Instance.levelScenes.Count];
        }
        private LevelScene FindNextLevel()
        {
            World.Instance.lastPlayedLevelIndex++;
            return World.Instance.levelScenes[World.Instance.lastPlayedLevelIndex % World.Instance.levelScenes.Count];
        }

    }
}

