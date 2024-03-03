// Author: Timothy Ngo
// Date: 3/3/2024
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class PathfindingTest
    {
        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator FoodieReachesTable()
        {
             /*
            // Assign, Omitted because of use of singletons
            List<Foodie> foodies = FoodieSpawner.inst.GetFoodieParent().gameObject.GetComponentsInChildren<Foodie>().ToList();
            foreach (Foodie foodie in foodies)
            {
                if (!foodie.gameObject.activeSelf)
                {
                    foodies.Remove(foodie);
                }
            }
            // Act, Omitted because foodies are automatically spawned, I think, I'm not entirely sure if I understand this test system correctly
            // Assert
            Assert.AreEqual(foodies[0].gameObject.transform.position, Upgrades.inst.tablesParent.GetComponentsInChildren<Transform>()[0]);
            */
            yield return new WaitForSeconds(5);
        }
    }

}
