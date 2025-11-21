using CardBullet.Core.Card;
using CardBullet.Core.Deck;
using CardBullet.Core.Hand;
using Card = CardBullet.Core.Card.Card;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 전투 관리 오케스트레이터
    /// SRP: 전투 흐름 오케스트레이션만 담당
    /// 기획서 2.1, 2.2 참조
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("시스템 참조")]
        [SerializeField] private APManager playerAP;
        [SerializeField] private TCManager playerTC;
        [SerializeField] private APManager enemyAP;
        [SerializeField] private TCManager enemyTC;
        [SerializeField] private HealthManager playerHealth;
        [SerializeField] private HealthManager enemyHealth;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private CardEffectProcessor cardEffectProcessor;
        [SerializeField] private RaiseSystem raiseSystem;

        // 이벤트
        public System.Action<bool> OnBattleEnded;

        private CardBullet.Core.Card.Card lastPlayedCard;

        private void Awake()
        {
            InitializeBattle();
        }

        /// <summary>
        /// 전투 초기화
        /// </summary>
        private void InitializeBattle()
        {
            playerAP?.Initialize();
            playerTC?.Initialize();
            enemyAP?.Initialize();
            enemyTC?.Initialize();
            playerHealth?.Initialize();
            enemyHealth?.Initialize();

            // 이벤트 구독
            turnManager.OnPlayerTurnStart += OnPlayerTurnStarted;
            turnManager.OnEnemyTurnStart += OnEnemyTurnStarted;
            
            if (playerHealth != null)
            {
                playerHealth.OnHPDepleted += () => EndBattle(false);
            }
            
            if (enemyHealth != null)
            {
                enemyHealth.OnHPDepleted += () => EndBattle(true);
            }
        }

        /// <summary>
        /// 카드 사용 시도
        /// SRP: 오케스트레이션만 담당 (실제 로직은 각 매니저에 위임)
        /// </summary>
        public bool TryPlayCard(CardBullet.Core.Card.Card card)
        {
            // 유효성 검증
            if (!ValidateCardPlay(card))
            {
                return false;
            }

            // 자원 소모
            if (playerAP != null && !playerAP.SpendAP(card.GetFinalAPCost()))
            {
                Debug.LogWarning($"AP가 부족합니다! (필요: {card.apCost}, 현재: {playerAP.GetCurrentAP()})");
                return false;
            }

            if (playerTC != null)
            {
                playerTC.AddTC(card.GetFinalTCCost());
            }

            // 카드 효과 처리
            if (cardEffectProcessor != null)
            {
                var effectResult = cardEffectProcessor.ProcessCardEffect(card, lastPlayedCard);
                ApplyCardEffectResult(effectResult);
            }

            // 카드 사용 처리
            deckManager?.PlayCard(card);
            lastPlayedCard = card;

            // 레이즈 가능 여부 체크
            if (raiseSystem != null)
            {
                raiseSystem.CheckRaiseAvailability();
            }

            // 턴 전환 체크
            turnManager?.CheckTurnTransition();

            return true;
        }

        /// <summary>
        /// 카드 사용 유효성 검증
        /// </summary>
        private bool ValidateCardPlay(CardBullet.Core.Card.Card card)
        {
            if (!turnManager.IsPlayerTurn())
            {
                Debug.LogWarning("플레이어 턴이 아닙니다!");
                return false;
            }

            if (deckManager != null && !deckManager.GetCurrentHand().Contains(card))
            {
                Debug.LogWarning("손패에 없는 카드입니다!");
                return false;
            }

            if (playerAP != null && !card.CanUse(playerAP.GetCurrentAP()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 카드 효과 결과 적용
        /// </summary>
        private void ApplyCardEffectResult(CardEffectResult result)
        {
            var effectType = cardEffectProcessor.GetEffectType(result.card);
            
            switch (effectType)
            {
                case CardEffectType.Attack:
                    if (enemyHealth != null)
                    {
                        enemyHealth.TakeDamage(result.finalDamage);
                    }
                    break;
                case CardEffectType.Heal:
                    if (playerHealth != null)
                    {
                        playerHealth.Heal(result.finalDamage);
                    }
                    break;
                case CardEffectType.Defense:
                    // TODO: 방어 효과 구현
                    break;
                case CardEffectType.Resource:
                    // TODO: 자원 효과 구현
                    break;
            }
        }

        /// <summary>
        /// 플레이어 데미지 (외부에서 호출, 예: 적 공격)
        /// </summary>
        public void DealDamageToPlayer(int damage)
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 레이즈 가능 여부 체크
            if (raiseSystem != null)
            {
                raiseSystem.CheckRaiseAvailability();
            }
        }

        /// <summary>
        /// 전투 종료
        /// </summary>
        private void EndBattle(bool playerWon)
        {
            OnBattleEnded?.Invoke(playerWon);
            Debug.Log($"전투 종료: {(playerWon ? "승리" : "패배")}");
        }

        private void OnPlayerTurnStarted()
        {
            Debug.Log("플레이어 턴 시작");
        }

        private void OnEnemyTurnStarted()
        {
            Debug.Log("적 턴 시작");
            // TODO: 적 AI 행동 처리
        }

        // Getter 메서드들 (기존 호환성 유지)
        public int GetPlayerHP() => playerHealth?.GetCurrentHP() ?? 0;
        public int GetPlayerMaxHP() => playerHealth?.GetMaxHP() ?? 100;
        public int GetEnemyHP() => enemyHealth?.GetCurrentHP() ?? 0;
        public int GetEnemyMaxHP() => enemyHealth?.GetMaxHP() ?? 100;
    }
}

