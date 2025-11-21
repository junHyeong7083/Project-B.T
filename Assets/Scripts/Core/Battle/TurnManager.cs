using System;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 턴 관리 시스템
    /// 기획서 2.1, 2.2 참조: TC 기반 턴 결정
    /// </summary>
    public class TurnManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private ResourceManager playerResources;
        [SerializeField] private ResourceManager enemyResources;

        public enum TurnState
        {
            PlayerTurn,
            EnemyTurn,
            Processing
        }

        private TurnState currentTurn = TurnState.PlayerTurn;

        // 이벤트
        public event Action<TurnState> OnTurnChanged;
        public event Action OnPlayerTurnStart;
        public event Action OnEnemyTurnStart;

        public TurnState GetCurrentTurn() => currentTurn;
        public bool IsPlayerTurn() => currentTurn == TurnState.PlayerTurn;

        /// <summary>
        /// 카드 사용 후 TC 비교하여 턴 결정
        /// </summary>
        public void CheckTurnTransition()
        {
            if (currentTurn == TurnState.Processing)
                return;

            int playerTC = playerResources.GetCurrentTC();
            int enemyTC = enemyResources.GetCurrentTC();

            // 기획서 2.2: PC_TC > NPC_TC 이면 적 턴으로 전환
            if (currentTurn == TurnState.PlayerTurn && playerTC > enemyTC)
            {
                TransitionToEnemyTurn();
            }
            else if (currentTurn == TurnState.EnemyTurn && enemyTC > playerTC)
            {
                TransitionToPlayerTurn();
            }
        }

        /// <summary>
        /// 플레이어 턴으로 전환
        /// </summary>
        private void TransitionToPlayerTurn()
        {
            currentTurn = TurnState.PlayerTurn;
            playerResources.ResetTC();
            playerResources.RestoreAP();
            OnTurnChanged?.Invoke(currentTurn);
            OnPlayerTurnStart?.Invoke();
        }

        /// <summary>
        /// 적 턴으로 전환
        /// </summary>
        private void TransitionToEnemyTurn()
        {
            currentTurn = TurnState.EnemyTurn;
            enemyResources.ResetTC();
            OnTurnChanged?.Invoke(currentTurn);
            OnEnemyTurnStart?.Invoke();
        }

        /// <summary>
        /// 수동으로 턴 종료 (레이즈 등)
        /// </summary>
        public void ForceEndTurn()
        {
            if (currentTurn == TurnState.PlayerTurn)
            {
                TransitionToEnemyTurn();
            }
            else
            {
                TransitionToPlayerTurn();
            }
        }
    }
}

