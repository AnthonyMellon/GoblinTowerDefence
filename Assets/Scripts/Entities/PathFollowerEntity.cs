using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFollowerEntity : MonoBehaviour
{
    private Path _currentPath;
    private int _pathProgress;

    private void Start()
    {
        StartCoroutine(FollowPathRoutine());   
    }

    private IEnumerator FollowPathRoutine()
    {
        while (true)
        {
            ProgressOnPath();
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ProgressOnPath()
    {
        // No point trying to progress on a path that doesn't exist 
        if (_currentPath == null) return;

        _pathProgress++;

        // Have we reached the end of the path?
        if(_pathProgress == _currentPath.Points.Count)
        {
            _currentPath.TargetStructure.OnReached(this);
            return;
        }

        // Move to the next point along the path
        Vector3 newPosition = new Vector3(
            _currentPath.Points[_pathProgress].x,
            _currentPath.Points[_pathProgress].y
            );
        transform.localPosition = newPosition;        
    }

    public void SetPath(List<Vector2Int> points, StructureBase targetStructure)
    {
        _currentPath = new Path(points, targetStructure);
        _pathProgress = 0;
    }

    protected class Path
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
