using System;
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
        [SerializeField] private ResourceManager playerResources;
        
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
        /// </summary>
        public bool CheckRaiseAvailability(float currentHP, float maxHP)
        {
            bool hpCondition = (currentHP / maxHP) < hpThreshold;
            bool apCondition = playerResources.GetCurrentAP() == 0;

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
        public RaiseResult ExecuteRaise(float playerHP, float enemyHP, float playerMaxHP)
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
            ApplyRaiseResult(result, playerHP, enemyHP, playerMaxHP);

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
        /// </summary>
        private void ApplyRaiseResult(RaiseResult result, float playerHP, float enemyHP, float playerMaxHP)
        {
            switch (result)
            {
                case RaiseResult.AverageHP:
                    // 기획서: (PC_HP + NPC_HP) / 2 값으로 쌍방 체력 동기화
                    float averageHP = (playerHP + enemyHP) / 2f;
                    // TODO: 실제 체력 적용은 BattleManager에서 처리
                    Debug.Log($"[레이즈 {result}] 체력 평균화: {averageHP}");
                    break;

                case RaiseResult.FullHealStun:
                    // 기획서: PC HP 100% 회복 + 1턴 행동 불능(스턴)
                    Debug.Log($"[레이즈 {result}] 완전 회복 + 1턴 스턴");
                    // TODO: 실제 체력 회복 및 스턴 적용은 BattleManager에서 처리
                    break;

                case RaiseResult.FullDrawRecover:
                    // 기획서: 손패 풀 드로우 + AP 완전 회복
                    playerResources.FullRestoreAP();
                    Debug.Log($"[레이즈 {result}] 풀 드로우 + AP 완전 회복");
                    // TODO: 풀 드로우는 DeckManager에서 처리
                    break;
            }
        }

        public bool IsRaiseAvailable() => isRaiseAvailable;
    }
}

