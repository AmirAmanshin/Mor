using UnityEngine;

[RequireComponent(typeof(EnemyMovementAI))]
[RequireComponent(typeof(EnemyCombatAI))]
[RequireComponent(typeof(EnemySensesAI))]
public class EnemyHubAI : MonoBehaviour
{
    [Header("Tactics properties")]
    public float shootingRange = 10f;

    private EnemyMovementAI gilbert;
    private EnemyCombatAI gustav;
    private EnemySensesAI henry;

    private Animator animator;

    private bool isDeadHandled = false;

    void Awake()
    {
        gilbert = GetComponent<EnemyMovementAI>();
        gustav = GetComponent<EnemyCombatAI>();
        henry = GetComponent<EnemySensesAI>();

        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        if (gustav.IsDead())
        {
            if (!isDeadHandled) HandleDeath();
            return;
        }

        AnalyzeAndAct();
    }

    private void AnalyzeAndAct()
    {
        // Обязательная проверка на случай, если сенсоры вернут ошибку
        if (henry == null || gilbert == null || gustav == null) return;

        bool targetConfirmed = henry.IsTargetConfirmed();
        bool isSuspicious = henry.IsSuspicious();
        Vector3 targetPos = henry.GetPointOfInterest();

        int warningLevel = 0;
        bool isWalkingToTarget = false;
        bool isChasing = false;
        bool isTargetLocked = false;

        if (targetConfirmed)
        {
            warningLevel = 1;
            float distanceToTarget = gilbert.GetDistanceTo(targetPos);

            if (distanceToTarget > shootingRange)
            {
                isChasing = true;
                gilbert.RunTo(targetPos);
            }
            else
            {
                isTargetLocked = true;
                gilbert.StopMoving();
                gilbert.FaceTarget(targetPos);
                gustav.FireAt(targetPos);
            }
        }
        else if (isSuspicious)
        {
            // ИСПРАВЛЕННАЯ ЛОГИКА ПОДХОДА
            if (gilbert.GetDistanceTo(targetPos) > 1.5f)
            {
                isWalkingToTarget = true;
                gilbert.WalkTo(targetPos);
            }
            else
            {
                isWalkingToTarget = false;
                gilbert.StopMoving();
            }
        }
        else
        {
            gilbert.Patrol();
        }

        // Выводим лог. Обязательно проверь консоль (вкладку Console), нет ли там красного текста!
        Debug.Log($"Текущая цель: {targetPos}");

        UpdateAnimator(warningLevel, isWalkingToTarget, isChasing, isTargetLocked);
    }

    private void UpdateAnimator(int warningLvl, bool walking, bool chasing, bool locked)
    {
        if (animator == null) return;

        animator.SetInteger("warningLevel", warningLvl);
        animator.SetBool("isWalkingToTarget", walking);
        animator.SetBool("isChasing", chasing);
        animator.SetBool("isTargetLocked", locked);
    }

    private void HandleDeath()
    {
        isDeadHandled = true;

        gilbert.StopMoving();

        UpdateAnimator(0, false, false, false);
        if (animator != null)
        {
            // Убедись, что в Аниматоре есть триггер "Die" для перехода в состояние смерти
            animator.SetTrigger("Die");
        }

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Debug.Log("Enemy is totaly dead");
    }
}