using System.Collections;
using UnityEngine;

 
namespace MalbersAnimations.Controller.Reactions
{
    /// <summary>Abstract Class to anything react </summary>
    public abstract class Reaction<T> : ScriptableObject where T : Component
    {
        [HelpBox]
        public string description;
        public bool active = true;
        public float delay = 0f;
        [HideInInspector] public string fullName;

        /// <summary>Instant Reaction ... without considering Active or Delay parameters</summary>
        protected abstract void _React(T reactor);

        /// <summary>Instant Reaction ... without considering Active or Delay parameters</summary>
        protected abstract bool _TryReact(T reactor);


        public void React(Component component) => React(component.FindComponent<T>());
        public void React(GameObject go) => React(go.FindComponent<T>());

        public void React(MonoBehaviour Mono)
        {
            if (Mono is T) _React(Mono as T);
        }


        public void React(T reactor)
        {
            if (reactor != null && active)
            {
                if (delay > 0 && (reactor is MonoBehaviour))
                    (reactor as MonoBehaviour).StartCoroutine(DelayedReaction(reactor));
                else
                    _React(reactor);
            }
        }


        private IEnumerator DelayedReaction(T reactor)
        {
            yield return new WaitForSeconds(delay);
            _React(reactor);
        }

        public bool TryReact(Component component) => TryReact(component.FindComponent<T>());

        public bool TryReact(GameObject gameObj) 
        {
            var reactor = gameObj.FindComponent<T>();
            return TryReact(reactor);
        }

        public bool TryReact(T reactor)
        {
            if (reactor != null && active)
                return _TryReact(reactor);

            return false;
        }


        public MonoBehaviour FindTComponent(MonoBehaviour[] monos)
        {
            foreach (var m in monos) if (m is T) return m; //Find the reactor Monobehaviour
            return null;
        }
    }
}