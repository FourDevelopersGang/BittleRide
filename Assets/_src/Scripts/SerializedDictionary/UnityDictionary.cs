using System;
using System.Collections.Generic;
using UnityEngine;

    [Serializable]
    public class UnityDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField, HideInInspector]
        private List<TKey> _keyData = new List<TKey>();


        [SerializeField, HideInInspector]
        private List<TValue> _valueData = new List<TValue>();


        public UnityDictionary() : base()
        {
			
        }


        public UnityDictionary(IDictionary<TKey, TValue> source) : base(source)
        {
			
        }


        protected UnityDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
			
        }


        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();

            for (var i = 0; i < _keyData.Count && i < _valueData.Count; i++)
                this[_keyData[i]] = _valueData[i];
        }


        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _keyData.Clear();
            _valueData.Clear();

            foreach (var item in this)
            {
                _keyData.Add(item.Key);
                _valueData.Add(item.Value);
            }
        }
		
    }
