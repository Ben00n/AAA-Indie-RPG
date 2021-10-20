// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Sets a quest state.
    /// </summary>
    public class SetQuestStateQuestAction : QuestAction
    {

        [Tooltip("ID of quest. Leave blank to set this quest's state.")]
        [SerializeField]
        private StringField m_questID;

        [Tooltip("New quest state.")]
        [SerializeField]
        private QuestState m_state;

        public StringField questID
        {
            get { return (StringField.IsNullOrEmpty(m_questID) && quest != null) ? quest.id : m_questID; }
            set { m_questID = value; }
        }

        public QuestState state
        {
            get { return m_state; }
            set { m_state = value; }
        }

        public override string GetEditorName()
        {
            return StringField.IsNullOrEmpty(questID) ? ("Set Quest State: " + state) : ("Set Quest State: Quest '" + questID + "' to " + state);
        }

        public override void Execute()
        {
            if (QuestMachine.GetQuestState(questID) != state)
            {
                QuestMachine.SetQuestState(questID, state);
            }
        }

    }

}
