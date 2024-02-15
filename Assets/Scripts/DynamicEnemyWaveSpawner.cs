using MoreMountains.TopDownEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WeAreGladiators;
using WeAreGladiators.Utilities;

namespace GameEngine
{
    public class DynamicEnemyWaveSpawner : MonoBehaviour
    {

        [Header("Settings")]
        [SerializeField] private bool allowTimedSpawning = true;
        [Space(10)]

        [SerializeField] private int minimumWaveCooldown = 60;
        [SerializeField] private int maximumWaveCooldown = 240;
        [Space(10)]

        [SerializeField] private int minEnemiesPerWave = 15;
        [SerializeField] private int maxEnemiesPerWave = 30;
        [Space(10)]

        [SerializeField] private GameObject enemyPrefab;

        private float secondsUntilNextWave = 0;

        [Header("Testing Stuff")]
        [SerializeField] private bool forceSpawnWave = false;
        [SerializeField] private bool cleanupWave = false;


        private SpawnerNode[] spawnNodes;
        public SpawnerNode[] SpawnerNodes
        {
            get
            {
                if(spawnNodes == null || spawnNodes.Length == 0)
                {
                    spawnNodes = FindObjectsOfType<SpawnerNode>();
                }

                return spawnNodes;
            }
        }

        private void Start()
        {
            RandomizeNextWaveCountdown();
        }

        private void Update()
        {
            if (allowTimedSpawning)
            {
                secondsUntilNextWave -= Time.deltaTime;
                if (secondsUntilNextWave >= 0) return;

                StartCoroutine(SpawnWave());
                RandomizeNextWaveCountdown();
            }          
            
            if (forceSpawnWave)
            {
                forceSpawnWave = false;
                StartCoroutine(SpawnWave());
            }
            if (cleanupWave)
            {
                cleanupWave = false;
                StopAllCoroutines();
                var enemies = Array.FindAll(FindObjectsOfType<Character>(), character => character.CharacterType == Character.CharacterTypes.AI);
                for(int i = 0; i < enemies.Length; i++)
                {
                    Destroy(enemies[i].gameObject);
                }
            }
        }

        private void RandomizeNextWaveCountdown()
        {
            Debug.Log("DynamicEnemyWaveSpawner.RandomizeNextWaveCountdown()");
            secondsUntilNextWave = RandomGenerator.NumberBetween(minimumWaveCooldown, maximumWaveCooldown);
            Debug.Log("Next wave in " + secondsUntilNextWave.ToString() + " seconds");
        }

        private IEnumerator SpawnWave()
        {
            Debug.Log("DynamicEnemyWaveSpawner.SpawnWave()");
            Character player = Array.Find(FindObjectsOfType<Character>(), character => character.CharacterType == Character.CharacterTypes.Player);
            float minDistance = 20f;
            float maxDistance = 40f;
            var possibleSpawnLocations = Array.FindAll(SpawnerNodes, node =>
                Vector2.Distance(node.transform.position, player.transform.position) <= maxDistance &&
                Vector2.Distance(node.transform.position, player.transform.position) >= minDistance).ToList();

            possibleSpawnLocations.Shuffle();
            List<Transform> chosenPoints = new List<Transform>();
            List<Transform> finalPoints = new List<Transform>();
            for (int i = 0; i < possibleSpawnLocations.Count && i < 3; i++)
            {
                chosenPoints.Add(possibleSpawnLocations[i].transform);
            }

            // Need to do something cleaner than this in future
            for(int i = 0; i < 20; i++)
            {
                finalPoints.AddRange(chosenPoints);
            }

            int enemiesToSpawn = RandomGenerator.NumberBetween(minEnemiesPerWave, maxEnemiesPerWave);

            // Spawn enemies
            for(int i = 0; i < enemiesToSpawn; i++)
            {
                Transform spawnPoint = finalPoints[i];
                GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(0.5f);
            }

        }


    }
}