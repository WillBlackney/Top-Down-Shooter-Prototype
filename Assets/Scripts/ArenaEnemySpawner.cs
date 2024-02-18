using MoreMountains.TopDownEngine;
using System;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators;

namespace GameEngine
{
    public class ArenaEnemySpawner : MonoBehaviour
    {
        
        [SerializeField] private GameObject[] enemyPrefabs;
        [SerializeField] private float spawningStartDelay = 3f;
        [SerializeField] private float startingSpawnCooldown = 5f;
        [SerializeField] private Transform[] spawnPositions;

        private float currentSpawnCooldown;
        private int currentMaxAllowedEnemies = 1;

        [SerializeField] private float secondsBetweenDifficultyIncreases = 20f;
        private float secondsTilNextDifficultyIncrease = 20f;


        // max allowed enemies on the field
        // seconds between spawns
        // which enemy to spawn

        private float secondsTilNextEnemySpawn;
        private void Awake()
        {
            secondsTilNextDifficultyIncrease = secondsBetweenDifficultyIncreases;
            currentSpawnCooldown = startingSpawnCooldown;
            secondsTilNextEnemySpawn = 0;
        }
        private void Update()
        {
            spawningStartDelay -= Time.deltaTime;
            if(spawningStartDelay > 0)
            {
                return;
            }

            secondsTilNextEnemySpawn -= Time.deltaTime;
            if (secondsTilNextEnemySpawn <= 0)
            {
                SpawnEnemy();
                secondsTilNextEnemySpawn = currentSpawnCooldown;
            }

            secondsTilNextDifficultyIncrease -= Time.deltaTime;
            if (secondsTilNextDifficultyIncrease <= 0)
            {
                IncreaseDifficulty();
                secondsTilNextDifficultyIncrease = secondsBetweenDifficultyIncreases;
            }
        }

        private void SpawnEnemy()
        {
            Character player = Array.Find(FindObjectsOfType<Character>(), character => character.CharacterType == Character.CharacterTypes.Player);
            float minDistance = 3f;
            var possibleSpawnLocations = Array.FindAll(spawnPositions, node =>
                Vector2.Distance(node.transform.position, player.transform.position) >= minDistance);

            Vector3 spawnPoint = possibleSpawnLocations.GetRandomElement().position;
            GameObject newEnemy = Instantiate(enemyPrefabs.GetRandomElement(), spawnPoint, Quaternion.identity);
        }

        private void IncreaseDifficulty()
        {
            currentMaxAllowedEnemies += 1;
            currentSpawnCooldown -= 0.5f;
            if(currentSpawnCooldown <= 1)
            {
                currentSpawnCooldown = 1;

            }
        }
    }
}
