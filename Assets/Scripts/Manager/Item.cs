using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Custom/Item")]
public class Item : ScriptableObject
{
    [SerializeField] private Sprite itemSprite;
    [SerializeField] private GameObject itemPrefab;

    public Sprite Sprite => itemSprite;
    public GameObject Prefab => itemPrefab;
}
