using UnityEngine;
using UnityEngine.Events;

public abstract class Objective : MonoBehaviour
{
    public UnityEvent OnProgressChanged = new UnityEvent();
    
    protected abstract void UpdateProgress();
    public abstract int GetProgressPercentage();

    public abstract void OnSetActive();
    public abstract void OnSetInactive();
}
