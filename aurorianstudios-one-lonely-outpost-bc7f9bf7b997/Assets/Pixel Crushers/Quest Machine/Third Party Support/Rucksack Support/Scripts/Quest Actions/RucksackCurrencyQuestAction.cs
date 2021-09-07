// Copyright © Pixel Crushers. All rights reserved.

using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Currencies;
using UnityEngine;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Adds or removes currency.
    /// </summary>
    public class RucksackCurrencyQuestAction : QuestAction
    {

        #region Serialized Fields

        [Tooltip("Currency to add or remove.")]
        [SerializeField]
        private UnityCurrency m_currency;

        [Tooltip("Add or remove currency?")]
        [SerializeField]
        private ActionEffect.Operation m_operation = ActionEffect.Operation.Add;

        [Tooltip("Amount to add/remove.")]
        [SerializeField]
        private QuestNumber m_amount = new QuestNumber(1);

        #endregion

        #region Public Properties

        public UnityCurrency currency
        {
            get { return m_currency; }
            set { m_currency = value; }
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
            if (currency == null) return "Rucksack Currency Quest Action";
            return "Rucksack " + operation + " " + amount.GetValue(quest) + " " + currency.name;
        }

        public override void Execute()
        {
            base.Execute();
            if (currency == null)
            {
                Debug.LogWarning("Quest Machine: RucksackCurrencyQuestAction can't run. The Currency is unassigned.");
                return;
            }
            var operationValue = amount.GetValue(quest);
            if (QuestMachine.debug) Debug.Log("Quest Machine: RucksackCurrencyQuestAction: " + currency.name + " " + operation + " " + operationValue + ".", quest);
            var inventoryPlayer = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
            if (inventoryPlayer == null)
            {
                Debug.LogWarning("Quest Machine: RucksackCurrencyQuestAction can't run. The player doesn't have an Inventory Player component.");
                return;
            }
            var actualAmount = amount.GetValue(quest);
            switch (m_operation)
            {
                case ActionEffect.Operation.Add:
                    inventoryPlayer.currencyCollectionGroup.Add(currency, actualAmount);
                    break;
                case ActionEffect.Operation.Remove:
                    inventoryPlayer.currencyCollectionGroup.Remove(currency, actualAmount);
                    break;
            }
        }
    }

}
