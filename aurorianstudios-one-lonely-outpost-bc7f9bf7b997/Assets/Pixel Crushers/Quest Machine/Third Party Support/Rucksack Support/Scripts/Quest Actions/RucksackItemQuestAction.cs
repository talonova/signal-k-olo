// Copyright © Pixel Crushers. All rights reserved.

using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Items;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Adds or removes items.
    /// </summary>
    public class RucksackItemQuestAction : QuestAction
    {

        #region Serialized Fields

        [Tooltip("Item to add or remove.")]
        [SerializeField]
        private UnityItemDefinition m_itemDefinition;

        [Tooltip("Add or remove items?")]
        [SerializeField]
        private ActionEffect.Operation m_operation = ActionEffect.Operation.Add;

        [Tooltip("Amount to add/remove.")]
        [SerializeField]
        private QuestNumber m_amount = new QuestNumber(1);

        #endregion

        #region Public Properties

        public UnityItemDefinition itemDefinition
        {
            get { return m_itemDefinition; }
            set { m_itemDefinition = value; }
        }

        public ActionEffect.Operation operation
        {
            get { return m_operation; }
            set { m_operation = value; }
        }

        public QuestNumber amount
        {
            get { return m_amount; }
            set { m_amount = value; }
        }

        #endregion

        public override string GetEditorName()
        {
            if (itemDefinition == null) return "Rucksack Item Quest Action";
            return "Rucksack " + operation + " " + amount.GetValue(quest) + " " + itemDefinition.name;
        }

        public override void Execute()
        {
            base.Execute();
            if (itemDefinition == null)
            {
                Debug.LogWarning("Quest Machine: RucksackItemQuestAction can't run. The Item Definition is unassigned.");
                return;
            }
            var operationValue = amount.GetValue(quest);
            if (QuestMachine.debug) Debug.Log("Quest Machine: RucksackItemQuestAction: " + itemDefinition.name + " " + operation + " " + operationValue + ".", quest);
            var inventoryPlayer = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
            if (inventoryPlayer == null)
            {
                Debug.LogWarning("Quest Machine: RucksackItemQuestAction can't run. The player doesn't have an Inventory Player component.");
                return;
            }
            var itemInstance = ItemFactory.CreateInstance(itemDefinition, System.Guid.NewGuid());
            var actualAmount = amount.GetValue(quest);
            switch (m_operation)
            {
                case ActionEffect.Operation.Add:
                    inventoryPlayer.itemCollectionGroup.Add(itemInstance, actualAmount);
                    break;
                case ActionEffect.Operation.Remove:
                    inventoryPlayer.itemCollectionGroup.Remove(itemInstance, actualAmount);
                    break;
            }
        }
    }

}
