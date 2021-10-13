using UnityEngine;

public interface IGraphVisual
{
    void AddGraphVisual(Vector2 graphPosition, float graphPositionWidth, string tooltipText);
    void Reset();
}
