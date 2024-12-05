using UnityEngine;

public class AstralAnimStateMachine
{
    Animator animator;

    public float attackAnimationClipLength;
    public float moveAnimationClipLength;
    float ritualAnimationClipLength;
    float manaAbilityAnimationClipLength;
    float conditionAbilityAnimationClipLength;
    float deadAbilityAnimationClipLength;

    public AstralAnimStateMachine(Animator animator)
    {
        this.animator = animator;

        NormalizeAllAnimationClip();
    }

    public void NormalizeAllAnimationClip()
    {

        attackAnimationClipLength = 1f;
        moveAnimationClipLength = 1.5f; // 0.6667초
        ritualAnimationClipLength = 0.7f; // 1.4286초
        manaAbilityAnimationClipLength = 0.5f; // 2초
        conditionAbilityAnimationClipLength = 0.5f; // 2초
        deadAbilityAnimationClipLength = 0.5f; // 2초

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
        PhaseManager.Instance.SetAstralActionTerm(1f / attackAnimationClipLength + 0.2f); // 영체의 한 턴의 길이를 애니메이션 클립 길이로 설정
    }
    public void OnMove()
    {
        animator.SetTrigger("Move");
        PhaseManager.Instance.SetAstralActionTerm(1f / moveAnimationClipLength + 0.2f);
    }
    public void OnRitual()
    {
        animator.SetTrigger("Ritual");
        PhaseManager.Instance.SetAstralActionTerm(1f / ritualAnimationClipLength + 0.2f);
    }
    public void OnManaAbility()
    {
        animator.SetTrigger("ManaAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / manaAbilityAnimationClipLength + 0.2f);
    }
    public void OnConditionAbility()
    {
        animator.SetTrigger("ConditionAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / conditionAbilityAnimationClipLength + 0.2f);
    }
    public void OnDeadAbility()
    {
        animator.SetTrigger("DeadAbility");
        PhaseManager.Instance.SetAstralActionTerm(1f / deadAbilityAnimationClipLength + 0.2f);
    }
}
