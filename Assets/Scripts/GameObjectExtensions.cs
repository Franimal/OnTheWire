using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameObjectExtensions
    {
        public static List<GameObject> Children(this GameObject go)
        {
            var list = new List<GameObject>();

            foreach (Transform child in go.transform)
            {
                list.Add(child.gameObject);
            }

            return list;
        }
    }
}
