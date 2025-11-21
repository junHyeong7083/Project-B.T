using System.Collections.Generic;
using CardBullet.Core.Card;
using CardBullet.Core.Deck;
using UnityEngine;

namespace CardBullet.Core.Hand
{
    /// <summary>
    /// 손패 관리 및 족보 패턴 인식
    /// 기획서 1.1 참조: 포커 및 화투 족보 시스템을 패시브 룰로 재해석
    /// </summary>
    public class HandManager : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private DeckManager deckManager;

        private List<CardBullet.Core.Card.Card> currentHand = new List<CardBullet.Core.Card.Card>();

        // 이벤트
        public System.Action<List<PokerPattern>> OnAvailablePatternsChanged;

        public enum PokerPattern
        {
            None,
            // 기본 패턴
            OnePair,        // 원페어
            TwoPair,        // 투페어
            ThreeOfAKind,   // 트리플
            Straight,       // 스트레이트
            Flush,          // 플러시
            FullHouse,      // 풀하우스
            FourOfAKind,    // 포카드
            StraightFlush,  // 스트레이트 플러시
            RoyalFlush      // 로얄 플러시
        }

        private void OnEnable()
        {
            if (deckManager != null)
            {
                deckManager.OnHandChanged += UpdateHand;
            }
        }

        private void OnDisable()
        {
            if (deckManager != null)
            {
                deckManager.OnHandChanged -= UpdateHand;
            }
        }

        /// <summary>
        /// 손패 업데이트 및 족보 체크
        /// </summary>
        private void UpdateHand()
        {
            currentHand = deckManager.GetCurrentHand();
            List<PokerPattern> availablePatterns = CheckAvailablePatterns();
            OnAvailablePatternsChanged?.Invoke(availablePatterns);
        }

        /// <summary>
        /// 현재 손패에서 가능한 족보 패턴 검사
        /// </summary>
        public List<PokerPattern> CheckAvailablePatterns()
        {
            if (currentHand == null || currentHand.Count == 0)
                return new List<PokerPattern>();

            // PokerPatternDetector를 사용하여 패턴 감지
            return PokerPatternDetector.DetectAllPatterns(currentHand);
        }

        /// <summary>
        /// 족보 패턴별 데미지 배율 가져오기
        /// </summary>
        public float GetPatternDamageMultiplier(PokerPattern pattern)
        {
            return PokerPatternDetector.GetPatternDamageMultiplier(pattern);
        }

        /// <summary>
        /// 특정 족보 패턴 발동 가능 여부
        /// </summary>
        public bool CanActivatePattern(PokerPattern pattern)
        {
            List<PokerPattern> available = CheckAvailablePatterns();
            return available.Contains(pattern);
        }
    }
}

