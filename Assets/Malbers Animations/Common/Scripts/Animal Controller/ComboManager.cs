using MalbersAnimations.Scriptables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using MalbersAnimations.Events;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/Animal Controller/Combo Manager")]
    public class ComboManager : MonoBehaviour
    {
        public  MAnimal animal { get; private set; }

        public int Branch = 0;
        public List<Combo> combos;

        public int CurrentComboIndex { get; private set; }
        public int CurrentComboSequenceIndex { get; internal set; }
        public Combo CurrentCombo { get; internal set; }

        /// <summary> Is the manager playing a combo? </summary>
        public bool PlayingCombo { get; internal set; }
        public ComboSequence CurrentComboSequence => CurrentCombo.CurrentSequence;

        public bool debug;

        // public IntEvent ComboEnded = new IntEvent();

        private void OnEnable()
        {
            animal = animal ?? GetComponent<MAnimal>();
            animal.OnModeEnd.AddListener(OnModeEnd);
            Restart();
        }
        private void OnDisable()  { animal.OnModeEnd.RemoveListener(OnModeEnd); }

        private void OnModeEnd(int modeID, int Ability)
        {
            if (PlayingCombo)
            {
                if (CurrentComboSequence.Finisher)
                {
                    CurrentCombo.OnComboFinished.Invoke(CurrentComboSequenceIndex);
                    if (debug) Debug.Log($"<b>[{name}]</b> - Combo Finished <b>[{CurrentComboSequenceIndex}]</b> Branch - <b>[{Branch}]</b>- Restarting");
                    Restart();
                }
                else
                {
                    StartCoroutine(ComboInterrupted());
                }
            }
        }

        protected IEnumerator ComboInterrupted()
        {
            yield return null;
            yield return null; //Wait 2 frames

            if (!animal.IsPlayingMode) // if is no longer playing a Mode then means it was interruptedd
            {
                if (debug) Debug.Log($"<b>[{name}]</b> - Incomplete <b>[{CurrentComboSequenceIndex}]</b> Branch - <b>[{Branch}]</b>- Restarting");
                CurrentCombo.OnComboInterrupted.Invoke(CurrentComboSequenceIndex);
                Restart();//meaning it got to the end of the combo
            }
            yield return null;
        }





        public virtual void Activate(int index)
        {
            if (animal.Sleep) return;
            if (!enabled) return;

            if (!animal.IsPlayingMode && !animal.IsPreparingMode)
                Restart();   //Means is not Playing any mode so Restart

            if (CurrentComboIndex == -1)   //First Entry
            {
                CurrentComboIndex = index;
                CurrentCombo = combos[CurrentComboIndex];
            }

            if (CurrentCombo != null && CurrentCombo.Active)
            {
                CurrentCombo.Play(this); 
            }
        } 

        public virtual void Activate_XXYY(int index)
        {
            var branch = index / 100;
            var combo = index % 100;

            SetBranch(branch);
            Activate(combo);
        }
        public virtual void Restart()
        {
            CurrentCombo = null;
            CurrentComboIndex = -1;
            CurrentComboSequenceIndex = 0;
            PlayingCombo = false;

            foreach (var m in combos)
            { 
                m.CurrentSequence = null;  //Clean the current combo secuence
                m.ActiveSecuenceIndex = -1;  //Clean the current combo secuence

                foreach (var seq in m.Sequence) 
                    seq.Used = false; //Set that the secuenced is used to 
            }
        }
        public virtual void SetBranch(int value) => Branch = value;
        public virtual void Combo_Disable(int index) => Combo_Enable(index, false);
        public virtual void Combo_Enable(int index) => Combo_Enable(index, true);
        public virtual void Combo_Enable(int index, bool value) => combos[index].Active = value;


        [HideInInspector] public int selectedCombo = -1;
    }

    [System.Serializable]
    public class Combo
    {
        public ModeID Mode;
        public string Name = "Combo1";
        public BoolReference m_Active = new BoolReference(true);

        public bool Active { get => m_Active.Value; set => m_Active.Value = value; }

        public List<ComboSequence> Sequence = new List<ComboSequence>();
        public ComboSequence CurrentSequence { get; internal set; }

        /// <summary> Current Index on the list to search combos. This is used to avoid searching already used Sequences on the list</summary>
        public int ActiveSecuenceIndex{ get; internal set; }

        public IntEvent OnComboFinished = new IntEvent();
        public IntEvent OnComboInterrupted = new IntEvent();

        public void Play(ComboManager M)
        {
            var animal = M.animal;

            if (!animal.IsPlayingMode) //Means is starting the combo
            {
                // var Starter = Sequence.Find(s => !s.Used && s.Branch == M.Branch && s.PreviewsAbility == 0);

                for (int i = 0; i < Sequence.Count; i++)
                {
                    var Starter = Sequence[i];

                    if (!Starter.Used && Starter.Branch == M.Branch && Starter.PreviewsAbility == 0) //Only Start with Started Abilities
                    {
                        if (animal.Mode_TryActivate(Mode, Starter.Ability))
                        {
                            M.PlayingCombo = true;
                            PlaySequence(M, Starter);
                            ActiveSecuenceIndex = i; //Finding which is the active secuence index;
                            break;
                        }
                    }
                }
                 
            }
            else
            {
                if (animal.IsPlayingMode)               //Means is Playing a mode so check for the Next sequence on the Combo
                {
                    var aMode = animal.ActiveMode;      //Get the Animal Active Mode 

                    for (int i = ActiveSecuenceIndex + 1; i < Sequence.Count; i++) //Search from the next one
                    {
                        var s = Sequence[i];

                        if (!s.Used && s.Branch == M.Branch && s.PreviewsAbility == aMode.AbilityIndex && s.Activation.IsInRange(animal.ModeTime))
                        {
                            if (animal.Mode_ForceActivate(Mode, s.Ability)) //Play the nex animation on the sequence
                            {
                                PlaySequence(M, s);
                                ActiveSecuenceIndex = i; //Finding which is the active secuence index;
                                break;
                            }
                        }
                    }



                   //var NextSequence = Sequence.Find(s => !s.Used && s.Branch == M.Branch && s.PreviewsAbility == aMode.AbilityIndex && s.Activation.IsInRange(animal.ModeTime));

                   // if (NextSequence != null && animal.Mode_ForceActivate(Mode, NextSequence.Ability)) //Play the nex animation on the sequence
                   // {
                   //     PlaySequence(M, NextSequence);
                   // }
                }
            }
        }

        private void PlaySequence(ComboManager M, ComboSequence sequence)
        {
            M.CurrentComboSequenceIndex = Mode.ID * 1000 + sequence.Ability;
            CurrentSequence = sequence; //Store the current sequence
            CurrentSequence.Used = true;
            CurrentSequence.OnSequencePlay.Invoke(M.CurrentComboSequenceIndex);

            if (M.debug)
                Debug.Log($"<b>[{M.name}]</b> - Sequence: <b>[{M.CurrentComboSequenceIndex}]</b> - Branch: <b>[{M.Branch}]</b> - Time: {Time.time:F3}");
        }
    }


    [System.Serializable]
    public class ComboSequence
    {
        [MinMaxRange(0, 1)]
        public RangedFloat Activation = new RangedFloat(0.3f, 0.6f);
        public int PreviewsAbility = 0;
        public int Ability = 0;
        public int Branch = 0;
        public bool Used;
        public bool Finisher;
        public IntEvent OnSequencePlay = new IntEvent(); 
    }



