using System;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 전투 자원 관리 (AP, TC)
    /// 기획서 2.2 참조
    /// </summary>
    public class ResourceManager : MonoBehaviour
    {
        [Header("AP 설정")]
        [SerializeField] private int maxAP = 3;
        [SerializeField] private int startingAP = 3;
        [SerializeField] private int apPerTurn = 3;

        [Header("TC 설정")]
        [SerializeField] private int startingTC = 0;

        // 플레이어 자원
        private int currentAP;
        private int currentTC;

        // 이벤트
        public event Action<int> OnAPChanged;
        public event Action<int> OnTCChanged;

        private void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            currentAP = startingAP;
            currentTC = startingTC;
        }

        #region AP 관리

        public int GetCurrentAP() => currentAP;
        public int GetMaxAP() => maxAP;

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

        #endregion

        #region TC 관리

        public int GetCurrentTC() => currentTC;

        /// <summary>
        /// TC 누적
        /// </summary>
        public void AddTC(int amount)
        {
            currentTC += amount;
            OnTCChanged?.Invoke(currentTC);
        }

        /// <summary>
        /// TC 초기화 (턴 전환 시)
        /// </summary>
        public void ResetTC()
        {
            currentTC = 0;
            OnTCChanged?.Invoke(currentTC);
        }

        /// <summary>
        /// TC 비교하여 턴 결정
        /// 기획서 2.2: PC_TC <= NPC_TC 이면 플레이어 턴 유지
        /// </summary>
        public bool ShouldPlayerTurnContinue(int enemyTC)
        {
            return currentTC <= enemyTC;
        }

        #endregion
    }
}

