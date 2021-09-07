// Copyright © Pixel Crushers. All rights reserved.

using Devdog.General2;
using Devdog.Rucksack;
using Devdog.Rucksack.Items;

namespace PixelCrushers.QuestMachine.RucksackSupport
{

    /// <summary>
    /// This type of item instance sends a message to Quest Machine's message system,
    /// passing the character name as the first value and the item instance as the second
    /// value.
    /// </summary>
    public partial class MessageItemInstance : UnityItemInstance
    {
        public override Result<ItemUsedResult> DoUse(Character character, ItemContext useContext)
        {
            TrySendMessage(character, useContext);
            return base.DoUse(character, useContext);
        }

        public void TrySendMessage(Character character, ItemContext useContext)
        {
            if (itemDefinition is MessageItemDefinition)
            {
                var def = itemDefinition as MessageItemDefinition;
                var charName = (character != null) ? character.name : null;
                MessageSystem.SendMessage(this, def.message, def.parameter, charName, this);
            }
        }
    }
}