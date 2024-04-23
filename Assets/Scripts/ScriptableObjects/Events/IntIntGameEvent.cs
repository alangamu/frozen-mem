using System;
using UnityEngine;

namespace Assets.Scripts.ScriptableObjects.Events
{
    [CreateAssetMenu(menuName = "Events/Int Int GameEvent")]
    public class IntIntGameEvent : ScriptableObject
    {
        public event Action<int, int> OnRaise;

        public void Raise(int a, int b)
        {
            OnRaise?.Invoke(a, b);
        }
    }
}