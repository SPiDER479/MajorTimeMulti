using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MT
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public List<Image> inventoryIcons = new List<Image>();
        public List<Sprite> slimeDefaultIcons = new List<Sprite>(); 
        public List<Image> inventoryBackIcons = new List<Image>();
        [SerializeField] public Sprite defaultSlot;
        [SerializeField] public Sprite selectedSlot;
    }
}


