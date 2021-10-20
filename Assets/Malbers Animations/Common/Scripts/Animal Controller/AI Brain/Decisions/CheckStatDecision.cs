using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Decision/Check Stat", order = 3)]
    public class CheckStatDecision : MAIDecision
    {
        public enum checkStatOption { Compare, CompareNormalized, isInmune, Regenerating, Degenerating, Empty, Full, Active, ValueChanged, ValueReduced,ValueIncreased}

        [Tooltip("Check the Decision on the Animal(Self) or the Target(Target)")]
        public Affected checkOn = Affected.Self;
        /// <summary>Range for Looking forward and Finding something</summary>
        
        [Space(25),Tooltip("Stat you want to find")]
        public StatID Stat;
        [Tooltip("What do you want to do with the Stat?")]
        public checkStatOption Option = checkStatOption.Compare;
        [Tooltip("(Option Compare Only) Type of the comparation"), Hide("hideVars", true,true)]
        public ComparerInt StatIs = ComparerInt.Less;
        [Tooltip("(Option Compare Only) Value to Compare the Stat"),Hide("hideVars",true,true)]
        public float Value;
        [Space,Tooltip("Uses TryGet Value in case you don't know if your target or your animal has the Stat you are looking for. Disabling this Improves performance")]
        public bool TryGetValue = true;

        public override void PrepareDecision(MAnimalBrain brain, int Index)
        {
            //Store the Value the Stat has starting this Decision

            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue)
                    {
                        if (brain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                            brain.DecisionsVars[Index].floatValue = statS.Value;
                    }
                    else
                    {
                        brain.DecisionsVars[Index].floatValue = brain.AnimalStats[Stat.ID].Value;
                    }
                    break;
 
                case Affected.Target:

                    if (brain.TargetHasStats)
                    {
                        if (TryGetValue)
                        {
                            if (brain.TargetStats.TryGetValue(Stat.ID, out Stat statS))
                                brain.DecisionsVars[Index].floatValue = statS.Value;
                        }
                        else
                        {
                            brain.DecisionsVars[Index].floatValue = brain.TargetStats[Stat.ID].Value;
                        }
                    }
                    break;
                default:
                    break;
            }
        }


        public override bool Decide(MAnimalBrain brain, int index)
        {
            bool result = false;

            switch (checkOn)
            {
                case Affected.Self:
                    if (TryGetValue)
                    {
                        if (brain.AnimalStats.TryGetValue(Stat.ID, out Stat statS))
                            result = CheckStat(statS, brain, index);
                    }
                    else
                    {
                        var SelfStatValue = brain.AnimalStats[Stat.ID];
                        result = CheckStat(SelfStatValue, brain, index);
                    }
                    break;
                case Affected.Target:

                    if (brain.TargetHasStats)
                    {
                        if (TryGetValue)
                        {
                            if (brain.TargetStats.TryGetValue(Stat.ID, out Stat statT))
                                result = CheckStat(statT, brain, index);
                        }
                        else
                        {
                            var TargetStatValue = brain.TargetStats[Stat.ID];
                            result = CheckStat(TargetStatValue, brain, index);
                        }
                    }
                    break;
            }
            return result;
        }
         

        private bool CheckStat(Stat stat, MAnimalBrain brain, int Index)
        {
            switch (Option)
            {
                case checkStatOption.Compare:
                    return CompareWithValue(stat.Value);
                case checkStatOption.CompareNormalized:
                    return CompareWithValue(stat.NormalizedValue);
                case checkStatOption.isInmune:
                    return stat.IsInmune;
                case checkStatOption.Regenerating:
                    return stat.IsRegenerating;
                case checkStatOption.Degenerating:
                    return stat.IsDegenerating;
                case checkStatOption.Empty:
                    return stat.Value == stat.MinValue;
                case checkStatOption.Full:
                    return stat.Value == stat.MaxValue;
                case checkStatOption.Active:
                    return stat.Active;
                case checkStatOption.ValueChanged:
                    return  stat.value  != brain.DecisionsVars[Index].floatValue; ;
                case checkStatOption.ValueReduced:
                    return stat.value < brain.DecisionsVars[Index].floatValue; ;
                case checkStatOption.ValueIncreased:
                    return stat.value > brain.DecisionsVars[Index].floatValue; ;
                default:
                    return false;
            }
        }
        private bool CompareWithValue(float stat)
        {
            switch (StatIs)
            {
                case ComparerInt.Equal:
                    return stat == Value;
                case ComparerInt.Greater:
                    return stat > Value;
                case ComparerInt.Less:
                    return stat < Value;
                case ComparerInt.NotEqual:
                    return stat != Value;
                default:
                    return false;
            }
        }


        [HideInInspector] public bool hideVars = false;
        private void OnValidate()
        {
            hideVars = (Option != checkStatOption.Compare && Option != checkStatOption.CompareNormalized); 

            if (Option == checkStatOption.CompareNormalized)
            {
                Value = Mathf.Clamp(Value, 0f, 1f);
            }
        }


        private void Reset() { Description = "Checks for a Stat value, Compares or search for a Stat Property and returns the succeded value"; }
    }
}
