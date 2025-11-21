using System;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// AP 관리 전담 클래스
    /// SRP: AP 관련 로직만 담당
    /// </summary>
    public class APManager : MonoBehaviour
    {
        [Header("AP 설정")]
        [SerializeField] private int maxAP = 3;
        [SerializeField] private int startingAP = 3;
        [SerializeField] private int apPerTurn = 3;

        private int currentAP;

        // 이벤트
        public event Action<int> OnAPChanged;

        public int GetCurrentAP() => currentAP;
        public int GetMaxAP() => maxAP;

        public void Initialize()
        {
            currentAP = startingAP;
            OnAPChanged?.Invoke(currentAP);
        }

        public void Initialize(int maxAPValue, int startingAPValue, int apPerTurnValue)
        {
            maxAP = maxAPValue;
            startingAP = startingAPValue;
            apPerTurn = apPerTurnValue;
            currentAP = startingAP;
            OnAPChanged?.Invoke(currentAP);
        }

        /// <summary>
        /// AP 소모
        /// </summary>
        public bool SpendAP(int amount)
        {
            if (currentAP >= amount || amount == 0) // 스프린트는 AP 0
            {
                currentAP = Mathf.Max(0, currentAP - amount);
                OnAPChanged?.Invoke(currentAP);
                return true;
            }
            return false;
        }

        /// <summary>
        /// AP 회복 (턴 시작 시)
        /// </summary>
        public void RestoreAP()
        {
            currentAP = Mathf.Min(maxAP, currentAP + apPerTurn);
            OnAPChanged?.Invoke(currentAP);
        }

        /// <summary>
        /// AP 완전 회복 (레이즈 등)
        /// </summary>
        public void FullRestoreAP()
        {
            currentAP = maxAP;
            OnAPChanged?.Invoke(currentAP);
        }
    }
}

