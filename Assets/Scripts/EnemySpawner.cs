using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators;

namespace GameEngine
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] enemyPrefabs;
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
            GameObject newEnemy = Instantiate(enemyPrefabs.GetRandomElement(), spawnPoint, Quaternion.identity);
        }
    }
}