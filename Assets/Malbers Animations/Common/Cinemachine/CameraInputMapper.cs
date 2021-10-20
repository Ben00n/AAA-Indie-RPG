using Cinemachine;
using UnityEngine;

/// <summary>
/// Thanks Tahir Alir!
/// </summary>
namespace MalbersAnimations
{
    public class CameraInputMapper : MonoBehaviour
    {
        public string TouchXInputMapTo = "Camera X";
        public string TouchYInputMapTo = "Camera Y";

        Vector2 delta;
        void Start()
        {
            CinemachineCore.GetInputAxis = GetInputAxis;
        }

        private float GetInputAxis(string axisName)
        {
            if (string.Equals(axisName, TouchXInputMapTo))
                return delta.x;
            if (string.Equals(axisName, TouchYInputMapTo))
                return delta.y;

            return 0;
        }

        public void CameraInput(Vector2 value) => delta = value;
    }
}