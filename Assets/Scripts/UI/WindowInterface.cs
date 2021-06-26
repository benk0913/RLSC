
using System;

public interface WindowInterface 
{
    [Obsolete("Bob stop using this method directly. Call CORE.Instance.ShowWindow instead.")]
    void Show(ActorData actorData, object data);

    void Hide();
}