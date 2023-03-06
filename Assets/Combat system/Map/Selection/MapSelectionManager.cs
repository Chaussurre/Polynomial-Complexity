using System;
using System.Collections;
using System.Collections.Generic;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

public class MapSelectionManager : MonoBehaviour
{
    [SerializeField] private BattleMap BattleMap;

    public readonly List<TileSelector> Tiles = new();

    public TileSelector Selected { get; private set; }
    public TileSelector Hovered { get; private set; }

    private void Start()
    {
        Tiles.AddRange(GetComponentsInChildren<TileSelector>());
    }

    void CheckHover(Vector2 mousePos)
    {
        foreach (var selector in Tiles)
            if (selector != Selected && selector.isOverMe(mousePos))
            {
                Hovered = selector;
                selector.Hover();
            }
    }

    void CheckSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Selected?.Unselect();
            Selected = Hovered;
            Hovered = null;
            Selected?.Select();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Selected?.Unselect();
            Selected = null;
        }
    }
    
    private void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Hovered == null)
            CheckHover(mousePos);
        else if (!Hovered.isOverMe(mousePos))
        {
            Hovered.UnHover();
            CheckHover(mousePos);
        }

        CheckSelect();
    }
}
