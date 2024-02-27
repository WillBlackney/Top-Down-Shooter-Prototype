using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeAreGladiators;

namespace GameEngine
{
    public class DungeonRoom : MonoBehaviour
    {
        [SerializeField] private WaveData enemiesSpawnedByRoomTrigger;
        [SerializeField] private SpawnerNode[] enemySpawnPositions;
        [SerializeField] private RoomEntryTrigger roomEntryTrigger;

        private void Start()
        {
            if(roomEntryTrigger != null)
            {
                roomEntryTrigger.onTriggerZonePlayer += SpawnRoomEnemies;
            }
            
        }

        private void SpawnRoomEnemies()
        {
            if (roomEntryTrigger != null)
            {
                roomEntryTrigger.onTriggerZonePlayer -= SpawnRoomEnemies;
            }

            StartCoroutine(SpawnRoomEnemiesCoroutine());
        }

        private IEnumerator SpawnRoomEnemiesCoroutine()
        {
            var wave = enemiesSpawnedByRoomTrigger;
            List<SpawnerNode> spawnPoints = enemySpawnPositions.ShuffledCopy();

            if (wave == null)
            {
                yield break;
            }

            for (int i = 0; i < wave.enemiesInWave.Length; i++)
            {
                yield return new WaitForSeconds(wave.enemiesInWave[i].spawnDelay);

                Vector3 spawnPoint = spawnPoints[0].transform.position;
                spawnPoints.RemoveAt(0);
                if(spawnPoints.Count <= 0)
                {
                    spawnPoints = enemySpawnPositions.ShuffledCopy();
                }
                Instantiate(wave.enemiesInWave[i].prefabSpawned, spawnPoint, Quaternion.identity);
            }

            yield return null;
        }

        private void OnDestroy()
        {
            if (roomEntryTrigger != null)
            {
                roomEntryTrigger.onTriggerZonePlayer -= SpawnRoomEnemies;
            }

        }
    }
}