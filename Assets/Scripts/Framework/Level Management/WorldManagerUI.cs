using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dogabeey
{
    
    public class WorldManagerUI : MonoBehaviour
    {
        public WorldManager worldManager;
        public LevelListUI levelListUI;
        public Transform container;
        [AssetsOnly]
        public Button nodePrefab;
        public Button backButton;

        private void Start()
        {
            backButton.onClick.AddListener(() =>
            {
                ScreenManager.Instance.Show(Screens.MainMenu);
            });
            DrawUI();
        }

        public void DrawUI()
        {
            foreach (World world in worldManager.worlds)
            {
                if (world.mainWorld) continue;

                var node = Instantiate(nodePrefab, container);
                node.onClick.AddListener(() =>
                {
                    LoadWorld(world);
                });
                node.GetComponentInChildren<TMP_Text>().text = world.worldName;
            }

        }
        private void LoadWorld(World world)
        {
            WorldManager.Instance.CurrentWorld = world;
            ScreenManager.Instance.Show(Screens.LevelList);
        }
    }
    
}