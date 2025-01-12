using System;
using System.Collections.Generic;
using Clicker.Utils;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Clicker.ContentData
{
    [Serializable]
    public class CreatureStat
    {
        public float Value
        {
            get
            {
                if (_isDirty)
                {
                    _isDirty = false;
                    _value = CalculateFinalValue();
                }
                
                return _value;
            }
        }

        [SerializeField][ReadOnly] private float _value;
        [SerializeField][ReadOnly] private float _baseValue;
        [SerializeField] private float _debugCalcuatedValue;
        
        private bool _isDirty = true;
        private List<StatModifer> _statModiferList = new List<StatModifer>();
        
        public CreatureStat(float baseValue)
        {
            _value = _baseValue = baseValue;
        }

        public void AddStat(StatModifer statModifer)
        {
            if (!_isDirty)
            {
                _isDirty = true;
            }
            
            // Debug.Log($"{statModifer.EStatModType} / {Sys}");
            _statModiferList.Add(statModifer);
            _debugCalcuatedValue = CalculateFinalValue();
        }

        public void RemoveStat(StatModifer statModifer)
        {
            if (_statModiferList.Contains(statModifer))
            {
                _statModiferList.Remove(statModifer);
                _isDirty = true;
                _debugCalcuatedValue = CalculateFinalValue();
            }
        }

        public void RemoveStatBySource(object source)
        {
            int count = _statModiferList.RemoveAll(x=> x.Source == source);
            if (count > 0)
            {
                _isDirty = true;
                _debugCalcuatedValue = CalculateFinalValue();
            }
        }

        public int Compare(StatModifer a, StatModifer b)
        {
            if (a.Order == b.Order)
            {
                return 0;
            }
            
            return a.Order < b.Order ? -1 : 1;
        }

        public float CalculateFinalValue()
        {
            if (_statModiferList.Count == 0)
            {
                return _baseValue;
            }

            _statModiferList.Sort(Compare);

            float sumValue = 0;
            float finalValue = _baseValue;
            for (int i = 0; i < _statModiferList.Count; i++)
            {
                StatModifer statModifer = _statModiferList[i];
                switch (statModifer.EStatModType)
                {
                    case Define.EStatModType.PercentAdd:
                        sumValue += statModifer.Value;
                        if (i == _statModiferList.Count - 1 ||
                            _statModiferList[i + 1].EStatModType != Define.EStatModType.PercentAdd)
                        {
                            finalValue *= 1 + sumValue;
                            sumValue = 0;
                        }
                        break;
                    case Define.EStatModType.PercentMult:
                        finalValue *= 1 + statModifer.Value;
                        break;
                    case Define.EStatModType.Add:
                        finalValue += statModifer.Value;
                        break;
                }
            }

            return finalValue;
        }

    }

    [Serializable]
    public class StatModifer
    {
        public readonly Define.EStatModType EStatModType;
        public readonly float Value;
        public readonly int Order;
        public readonly object Source;
        
        public StatModifer(Define.EStatModType eStatModType, float value, object source, int order)
        {
            EStatModType = eStatModType;
            Value = value;
            Order = order;
            Source = source;
        }

    }
}