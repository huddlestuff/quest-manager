﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace QuestManager.Data.UnitTest
{
    [TestClass]
    public class QuestManagerContextTests
    {
        [TestMethod]
        public void FindQuestByQuestId_ShouldMatchAddedQuest()
        {
            var expectedQuest = new Quest { Name = "Test Quest #1" };

            using (var db = new QuestManagerContext())
            {
                db.Quests.Add(expectedQuest);
                db.SaveChanges();
            }

            using (var db = new QuestManagerContext())
            {
                var actualQuest = db.Quests.Find(expectedQuest.QuestId);

                Assert.AreEqual(expectedQuest.QuestId, actualQuest.QuestId);
                Assert.AreEqual(expectedQuest.Name, actualQuest.Name);
            }
        }

        [TestMethod]
        public void FindQuest_WithNoRewards_RewardsShouldBeAnEmptyNonNullList()
        {
            var quest = new Quest { Name = "Test Quest #1" };

            using (var db = new QuestManagerContext())
            {
                quest.Name = "Test Quest #1";
                db.Quests.Add(quest);
                db.SaveChanges();
            }

            using (var db = new QuestManagerContext())
            {
                var actualQuest = db.Quests.Find(quest.QuestId);

                Assert.IsNotNull(actualQuest.Rewards);
                Assert.IsFalse(actualQuest.Rewards.Any());
            }
        }

        [TestMethod]
        public void FindQuest_WithRewards_RewardsShouldMatch()
        {
            var quest = new Quest { Name = "Test Quest #1" };

            using (var db = new QuestManagerContext())
            {
                quest.Name = "Test Quest #1";
                quest.Rewards = new List<Reward>
                {
                    new Reward{Type = "gold", Quantity = 500},
                    new Reward{Type = "item", Quantity = 600},
                };
                db.Quests.Add(quest);
                db.SaveChanges();
            }

            using (var db = new QuestManagerContext())
            {
                var actualQuest = db.Quests.Find(quest.QuestId);

                Assert.IsNotNull(actualQuest.Rewards);
                CollectionAssert.AllItemsAreNotNull(actualQuest.Rewards);
                //CollectionAssert.AreEquivalent(quest.Rewards, actualQuest.Rewards);
            }
        }

        [TestMethod]
        public void FindQuest_RewardAssociatedToItem_ItemShouldMatch()
        {
            var quest = new Quest { Name = "Test Quest #1" };
            var item = new Item { Name = "Item #1" };

            using (var db = new QuestManagerContext())
            {
                quest.Name = "Test Quest #1";
                quest.Rewards = new List<Reward>
                {
                    new Reward{Type = "item", Quantity = 1, Item = item },
                };
                db.Quests.Add(quest);
                db.SaveChanges();
            }

            using (var db = new QuestManagerContext())
            {
                var actualQuest = db.Quests.Find(quest.QuestId);

                Assert.AreEqual(1, actualQuest.Rewards.Count);
                Assert.AreEqual(item.ItemId, actualQuest.Rewards.ElementAt(0).Item.ItemId);
                Assert.AreEqual(item.Name, actualQuest.Rewards.ElementAt(0).Item.Name);
            }
        }

        [TestMethod]
        public void FindQuest_RewardNotAssociatedToItem_ShouldNotRequireAnItem()
        {
            var quest = new Quest { Name = "Test Quest #1" };

            using (var db = new QuestManagerContext())
            {
                quest.Name = "Test Quest #1";
                quest.Rewards = new List<Reward>
                {
                    new Reward{Type = "gold", Quantity = 500 },
                };
                db.Quests.Add(quest);
                db.SaveChanges();
            }

            using (var db = new QuestManagerContext())
            {
                var actualQuest = db.Quests.Find(quest.QuestId);

                Assert.AreEqual(1, actualQuest.Rewards.Count);
                Assert.IsNull(actualQuest.Rewards.ElementAt(0).Item);
            }
        }
    }
}
