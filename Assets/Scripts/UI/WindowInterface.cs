
using System;

public interface WindowInterface 
{
    [Obsolete("Do not call Show directly. Call `CORE.Instance.ShowWindow()` instead.")]
    void Show(ActorData actorData, object data);

    [Obsolete("Do not call Hide directly. Call `CORE.Instance.CloseCurrentWindow()` instead.")]
    void Hide();
}