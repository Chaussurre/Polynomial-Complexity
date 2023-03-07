using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CombatSystem.Map;
using CombatSystem.Selection;
using UnityEngine;

struct SelectionLayer
{
    public List<Vector2Int> Positions;

    public Vector2Int Origin;
    public SelectionLayerFilter Filter;
    public Action<Vector2Int, MapSelectionManager> OnSelected;

    public SelectionLayer(BattleMap Map, 
        Vector2Int Origin, 
        SelectionLayerFilter Filter, 
        Action<Vector2Int, MapSelectionManager> OnSelected)
    {
        Positions = new();
        for (int i = 0; i < Map.Tiles.Count; i++)
        {
            var pos = Map.DeltaToPos(i);
            if(Filter.Filter(Map, Origin, pos))
                Positions.Add(pos);
        }

        this.Origin = Origin;
        this.Filter = Filter;
        this.OnSelected = OnSelected;
    }
}

public class MapSelectionManager : MonoBehaviour
{
    public BattleMap BattleMap;

    public readonly List<TileSelector> Tiles = new();
    public TileSelector Hovered { get; private set; }

    private readonly Stack<SelectionLayer> SelectionLayers = new();

    private void Start()
    {
        Tiles.AddRange(GetComponentsInChildren<TileSelector>());
    }

    public Vector2Int GetSelectorPos(TileSelector selector)
    {
        return BattleMap.DeltaToPos(Tiles.IndexOf(selector));
    }

    public void AddSelectionLayer(Vector2Int Origin, SelectionLayerFilter Filter, 
        Action<Vector2Int, MapSelectionManager> OnSelected)
    {
        SelectionLayers.Push(new SelectionLayer(BattleMap, Origin, Filter, OnSelected));
        
        ApplySelectionFilter();
    }

    void ApplySelectionFilter()
    {
        for(int i = 0; i < Tiles.Count; i++)
        {
            var pos = BattleMap.DeltaToPos(i);
            var layer = SelectionLayers.Last();
            Tiles[i].SetState(layer.Filter.Filter(BattleMap, layer.Origin, pos) 
                ? TileSelectorState.OnLayer
                : TileSelectorState.OffLayer);
        }
    }

    void CheckHover(Vector2 mousePos)
    {
        Hovered = null;
        foreach (var selector in Tiles)
            if (selector.isOverMe(mousePos))
            {
                Hovered = selector;
                selector.Hover();
            }
    }

    private void Update()
    {
        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!Hovered)
            CheckHover(mousePos);
        else if (!Hovered.isOverMe(mousePos))
        {
            Hovered.UnHover();
            CheckHover(mousePos);
        }

        if (Input.GetMouseButtonDown(0))
            Select();

        if (Input.GetMouseButtonDown(1))
            Cancel();
    }

    void Select()
    {
        if (SelectionLayers.Count == 0)
        {
            var pos = GetSelectorPos(Hovered);
            if(BattleMap.CombatEntitiesPos.TryGetValue(pos, out var entity) 
               && entity.MovementManager)
                AddSelectionLayer(pos, entity.MovementManager.MovementFilter, entity.MovementManager.Move);
        }
        else
        {
            var layer = SelectionLayers.Last();
            var pos = GetSelectorPos(Hovered);
            if (layer.Positions.Contains(pos))
                layer.OnSelected(pos, this);
        }
        
    }

    void Cancel()
    {
        SelectionLayers.TryPop(out _);
        if (SelectionLayers.Count > 0)
        {
            ApplySelectionFilter();
        }
        else
            EndStack();
    }

    public void EndStack()
    {
        SelectionLayers.Clear();
        foreach (var tile in Tiles)
            tile.SetState(TileSelectorState.NoCurrentLayer);
    }
}
