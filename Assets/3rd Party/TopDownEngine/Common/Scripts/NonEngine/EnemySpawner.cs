using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators;

namespace GameEngine
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] private float spawnSpeed = 2f;
        [SerializeField] private List<Transform> spawnPositions;

        private float currentSpawnCooldown;
        private void Awake()
        {
            currentSpawnCooldown = spawnSpeed;
        }
        private void Update()
        {
            currentSpawnCooldown -= Time.deltaTime;
            if(currentSpawnCooldown <= 0) 
            {
                SpawnEnemy();
                currentSpawnCooldown = spawnSpeed;
            }
        }

        private void SpawnEnemy()
        {
            Vector3 spawnPoint = spawnPositions.GetRandomElement().position;
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
        }
    }
}