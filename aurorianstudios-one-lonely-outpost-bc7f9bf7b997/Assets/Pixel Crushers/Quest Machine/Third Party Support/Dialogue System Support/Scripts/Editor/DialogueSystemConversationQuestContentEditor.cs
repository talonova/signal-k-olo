// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using UnityEditor;

namespace PixelCrushers.QuestMachine.DialogueSystemSupport
{

    /// <summary>
    /// Custom editor for DialogueSystemConversationQuestContent that
    /// provides a button to open the Dialogue Editor window on the
    /// selected conversation, or create a new conversation if no
    /// conversation is assigned to the content yet.
    /// </summary>
    [CustomEditor(typeof(DialogueSystemConversationQuestContent), true)]
    public class DialogueSystemConversationQuestContentEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawDialogueEditorButton();
        }

        private void DrawDialogueEditorButton()
        {
            var content = target as DialogueSystemConversationQuestContent;
            if (content == null)
            {
                Debug.LogWarning("Quest Machine: Internal error: Target is null in DialogueSystemConversationQuestContent editor.");
                return;
            }
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var database = GetDialogueDatabase();
            EditorGUI.BeginDisabledGroup(database == null);
            if (string.IsNullOrEmpty(content.conversation))
            {
                if (QuestEditorWindow.selectedQuest != null)
                {
                    var title = GetDefaultConversationTitle();
                    if (GUILayout.Button(new GUIContent("Create Conversation", "Create a new conversation named '" + title + "'. Scene must have a Dialogue Manager with a database."), GUILayout.Width(140)))
                    {
                        CreateConversation(content, title, database);
                    }
                }
            }
            else
            {
                if (GUILayout.Button(new GUIContent("Dialogue Editor", "Open '" + content.conversation + "' in Dialogue Editor. Scene must have a Dialogue Manager with database containing this conversation."), GUILayout.Width(100)))
                {
                    OpenDialogueEditor(content, database);
                }
            }
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndHorizontal();
        }

        private void CreateConversation(DialogueSystemConversationQuestContent content, string title, DialogueSystem.DialogueDatabase database)
        {
            if (content == null || database == null || string.IsNullOrEmpty(title)) return;
            Undo.RecordObject(content, "Create Conversation");
            content.conversation = title;
            var template = DialogueSystem.TemplateTools.LoadFromEditorPrefs();
            var conversation = template.CreateConversation(GetNextConversationID(database), title);
            database.conversations.Add(conversation);
            OpenDialogueEditor(content, database);
        }

        private void OpenDialogueEditor(DialogueSystemConversationQuestContent content, DialogueSystem.DialogueDatabase database)
        {
            if (database == null || content == null) return;
            var conversation = database.GetConversation(content.conversation);
            if (conversation == null)
            {
                Debug.LogWarning("Quest Machine: Conversation '" + content.conversation + "' not found in database.");
                DialogueSystem.DialogueEditor.DialogueEditorWindow.OpenDialogueEditorWindow();
                return;
            }
            var startEntry = conversation.GetFirstDialogueEntry();
            var entryID = (startEntry != null) ? startEntry.id : 0;
            DialogueSystem.DialogueEditor.DialogueEditorWindow.OpenDialogueEntry(database, conversation.id, entryID);
        }

        private DialogueSystem.DialogueDatabase GetDialogueDatabase()
        {
            var database = DialogueSystem.EditorTools.selectedDatabase;
            if (database == null) database = DialogueSystem.EditorTools.FindInitialDatabase();
            return database;
        }

        private string GetDefaultConversationTitle()
        {
            var quest = QuestEditorWindow.selectedQuest;
            if (quest == null) return string.Empty;
            var title = StringField.GetStringValue(quest.title);
            if (string.IsNullOrEmpty(title)) title = StringField.GetStringValue(quest.id);
            if (string.IsNullOrEmpty(title)) title = quest.name;
            return title;
        }

        private int GetNextConversationID(DialogueSystem.DialogueDatabase database)
        {
            if (database == null) return -1;
            int id = 0;
            for (int i = 0; i < database.conversations.Count; i++)
            {
                id = Mathf.Max(id, database.conversations[i].id);
            }
            return id + 1;
        }

    }
}
