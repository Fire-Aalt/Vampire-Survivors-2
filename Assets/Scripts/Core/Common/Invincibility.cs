using Cysharp.Threading.Tasks;

public class Invincibility
{
    public bool IsInvincible => IsActionInvincible || IsHitInvincible;

    public bool IsActionInvincible { get; set; }
    public bool IsHitInvincible { get; protected set; }

    public Invincibility(float awakeIFramesDuration = 1f)
    {
        StartActionInvincibility(awakeIFramesDuration);
    }

    public virtual void StartHitInvincibility(float iFramesDuration)
    {
        _ = HitInvincibility(iFramesDuration);
    }

    protected virtual async UniTaskVoid HitInvincibility(float iFramesDuration)
    {
        IsHitInvincible = true;

        await UniTask.WaitForSeconds(iFramesDuration);

        IsHitInvincible = false;
    }

    public virtual void StartActionInvincibility(float iFramesDuration)
    {
        _ = ActionInvincibility(iFramesDuration);
    }

    protected virtual async UniTaskVoid ActionInvincibility(float iFramesDuration)
    {
        IsActionInvincible = true;

        await UniTask.WaitForSeconds(iFramesDuration);

        IsActionInvincible = false;
    }
}