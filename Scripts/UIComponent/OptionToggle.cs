using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UIComponent
{
    [RequireComponent(typeof(Button))]
    public class OptionToggle : MonoBehaviour
    {
        [SerializeField] private Button buttonComponent;
        [SerializeField] private TextMeshProUGUI tmp;
        private readonly List<string> _items = new();
        private int _currentItemIndex = 0;

        private void Reset()
        {
            buttonComponent = gameObject.GetComponent<Button>();
        }

        private void Start()
        {
            buttonComponent.onClick.AddListener(ChangeItems);
        }

        public string GetCurrentItem() => _items[_currentItemIndex];

        private void ChangeItems()
        {
            _currentItemIndex += 1;
            if (_currentItemIndex >= _items.Count) _currentItemIndex = 0;
            tmp.text = _items[_currentItemIndex];
        }

        public void ChangeIndex(int index)
        {
            _currentItemIndex = index;
            tmp.text = _items[_currentItemIndex];
        }

        public void FindItemAndChangeIndex(string item)
        {
            var index = _items.FindIndex(a => a == item);
            
            ChangeIndex(index);
        }

        public void SetItems(IEnumerable<string> newItems) => _items.AddRange(newItems);
        public void AddItem(string item) => _items.Add(item);
        public void RemoveItem(string item) => _items.Remove(item);
        public void RemoveItemAt(int index) => _items.RemoveAt(index);
        public void AddCallback(UnityAction action) => buttonComponent.onClick.AddListener(action);
        public void RemoveCallback(UnityAction action) => buttonComponent.onClick.RemoveListener(action);
    }
}