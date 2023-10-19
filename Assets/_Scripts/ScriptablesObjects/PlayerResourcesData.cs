using System;
using UnityEngine;

namespace _Scripts.ScriptablesObjects
{
    [Serializable]
    public class ResourcesAmount
    {
        public int _money;
        public int _diamonds;

        public int ResourcesSum => _money + _diamonds;

        public ResourcesAmount(int money = 0, int diamonds = 0)
        {
            _money = money;
            _diamonds = diamonds;
        }

        public override string ToString()
        {
            var str = "";
            if (_money > 0) str += _money;
            if (_diamonds > 0) str += _diamonds;

            return str;
        }

        public static ResourcesAmount operator *(ResourcesAmount a, float b)
        {
            return new ResourcesAmount
            {
                _money = (int) (a._money * b),
                _diamonds = (int) (a._diamonds * b)
            };
        }

        public static bool operator >=(ResourcesAmount a, ResourcesAmount b)
        {
            return a._money >= b._money && a._diamonds >= b._diamonds;
        }

        public static bool operator <=(ResourcesAmount a, ResourcesAmount b)
        {
            return a._money <= b._money && a._diamonds <= b._diamonds;
        }

        public static ResourcesAmount operator +(ResourcesAmount a, ResourcesAmount b)
        {
            return new ResourcesAmount
            {
                _money = a._money + b._money,
                _diamonds = a._diamonds + b._diamonds
            };
        }

        public static ResourcesAmount operator -(ResourcesAmount a, ResourcesAmount b)
        {
            return new ResourcesAmount
            {
                _money = a._money - b._money,
                _diamonds = a._diamonds - b._diamonds
            };
        }

        public static bool operator >(ResourcesAmount a, ResourcesAmount b)
        {
            return a._money > b._money && a._diamonds > b._diamonds;
        }

        public static bool operator <(ResourcesAmount a, ResourcesAmount b)
        {
            return a._money < b._money && a._diamonds < b._diamonds;
        }

        public static ResourcesAmount operator *(ResourcesAmount a, int b)
        {
            return new ResourcesAmount
            {
                _money = a._money * b,
                _diamonds = a._diamonds * b
            };
        }

        public static ResourcesAmount operator *(ResourcesAmount a, ResourcesAmount b)
        {
            return new ResourcesAmount
            {
                _money = a._money / b._money,
                _diamonds = a._diamonds / b._diamonds
            };
        }

        public static ResourcesAmount operator /(ResourcesAmount a, int b)
        {
            return new ResourcesAmount
            {
                _money = a._money / b,
                _diamonds = a._diamonds / b
            };
        }

        public static ResourcesAmount operator /(ResourcesAmount a, ResourcesAmount b)
        {
            return new ResourcesAmount
            {
                _money = a._money / b._money,
                _diamonds = a._diamonds / b._diamonds
            };
        }

        public void RemoveResources(int toRemove)
        {
            if (toRemove > _money)
            {
                toRemove -= _money;
                _money = 0;
                _diamonds -= toRemove;
                if (_diamonds < 0) _diamonds = 0;
            }
            else
            {
                _money -= toRemove;
            }
        }
    }

    [CreateAssetMenu(fileName = "PlayerResourceData", menuName = "Data/PlayerResourcesData", order = 0)]
    public class PlayerResourcesData : ScriptableObject
    {
        public ResourcesAmount _currentResourcesAmount = new();
    }
}