using UnityEngine;
using System.Collections.Generic;

namespace MalbersAnimations.Scriptables
{
    ///<summary> Store a list of Materials</summary>
    [CreateAssetMenu(menuName = "Malbers Animations/Scriptables/Variables/Material List", order = 2000)]
    public class MaterialListVar : ScriptableObject
    {
        [SerializeField] private List<Material> materials;
    }
}