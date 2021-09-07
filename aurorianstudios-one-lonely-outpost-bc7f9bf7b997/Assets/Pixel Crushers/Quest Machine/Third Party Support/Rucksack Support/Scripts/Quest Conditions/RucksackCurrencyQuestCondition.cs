// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Currencies;

namespace PixelCrushers.QuestMachine
{

    /// <summary>
    /// Checks if a player has an amount of a currency.
    /// </summary>
    public class RucksackCurrencyQuestCondition : QuestCondition, IQuestTimer
    {

        #region Serialized Fields

        [Tooltip("Current player's character collection to check. If blank, use first collection.")]
        [SerializeField]
        private StringField m_collectionName = new StringField();

        [Tooltip("Currency to check.")]
        [SerializeField]
        private UnityCurrency m_currency;

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

        public UnityCurrency currency
        {
            get { return m_currency; }
            set { m_currency = value; }
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

        private InventoryPlayer inventoryPlayer = null;

        public override string GetEditorName()
        {
            if (currency == null) return "Rucksack Currency Quest Condition";
            return "Rucksack " + currency.name + (mode == CounterValueConditionMode.AtLeast ? " >= " : " <= ") + requiredValue.GetEditorName(quest);
        }

        public override void StartChecking(System.Action trueAction)
        {
            base.StartChecking(trueAction);
            if (currency == null)
            {
                Debug.LogWarning("Quest Machine: RucksackCurrencyQuestCondition can't run. No currency is specified.", quest);
                return;
            }
            var inventoryPlayer = PlayerManager.currentPlayer.GetComponent<InventoryPlayer>();
            if (inventoryPlayer == null)
            {
                Debug.LogWarning("Quest Machine: RucksackCurrencyQuestCondition can't run. The player doesn't have an Inventory Player component.");
                return;
            }
            if (IsConditionTrue())
            {
                SetTrue();
            }
            else
            {
                QuestTimerManager.RegisterTimer(this);
            }
        }

        public override void StopChecking()
        {
            base.StopChecking();
            QuestTimerManager.UnregisterTimer(this);
        }

        public void Tick()
        {
            if (IsConditionTrue())
            {
                SetTrue();
            }
        }

        private bool IsConditionTrue()
        {
            if (inventoryPlayer == null) return false;
            var currentCount = inventoryPlayer.currencyCollectionGroup.GetAmount(currency);
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
