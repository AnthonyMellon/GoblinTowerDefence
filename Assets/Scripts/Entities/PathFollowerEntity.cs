using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowerEntity : MonoBehaviour
{
    private Path _currentPath;
    private int _currentPathPointIndex;
    private float _progressTowardsTargetPoint;
    private Vector2Int _targetPoint;
    private Vector2Int _currentPoint;
    private float _speed = 2;

    public Action<StructureBase> OnTargetStructureChange;
    public Action OnPathEnded;

    public void SetStats(float speed)
    {
        _speed = speed;
    }

    private void Update()
    {
        MoveAlongPath();
    }

    private void MoveAlongPath()
    {
        // No point trying to progress on a path that doesn't exist 
        if (_currentPath == null) return;

        _progressTowardsTargetPoint += Time.deltaTime * _speed;
        if (_progressTowardsTargetPoint >= 1)
        {
            GetNextPathPoint();
        }

        UpdatePosition();
    }

    private void GetNextPathPoint()
    {
        _currentPathPointIndex++;

        // If we're out of points we must have reached the target structure
        if (_currentPathPointIndex == _currentPath.Points.Count - 1)
        {
            _currentPath.TargetStructure.OnReached(this);
            return;
        }

        _currentPoint = _currentPath.Points[_currentPathPointIndex];
        _targetPoint = _currentPath.Points[_currentPathPointIndex + 1];
        _progressTowardsTargetPoint = 0;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        transform.localPosition = Vector2.Lerp(_currentPoint, _targetPoint, _progressTowardsTargetPoint);
    }

    public void SetPath(List<Vector2Int> points, StructureBase targetStructure)
    {
        _currentPath = new Path(points, targetStructure);        

        _currentPathPointIndex = -1;
        GetNextPathPoint();

        OnTargetStructureChange?.Invoke(targetStructure);
    }

    private class Path
    {
        public List<Vector2Int> Points { get; private set; } = new List<Vector2Int>();
        public StructureBase TargetStructure { get; private set; }        

        public Path(List<Vector2Int> points, StructureBase targetStructure)
        {
            Points = points;
            TargetStructure = targetStructure;
        }
    }
}
