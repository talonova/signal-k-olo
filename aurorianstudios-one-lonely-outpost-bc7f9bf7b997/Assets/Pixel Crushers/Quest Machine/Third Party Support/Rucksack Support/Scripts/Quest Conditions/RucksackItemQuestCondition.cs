// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using Devdog.Rucksack.Items;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Checks if a player has an amount of an item.
    /// </summary>
    public class RucksackItemQuestCondition : QuestCondition
    {

        #region Serialized Fields

        [Tooltip("Current player's character collection to check. If blank, use first collection.")]
        [SerializeField]
        private StringField m_collectionName = new StringField();

        [Tooltip("Item to check.")]
        [SerializeField]
        private UnityItemDefinition m_itemDefinition;

        [Tooltip("How the required value applies to the condition.")]
        [SerializeField]
        private CounterValueConditionMode m_mode = CounterValueConditionMode.AtLeast;

        [Tooltip("The required amount.")]
        [SerializeField]
        private QuestNumber m_requiredValue = new QuestNumber();

        #endregion

        #region Public Properties

        public StringField collectionName
        {
            get { return m_collectionName; }
            set { m_collectionName = value; }
        }

        public UnityItemDefinition itemDefinition
        {
            get { return m_itemDefinition; }
            set { m_itemDefinition = value; }
        }

        public CounterValueConditionMode mode
        {
            get { return m_mode; }
            set { m_mode = value; }
        }

        public QuestNumber requiredValue
        {
            get { return m_requiredValue; }
            set { m_requiredValue = value; }
        }

        #endregion

        private Devdog.Rucksack.Collections.ICollection<IItemInstance> collection = null;

        public override string GetEditorName()
        {
            if (itemDefinition == null) return "Rucksack Item Quest Condition";
            return "Rucksack " + itemDefinition.name + (mode == CounterValueConditionMode.AtLeast ? " >= " : " <= ") + requiredValue.GetEditorName(quest);
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            if (itemDefinition == null)
            {
                Debug.LogWarning("Quest Machine: RucksackItemQuestCondition can't run. No item definition is specified.", quest);
                return;
            }
            var inventoryPlayer = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
            if (inventoryPlayer == null)
            {
                Debug.LogWarning("Quest Machine: RucksackItemQuestCondition can't run. The player doesn't have an Inventory Player component.");
                return;
            }
            var actualCollectionName = StringField.IsNullOrEmpty(collectionName) ? ((inventoryPlayer.itemCollections.Length > 0) ? inventoryPlayer.itemCollections[0] : null) : collectionName.value;
            collection = CollectionRegistry.byName.Get(actualCollectionName) as Devdog.Rucksack.Collections.ICollection<IItemInstance>;
            if (collection == null)
            {
                Debug.LogWarning("Quest Machine: RucksackItemQuestCondition can't run. Can't find a collection named '" + actualCollectionName + ".");
                return;
            }
            else if (IsConditionTrue())
            {
                SetTrue();
            }
            else
            {
                collection.OnAddedItem += OnAddedItem;
                collection.OnRemovedItem += OnRemovedItem;
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            if (collection == null) return;
            collection.OnAddedItem -= OnAddedItem;
            collection.OnRemovedItem -= OnRemovedItem;
        }

        private void OnAddedItem(object sender, CollectionAddResult args)
        {
            Check();
        }

        private void OnRemovedItem(object sender, CollectionRemoveResult<IItemInstance> args)
        {
            Check();
        }

        private void Check()
        {
            if (IsConditionTrue())
            {
                SetTrue();
            }
        }

        private bool IsConditionTrue()
        {
            if (collection == null) return false;
            var index = collection.IndexOf(o => o != null && o.itemDefinition != null && o.itemDefinition is UnityItemDefinition && (UnityItemDefinition)o.itemDefinition == itemDefinition);
            if (index == -1) return false;
            var currentCount = collection.GetAmount(index);
            switch (mode)
            {
                case CounterValueConditionMode.AtLeast:
                    if (currentCount >= requiredValue.GetValue(quest)) return true;
                    break;
                case CounterValueConditionMode.AtMost:
                    if (currentCount <= requiredValue.GetValue(quest)) return true;
                    break;
            }
            return false;
        }

        public override void SetTrue()
        {
            if (QuestMachine.debug) Debug.Log("Quest Machine: " + GetEditorName() + " is true.", quest);
            base.SetTrue();
        }
    }
}
