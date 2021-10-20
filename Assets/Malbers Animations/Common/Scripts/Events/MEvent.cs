using MalbersAnimations.Scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MalbersAnimations.Events
{
    ///<summary>
    /// The list of listeners that this event will notify if it is Invoked. 
    /// Based on the Talk - Game Architecture with Scriptable Objects by Ryan Hipple
    /// </summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Event", fileName = "New Event Asset", order = 1000)]
    public class MEvent : ScriptableObject
    {
        /// <summary>The list of listeners that this event will notify if it is raised.</summary>
        private readonly List<MEventItemListener> eventListeners = new List<MEventItemListener>();


#if UNITY_EDITOR
        [TextArea(3,10)]
        public string Description;
#endif
        public bool debug;

        public virtual void Invoke()
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked();

            if (debug) Debug.Log($"{name} Invoke()");
        }

        public virtual void Invoke(float value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }
        public virtual void Invoke(FloatVar value)
        {
            float val = value;

            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(val);

            DebugEvent(value);

        }
        public virtual void Invoke(bool value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }
        public virtual void Invoke(BoolVar value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value.Value);

            DebugEvent(value.Value);
        }

        public virtual void Invoke(string value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

        public virtual void Invoke(StringVar value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value.Value);

            DebugEvent(value.Value);
        }

        public virtual void Invoke(int value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);

        }

        public virtual void Invoke(IntVar value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value.Value);

            DebugEvent(value.Value);

        }

        public virtual void Invoke(IDs value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value.ID);

#if UNITY_EDITOR
            if (debug) Debug.Log($"{name} Invoke({value.name} - {value.ID})");
#endif
        }



   
        public virtual void Invoke(GameObject value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);
          
            DebugEvent(value);
        }

        public virtual void Invoke(Transform value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

        public virtual void Invoke(Vector3 value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

        public virtual void Invoke(Vector2 value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

        public virtual void Invoke(Component value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

         
        public virtual void Invoke(Sprite value)
        {
            for (int i = eventListeners.Count - 1; i >= 0; i--)
                eventListeners[i].OnEventInvoked(value);

            DebugEvent(value);
        }

        public virtual void RegisterListener(MEventItemListener listener)
        {
            if (!eventListeners.Contains(listener))  eventListeners.Add(listener);
        }

        public virtual void UnregisterListener(MEventItemListener listener)
        {
            if (eventListeners.Contains(listener))  eventListeners.Remove(listener);
        }

        private void DebugEvent(object value)
        {
#if UNITY_EDITOR
            if (debug) Debug.Log($"<B>{name}</B> - Invoke({value})");
#endif
        }


        ////This is for Debugin porpuses
        #region Debuging Tools
        public virtual void LogDeb(string text) => Debug.Log(text);
        public virtual void Pause() => Debug.Break();
        public virtual void LogDeb(bool value) => Debug.Log(name + ": " + value);
        public virtual void LogDeb(Vector3 value) => Debug.Log(name + ": " + value);
        public virtual void LogDeb(int value) => Debug.Log(name + ": " + value);
        public virtual void LogDeb(float value) => Debug.Log(name + ": " + value);
        public virtual void LogDeb(object value) => Debug.Log(name + ": " + value);
        public virtual void LogDeb(Component value) => Debug.Log(name + ": " + value);
        #endregion
    }
}
