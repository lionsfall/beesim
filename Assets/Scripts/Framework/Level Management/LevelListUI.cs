using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Sirenix.OdinInspector;

namespace Dogabeey
{
    public class LevelListUI : MonoBehaviour
    {
        [AssetsOnly]
        public Button levelButtonPrefab;
        public Button backButton;
        public Transform container;


        private void OnEnable()
        {
            DrawUI();
        }
        private void OnDisable()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            backButton.onClick.AddListener(() =>
            {
                ScreenManager.Instance.Show(Screens.WorldList);
            });
        }

        // Update is called once per frame
        void Update()
        {

        }
        
        private void DrawUI()
        {
            Debug.Log("Drawing the levels");

            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            if(WorldManager.Instance.CurrentWorld != null)
            {
                foreach (LevelScene level in WorldManager.Instance.CurrentWorld.levelScenes)
                {
                    Button node = Instantiate(levelButtonPrefab, container);
                    node.onClick.AddListener(() =>
                    {
                        WorldManager.Instance.LoadLevel(level);
                    });
                    node.GetComponentInChildren<TMP_Text>().text = level.levelName;
                }
            }
            else
            {
                Debug.LogError("Current World is null!");
            }
        }
    }

}