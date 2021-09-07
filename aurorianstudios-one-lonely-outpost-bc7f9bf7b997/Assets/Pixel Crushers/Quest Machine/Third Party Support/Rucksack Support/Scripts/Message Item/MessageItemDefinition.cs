// Copyright © Pixel Crushers. All rights reserved.

using UnityEngine;
using Devdog.Rucksack.Items;

namespace PixelCrushers.QuestMachine.RucksackSupport
{

    /// <summary>
    /// This type of item definition is used for item instances that send a message to Quest Machine's 
    /// message system, passing the character name as the first value and the item instance as the 
    /// second value.
    /// </summary>
    public partial class MessageItemDefinition : UnityItemDefinition
    {
        [Tooltip("Message to send to Quest Machine message system.")]
        public string message;

        [Tooltip("Optional parameter to send. Name of character that used item will be sent as message's value.")]
        public string parameter;
    }
}