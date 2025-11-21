using System.Collections.Generic;
using CardBullet.Core.Card;
using CardBullet.Core.Deck;
using Card = CardBullet.Core.Card.Card;
using UnityEngine;

namespace CardBullet.Core.Maintenance
{
    /// <summary>
    /// 운명의 수레바퀴 페이즈 (정비 페이즈)
    /// 기획서 5.1 참조: 전투 종료 후 덱 압축, 낙인, 상점 이용
    /// </summary>
    public class MaintenancePhase : MonoBehaviour
    {
        [Header("참조")]
        [SerializeField] private DeckManager deckManager;

        [Header("정비 설정")]
        [SerializeField] private int maxRemovalPerPhase = 10; // 1회 정비 당 최대 10장까지 제거
        [SerializeField] private int minDeckSize = 15;        // 최소 덱 용량 15장

        private int currentRemovalCount = 0;

        // 이벤트
        public System.Action OnMaintenancePhaseStart;
        public System.Action OnMaintenancePhaseEnd;
        public System.Action<int> OnCardRemoved;
        public System.Action OnCardPackOpened;

        /// <summary>
        /// 정비 페이즈 시작
        /// </summary>
        public void StartMaintenancePhase()
        {
            currentRemovalCount = 0;
            OnMaintenancePhaseStart?.Invoke();
            Debug.Log("[운명의 수레바퀴] 정비 페이즈 시작");
        }

        /// <summary>
        /// 정비 페이즈 종료
        /// </summary>
        public void EndMaintenancePhase()
        {
            OnMaintenancePhaseEnd?.Invoke();
            Debug.Log("[운명의 수레바퀴] 정비 페이즈 종료");
        }

        #region 덱 관리

        /// <summary>
        /// 카드 뭉치 획득 (10장 단위)
        /// 기획서: 1회 획득 시 10장의 카드 뭉치 단위로 제시
        /// </summary>
        public void AcquireCardPack(List<CardBullet.Core.Card.Card> cardPack)
        {
            if (cardPack == null || cardPack.Count == 0)
            {
                Debug.LogWarning("카드 뭉치가 비어있습니다!");
                return;
            }

            // TODO: 카드 뭉치를 플레이어에게 제시하고 선택하도록
            // 현재는 바로 덱에 추가
            foreach (CardBullet.Core.Card.Card card in cardPack)
            {
                deckManager.AddCardToDeck(card);
            }

            OnCardPackOpened?.Invoke();
            Debug.Log($"카드 뭉치 획득: {cardPack.Count}장");
        }

        /// <summary>
        /// 카드 제거 (덱 압축)
        /// 기획서: 1회 정비 당 최대 10장까지 제거 가능
        /// </summary>
        public bool RemoveCardFromDeck(CardBullet.Core.Card.Card card)
        {
            if (currentRemovalCount >= maxRemovalPerPhase)
            {
                Debug.LogWarning($"이번 정비에서 이미 {maxRemovalPerPhase}장 제거했습니다!");
                return false;
            }

            if (deckManager.GetDeckSize() <= minDeckSize)
            {
                Debug.LogWarning($"최소 덱 용량({minDeckSize}장) 이하로 줄일 수 없습니다!");
                return false;
            }

            if (deckManager.RemoveCardFromDeck(card))
            {
                currentRemovalCount++;
                OnCardRemoved?.Invoke(currentRemovalCount);
                Debug.Log($"카드 제거: {card.power} {card.theme} ({currentRemovalCount}/{maxRemovalPerPhase})");
                return true;
            }

            return false;
        }

        public int GetRemainingRemovals() => maxRemovalPerPhase - currentRemovalCount;

        #endregion

        #region 낙인 시스템

        /// <summary>
        /// 카드에 낙인 부여 (접두어/접미어)
        /// 기획서 5.2 참조
        /// </summary>
        public bool ApplyStigma(CardBullet.Core.Card.Card card, string markID, bool isPrefix)
        {
            if (card == null)
            {
                Debug.LogWarning("유효하지 않은 카드입니다!");
                return false;
            }

            if (isPrefix)
            {
                card.prefixMarkID = markID;
                Debug.Log($"접두사 낙인 부여: {markID}");
            }
            else
            {
                card.suffixMarkID = markID;
                Debug.Log($"접미사 낙인 부여: {markID}");
            }

            return true;
        }

        #endregion

        #region 상점

        // TODO: 상점 시스템 구현
        // - 유물 구매/판매
        // - 카드 구매
        // - 낙인 구매

        #endregion
    }
}

