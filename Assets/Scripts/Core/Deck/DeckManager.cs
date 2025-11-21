using System.Collections.Generic;
using System.Linq;
using CardBullet.Core.Card;
using UnityEngine;

namespace CardBullet.Core.Deck
{
    /// <summary>
    /// 덱 관리 시스템
    /// 기획서 5.1 참조: 최소 덱 용량 15장
    /// </summary>
    public class DeckManager : MonoBehaviour
    {
        [Header("덱 설정")]
        [SerializeField] private int minDeckSize = 15;
        [SerializeField] private int startingHandSize = 5;
        [SerializeField] private int maxHandSize = 10;

        // 덱 구성
        private List<CardBullet.Core.Card.Card> mainDeck = new List<CardBullet.Core.Card.Card>();      // 메인 덱
        private List<CardBullet.Core.Card.Card> discardPile = new List<CardBullet.Core.Card.Card>();   // 버린 카드
        private List<CardBullet.Core.Card.Card> currentHand = new List<CardBullet.Core.Card.Card>();   // 현재 손패
        private List<CardBullet.Core.Card.Card> exhaustedCards = new List<CardBullet.Core.Card.Card>(); // 소진된 카드 (턴 종료까지 사용 불가)

        // 이벤트
        public System.Action<CardBullet.Core.Card.Card> OnCardDrawn;
        public System.Action<CardBullet.Core.Card.Card> OnCardPlayed;
        public System.Action OnDeckShuffled;
        public System.Action OnHandChanged;

        public List<CardBullet.Core.Card.Card> GetCurrentHand() => currentHand;
        public int GetDeckSize() => mainDeck.Count;
        public int GetHandSize() => currentHand.Count;
        public int GetDiscardPileSize() => discardPile.Count;

        /// <summary>
        /// 덱 초기화
        /// </summary>
        public void InitializeDeck(List<CardBullet.Core.Card.Card> initialCards)
        {
            mainDeck = new List<CardBullet.Core.Card.Card>(initialCards);
            discardPile.Clear();
            currentHand.Clear();
            exhaustedCards.Clear();
            ShuffleDeck();
            DrawStartingHand();
        }

        /// <summary>
        /// 시작 핸드 드로우
        /// </summary>
        public void DrawStartingHand()
        {
            currentHand.Clear();
            DrawCards(startingHandSize);
        }

        /// <summary>
        /// 카드 드로우
        /// </summary>
        public CardBullet.Core.Card.Card DrawCard()
        {
            if (mainDeck.Count == 0)
            {
                ReshuffleDiscardPile();
            }

            if (mainDeck.Count == 0)
            {
                Debug.LogWarning("드로우할 카드가 없습니다!");
                return null;
            }

            if (currentHand.Count >= maxHandSize)
            {
                Debug.LogWarning($"손패가 가득 찼습니다! (최대 {maxHandSize}장)");
                return null;
            }

            CardBullet.Core.Card.Card drawnCard = mainDeck[0];
            mainDeck.RemoveAt(0);
            currentHand.Add(drawnCard);
            OnCardDrawn?.Invoke(drawnCard);
            OnHandChanged?.Invoke();
            return drawnCard;
        }

        /// <summary>
        /// 여러 장 드로우
        /// </summary>
        public void DrawCards(int count)
        {
            for (int i = 0; i < count; i++)
            {
                DrawCard();
            }
        }

        /// <summary>
        /// 카드 사용 (손패에서 제거하여 버림 더미로)
        /// </summary>
        public bool PlayCard(CardBullet.Core.Card.Card card)
        {
            if (!currentHand.Contains(card))
            {
                Debug.LogWarning("손패에 없는 카드입니다!");
                return false;
            }

            currentHand.Remove(card);
            discardPile.Add(card);
            OnCardPlayed?.Invoke(card);
            OnHandChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// 카드 버리기 (손패에서 버림 더미로)
        /// </summary>
        public void DiscardCard(CardBullet.Core.Card.Card card)
        {
            if (currentHand.Remove(card))
            {
                discardPile.Add(card);
                OnHandChanged?.Invoke();
            }
        }

        /// <summary>
        /// 손패 전부 버리기
        /// </summary>
        public void DiscardHand()
        {
            discardPile.AddRange(currentHand);
            currentHand.Clear();
            OnHandChanged?.Invoke();
        }

        /// <summary>
        /// 덱 셔플
        /// </summary>
        public void ShuffleDeck()
        {
            mainDeck = mainDeck.OrderBy(x => Random.Range(0f, 1f)).ToList();
            OnDeckShuffled?.Invoke();
        }

        /// <summary>
        /// 버림 더미를 덱으로 재셔플
        /// </summary>
        private void ReshuffleDiscardPile()
        {
            if (discardPile.Count == 0)
                return;

            mainDeck.AddRange(discardPile);
            discardPile.Clear();
            ShuffleDeck();
        }

        /// <summary>
        /// 덱에 카드 추가
        /// </summary>
        public void AddCardToDeck(CardBullet.Core.Card.Card card)
        {
            mainDeck.Add(card);
        }

        /// <summary>
        /// 덱에서 카드 제거 (정비 페이즈)
        /// </summary>
        public bool RemoveCardFromDeck(CardBullet.Core.Card.Card card)
        {
            if (mainDeck.Count <= minDeckSize)
            {
                Debug.LogWarning($"최소 덱 용량({minDeckSize}장) 이하로 줄일 수 없습니다!");
                return false;
            }

            return mainDeck.Remove(card);
        }

        /// <summary>
        /// 풀 드로우 (레이즈 효과)
        /// </summary>
        public void FullDraw()
        {
            int cardsToDraw = maxHandSize - currentHand.Count;
            DrawCards(cardsToDraw);
        }
    }
}

