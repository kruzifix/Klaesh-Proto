using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Klaesh
{
    public class GameInputComponent : MonoBehaviour, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log($"POINTER CLICK {eventData.pointerEnter.name}");
        }
    }
}
