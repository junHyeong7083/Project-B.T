using System;
using CardBullet.Core.Deck;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 레이즈 시스템 (일발 역전 기믹)
    /// 기획서 2.3 참조: 주사위 기반 역전 시스템
    /// </summary>
    public class RaiseSystem : MonoBehaviour
    {
        [Header("레이즈 조건")]
        [SerializeField] private float hpThreshold = 0.1f; // 10% 이하

        [Header("참조")]
        [SerializeField] private HealthManager playerHealth;
        [SerializeField] private HealthManager enemyHealth;
        [SerializeField] private APManager playerAP;
        [SerializeField] private DeckManager deckManager;
        
        // 이벤트
        public event Action<int, RaiseResult> OnRaiseExecuted;
        public event Action<bool> OnRaiseAvailabilityChanged;

        public enum RaiseResult
        {
            AverageHP,      // [1~4]: 체력 평균화
            FullHealStun,   // [5]: 완전 회복 + 1턴 스턴
            FullDrawRecover // [6]: 풀 드로우 + AP 완전 회복
        }

        private bool isRaiseAvailable = false;

        /// <summary>
        /// 레이즈 발동 가능 여부 체크
        /// 조건: PC HP < 10% AND Current AP == 0
        /// SRP: 레이즈 발동 조건 체크만 담당
        /// </summary>
        public bool CheckRaiseAvailability()
        {
            if (playerHealth == null || playerAP == null)
                return false;

            float hpRatio = playerHealth.GetHPRatio();
            bool hpCondition = hpRatio < hpThreshold;
            bool apCondition = playerAP.GetCurrentAP() == 0;

            bool wasAvailable = isRaiseAvailable;
            isRaiseAvailable = hpCondition && apCondition;

            if (wasAvailable != isRaiseAvailable)
            {
                OnRaiseAvailabilityChanged?.Invoke(isRaiseAvailable);
            }

            return isRaiseAvailable;
        }

        /// <summary>
        /// 레이즈 실행
        /// </summary>
        public RaiseResult ExecuteRaise()
        {
            if (!isRaiseAvailable)
            {
                Debug.LogWarning("레이즈 발동 조건이 충족되지 않았습니다!");
                return RaiseResult.AverageHP;
            }

            // 1D6 주사위 굴림
            int diceRoll = UnityEngine.Random.Range(1, 7);
            RaiseResult result = GetRaiseResult(diceRoll);

            // 결과 적용
            ApplyRaiseResult(result);

            OnRaiseExecuted?.Invoke(diceRoll, result);
            isRaiseAvailable = false;

            return result;
        }

        /// <summary>
        /// 주사위 결과에 따른 레이즈 효과 결정
        /// </summary>
        private RaiseResult GetRaiseResult(int diceRoll)
        {
            switch (diceRoll)
            {
                case 1:
                case 2:
                case 3:
                case 4:
                    return RaiseResult.AverageHP;
                case 5:
                    return RaiseResult.FullHealStun;
                case 6:
                    return RaiseResult.FullDrawRecover;
                default:
                    return RaiseResult.AverageHP;
            }
        }

        /// <summary>
        /// 레이즈 결과 적용
        /// SRP: 레이즈 효과 적용만 담당
        /// </summary>
        private void ApplyRaiseResult(RaiseResult result)
        {
            switch (result)
            {
                case RaiseResult.AverageHP:
                    // 기획서: (PC_HP + NPC_HP) / 2 값으로 쌍방 체력 동기화
                    if (playerHealth != null && enemyHealth != null)
                    {
                        float averageHP = (playerHealth.GetCurrentHP() + enemyHealth.GetCurrentHP()) / 2f;
                        playerHealth.SetHP(Mathf.RoundToInt(averageHP));
                        enemyHealth.SetHP(Mathf.RoundToInt(averageHP));
                    }
                    Debug.Log($"[레이즈 {result}] 체력 평균화");
                    break;

                case RaiseResult.FullHealStun:
                    // 기획서: PC HP 100% 회복 + 1턴 행동 불능(스턴)
                    if (playerHealth != null)
                    {
                        playerHealth.FullHeal();
                    }
                    // TODO: 스턴 효과는 TurnManager나 별도 시스템에서 처리
                    Debug.Log($"[레이즈 {result}] 완전 회복 + 1턴 스턴");
                    break;

                case RaiseResult.FullDrawRecover:
                    // 기획서: 손패 풀 드로우 + AP 완전 회복
                    playerAP?.FullRestoreAP();
                    deckManager?.FullDraw();
                    Debug.Log($"[레이즈 {result}] 풀 드로우 + AP 완전 회복");
                    break;
            }
        }

        public bool IsRaiseAvailable() => isRaiseAvailable;
    }
}

