using System.Collections.Generic;
using System.Linq;
using CardBullet.Core.Card;
using UnityEngine;

namespace CardBullet.Core.Hand
{
    /// <summary>
    /// 족보 패턴 감지기
    /// 기획서 1.1 참조: 포커 및 화투 족보 시스템을 패시브 룰로 재해석
    /// </summary>
    public static class PokerPatternDetector
    {
        /// <summary>
        /// 손패에서 가능한 모든 족보 패턴 검사
        /// </summary>
        public static List<HandManager.PokerPattern> DetectAllPatterns(List<CardBullet.Core.Card.Card> hand)
        {
            List<HandManager.PokerPattern> patterns = new List<HandManager.PokerPattern>();

            if (hand == null || hand.Count < 2)
                return patterns;

            // 우선순위가 높은 패턴부터 체크
            var royalFlush = DetectRoyalFlush(hand);
            if (royalFlush != HandManager.PokerPattern.None)
            {
                patterns.Add(royalFlush);
                return patterns; // 최고 패턴이므로 중단
            }

            var straightFlush = DetectStraightFlush(hand);
            if (straightFlush != HandManager.PokerPattern.None)
            {
                patterns.Add(straightFlush);
                return patterns;
            }

            var fourOfAKind = DetectFourOfAKind(hand);
            if (fourOfAKind != HandManager.PokerPattern.None)
            {
                patterns.Add(fourOfAKind);
                return patterns;
            }

            var fullHouse = DetectFullHouse(hand);
            if (fullHouse != HandManager.PokerPattern.None)
            {
                patterns.Add(fullHouse);
                return patterns;
            }

            var flush = DetectFlush(hand);
            if (flush != HandManager.PokerPattern.None)
            {
                patterns.Add(flush);
            }

            var straight = DetectStraight(hand);
            if (straight != HandManager.PokerPattern.None)
            {
                patterns.Add(straight);
            }

            var threeOfAKind = DetectThreeOfAKind(hand);
            if (threeOfAKind != HandManager.PokerPattern.None)
            {
                patterns.Add(threeOfAKind);
                return patterns; // 트리플이면 페어는 무의미
            }

            var twoPair = DetectTwoPair(hand);
            if (twoPair != HandManager.PokerPattern.None)
            {
                patterns.Add(twoPair);
                return patterns;
            }

            var onePair = DetectOnePair(hand);
            if (onePair != HandManager.PokerPattern.None)
            {
                patterns.Add(onePair);
            }

            return patterns;
        }

