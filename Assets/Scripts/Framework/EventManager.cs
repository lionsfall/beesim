using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dogabeey
{
    public class EventManager : MonoBehaviour
    {

        private Dictionary<string, Action<EventParam>> eventDictionary;

        private static EventManager eventManager;

        public static EventManager instance
        {
            get
            {
                if (!eventManager)
                {
                    eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                    if (!eventManager)
                    {
                        Debug.LogError("There needs to be one active EventManger script on a GameObject in your scene.");
                    }
                    else
                    {
                        eventManager.Init();
                    }
                }
                return eventManager;
            }
        }

        void Init()
        {
            if (eventDictionary == null)
            {
                eventDictionary = new Dictionary<string, Action<EventParam>>();
            }
        }

        public static void StartListening(string eventName, Action<EventParam> listener)
        {
            Action<EventParam> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Add more event to the existing one
                thisEvent += listener;

                //Update the Dictionary
                instance.eventDictionary[eventName] = thisEvent;
            }
            else
            {
                //Add event to the Dictionary for the first time
                thisEvent += listener;
                instance.eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening(string eventName, Action<EventParam> listener)
        {
            if (eventManager == null) return;
            Action<EventParam> thisEvent;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                //Remove event from the existing one
                thisEvent -= listener;

                //Update the Dictionary
                instance.eventDictionary[eventName] = thisEvent;
            }
        }

        public static void TriggerEvent(string eventName)
        {
            EventParam eventParam = new EventParam();
            Action<EventParam> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
        public static void TriggerEvent(string eventName, EventParam eventParam)
        {
            Action<EventParam> thisEvent = null;
            if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            {
                thisEvent.Invoke(eventParam);
                // OR USE  instance.eventDictionary[eventName](eventParam);
            }
        }
    }


    public class EventParam
    {
        public GameObject paramObj;
        public int paramInt;
        public float paramFloat;
        public string paramStr;
        public Type paramType;
        public bool paramBool;
        public Dictionary<string, object> paramDictionary;

        public EventParam()
        {

        }

        public EventParam(Dictionary<string, object> paramDictionary)
        {
            this.paramDictionary = paramDictionary;
        }

        public EventParam(GameObject paramObj = null, int paramInt = 0, float paramFloat = 0f, string paramStr = "", Type paramType = null, Dictionary<string, object> paramDictionary = null,
        Vector3[] vectorList = null, bool paramBool = false)
        {
            this.paramObj = paramObj;
            this.paramInt = paramInt;
            this.paramFloat = paramFloat;
            this.paramStr = paramStr;
            this.paramType = paramType;
            this.paramDictionary = paramDictionary;
            this.paramBool = paramBool;
        }
    }
}