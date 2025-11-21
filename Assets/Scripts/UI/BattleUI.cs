using CardBullet.Core.Battle;
using CardBullet.Core.Card;
using CardBullet.Core.Deck;
using CardBullet.Core.Hand;
using Card = CardBullet.Core.Card.Card;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CardBullet.UI
{
    /// <summary>
    /// 전투 UI 관리
    /// 기획서 6.1 참조: TC 타임라인, AP/TC 미리보기, 족보 알림
    /// </summary>
    public class BattleUI : MonoBehaviour
    {
        [Header("시스템 참조")]
        [SerializeField] private BattleManager battleManager;
        [SerializeField] private APManager playerAP;
        [SerializeField] private TCManager playerTC;
        [SerializeField] private TCManager enemyTC;
        [SerializeField] private HealthManager playerHealth;
        [SerializeField] private HealthManager enemyHealth;
        [SerializeField] private TurnManager turnManager;
        [SerializeField] private DeckManager deckManager;
        [SerializeField] private HandManager handManager;
        [SerializeField] private RaiseSystem raiseSystem;

        [Header("AP/TC 표시")]
        [SerializeField] private TextMeshProUGUI apText;
        [SerializeField] private TextMeshProUGUI playerTCText;
        [SerializeField] private TextMeshProUGUI enemyTCText;

        [Header("TC 타임라인 바")]
        [SerializeField] private Slider tcTimelineBar;
        [SerializeField] private RectTransform playerTCMarker;
        [SerializeField] private RectTransform enemyTCMarker;

        [Header("HP 표시")]
        [SerializeField] private Slider playerHPBar;
        [SerializeField] private Slider enemyHPBar;
        [SerializeField] private TextMeshProUGUI playerHPText;
        [SerializeField] private TextMeshProUGUI enemyHPText;

        [Header("레이즈 버튼")]
        [SerializeField] private Button raiseButton;
        [SerializeField] private GameObject raiseButtonObject;

        [Header("족보 알림")]
        [SerializeField] private GameObject patternNotification;
        [SerializeField] private TextMeshProUGUI patternText;

        [Header("카드 미리보기")]
        [SerializeField] private GameObject cardPreviewPanel;
        [SerializeField] private TextMeshProUGUI cardPreviewAP;
        [SerializeField] private TextMeshProUGUI cardPreviewTC;

        private CardBullet.Core.Card.Card hoveredCard;

        private void OnEnable()
        {
            // 이벤트 구독
            if (playerAP != null)
            {
                playerAP.OnAPChanged += UpdateAPDisplay;
            }

            if (playerTC != null)
            {
                playerTC.OnTCChanged += _ => UpdateTCTimeline(0);
            }

            if (enemyTC != null)
            {
                enemyTC.OnTCChanged += _ => UpdateTCTimeline(0);
            }

            if (playerHealth != null)
            {
                playerHealth.OnHPChanged += UpdatePlayerHP;
            }

            if (enemyHealth != null)
            {
                enemyHealth.OnHPChanged += UpdateEnemyHP;
            }

            if (raiseSystem != null)
            {
                raiseSystem.OnRaiseAvailabilityChanged += UpdateRaiseButton;
            }

            if (handManager != null)
            {
                handManager.OnAvailablePatternsChanged += UpdatePatternNotification;
            }

            if (raiseButton != null)
            {
                raiseButton.onClick.AddListener(OnRaiseButtonClicked);
            }
        }

        private void OnDisable()
        {
            // 이벤트 해제
            if (playerAP != null)
            {
                playerAP.OnAPChanged -= UpdateAPDisplay;
            }

            if (playerTC != null)
            {
                playerTC.OnTCChanged -= _ => UpdateTCTimeline(0);
            }

            if (enemyTC != null)
            {
                enemyTC.OnTCChanged -= _ => UpdateTCTimeline(0);
            }

            if (playerHealth != null)
            {
                playerHealth.OnHPChanged -= UpdatePlayerHP;
            }

            if (enemyHealth != null)
            {
                enemyHealth.OnHPChanged -= UpdateEnemyHP;
            }

            if (raiseSystem != null)
            {
                raiseSystem.OnRaiseAvailabilityChanged -= UpdateRaiseButton;
            }

            if (handManager != null)
            {
                handManager.OnAvailablePatternsChanged -= UpdatePatternNotification;
            }

            if (raiseButton != null)
            {
                raiseButton.onClick.RemoveListener(OnRaiseButtonClicked);
            }
        }

        private void Start()
        {
            InitializeUI();
        }

        /// <summary>
        /// UI 초기화
        /// </summary>
        private void InitializeUI()
        {
            UpdateAPDisplay(playerAP != null ? playerAP.GetCurrentAP() : 0);
            UpdateTCTimeline(0);
            UpdatePlayerHP(playerHealth != null ? playerHealth.GetCurrentHP() : 100);
            UpdateEnemyHP(enemyHealth != null ? enemyHealth.GetCurrentHP() : 100);
            UpdateRaiseButton(false);

            if (patternNotification != null)
                patternNotification.SetActive(false);

            if (cardPreviewPanel != null)
                cardPreviewPanel.SetActive(false);
        }

        #region AP/TC 표시 업데이트

        /// <summary>
        /// AP 표시 업데이트
        /// </summary>
        private void UpdateAPDisplay(int currentAP)
        {
            if (apText != null)
            {
                int maxAP = playerAP != null ? playerAP.GetMaxAP() : 3;
                apText.text = $"AP: {currentAP}/{maxAP}";
            }
        }

        /// <summary>
        /// TC 타임라인 업데이트
        /// 기획서 6.1: 플레이어와 적의 TC 상태를 바 형태로 시각화
        /// </summary>
        private void UpdateTCTimeline(int tc)
        {
            if (playerTC == null || enemyTC == null)
                return;

            int playerTCValue = playerTC.GetCurrentTC();
            int enemyTCValue = enemyTC.GetCurrentTC();

            // TC 텍스트 업데이트
            if (playerTCText != null)
                playerTCText.text = $"플레이어 TC: {playerTCValue}";

            if (enemyTCText != null)
                enemyTCText.text = $"적 TC: {enemyTCValue}";

            // TC 타임라인 바 업데이트
            if (tcTimelineBar != null)
            {
                int maxTC = Mathf.Max(playerTCValue, enemyTCValue, 100); // 최소 100으로 설정
                float normalizedPlayerTC = (float)playerTCValue / maxTC;
                float normalizedEnemyTC = (float)enemyTCValue / maxTC;

                // 타임라인 바 전체 길이
                float barWidth = tcTimelineBar.GetComponent<RectTransform>().rect.width;

                // 마커 위치 업데이트
                if (playerTCMarker != null)
                {
                    float playerX = (normalizedPlayerTC - 0.5f) * barWidth;
                    playerTCMarker.anchoredPosition = new Vector2(playerX, playerTCMarker.anchoredPosition.y);
                }

                if (enemyTCMarker != null)
                {
                    float enemyX = (normalizedEnemyTC - 0.5f) * barWidth;
                    enemyTCMarker.anchoredPosition = new Vector2(enemyX, enemyTCMarker.anchoredPosition.y);
                }
            }
        }

        #endregion

        #region HP 표시 업데이트

        /// <summary>
        /// 플레이어 HP 업데이트
        /// </summary>
        private void UpdatePlayerHP(int currentHP)
        {
            if (playerHealth == null)
                return;

            int maxHP = playerHealth.GetMaxHP();
            float hpRatio = (float)currentHP / maxHP;

            if (playerHPBar != null)
                playerHPBar.value = hpRatio;

            if (playerHPText != null)
                playerHPText.text = $"{currentHP}/{maxHP}";

            // 레이즈 가능 여부 체크
            if (raiseSystem != null)
            {
                raiseSystem.CheckRaiseAvailability();
            }
        }

        /// <summary>
        /// 적 HP 업데이트
        /// </summary>
        private void UpdateEnemyHP(int currentHP)
        {
            if (enemyHealth == null)
                return;

            int maxHP = enemyHealth.GetMaxHP();
            float hpRatio = (float)currentHP / maxHP;

            if (enemyHPBar != null)
                enemyHPBar.value = hpRatio;

            if (enemyHPText != null)
                enemyHPText.text = $"{currentHP}/{maxHP}";
        }

        #endregion

        #region 레이즈 버튼

        /// <summary>
        /// 레이즈 버튼 활성화/비활성화
        /// </summary>
        private void UpdateRaiseButton(bool available)
        {
            if (raiseButtonObject != null)
                raiseButtonObject.SetActive(available);
        }

        /// <summary>
        /// 레이즈 버튼 클릭
        /// </summary>
        private void OnRaiseButtonClicked()
        {
            if (raiseSystem == null)
                return;

            var result = raiseSystem.ExecuteRaise();
            Debug.Log($"레이즈 발동! 결과: {result}");

            // 턴 강제 종료
            if (turnManager != null)
            {
                turnManager.ForceEndTurn();
            }
        }

        #endregion

        #region 족보 알림

        /// <summary>
        /// 족보 알림 업데이트
        /// 기획서 6.1: 완성 가능한 족보를 하이라이트로 표시
        /// </summary>
        private void UpdatePatternNotification(System.Collections.Generic.List<HandManager.PokerPattern> patterns)
        {
            if (patternNotification == null || patternText == null)
                return;

            if (patterns == null || patterns.Count == 0)
            {
                patternNotification.SetActive(false);
                return;
            }

            // 가장 높은 우선순위 패턴만 표시
            HandManager.PokerPattern topPattern = patterns[0];
            string patternName = GetPatternName(topPattern);

            patternText.text = $"족보: {patternName}";
            patternNotification.SetActive(true);
        }

        /// <summary>
        /// 족보 이름 가져오기
        /// </summary>
        private string GetPatternName(HandManager.PokerPattern pattern)
        {
            switch (pattern)
            {
                case HandManager.PokerPattern.RoyalFlush:
                    return "로얄 플러시";
                case HandManager.PokerPattern.StraightFlush:
                    return "스트레이트 플러시";
                case HandManager.PokerPattern.FourOfAKind:
                    return "포카드";
                case HandManager.PokerPattern.FullHouse:
                    return "풀하우스";
                case HandManager.PokerPattern.Flush:
                    return "플러시";
                case HandManager.PokerPattern.Straight:
                    return "스트레이트";
                case HandManager.PokerPattern.ThreeOfAKind:
                    return "트리플";
                case HandManager.PokerPattern.TwoPair:
                    return "투페어";
                case HandManager.PokerPattern.OnePair:
                    return "원페어";
                default:
                    return "";
            }
        }

        #endregion

        #region 카드 미리보기

        /// <summary>
        /// 카드 호버 시 미리보기 표시
        /// 기획서 6.1: 카드 드래그(Hover)할 때 소모될 AP와 누적될 TC를 미리 표기
        /// </summary>
        public void ShowCardPreview(CardBullet.Core.Card.Card card)
        {
            if (card == null || cardPreviewPanel == null)
                return;

            hoveredCard = card;

            if (cardPreviewAP != null)
                cardPreviewAP.text = $"AP: {card.apCost}";

            if (cardPreviewTC != null)
                cardPreviewTC.text = $"TC: {card.tcCost}";

            cardPreviewPanel.SetActive(true);
        }

        /// <summary>
        /// 카드 미리보기 숨기기
        /// </summary>
        public void HideCardPreview()
        {
            if (cardPreviewPanel != null)
                cardPreviewPanel.SetActive(false);

            hoveredCard = null;
        }

        #endregion
    }
}

