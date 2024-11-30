using UnityEngine;

public class AstralAnimStateMachine
{
    Animator animator;

    float attackAnimationClipLength = 1f;
    float moveAnimationClipLength = 1f;
    float ritualAnimationClipLength = 1.5f;
    float manaAbilityAnimationClipLength = 2.5f;
    float conditionAbilityAnimationClipLength = 2.5f;
    float deadAbilityAnimationClipLength = 2.5f;

    public AstralAnimStateMachine(Animator animator)
    {
        this.animator = animator;

        NormalizeAllAnimationClip();
    }

    public void NormalizeAllAnimationClip()
    {
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
        PhaseManager.Instance.SetAstralActionTerm(attackAnimationClipLength);
    }
    public void OnMove()
    {
        animator.SetTrigger("Move");
        PhaseManager.Instance.SetAstralActionTerm(moveAnimationClipLength);
    }
    public void OnRitual()
    {
        animator.SetTrigger("Ritual");
        PhaseManager.Instance.SetAstralActionTerm(ritualAnimationClipLength);
    }
    public void OnManaAbility()
    {
        animator.SetTrigger("ManaAbility");
        PhaseManager.Instance.SetAstralActionTerm(manaAbilityAnimationClipLength);
    }
    public void OnConditionAbility()
    {
        animator.SetTrigger("ConditionAbility");
        PhaseManager.Instance.SetAstralActionTerm(conditionAbilityAnimationClipLength);
    }
    public void OnDeadAbility()
    {
        animator.SetTrigger("DeadAbility");
        PhaseManager.Instance.SetAstralActionTerm(deadAbilityAnimationClipLength);
    }
}
