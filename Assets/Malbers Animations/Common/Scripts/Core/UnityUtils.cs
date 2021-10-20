using UnityEngine;
using System.Collections;
 

namespace MalbersAnimations
{
    [AddComponentMenu("Malbers/Utilities/Tools/Unity Utilities")]
    public class UnityUtils : MonoBehaviour, IAnimatorListener
    {
        public virtual void Freeze_Time(bool value) => Time.timeScale = value ? 0 : 1;

        /// <summary>Destroy this GameObject by a time </summary>
        public void DestroyMe(float time) => Destroy(gameObject, time);

        /// <summary>Destroy this GameObject</summary>
        public void DestroyMe() => Destroy(gameObject);

        /// <summary>Destroy this GameObject on the Next Frame</summary>
        public void DestroyMeNextFrame() => this.StartCoroutine(DestroyNextFrame());

        IEnumerator DestroyNextFrame()
        {
            yield return null;
            Destroy(gameObject);
        }

        /// <summary>Hide this GameObject after X Time</summary>
        public void GameObjectHide(float time) => Invoke(nameof(DisableGo), time);

        /// <summary>Enable the component if the parameter is not null</summary>
        public void SetActive(GameObject go) => enabled = go != null;

        /// <summary>Enable the gameobject if the parameter is not null</summary>
        public void SetActiveGO(GameObject go) => this.gameObject.SetActive(go != null);

        /// <summary>Enable the component if the parameter is not null</summary>
        public void SetActive(Component go) => enabled = go != null;

        /// <summary>Enable the gameobject if the parameter is not null</summary>
        public void SetActiveGO(Component go) => this.gameObject.SetActive(go != null);

        /// <summary>Destroy a GameObject</summary>
        public void DestroyGameObject(GameObject go) => Destroy(go);

        /// <summary>Random Rotate around X</summary>
        public void RandomRotateAroundX() => transform.Rotate(new Vector3(Random.Range(0, 360), 0, 0), Space.Self);

        /// <summary>Random Rotate around X</summary>
        public void RandomRotateAroundY() => transform.Rotate(new Vector3(0, Random.Range(0, 360), 0), Space.Self);
        /// <summary>Random Rotate around X</summary>
        public void RandomRotateAroundZ() => transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)), Space.Self);
        public void MoveLocal(Vector3 vector) => transform.Translate(vector, Space.Self);
        public void MoveWorld(Vector3 vector) => transform.Translate(vector, Space.World);
        public void Rotation_ResetLocal() => transform.localRotation = Quaternion.identity;
        public void Move_ResetLocal() => transform.localRotation = Quaternion.identity;

        /// <summary>Destroy a Component</summary>
        public void DestroyComponent(Component component) => Destroy(component);

        /// <summary>Parent this Game Object to a new Transform, retains its World Position</summary>
        public void Parent(Transform newParent) => transform.parent = newParent;
        public void Parent(GameObject newParent) => transform.parent = newParent.transform;
        public void Parent(Component newParent) => transform.parent = newParent.transform;
       

        /// <summary>Remove the Parent of a transform</summary>
        public void Unparent(Transform unparent) => unparent.parent = null;
        public void Unparent(GameObject unparent) => unparent.transform.parent = null;
        public void Unparent(Component unparent) => unparent.transform.parent = null;

        /// <summary>Disable a Behaviour</summary>
        public void Behaviour_Disable(int index)
        {
            var components = GetComponents<Behaviour>();
            if (components != null)
            {
                components[index % components.Length].enabled = false;
            }
        }
        public void Behaviour_Enable(int index)
        {
            var components = GetComponents<Behaviour>();
            if (components != null)
            {
                components[index % components.Length].enabled = true;
            }
        }



        /// <summary>Parent this Game Object to a new Transform</summary>
        public void Parent_Local(Transform newParent)
        {
            transform.parent = newParent;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>Instantiate a GameObject in the position of this gameObject</summary>
        public void Instantiate(GameObject go) => Instantiate(go, transform.position, transform.rotation);

        /// <summary>Instantiate a GameObject in the position of this gameObject and parent to this object</summary>
        public void InstantiateAndParent(GameObject go) => Instantiate(go, transform.position, transform.rotation, transform);

        public static void ShowCursor(bool value)
        {
            Cursor.lockState = !value ? CursorLockMode.Locked : CursorLockMode.None;  // Lock or unlock the cursor.
            Cursor.visible = value;
        }


        public virtual bool OnAnimatorBehaviourMessage(string message, object value) =>
          this.InvokeWithParams(message, value);

        public void RigidBody_SetKinematic(bool value)
        {
            var allRB = transform.GetComponentsInChildren<Rigidbody>();
            foreach (var rb in allRB)     rb.isKinematic = value;
        } 

        private void DisableGo() => gameObject.SetActive(false);
    }



#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(UnityUtils))]
    public class UnityUtilsEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        { 
            UnityEditor.EditorGUILayout.BeginVertical();
            UnityEditor.EditorGUILayout.HelpBox("Use this component to execute simple unity logics\n" +
                 "Like Parent, Instantiate, Destroy, Disable..\n Use it via Unity Events", UnityEditor.MessageType.None);
            UnityEditor.EditorGUILayout.EndVertical();
        }
    }
#endif
}
