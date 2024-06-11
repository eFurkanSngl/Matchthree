using UnityEngine;
using UnityEngine.Events;
using Components;

namespace Events
{
    public class InputEvents
    {
        public UnityAction<Tile, Vector3> MouseDownGrid;
        public UnityAction<Vector3> MouseUpGrid;
    }
}