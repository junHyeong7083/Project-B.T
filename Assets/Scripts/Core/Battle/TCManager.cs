using System;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// TC 관리 전담 클래스
    /// SRP: TC 관련 로직만 담당
    /// </summary>
    public class TCManager : MonoBehaviour
    {
        [Header("TC 설정")]
        [SerializeField] private int startingTC = 0;

        private int currentTC;

        // 이벤트
        public event Action<int> OnTCChanged;

        public int GetCurrentTC() => currentTC;

        public void Initialize()
        {
            currentTC = startingTC;
            OnTCChanged?.Invoke(currentTC);
        }

        public void Initialize(int startingTCValue)
        {
            startingTC = startingTCValue;
            currentTC = startingTC;
            OnTCChanged?.Invoke(currentTC);
        }

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
    }
}