#if UNITY_EDITOR
    [CustomEditor(typeof(ComboManager))]

    public class ComboEditor : Editor
    {
        public static GUIStyle StyleGray => MTools.Style(new Color(0.5f, 0.5f, 0.5f, 0.3f));
        public static GUIStyle StyleBlue => MTools.Style(new Color(0, 0.5f, 1f, 0.3f));

        private int branch, prev, current;

        SerializedProperty Branch, combos, selectedCombo, debug;
        private Dictionary<string, ReorderableList> SequenceReordable = new Dictionary<string, ReorderableList>();
        private ReorderableList CombosReor;

        private ComboManager M;
        private int abiliIndex;

        private void OnEnable()
        {
            M= (ComboManager )target;

            combos = serializedObject.FindProperty("combos");
            Branch = serializedObject.FindProperty("Branch");
            selectedCombo = serializedObject.FindProperty("selectedCombo");
            debug = serializedObject.FindProperty("debug");

            CombosReor = new ReorderableList(serializedObject, combos, true, true, true, true)
            {
                drawElementCallback = Draw_Element_Combo,
                drawHeaderCallback = Draw_Header_Combo,
                onSelectCallback = Selected_ComboCB,
                onRemoveCallback = OnRemoveCallback_Mode
            };
        }

        private void Selected_ComboCB(ReorderableList list)
        {
            selectedCombo.intValue = list.index;
        }

        private void OnRemoveCallback_Mode(ReorderableList list)
        {
            // The reference value must be null in order for the element to be removed from the SerializedProperty array.
            combos.DeleteArrayElementAtIndex(list.index);
            list.index -= 1;

            if (list.index == -1 && combos.arraySize > 0) list.index = 0;   //In Case you remove the first one

            selectedCombo.intValue--;

            list.index = Mathf.Clamp(list.index, 0, list.index - 1);

            EditorUtility.SetDirty(target);
        }

        private void Draw_Header_Combo(Rect rect)
        {
            float half = rect.width / 2;
            var IDIndex = new Rect(rect.x, rect.y, 45, EditorGUIUtility.singleLineHeight);
            var IDName = new Rect(rect.x + 45, rect.y, half - 15 - 45, EditorGUIUtility.singleLineHeight);
            var IDRect = new Rect(rect.x + half + 10, rect.y, half - 10, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(IDIndex, "Index");
            EditorGUI.LabelField(IDName, " Name");
            EditorGUI.LabelField(IDRect, "  Mode");
        }

        private void Draw_Element_Combo(Rect rect, int index, bool isActive, bool isFocused)
        {
            var element = combos.GetArrayElementAtIndex(index);
            var Mode = element.FindPropertyRelative("Mode");
            var Name = element.FindPropertyRelative("Name");
            rect.y += 2;

            float half = rect.width / 2;

            var IDIndex = new Rect(rect.x, rect.y, 25, EditorGUIUtility.singleLineHeight);
            var IDName = new Rect(rect.x + 25, rect.y, half - 15 - 25, EditorGUIUtility.singleLineHeight);
            var IDRect = new Rect(rect.x + half + 10, rect.y, half - 10, EditorGUIUtility.singleLineHeight);

            EditorGUI.LabelField(IDIndex, "(" + index.ToString() + ")");
            EditorGUI.PropertyField(IDName, Name, GUIContent.none);
            EditorGUI.PropertyField(IDRect, Mode, GUIContent.none);
        }

        private void DrawSequence(int ModeIndex, SerializedProperty combo, SerializedProperty sequence)
        {
            ReorderableList Reo_AbilityList;
            string listKey = combo.propertyPath;

            if (SequenceReordable.ContainsKey(listKey))
            {
                Reo_AbilityList = SequenceReordable[listKey]; // fetch the reorderable list in dict
            }
            else
            {
                Reo_AbilityList = new ReorderableList(combo.serializedObject, sequence, true, true, true, true)
                {
                    drawElementCallback = (rect, index, isActive, isFocused) =>
                    {
                        rect.y += 2;

                        var Height = EditorGUIUtility.singleLineHeight;
                        var element = sequence.GetArrayElementAtIndex(index);

                        //var Activation = element.FindPropertyRelative("Activation");
                        var PreviewsAbility = element.FindPropertyRelative("PreviewsAbility");
                        var Ability = element.FindPropertyRelative("Ability");
                        var Branch = element.FindPropertyRelative("Branch");
                        var useD = element.FindPropertyRelative("Used");
                        var finisher = element.FindPropertyRelative("Finisher");

                        var IDRect = new Rect(rect) { height = Height };

                        float wid = rect.width / 3;

                        var IRWidth = 30f;
                        var Sep = -10f;
                        var Offset = 40f;

                        float xx = IRWidth + Offset;

                        var IndexRect = new Rect(IDRect) { width = IRWidth };
                        var BranchRect = new Rect(IDRect) { x = xx, width = wid - 15};
                        var PrevARect = new Rect(IDRect) { x = wid + xx+ Sep, width = wid - 15 - Sep };
                        var AbilityRect = new Rect(IDRect) { x = wid * 2 + xx+ Sep, width = wid - 15 - 20 };
                        var FinisherRect = new Rect(IDRect) { x = IDRect.width +30, width =  20 };

                        var style = new GUIStyle(EditorStyles.label);

                        if (!useD.boolValue && Application.isPlaying)style.normal.textColor = Color.green; //If the Combo is not used turn the combos to Green
                      

                        EditorGUI.LabelField(IndexRect, "(" + index.ToString() + ")", style);
                        var oldCColor = GUI.contentColor;
                        var oldColor = GUI.color;

                        if (PreviewsAbility.intValue <= 0)
                        {
                            GUI.contentColor = Color.green;
                        }

                        if (Application.isPlaying)
                        {
                            if (M.CurrentCombo != null)
                            {
                                var Index = M.CurrentCombo.ActiveSecuenceIndex;

                                if (Index == index) //Paint Active Index
                                {
                                    GUI.contentColor =  
                                    GUI.color = Color.yellow;

                                    if (M.CurrentComboSequence.Finisher) //Paint finisher
                                    {
                                        GUI.contentColor =
                                        GUI.color = (Color.red + Color.yellow) / 2;
                                    }
                                }
                                else if (Index > index)  //Paint Used Index
                                {
                                    GUI.contentColor =  
                                    GUI.color = Color.gray;
                                }
                               
                            }
                        }

                        EditorGUI.PropertyField(BranchRect, Branch, GUIContent.none);
                        EditorGUI.PropertyField(PrevARect, PreviewsAbility, GUIContent.none);
                        EditorGUI.PropertyField(AbilityRect, Ability, GUIContent.none);
                        EditorGUI.PropertyField(FinisherRect, finisher, GUIContent.none);
                        GUI.contentColor = oldCColor;
                        GUI.color = oldColor;

                        if (index == abiliIndex)
                        {
                            branch = Branch.intValue;
                            prev = PreviewsAbility.intValue;
                            current = Ability.intValue;
                        } 
                    },

                    drawHeaderCallback = rect =>
                    {
                        var Height = EditorGUIUtility.singleLineHeight;
                        var IDRect = new Rect(rect) { height = Height };

                        float wid = rect.width / 3;
                        var IRWidth = 30f;
                        var Sep = -10f;
                        var Offset = 40f;

                        float xx = IRWidth + Offset;

                        var IndexRect = new Rect(IDRect) { width = IRWidth +5};
                        var BranchRect = new Rect(IDRect) { x = xx, width = wid - 15 };
                        var PrevARect = new Rect(IDRect) { x = wid + xx + Sep, width = wid - 15 };
                        var AbilityRect = new Rect(IDRect) { x = wid * 2 + xx + Sep - 10, width = wid - 80};
                        var FinisherRect = new Rect(IDRect) { x = IDRect.width-15, width = 45 };

                        EditorGUI.LabelField(IndexRect, "Index");
                        EditorGUI.LabelField(BranchRect, " Branch");
                        EditorGUI.LabelField(PrevARect, new GUIContent("Activation Ability" ,"Current Mode Ability [Index] Playing on the Animal needed to activate a sequence"));
                        EditorGUI.LabelField(AbilityRect, new GUIContent("Next Ability", "Next Mode Ability [Index] to Play on the Animal if the Active Mode Animation is withing the Activation Range limit "));
                        EditorGUI.LabelField(FinisherRect, new GUIContent("Finisher", "Combo Finisher"));
                    },

                    //elementHeightCallback = (index) =>
                    //{
                    //    Repaint();

                    //    if (index == abiliIndex)

                    //        return EditorGUIUtility.singleLineHeight * 3;
                    //    else
                    //        return EditorGUIUtility.singleLineHeight + 5;
                    //}
                };

                SequenceReordable.Add(listKey, Reo_AbilityList);  //Store it on the Editor
            }

            Reo_AbilityList.DoLayoutList();

            abiliIndex = Reo_AbilityList.index;

            if (abiliIndex != -1)
            {
                var element = sequence.GetArrayElementAtIndex(abiliIndex);

                var Activation = element.FindPropertyRelative("Activation");
                var OnSequencePlay = element.FindPropertyRelative("OnSequencePlay");

                var lbl = "B[" + branch + "] AA[" + prev + "] NA[" + current + "]";

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {

                    EditorGUILayout.LabelField("Sequence Properties - " + lbl);
                    EditorGUILayout.PropertyField(Activation, new GUIContent("Activation", "Range of the Preview Animation the Sequence can be activate"));
                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.PropertyField(OnSequencePlay, new GUIContent("Sequence Play - " + lbl));
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            MalbersEditor.DrawDescription("Use a Mode on the Animal Controller to create combo sequences  \nActivate the combos using Activate(int) or Activate_XXYY(int) XX(Branch) YY(Combo Index)");

            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                EditorGUILayout.PropertyField(Branch, new GUIContent("Branch", "Current Branch ID for the Combo Sequence, if this value change then the combo will play different sequences"));
                debug.boolValue = GUILayout.Toggle(debug.boolValue,new GUIContent("D","Debug"), EditorStyles.miniButton, GUILayout.Width(23));
            }
            EditorGUILayout.EndHorizontal();


            CombosReor.DoLayoutList();

            CombosReor.index = selectedCombo.intValue;
            int IndexCombo = CombosReor.index;

            if (IndexCombo != -1)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    var combo = combos.GetArrayElementAtIndex(IndexCombo);

                    if (combo != null)
                    {
                        var name = combo.FindPropertyRelative("Name");
                        EditorGUILayout.LabelField(name.stringValue, EditorStyles.boldLabel);
                        var active = combo.FindPropertyRelative("m_Active");
                        var OnComboFinished = combo.FindPropertyRelative("OnComboFinished");
                        var OnComboInterrupted = combo.FindPropertyRelative("OnComboInterrupted");
                        EditorGUILayout.PropertyField(active, new GUIContent("Active", "is the Combo Active?"));
                        EditorGUILayout.HelpBox("Green Sequences are starters combos",  MessageType.None);
                        EditorGUILayout.LabelField("Combo Sequence List", EditorStyles.boldLabel);
                        var sequence = combo.FindPropertyRelative("Sequence");
                        DrawSequence(IndexCombo, combo, sequence);
                        EditorGUILayout.PropertyField(OnComboFinished);
                        EditorGUILayout.PropertyField(OnComboInterrupted);
                    }
                }
                EditorGUILayout.EndVertical();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}