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

    public SelectionLayer(MapSelectionManager Map, 
        Vector2Int Origin, 
        SelectionLayerFilter Filter, 
        Action<Vector2Int, MapSelectionManager> OnSelected)
    {
        Positions = new();

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

    public List<int> PreviousVector;

    private void Start()
    {
        Tiles.AddRange(GetComponentsInChildren<TileSelector>());
        PreviousVector = new(Tiles.Count);
        for (int i = 0; i < Tiles.Count; i++)
            PreviousVector.Add(-1);
    }

    public Vector2Int GetSelectorPos(TileSelector selector)
    {
        return BattleMap.DeltaToPos(Tiles.IndexOf(selector));
    }

    public void AddSelectionLayer(Vector2Int Origin, SelectionLayerFilter Filter, 
        Action<Vector2Int, MapSelectionManager> OnSelected)
    {
        SelectionLayers.Push(new SelectionLayer(this, Origin, Filter, OnSelected));
        
        ApplySelectionFilter();
    }

    void ApplySelectionFilter()
    {
        var layer = SelectionLayers.Last(); 
        layer.Positions.Clear();
        
        if (layer.Filter.IsContinuous)
            ApplySelectionFilterContinuous();
        else
            ApplySelectionFilterNonContinuous();
    }

    void ApplySelectionFilterNonContinuous()
    {
        for (int i = 0; i < Tiles.Count; i++)
            ApplyFilterToDelta(i);
    }

    void ApplySelectionFilterContinuous()
    {
        for (int i = 0; i < PreviousVector.Count; i++) 
            PreviousVector[i] = -1;


        Dictionary<int, int> Visible = new(PreviousVector.Count);
        Dictionary<int, int> NextVisible = new(PreviousVector.Count);
        var deltaOrigin = BattleMap.PosToDelta(SelectionLayers.Last().Origin);
        Visible.Add(deltaOrigin, deltaOrigin);

        while (Visible.Count > 0)
        {
            NextVisible.Clear();
            foreach (var pair in Visible)
            {
                var prev = pair.Value;
                var current = pair.Key;
                
                if(PreviousVector[current] != -1)
                    continue;
                
                PreviousVector[current] = prev;

                if (!ApplyFilterToDelta(current) && current != deltaOrigin)
                    continue;
                
                var pos = BattleMap.DeltaToPos(current);
                for(int i = 0; i < 4; i++)
                {
                    var x = i % 2 == 0? i - 1 : 0;
                    var y = i % 2 == 0? 0 : i - 2;

                    var next = BattleMap.PosToDelta(pos + new Vector2Int(x, y));
                    
                    if(next == -1)
                        continue;
                    
                    if (!Visible.ContainsKey(next))
                        NextVisible[next] = current;
                }
            }
            Visible.Clear();
            foreach (var pair in NextVisible)
                Visible.Add(pair.Key, pair.Value);
        }
        
        for(int i = 0; i < PreviousVector.Count; i++)
            if(PreviousVector[i] == -1)
                Tiles[i].SetState(TileSelectorState.OffLayer);
    }

    bool ApplyFilterToDelta(int delta)
    {
        var pos = BattleMap.DeltaToPos(delta);
        var layer = SelectionLayers.Last();
        var isFiltered = layer.Filter.Filter(this, layer.Origin, pos);
        if (isFiltered) layer.Positions.Add(pos);
        Tiles[delta].SetState(isFiltered ? TileSelectorState.OnLayer : TileSelectorState.OffLayer);
        return isFiltered;
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
