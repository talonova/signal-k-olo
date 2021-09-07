// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// UI content that points to a Dialogue System conversation.
    /// </summary>
    public class DialogueSystemConversationQuestContent : QuestContent
    {

        [Tooltip("Run this Dialogue System conversation.")]
        [ConversationPopup(true)]
        [SerializeField]
        private string m_conversation;

        private StringField m_conversationStringField = null;
        private StringField conversationStringField
        {
            get
            {
                if (m_conversationStringField == null)
                {
                    m_conversationStringField = new StringField(m_conversation);
                }
                return m_conversationStringField;
            }
            set
            {
                m_conversationStringField = null;
                m_conversation = StringField.GetStringValue(value);
            }
        }

        /// <summary>
        /// Text to show in regular body text style.
        /// </summary>
        public string conversation
        {
            get { return m_conversation; }
            set { m_conversation = value; }
        }

        public override StringField originalText
        {
            get { return conversationStringField; }
            set { conversationStringField = value; }
        }

        public override string GetEditorName()
        {
            return (conversation == null) ? "Conversation" : "Conversation: " + conversation;
        }

    }

}
