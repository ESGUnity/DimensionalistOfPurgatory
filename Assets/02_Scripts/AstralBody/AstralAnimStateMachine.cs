using UnityEngine;

public class AstralAnimStateMachine
{
    Animator animator;

    public float attackAnimationClipLength;
    public float moveAnimationClipLength;
    public float ritualAnimationClipLength;
    public float manaAbilityAnimationClipLength;
    public float conditionAbilityAnimationClipLength;
    public float deadAbilityAnimationClipLength;

    public AstralAnimStateMachine(Animator animator)
    {
        this.animator = animator;

        NormalizeAllAnimationClip();
    }

    public void NormalizeAllAnimationClip()
    {

        attackAnimationClipLength = 1.5f; // 0.666초
        moveAnimationClipLength = 1.5f; // 0.666초
        ritualAnimationClipLength = 0.6f; // 1.25초
        manaAbilityAnimationClipLength = 0.6f; // 1.25초
        conditionAbilityAnimationClipLength = 0.6f; // 1.25초
        deadAbilityAnimationClipLength = 0.6f; // 1.25초

        RuntimeAnimatorController controller = animator.runtimeAnimatorController;
        AnimationClip[] clips = controller.animationClips;

        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Attack":
                    animator.SetFloat("AttackSpeed", attackAnimationClipLength * clip.length);
                    break;
                case "Move":
                    animator.SetFloat("MoveSpeed", moveAnimationClipLength * clip.length);
                    break;
                case "Ritual":
                    animator.SetFloat("RitualSpeed", ritualAnimationClipLength * clip.length);
                    break;
                case "ManaAbility":
                    animator.SetFloat("ManaAbilitySpeed", manaAbilityAnimationClipLength * clip.length);
                    break;
                case "ConditionAbility":
                    animator.SetFloat("ConditionAbilitySpeed", conditionAbilityAnimationClipLength * clip.length);
                    break;
                case "DeadAbility":
                    animator.SetFloat("DeadAbilitySpeed", deadAbilityAnimationClipLength * clip.length);
                    break;
            }
        }
    }
    public void OnIdle()
    {
        animator.SetTrigger("Idle");
    }
    public void OnAttack()
    {
        animator.SetTrigger("Attack");
        PhaseManager.Instance.SetAstralActionTerm(1f / attackAnimationClipLength + 0.1f); // 영체의 한 턴의 길이를 애니메이션 클립 길이로 설정
    }
    public void OnMove()
    {
        animator.SetTrigger("Move");
        PhaseManager.Instance.SetAstralActionTerm(1f / moveAnimationClipLength + 0.1f);
    }
    public void OnRitual()
    {
        animator.SetTrigger("Ritual");
        PhaseManager.Instance.SetAstralActionTerm(1f / ritualAnimationClipLength + 0.1f);
    }
    public void OnManaAbility()
    {
        animator.SetTrigger("ManaAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / manaAbilityAnimationClipLength + 0.1f);
    }
    public void OnConditionAbility()
    {
        animator.SetTrigger("ConditionAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / conditionAbilityAnimationClipLength + 0.1f);
    }
    public void OnDeadAbility()
    {
        animator.SetTrigger("DeadAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / deadAbilityAnimationClipLength + 0.1f);
    }
}
