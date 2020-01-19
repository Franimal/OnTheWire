using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
using System;

public class PipeGeneration : MonoBehaviour
{
    public GameObject[] SegmentPrefabs;

    public Player Player;

    // This is equivalent to the negative of the minimum X local rotation.
    public double MaxDownwardAngle;
    public int MaxSegmentsAtOneTime;

    public string EndPositionReferenceObjectName;

    private Transform _pipeStartPosition;
    private List<GameObject> _segments;
    
    private float _maxSpeed = 0.2f;

    private Color[] _segmentColors = { Color.blue, Color.cyan, Color.magenta, Color.green, Color.red, Color.yellow };
    private int _colorLerpSegmentAmount = 20;
    private int _currentColor = -1;
    private int _previousColor = -1;
    private int _numberOfThisColorToGo = 0;

    // Start is called before the first frame update
    void Start()
    {
        _segments = new List<GameObject>();

        _pipeStartPosition = transform;
        _currentColor = UnityEngine.Random.Range(0, _segmentColors.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if(_segments?.Count < MaxSegmentsAtOneTime)
        {
            _segments.Add(AddRandomSegment());
        }

        // SPEED HANDLING.  MOVE TO PLAYER?
       
        if (Input.GetKey(KeyCode.W))
        {
            Player.Speed += 0.3f * Time.deltaTime;
        }
        else
        {
            Player.Speed = Math.Max(0, Player.Speed - 0.1f * Time.deltaTime);
        }

        Player.Speed += 0.02f * Player.transform.localRotation.x;

        if (Player.Speed > _maxSpeed)
            Player.Speed = _maxSpeed;

        // 

        var guides = FindObjectsOfType<GuideTransform>(); // These should be registered once when the guides are created, not found every update.

        if (guides == null || guides.Length < 2)
            return;

        var closestGuide = guides.OrderBy(guide => Vector3.Distance(guide.transform.position, Vector3.zero)).First();
        var movementDirection = -closestGuide.transform.forward;

        Player.RotateTowards(closestGuide.transform.rotation, Player.Speed * 0.5f);

        var segmentsToDelete = new List<GameObject>();

        var amountToMoveEverything = Vector3.zero;

        //Do things to each segment.  Destroy if they are too far behind player.
        foreach (var seg in _segments)
        {
            // If a segment is pretty much at 0, 0, 0, set it to 0, 0, 0 to avoid drift.  Only do this if it's z position is greater than 0, 0, 0 though.
            var heading = seg.transform.position - Player.transform.position;
            var dot = Vector3.Dot(heading, Player.transform.forward);

            if (Vector3.Distance(seg.transform.localPosition, Vector3.zero) < 0.2f && dot > 0)
            {
                Debug.Log("Need to set to zero");
                amountToMoveEverything = Vector3.zero - seg.transform.localPosition;
            }

            seg.transform.position = seg.transform.position + movementDirection * Player.Speed;
        }

        foreach (var seg in _segments)
        {
            seg.transform.localPosition = seg.transform.localPosition + amountToMoveEverything;
        }

        if (Vector3.Distance(_segments[0].transform.position, Player.transform.position) > 20)
        {
            segmentsToDelete.Add(_segments[0]);
        }

        for (var i = 0; i < segmentsToDelete.Count; i++)
        {
            _segments.Remove(segmentsToDelete[i]);
            Destroy(segmentsToDelete[i]);
        }
    }

    private GameObject AddRandomSegment()
    {
        var randomSegmentIndex = (int)UnityEngine.Random.Range(0, SegmentPrefabs.Length);
        var segmentPrefab = SegmentPrefabs[randomSegmentIndex];

        var segment = Instantiate(segmentPrefab);

        segment.transform.parent = transform;

        // If this is the first segment, the reference position should be the start position.  Otherwise it should be on the end of the last one.
        var segmentTransform = _segments.Count == 0 ? _pipeStartPosition :
            _segments[_segments.Count - 1].Children().First(child => child.name == EndPositionReferenceObjectName).transform;


        //TODO: We should use the end marker of the last segment rotation and the end marker rotation of segment options and not use any that would be more at an angle than our max


        segment.transform.position = segmentTransform.position;
        segment.transform.rotation = segmentTransform.rotation;

        var color = GetSegmentColor();

        foreach (var seg in segment.GetComponentsInChildren<Renderer>())
        {
            seg.material.color = color;
        }

        return segment;
    }

    private Color GetSegmentColor()
    {
        // Color[] _segmentColors = { Color.blue, Color.cyan, Color.gray, Color.green, Color.red, Color.yellow };
        // _colorLerpSegmentAmount = 5;
        // _currentColor = -1;
        // _numberOfThisColorToGo = 0;

        if(_numberOfThisColorToGo == 0)
        {            
            _numberOfThisColorToGo = _colorLerpSegmentAmount;
            _previousColor = _currentColor;
            _currentColor = UnityEngine.Random.Range(0, _segmentColors.Length);
        } else
        {
            _numberOfThisColorToGo--;
        }

        return Color.Lerp(_segmentColors[_previousColor], _segmentColors[_currentColor], (_colorLerpSegmentAmount - _numberOfThisColorToGo) / (_colorLerpSegmentAmount * 1.0f));
    }
}
