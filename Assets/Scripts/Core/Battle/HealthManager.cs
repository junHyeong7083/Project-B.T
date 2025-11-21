using System;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// HP 관리 전담 클래스
    /// SRP: HP 관련 로직만 담당
    /// </summary>
    public class HealthManager : MonoBehaviour
    {
        [Header("HP 설정")]
        [SerializeField] private int maxHP = 100;
        [SerializeField] private int startingHP = 100;

        private int currentHP;

        // 이벤트
        public event Action<int> OnHPChanged;
        public event Action OnHPDepleted;

        public int GetCurrentHP() => currentHP;
        public int GetMaxHP() => maxHP;
        public float GetHPRatio() => (float)currentHP / maxHP;

        public void Initialize()
        {
            currentHP = startingHP;
            OnHPChanged?.Invoke(currentHP);
        }

        public void Initialize(int maxHPValue, int startingHPValue)
        {
            maxHP = maxHPValue;
            startingHP = startingHPValue;
            currentHP = startingHP;
            OnHPChanged?.Invoke(currentHP);
        }

        /// <summary>
        /// 데미지 적용
        /// </summary>
        public void TakeDamage(int damage)
        {
            currentHP = Mathf.Max(0, currentHP - damage);
            OnHPChanged?.Invoke(currentHP);

            if (currentHP <= 0)
            {
                OnHPDepleted?.Invoke();
            }
        }

        /// <summary>
        /// 회복
        /// </summary>
        public void Heal(int amount)
        {
            currentHP = Mathf.Min(maxHP, currentHP + amount);
            OnHPChanged?.Invoke(currentHP);
        }

        /// <summary>
        /// 완전 회복
        /// </summary>
        public void FullHeal()
        {
            currentHP = maxHP;
            OnHPChanged?.Invoke(currentHP);
        }

        /// <summary>
        /// HP 설정 (레이즈 등 특수 상황)
        /// </summary>
        public void SetHP(int hp)
        {
            currentHP = Mathf.Clamp(hp, 0, maxHP);
            OnHPChanged?.Invoke(currentHP);

            if (currentHP <= 0)
            {
                OnHPDepleted?.Invoke();
            }
        }
    }
}

