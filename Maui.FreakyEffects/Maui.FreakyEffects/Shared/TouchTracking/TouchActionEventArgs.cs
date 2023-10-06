namespace Maui.FreakyEffects.TouchTracking;

public class TouchActionEventArgs : EventArgs
{
    public TouchActionEventArgs(long id, TouchActionType type, TouchTrackingPoint location, bool isInContact)
    {
        Id = id;
        Type = type;
        Location = location;
        IsInContact = isInContact;
    }

    public long Id { private set; get; }

    public TouchActionType Type { private set; get; }

    public TouchTrackingPoint Location { private set; get; }

    public bool IsInContact { private set; get; }
}

