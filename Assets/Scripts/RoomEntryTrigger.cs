using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEngine
{
    public class RoomEntryTrigger : MonoBehaviour
    {

        private bool hasBeenTriggered = false;

        public event Action onTriggerZonePlayer;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            Character player = collision.GetComponent<Character>();
            if (hasBeenTriggered || player == null || player.CharacterType != Character.CharacterTypes.Player)
            {
                return;
            }

            hasBeenTriggered = true;
            onTriggerZonePlayer.Invoke();
        }
    }
}