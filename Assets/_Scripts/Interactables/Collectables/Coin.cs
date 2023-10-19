using _Scripts.Managers;
using _Scripts.ScriptablesObjects;
using UnityEngine;

namespace _Scripts.Interactables.Collectables
{
    public class Coin : CollectableBase
    {
        [SerializeField] private ResourcesAmount _toAddAmount;

        protected override void OnCollectedAction()
        {
        }
    }
}