        /// <summary>
        /// 로얄 플러시 감지 (같은 무늬 + A, K, Q, J, 10 연속)
        /// </summary>
        private static HandManager.PokerPattern DetectRoyalFlush(List<CardBullet.Core.Card.Card> hand)
        {
            // TODO: 구현
            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 스트레이트 플러시 감지 (같은 무늬 + 연속 숫자)
        /// </summary>
        private static HandManager.PokerPattern DetectStraightFlush(List<CardBullet.Core.Card.Card> hand)
        {
            if (hand.Count < 5)
                return HandManager.PokerPattern.None;

            // 무늬별로 그룹화
            var groupsByTheme = hand.GroupBy(c => c.theme);

            foreach (var group in groupsByTheme)
            {
                if (group.Count() >= 5)
                {
                    var sortedPowers = group.Select(c => c.power).OrderBy(p => p).ToList();
                    if (IsStraight(sortedPowers))
                    {
                        return HandManager.PokerPattern.StraightFlush;
                    }
                }
            }

            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 포카드 감지 (같은 숫자 4장)
        /// </summary>
        private static HandManager.PokerPattern DetectFourOfAKind(List<CardBullet.Core.Card.Card> hand)
        {
            var powerGroups = hand.GroupBy(c => c.power);
            if (powerGroups.Any(g => g.Count() >= 4))
            {
                return HandManager.PokerPattern.FourOfAKind;
            }
            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 풀하우스 감지 (트리플 + 페어)
        /// </summary>
        private static HandManager.PokerPattern DetectFullHouse(List<CardBullet.Core.Card.Card> hand)
        {
            if (hand.Count < 5)
                return HandManager.PokerPattern.None;

            var powerGroups = hand.GroupBy(c => c.power).OrderByDescending(g => g.Count()).ToList();

            bool hasThree = powerGroups.Any(g => g.Count() >= 3);
            bool hasPair = powerGroups.Any(g => g.Count() >= 2);

            if (hasThree && hasPair)
            {
                // 트리플과 페어가 다른 숫자인지 확인
                var threeOfKind = powerGroups.First(g => g.Count() >= 3);
                var pair = powerGroups.FirstOrDefault(g => g.Count() >= 2 && g.Key != threeOfKind.Key);

                if (pair != null)
                {
                    return HandManager.PokerPattern.FullHouse;
                }
            }

            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 플러시 감지 (같은 무늬 5장 이상)
        /// </summary>
        private static HandManager.PokerPattern DetectFlush(List<CardBullet.Core.Card.Card> hand)
        {
            if (hand.Count < 5)
                return HandManager.PokerPattern.None;

            var themeGroups = hand.GroupBy(c => c.theme);
            if (themeGroups.Any(g => g.Count() >= 5))
            {
                return HandManager.PokerPattern.Flush;
            }

            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 스트레이트 감지 (연속 숫자 5장 이상)
        /// </summary>
        private static HandManager.PokerPattern DetectStraight(List<CardBullet.Core.Card.Card> hand)
        {
            if (hand.Count < 5)
                return HandManager.PokerPattern.None;

            var uniquePowers = hand.Select(c => c.power).Distinct().OrderBy(p => p).ToList();

            if (IsStraight(uniquePowers))
            {
                return HandManager.PokerPattern.Straight;
            }

            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 스트레이트 여부 체크 (연속 숫자)
        /// </summary>
        private static bool IsStraight(List<int> sortedPowers)
        {
            if (sortedPowers.Count < 5)
                return false;

            for (int i = 0; i <= sortedPowers.Count - 5; i++)
            {
                bool isStraight = true;
                for (int j = 1; j < 5; j++)
                {
                    if (sortedPowers[i + j] != sortedPowers[i] + j)
                    {
                        isStraight = false;
                        break;
                    }
                }
                if (isStraight)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 트리플 감지 (같은 숫자 3장)
        /// </summary>
        private static HandManager.PokerPattern DetectThreeOfAKind(List<CardBullet.Core.Card.Card> hand)
        {
            var powerGroups = hand.GroupBy(c => c.power);
            if (powerGroups.Any(g => g.Count() >= 3))
            {
                return HandManager.PokerPattern.ThreeOfAKind;
            }
            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 투페어 감지 (페어 2개)
        /// </summary>
        private static HandManager.PokerPattern DetectTwoPair(List<CardBullet.Core.Card.Card> hand)
        {
            var powerGroups = hand.GroupBy(c => c.power).Where(g => g.Count() >= 2).ToList();
            if (powerGroups.Count >= 2)
            {
                return HandManager.PokerPattern.TwoPair;
            }
            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 원페어 감지 (같은 숫자 2장)
        /// </summary>
        private static HandManager.PokerPattern DetectOnePair(List<CardBullet.Core.Card.Card> hand)
        {
            var powerGroups = hand.GroupBy(c => c.power);
            if (powerGroups.Any(g => g.Count() >= 2))
            {
                return HandManager.PokerPattern.OnePair;
            }
            return HandManager.PokerPattern.None;
        }

        /// <summary>
        /// 족보 패턴별 데미지 보너스 계산
        /// </summary>
        public static float GetPatternDamageMultiplier(HandManager.PokerPattern pattern)
        {
            switch (pattern)
            {
                case HandManager.PokerPattern.RoyalFlush:
                    return 5.0f;
                case HandManager.PokerPattern.StraightFlush:
                    return 4.0f;
                case HandManager.PokerPattern.FourOfAKind:
                    return 3.5f;
                case HandManager.PokerPattern.FullHouse:
                    return 3.0f;
                case HandManager.PokerPattern.Flush:
                    return 2.5f;
                case HandManager.PokerPattern.Straight:
                    return 2.0f;
                case HandManager.PokerPattern.ThreeOfAKind:
                    return 1.8f;
                case HandManager.PokerPattern.TwoPair:
                    return 1.5f;
                case HandManager.PokerPattern.OnePair:
                    return 1.3f;
                default:
                    return 1.0f;
            }
        }
    }
}

