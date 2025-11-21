using CardBullet.Core.Card;
using CardBullet.Core.Deck;
using Card = CardBullet.Core.Card.Card;
using UnityEngine;

namespace CardBullet.Core.Battle
{
    /// <summary>
    /// 전투 관리 메인 시스템
    /// 기획서 2.1, 2.2 참조
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        [Header("시스템 참조")]
        [SerializeField] private ResourceManager playerResources;
        [SerializeField] private ResourceManager enemyResources;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private RaiseSystem raiseSystem;

        [Header("전투 설정")]
        [SerializeField] private int playerMaxHP = 100;
        [SerializeField] private int enemyMaxHP = 100;

        private int playerCurrentHP;
        private int enemyCurrentHP;

        // 이벤트
        public System.Action<int> OnPlayerHPChanged;
        public System.Action<int> OnEnemyHPChanged;
        public System.Action<bool> OnBattleEnded;

        private void Awake()
        {
            InitializeBattle();
        }

        /// <summary>
        /// 전투 초기화
        /// </summary>
        private void InitializeBattle()
        {
            playerCurrentHP = playerMaxHP;
            enemyCurrentHP = enemyMaxHP;

            playerResources.Initialize();
            enemyResources.Initialize();

            // 이벤트 구독
            turnManager.OnPlayerTurnStart += OnPlayerTurnStarted;
            turnManager.OnEnemyTurnStart += OnEnemyTurnStarted;
        }

        /// <summary>
        /// 카드 사용 시도
        /// </summary>
        public bool TryPlayCard(CardBullet.Core.Card.Card card)
        {
            if (!turnManager.IsPlayerTurn())
            {
                Debug.LogWarning("플레이어 턴이 아닙니다!");
                return false;
            }

            if (!deckManager.GetCurrentHand().Contains(card))
            {
                Debug.LogWarning("손패에 없는 카드입니다!");
                return false;
            }

            if (!card.CanUse(playerResources.GetCurrentAP()))
            {
                Debug.LogWarning($"AP가 부족합니다! (필요: {card.apCost}, 현재: {playerResources.GetCurrentAP()})");
                return false;
            }

            // 자원 소모
            playerResources.SpendAP(card.GetFinalAPCost());
            playerResources.AddTC(card.GetFinalTCCost());

            // 카드 효과 적용
            ApplyCardEffect(card);

            // 카드 사용 처리
            deckManager.PlayCard(card);

            // 레이즈 가능 여부 체크
            float hpRatio = (float)playerCurrentHP / playerMaxHP;
            raiseSystem.CheckRaiseAvailability(playerCurrentHP, playerMaxHP);

            // 턴 전환 체크
            turnManager.CheckTurnTransition();

            return true;
        }

        /// <summary>
        /// 카드 효과 적용
        /// </summary>
        private void ApplyCardEffect(CardBullet.Core.Card.Card card)
        {
            int damage = CalculateDamage(card);
            
            // 기획서: 무늬에 따른 효과
            switch (card.theme)
            {
                case CardTheme.Spade:    // 공격
                    DealDamageToEnemy(damage);
                    break;
                case CardTheme.Clover:   // 방어
                    // TODO: 방어 효과
                    break;
                case CardTheme.Diamond:  // 자원
                    // TODO: 자원 효과
                    break;
                case CardTheme.Heart:    // 회복
                    HealPlayer(damage);
                    break;
            }

            // TODO: 족보 효과, 유물 효과 적용
        }

        /// <summary>
        /// 데미지 계산
        /// </summary>
        private int CalculateDamage(CardBullet.Core.Card.Card card)
        {
            int baseDamage = card.power;

            // 카드 타입별 보정
            switch (card.type)
            {
                case CardType.Sprint:
                    // 스프린트는 낮은 데미지
                    baseDamage = Mathf.Max(1, baseDamage / 2);
                    break;
                case CardType.Fatal:
                    // 페이탈은 높은 데미지
                    baseDamage = baseDamage * 2;
                    break;
            }

            // TODO: 족보 보너스, 유물 보너스 적용

            return baseDamage;
        }

        /// <summary>
        /// 적에게 데미지
        /// </summary>
        private void DealDamageToEnemy(int damage)
        {
            enemyCurrentHP = Mathf.Max(0, enemyCurrentHP - damage);
            OnEnemyHPChanged?.Invoke(enemyCurrentHP);

            if (enemyCurrentHP <= 0)
            {
                EndBattle(true);
            }
        }

        /// <summary>
        /// 플레이어 회복
        /// </summary>
        private void HealPlayer(int amount)
        {
            playerCurrentHP = Mathf.Min(playerMaxHP, playerCurrentHP + amount);
            OnPlayerHPChanged?.Invoke(playerCurrentHP);
        }

        /// <summary>
        /// 플레이어 데미지
        /// </summary>
        public void DealDamageToPlayer(int damage)
        {
            playerCurrentHP = Mathf.Max(0, playerCurrentHP - damage);
            OnPlayerHPChanged?.Invoke(playerCurrentHP);

            // 레이즈 가능 여부 체크
            raiseSystem.CheckRaiseAvailability(playerCurrentHP, playerMaxHP);

            if (playerCurrentHP <= 0)
            {
                EndBattle(false);
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

        public int GetPlayerHP() => playerCurrentHP;
        public int GetPlayerMaxHP() => playerMaxHP;
        public int GetEnemyHP() => enemyCurrentHP;
        public int GetEnemyMaxHP() => enemyMaxHP;
    }
}

