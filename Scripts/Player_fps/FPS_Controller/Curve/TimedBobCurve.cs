using Godot;
using System;

[GlobalClass]
public partial class TimedBobCurve : Resource
{
    // Timed Bob Curve.
    // Used by [HeadMovement] for a jump bob.
    
    // Duration of the entire bob process
    [Export] public float Duration { get; set; } = 0.2f;
    
    // Max amount in bob offset
    [Export] public float Amount { get; set; } = 0.1f;
    
    // Actual offset of bob
    private float _offset;
    
    // Actual direction flag of bob
    // true - if initial state
    // false - for final state
    private bool _direction = true;
    
    // Current time for current direction flag
    private float _time;

    // Return actual offset of bob
    public float GetOffset()
    {
        return _offset;
    }
    
    // Init bob timer
    public void DoBobCycle()
    {
        _time = Duration;
        _direction = false;
    }
    
    // Tick process of bob timer
    public void BobProcess(double delta)
    {
        if (_time > 0)
        {
            _time -= (float)delta;
            if (_direction)
            {
                _offset = Mathf.Lerp(0.0f, Amount, _time / Duration);
            }
            else
            {
                _offset = Mathf.Lerp(Amount, 0.0f, _time / Duration);
            }

            if (_time < 0 && !_direction)
            {
                BackDoBobCycle();
            }
        }
    }

    private void BackDoBobCycle()
    {
        _time = Duration;
        _direction = true;
    }
    
}
