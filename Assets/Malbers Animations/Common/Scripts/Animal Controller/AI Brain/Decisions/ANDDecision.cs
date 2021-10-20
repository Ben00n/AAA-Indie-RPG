using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/AND Decision")]
    public class ANDDecision : MAIDecision
    {
        public List<MAIDecision> decisions;

        public override void PrepareDecision(MAnimalBrain brain, int Index)
        {
            foreach (var d in decisions) d.PrepareDecision(brain, Index);
        }

        public override bool Decide(MAnimalBrain brain, int Index)
        {
            foreach (var d in decisions)
            {
                bool Decision = d.Decide(brain, Index);
               // Debug.Log($"Decision {d.name} =  {Decision}");
                if (!Decision) return false;
            }

            return true;
        }
        public override void FinishDecision(MAnimalBrain brain, int Index)
        {
            foreach (var d in decisions) d?.FinishDecision(brain, Index);
        }

        public override void DrawGizmos(MAnimalBrain brain)
        {
            if (decisions != null)
            foreach (var d in decisions) d?.DrawGizmos(brain);
        }

        void Reset() { Description = "All Decisions on the list  must be TRUE in order to sent a True Decision"; }
    }
}
